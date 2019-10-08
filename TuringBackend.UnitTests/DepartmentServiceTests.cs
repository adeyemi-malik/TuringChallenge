using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TuringBackend.Api.Core;
using TuringBackend.Api.Services;
using TuringBackend.Models;
using TuringBackend.Models.Data;
using Xunit;

namespace TuringBackend.UnitTests
{
    public  class DepartmentServiceTests
    {
        private Mock<IDepartmentService> _departmentService;

        public DepartmentServiceTests()
        {
        }


        [Fact]
        public async Task Should_Return_Null_For_Invalid_Department()
        {
            _departmentService = new Mock<IDepartmentService>();
            _departmentService.Setup(d => d.GetDepartmentByIdAsync(1))
                .Returns(Task.FromResult(new Department { DepartmentId = 1, Name = "dep1", Description = "dep1" }));
            var response = await _departmentService.Object.GetDepartmentByIdAsync(2);
            Assert.Null(response);
            
        }


        [Fact]
        public async Task Should_Return_Value_For_Valid_Department()
        {
            _departmentService = new Mock<IDepartmentService>();
            _departmentService.Setup(d => d.GetDepartmentByIdAsync(1))
                .Returns(Task.FromResult(new Department { DepartmentId = 1, Name = "dep1", Description = "dep1" }));
            var response = await _departmentService.Object.GetDepartmentByIdAsync(1);
            Assert.NotNull(response);
            Assert.Equal("dep1",response.Name);

        }
    }
}
