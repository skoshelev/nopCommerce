﻿using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Security;
using Nop.Services.Stores;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    [TestFixture]
    public class CategoriesControllerTests_GetCategoriesById
    {
        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveCategoryId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<CategoriesApiController>();

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCategoryById(nonPositiveCategoryId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallCategoryApiService(int negativeCategoryId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<CategoriesApiController>();
            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);

            // Act
            autoMocker.ClassUnderTest.GetCategoryById(negativeCategoryId);

            // Assert
            autoMocker.Get<ICategoryApiService>().AssertWasNotCalled(x => x.GetCategoryById(negativeCategoryId));
        }

        [Test]
        public void WhenIdIsPositiveNumberButNoSuchCategoryExists_ShouldReturn404NotFound()
        {
            int nonExistingCategoryId = 5;

            // Arange
            var autoMocker = new RhinoAutoMocker<CategoriesApiController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoryById(nonExistingCategoryId)).Return(null);

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCategoryById(nonExistingCategoryId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void WhenIdEqualsToExistingCategoryId_ShouldSerializeThatCategory()
        {
            MappingExtensions.Maps.CreateMap<Category, CategoryDto>();

            int existingCategoryId = 5;
            var existingCategory = new Category() { Id = existingCategoryId };

            // Arange
            var autoMocker = new RhinoAutoMocker<CategoriesApiController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoryById(existingCategoryId)).Return(existingCategory);
            autoMocker.Get<IAclService>().Stub(x => x.GetAclRecords(new Category())).IgnoreArguments().Return(new List<AclRecord>());
            autoMocker.Get<IStoreMappingService>().Stub(x => x.GetStoreMappings(new Category())).IgnoreArguments().Return(new List<StoreMapping>());

            // Act
            autoMocker.ClassUnderTest.GetCategoryById(existingCategoryId);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<CategoriesRootObject>.Matches(
                        objectToSerialize =>
                               objectToSerialize.Categories.Count == 1 &&
                               objectToSerialize.Categories[0].Id == existingCategory.Id.ToString() &&
                               objectToSerialize.Categories[0].Name == existingCategory.Name),
                    Arg<string>.Is.Equal("")));
        }

        [Test]
        public void WhenIdEqualsToExistingCategoryIdAndFieldsSet_ShouldReturnJsonForThatCategoryWithSpecifiedFields()
        {
            MappingExtensions.Maps.CreateMap<Category, CategoryDto>();

            int existingCategoryId = 5;
            var existingCategory = new Category() { Id = existingCategoryId, Name = "some category name" };
            string fields = "id,name";

            // Arange
            var autoMocker = new RhinoAutoMocker<CategoriesApiController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoryById(existingCategoryId)).Return(existingCategory);
            autoMocker.Get<IAclService>().Stub(x => x.GetAclRecords(new Category())).IgnoreArguments().Return(new List<AclRecord>());
            autoMocker.Get<IStoreMappingService>().Stub(x => x.GetStoreMappings(new Category())).IgnoreArguments().Return(new List<StoreMapping>());

            // Act
            autoMocker.ClassUnderTest.GetCategoryById(existingCategoryId, fields);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<CategoriesRootObject>.Matches(objectToSerialize => objectToSerialize.Categories[0].Id == existingCategory.Id.ToString() &&
                                                                           objectToSerialize.Categories[0].Name == existingCategory.Name),
                Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
        }
    }
}