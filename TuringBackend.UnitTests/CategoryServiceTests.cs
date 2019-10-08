using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TuringBackend.Api.Services;
using TuringBackend.Models;
using Xunit;

namespace TuringBackend.UnitTests
{
    public class CategoryServiceTests
    {
        private Mock<ICategoryService> _categoryService;

        public CategoryServiceTests()
        {
        }


        [Fact]
        public async Task Should_Return_Null_For_Invalid_Category()
        {
            _categoryService = new Mock<ICategoryService>();
            _categoryService.Setup(d => d.GetCategoryByIAsync(1))
                .Returns(Task.FromResult(new Category() { DepartmentId = 1, Name = "cat1", Description = "cat1" }));
            var response = await _categoryService.Object.GetCategoryByIAsync(2);
            Assert.Null(response);

        }


        [Fact]
        public async Task Should_Return_Value_For_Valid_Category()
        {
            _categoryService = new Mock<ICategoryService>();
            _categoryService.Setup(d => d.GetCategoryByIAsync(1))
                .Returns(Task.FromResult(new Category() { DepartmentId = 1, Name = "cat1", Description = "cat1" }));
            var response = await _categoryService.Object.GetCategoryByIAsync(1);
            Assert.NotNull(response);
            Assert.Equal("cat1", response.Name);

        }
    }
}
