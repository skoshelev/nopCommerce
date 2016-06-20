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
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Security;
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
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CategoriesApiController : BaseApiController
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IDiscountService _discountService;
        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IFactory<Category> _factory; 

        public CategoriesApiController(ICategoryApiService categoryApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            IFactory<Category> factory) : base(jsonFieldsSerializer)
        {
            _categoryApiService = categoryApiService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _factory = factory;
            _customerService = customerService;
            _aclService = aclService;
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
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the categoryDelta object won't be null for sure so we don't need to check for this.

            Picture insertedPicture = null;

            // We need to insert the picture before the category so we can obtain the picture id and map it to the category.
            if (categoryDelta.Dto.Image.Binary != null)
            {
                insertedPicture = _pictureService.InsertPicture(categoryDelta.Dto.Image.Binary, categoryDelta.Dto.Image.MimeType, string.Empty);
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
            List<int> roleIds = null;

            if (categoryDelta.Dto.RoleIds.Count > 0)
            {
                roleIds = MapRoleToCategory(newCategory, categoryDelta.Dto);
            }

            List<int> discountIds = null;

            if (categoryDelta.Dto.DiscountIds.Count > 0)
            {
                discountIds = ApplyDiscountsToCategory(newCategory, categoryDelta.Dto);
            }

            List<int> storeIds = null;

            if (categoryDelta.Dto.StoreIds.Count > 0)
            {
                storeIds = MapCategoryToStores(newCategory, categoryDelta.Dto);
            }

            // Preparing the result dto of the new category
            CategoryDto newCategoryDto = newCategory.ToDto();

            //search engine name
            newCategoryDto.SeName = newCategory.ValidateSeName(newCategoryDto.SeName, newCategory.Name, true);
            _urlRecordService.SaveSlug(newCategory, newCategoryDto.SeName, 0);

            // Here we prepare the resulted dto image.
            PrepareImageDto(insertedPicture, newCategoryDto);
            
            if (storeIds != null)
            {
                newCategoryDto.StoreIds = storeIds;
            }

            if (discountIds != null)
            {
                newCategoryDto.DiscountIds = discountIds;
            }

            if (roleIds != null)
            {
                newCategoryDto.RoleIds = roleIds;
            }

            _customerActivityService.InsertActivity("AddNewCategory",
                _localizationService.GetResource("ActivityLog.AddNewCategory"), newCategory.Name);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(newCategoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult UpdateCategory(
            [ModelBinder(typeof (JsonModelBinder<CategoryDto>))] Delta<CategoryDto> categoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the category id, because this will happen in the model binder using the dto validator.
            int updateCategoryId = int.Parse(categoryDelta.Dto.Id);

            Category categoryEntityToUpdate = _categoryService.GetCategoryById(updateCategoryId);
            categoryDelta.Merge(categoryEntityToUpdate);
     
            Picture updatedPicture = UpdatePicture(categoryEntityToUpdate, categoryDelta.Dto.Image.Binary, categoryDelta.Dto.Image.MimeType);

            List<int> storeIds = MapCategoryToStores(categoryEntityToUpdate, categoryDelta.Dto);
         
            List<int> roleIds = MapRoleToCategory(categoryEntityToUpdate, categoryDelta.Dto);

            List<int> discountIds = ApplyDiscountsToCategory(categoryEntityToUpdate, categoryDelta.Dto);

            _categoryService.UpdateCategory(categoryEntityToUpdate);

            _customerActivityService.InsertActivity("UpdateCategory",
                _localizationService.GetResource("ActivityLog.UpdateCategory"), categoryEntityToUpdate.Name);

            CategoryDto updatedCategoryDto = categoryEntityToUpdate.ToDto();

            PrepareImageDto(updatedPicture, updatedCategoryDto);
            
            updatedCategoryDto.StoreIds = storeIds;
            
            updatedCategoryDto.RoleIds = roleIds;

            updatedCategoryDto.DiscountIds = discountIds;

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(updatedCategoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        public IHttpActionResult DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Category categoryToDelete = _categoryService.GetCategoryById(id);

            if (categoryToDelete == null)
            {
                return NotFound();
            }

            _categoryService.DeleteCategory(categoryToDelete);

            return new RawJsonActionResult("{}");
        }

        private Picture UpdatePicture(Category categoryEntityToUpdate, byte[] imageBytes, string mimeType)
        {
            Picture updatedPicture = null;
            Picture currentCategoryPicture = _pictureService.GetPictureById(categoryEntityToUpdate.PictureId);

            // when there is a picture set for the category
            if (currentCategoryPicture != null)
            {
                _pictureService.DeletePicture(currentCategoryPicture);

                // When the image attachment is null or empty.
                if (imageBytes == null)
                {
                    categoryEntityToUpdate.PictureId = 0;
                }
                else
                {
                    updatedPicture = _pictureService.InsertPicture(imageBytes, mimeType, string.Empty);
                    categoryEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }
            // when there isn't a picture set for the category
            else
            {
                if (imageBytes != null)
                {
                    updatedPicture = _pictureService.InsertPicture(imageBytes, mimeType, string.Empty);
                    categoryEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }

            return updatedPicture;
        }

        private void PrepareImageDto(Picture picture, CategoryDto newCategoryDto)
        {
            if (picture != null)
            {
                // We don't use the image from the passed dto directly 
                // because the picture may be passed with src and the result should only include the base64 format.
                newCategoryDto.Image = new ImageDto()
                {
                    Attachment = Convert.ToBase64String(picture.PictureBinary)
                };
            }
        }

        private List<int> MapCategoryToStores(Category category, CategoryDto dto)
        {
            IList<StoreMapping> existingStoreMappings = _storeMappingService.GetStoreMappings(category);
            IList<Store> allStores = _storeService.GetAllStores();

            var storeIds = new List<int>();
            
            foreach (var store in allStores)
            {
                if (dto.StoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                    {
                        _storeMappingService.InsertStoreMapping(category, store.Id);
                    }

                    storeIds.Add(store.Id);
                }
                else
                {
                    //remove store
                    StoreMapping storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);

                    if (storeMappingToDelete != null)
                    {
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                        storeIds.Remove(store.Id);
                    }
                }
            }

            category.LimitedToStores = storeIds.Count > 0;

            return storeIds;
        }

        private List<int> ApplyDiscountsToCategory(Category category, CategoryDto dto)
        {
            var discountIds = new List<int>();
            Dictionary<int, bool> appliedDiscounts = category.AppliedDiscounts.ToDictionary(x => x.Id, x => true);

            // Ensure that there won't be repeating discounts.
            var uniqueDiscounts = new HashSet<int>(dto.DiscountIds);

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true);

            foreach (var discount in allDiscounts)
            {
                // Apply the discount
                if (!appliedDiscounts.ContainsKey(discount.Id) && uniqueDiscounts.Contains(discount.Id))
                {
                    category.AppliedDiscounts.Add(discount);
                    discountIds.Add(discount.Id);
                }
                // Remove the discount
                else if(appliedDiscounts.ContainsKey(discount.Id) && !uniqueDiscounts.Contains(discount.Id))
                {
                    category.AppliedDiscounts.Remove(discount);
                    discountIds.Remove(discount.Id);
                }
            }

            return discountIds;
        }

        private List<int> MapRoleToCategory(Category category, CategoryDto dto)
        {
            var roleIds = new List<int>();

            IList<AclRecord> existingAclRecords = _aclService.GetAclRecords(category);
            IList<CustomerRole> allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            
            foreach (var customerRole in allCustomerRoles)
            {
                if (dto.RoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                    {
                        _aclService.InsertAclRecord(category, customerRole.Id);
                    }

                    roleIds.Add(customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);

                    if (aclRecordToDelete != null)
                    {
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                        roleIds.Remove(customerRole.Id);
                    }
                }
            }

            category.SubjectToAcl = roleIds.Count > 0;

            return roleIds;
        }
    }
}