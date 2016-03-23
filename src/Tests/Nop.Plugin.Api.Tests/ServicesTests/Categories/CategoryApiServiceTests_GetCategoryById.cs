using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Categories
{
    [TestFixture]
    public class CategoryApiServiceTests_GetCategoryById
    {
        [Test]
        public void WhenNullIsReturnedByTheRepository_ShouldReturnNull()
        {
            int categoryId = 3;
            
            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.GetById(categoryId)).Return(null);

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo);
            var result = cut.GetCategoryById(categoryId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void WhenCategoryIsReturnedByTheRepository_ShouldReturnTheSameCategory()
        {
            int categoryId = 3;
            Category category = new Category() { Id = 3, Name = "some name" };

            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.GetById(categoryId)).Return(category);

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo);
            var result = cut.GetCategoryById(categoryId);

            // Assert
            Assert.AreSame(category, result);
        }

        [Test]
        [TestCase(-2)]
        [TestCase(0)]
        public void WhenNegativeOrZeroCategoryIdPassed_ShouldReturnNull(int negativeOrZeroCategoryId)
        {
            // Arange
            var categoryRepoMock = MockRepository.GenerateMock<IRepository<Category>>();
            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            // Act
            var cut = new CategoryApiService(categoryRepoMock, productCategoryRepo);
            var result = cut.GetCategoryById(negativeOrZeroCategoryId);

            // Assert
            Assert.IsNull(result);
        }
    }
}
