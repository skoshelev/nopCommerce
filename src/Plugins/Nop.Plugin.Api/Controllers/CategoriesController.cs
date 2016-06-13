using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Services.Discounts;
using Nop.Services.Media;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CategoriesController : ApiController
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;
        private readonly IPictureService _pictureService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IDiscountService _discountService;
        private readonly IFactory<Category> _factory; 

        public CategoriesController(ICategoryApiService categoryApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IFactory<Category> factory)
        {
            _categoryApiService = categoryApiService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _factory = factory;
            _discountService = discountService;
            _storeService = storeService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
        }

        /// <summary>
        /// Receive a list of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult GetCategories(CategoriesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<Category> allCategories = _categoryApiService.GetCategories(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                             parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                             parameters.Limit, parameters.Page, parameters.SinceId,
                                                                             parameters.ProductId, parameters.PublishedStatus);

            IList<CategoryDto> categoriesAsDtos = allCategories.Select(x => x.ToDto()).ToList();

            var categoriesRootObject = new CategoriesRootObject()
            {
                Categories = categoriesAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesCountRootObject))]
        public IHttpActionResult GetCategoriesCount(CategoriesCountParametersModel parameters)
        {
            var allCategoriesCount = _categoryApiService.GetCategoriesCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                            parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                            parameters.PublishedStatus, parameters.ProductId);

            var categoriesCountRootObject = new CategoriesCountRootObject()
            {
                Count = allCategoriesCount
            };

            return Ok(categoriesCountRootObject);
        }

        /// <summary>
        /// Retrieve category by spcified id
        /// </summary>
        /// <param name="id">Id of the category</param>
        /// <param name="fields">Fields from the category you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult GetCategoryById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Category category = _categoryApiService.GetCategoryById(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(category.ToDto());

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult CreateCategory([ModelBinder(typeof(JsonModelBinder<CategoryDto>))] Delta<CategoryDto> categoryDelta)
        {
            bool imageSrcSet = ModelState.IsValid && !string.IsNullOrEmpty(categoryDelta.Dto.Image.Src);
            bool imageAttachmentSet = ModelState.IsValid && !string.IsNullOrEmpty(categoryDelta.Dto.Image.Attachment);
            
            byte[] imageBytes = null;
            string mimeType = string.Empty;
            Picture insertedPicture = null;

            if (imageSrcSet || imageAttachmentSet)
            {
                // Validation of the image object

                // We can't have both set.
                CheckIfBothImageSourceTypesAreSet(imageSrcSet, imageAttachmentSet);
                
                // Here we ensure that the validation to this point has passed 
                // and try to download the image or convert base64 format to byte array
                // depending on which format is passed. In both cases we should get a byte array and mime type.
                if (ModelState.IsValid)
                {
                    if (imageSrcSet)
                    {
                        DownloadFromSrc(categoryDelta.Dto.Image.Src, ref imageBytes, ref mimeType);
                    }
                    else if (imageAttachmentSet)
                    {
                        ValidateAttachmentFormat(categoryDelta.Dto.Image.Attachment);

                        if (ModelState.IsValid)
                        {
                            ConvertAttachmentToByteArray(categoryDelta.Dto.Image.Attachment, ref imageBytes,
                                ref mimeType);
                        }
                    }
                }

                // Here we handle the check if the file passed is actual image and if the image is valid according to the 
                // restrictions set in the administration.
                ValidatePictureBiteArray(imageBytes, mimeType);
            }

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the categoryDelta object won't be null for sure so we don't need to check for this.
            
            // We need to insert the picture before the category so we can obtain the picture id and map it to the category.
            if (imageBytes != null)
            {
                insertedPicture = _pictureService.InsertPicture(imageBytes, mimeType, string.Empty);
            }

            // Inserting the new category
            Category newCategory = _factory.Initialize();
            categoryDelta.Merge(newCategory);

            if (insertedPicture != null)
            {
                newCategory.PictureId = insertedPicture.Id;
            }

            _categoryService.InsertCategory(newCategory);

            // TODO: Localization
            // TODO: ACL 
            List<int> discountIds = null;

            if (categoryDelta.Dto.DiscountIds != null && categoryDelta.Dto.DiscountIds.Count > 0)
            {
                discountIds = ApplyDiscountsToCategory(newCategory, categoryDelta.Dto);
            }

            List<int> storeIds = null;

            if (categoryDelta.Dto.StoreIds != null && categoryDelta.Dto.StoreIds.Count > 0)
            {
                storeIds = MapCategoryToStores(newCategory, categoryDelta.Dto);
            }

            // Preparing the result dto of the new category
            CategoryDto newCategoryDto = newCategory.ToDto();

            //search engine name
            newCategoryDto.SeName = newCategory.ValidateSeName(newCategoryDto.SeName, newCategory.Name, true);
            _urlRecordService.SaveSlug(newCategory, newCategoryDto.SeName, 0);

            _customerActivityService.InsertActivity("AddNewCategory",
                _localizationService.GetResource("ActivityLog.AddNewCategory"), newCategory.Name);

            // Here we prepare the resulted dto image.
            if (insertedPicture != null)
            {
                // We don't use the image from the passed dto directly 
                // because the picture may be passed with src and the result should only include the base64 format.
                newCategoryDto.Image = new ImageDto()
                {
                    Attachment = Convert.ToBase64String(insertedPicture.PictureBinary)
                };
            }
            
            if (storeIds != null)
            {
                newCategoryDto.StoreIds = storeIds;
            }

            if (discountIds != null)
            {
                newCategoryDto.DiscountIds = discountIds;
            }

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(newCategoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private void ValidatePictureBiteArray(byte[] imageBytes, string mimeType)
        {
            if (imageBytes != null)
            {
                try
                {
                    imageBytes = _pictureService.ValidatePicture(imageBytes, mimeType);
                }
                catch (Exception ex)
                {
                    var key = string.Format(_localizationService.GetResource("Api.InvalidType"), "image");
                    string message = string.Format("{0} - {1}", _localizationService.GetResource("Api.Category.InvalidImageSrcType"), ex.Message);

                    ModelState.AddModelError(key, message);
                }
            }
            
            if (imageBytes == null)
            {
                var key = string.Format(_localizationService.GetResource("Api.InvalidType"), "image");
                string message = _localizationService.GetResource("Api.Category.InvalidImageSrcType");

                ModelState.AddModelError(key, message);
            }
        }

        private void ConvertAttachmentToByteArray(string attachment, ref byte[] imageBytes, ref string mimeType)
        {
            imageBytes = Convert.FromBase64String(attachment);
            mimeType = GetMimeTypeFromByteArray(imageBytes);
        }

        private void DownloadFromSrc(string imageSrc, ref byte[] imageBytes, ref string mimeType)
        {
            var key = string.Format(_localizationService.GetResource("Api.InvalidType"), "image");
            // TODO: discuss if we need our own web client so we can set a custom tmeout - this one's timeout is 100 sec.
            var client = new WebClient();

            try
            {
                imageBytes = client.DownloadData(imageSrc);
                // This needs to be after the downloadData is called from client, otherwise there won't be any response headers.
                mimeType = client.ResponseHeaders["content-type"];

                if (imageBytes == null)
                {
                    ModelState.AddModelError(key, _localizationService.GetResource("Api.Category.InvalidImageSrc"));
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("{0} - {1}",
                    _localizationService.GetResource("Api.Category.InvalidImageSrc"), ex.Message);

                ModelState.AddModelError(key, message);
            }
        }

        private static string GetMimeTypeFromByteArray(byte[] imageBytes)
        {
            MemoryStream stream = new MemoryStream(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(stream, true);
            ImageFormat format = image.RawFormat;
            ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == format.Guid);
            return codec.MimeType;
        }

        private void CheckIfBothImageSourceTypesAreSet(bool imageSrcSet, bool imageAttachmentSet)
        {
            if (imageSrcSet &&
                imageAttachmentSet)
            {
                var key = string.Format(_localizationService.GetResource("Api.InvalidType"), "image");
                ModelState.AddModelError(key, _localizationService.GetResource("Api.Category.ImageSrcAndAttachmentSet"));
            }
        }

        private void ValidateAttachmentFormat(string attachment)
        {
            Regex validBase64Pattern =
                new Regex("^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$");
            bool isMatch = validBase64Pattern.IsMatch(attachment);
            if (!isMatch)
            {
                var key = string.Format(_localizationService.GetResource("Api.InvalidType"), "image");
                ModelState.AddModelError(key, _localizationService.GetResource("Api.Category.InvalidImageAttachmentFormat"));
            }
        }

        private List<int> MapCategoryToStores(Category category, CategoryDto dto)
        {
            var existingStoreMappings = _storeMappingService.GetStoreMappings(category);
            var allStores = _storeService.GetAllStores();

            var storeIds = new List<int>();

            // TODO: Discuss the case where you have storeids but they are all non existing stores.
            var limitedToStores = dto.StoreIds != null && dto.StoreIds.Count > 0;

            foreach (var store in allStores)
            {
                if (limitedToStores && dto.StoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                    {
                        _storeMappingService.InsertStoreMapping(category, store.Id);
                        storeIds.Add(store.Id);
                    }
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                    {
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                        storeIds.Remove(store.Id);
                    }
                }
            }

            category.LimitedToStores = limitedToStores;

            return storeIds;
        }

        private List<int> ApplyDiscountsToCategory(Category category, CategoryDto dto)
        {
            var discountIds = new List<int>();
            HashSet<int> uniqueDiscounts = new HashSet<int>(dto.DiscountIds);

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true);

            foreach (var discount in allDiscounts)
            {
                if (dto.DiscountIds != null && uniqueDiscounts.Contains(discount.Id))
                {
                    category.AppliedDiscounts.Add(discount);
                    discountIds.Add(discount.Id);
                }
            }

            return discountIds;
        }

        private IHttpActionResult Error()
        {
            var errors = new Dictionary<string, List<string>>();

            foreach (var item in ModelState)
            {
                var errorMessages = item.Value.Errors.Select(x => x.ErrorMessage);

                if (errors.ContainsKey(item.Key))
                {
                    errors[item.Key].AddRange(errorMessages);
                }
                else
                {
                    errors.Add(item.Key, errorMessages.ToList());
                }
            }

            var errorsRootObject = new ErrorsRootObject()
            {
                Errors = errors
            };

            var errorsJson = _jsonFieldsSerializer.Serialize(errorsRootObject, null);

            return new ErrorActionResult(errorsJson);
        }
    }
}