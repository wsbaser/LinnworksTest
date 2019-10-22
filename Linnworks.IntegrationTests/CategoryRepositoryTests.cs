using LinnworksTest.DataAccess;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Linnworks.IntegrationTests
{
    [TestFixture]
    public class CategoryRepositoryTests : IntegrationTestBase
    {
        [Test]
        public async Task Create_Category_ShouldGenerateID()
        {
            // .Arrange
            Category category = new Category() { CategoryName = Guid.NewGuid().ToString() };

            // .Act
            using (var context = GetLinnworksIntegrationContext())
            {
                var sut = new GenericRepository<Category>(context);
                category = await sut.CreateAsync(category);
            }

            // .Assert
            Assert.NotNull(category.Id);
            Assert.AreNotEqual(new Guid().ToString(), category.Id);
        }

        [Test]
        public async Task Create_DuplicatedCategory_ShouldThrowException()
        {
            // .Arrange
            Category category = new Category() { CategoryName = "DUPLICATED" };
            using (var context = GetLinnworksIntegrationContext())
            {
                await context.AddAsync(category);
                await context.SaveChangesAsync();
            }
            var duplicatedCatgory = new Category() { CategoryName = category.CategoryName };

            // .Assert
            using (var context = GetLinnworksIntegrationContext())
            {
                var sut = new GenericRepository<Category>(context);
                Assert.ThrowsAsync<Exception>(async () =>
                    // .Act
                    await sut.CreateAsync(duplicatedCatgory));
            }
        }

        [Test]
        public async Task Update_CategoryWithoutId_ShouldUpdateCategory()
        {
            // .Arrange
            Category categoryToUpdate = new Category() { CategoryName = Guid.NewGuid().ToString() };
            using (var context = GetLinnworksIntegrationContext())
            {
                await context.AddAsync(categoryToUpdate);
                await context.SaveChangesAsync();
            }
            var updatedCategory = new Category() { CategoryName = Guid.NewGuid().ToString() };

            // .Act
            using (var context = GetLinnworksIntegrationContext())
            {
                var sut = new GenericRepository<Category>(context);
                await sut.UpdateAsync(categoryToUpdate.Id, updatedCategory);
            }
            // .Assert
            using (var context = GetLinnworksIntegrationContext())
            {
                Assert.AreEqual(updatedCategory.CategoryName, context.Find<Category>(categoryToUpdate.Id).CategoryName);
            }
        }

        [Test]
        public async Task Update_DuplicatedCategory_ShouldThrowException()
        {
            // .Arrange
            const string duplicatedName = "DUPLICATED";
            Category category = new Category() { CategoryName = duplicatedName };
            Category categoryToUpdate = new Category() { CategoryName = "UPDATE TO DUPLICATED" };
            using (var context = GetLinnworksIntegrationContext())
            {
                await context.AddAsync(category);
                await context.AddAsync(categoryToUpdate);
                await context.SaveChangesAsync();
            }

            // .Assert
            using (var context = GetLinnworksIntegrationContext())
            {
                var sut = new GenericRepository<Category>(context);
                var updatedCategory = new Category() { CategoryName = duplicatedName };
                Assert.ThrowsAsync<Exception>(async () =>
                    // .Act
                    await sut.UpdateAsync(categoryToUpdate.Id, updatedCategory));
            }
        }

        [Test]
        public async Task Update_NotExistingCategory_ShouldThrowException()
        {
            // .Assert
            using (var context = GetLinnworksIntegrationContext())
            {
                var sut = new GenericRepository<Category>(context);
                Assert.ThrowsAsync<Exception>(async () =>
                    // .Act
                    await sut.UpdateAsync(Guid.NewGuid(), new Category() { CategoryName = Guid.NewGuid().ToString() }));
            }
        }

        [Test]
        public async Task Delete_CategoryWithProducts_ShouldDeleteCategory()
        {
            // .Arrange
            Category category = new Category() { CategoryName = Guid.NewGuid().ToString() };
            using (var context = GetLinnworksIntegrationContext())
            {
                await context.AddAsync(category);
                await context.SaveChangesAsync();
                await context.AddAsync(new Product() { CategoryId = category.Id, Title = Guid.NewGuid().ToString() });
                await context.SaveChangesAsync();
            }

            // .Act
            using (var context = GetLinnworksIntegrationContext())
            {
                var sut = new GenericRepository<Category>(context);
                await sut.DeleteAsync(category.Id);
            }
            // .Assert
            using (var context = GetLinnworksIntegrationContext())
            {
                Assert.IsNull(context.Find<Category>(category.Id));
            }
        }

        [Test]
        public async Task Delete_NotExistingCategory_ShouldThrowException()
        {
            // .Assert
            using (var context = GetLinnworksIntegrationContext())
            {
                var sut = new GenericRepository<Category>(context);
                Assert.ThrowsAsync<Exception>(async () =>
                    // .Act
                    await sut.DeleteAsync(Guid.NewGuid()));
            }
        }
    }
}
