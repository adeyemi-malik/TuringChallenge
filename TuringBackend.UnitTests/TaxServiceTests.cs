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
    public class TaxServiceTests
    {
        private Mock<ITaxService> _taxService;

        public TaxServiceTests()
        {
        }


        [Fact]
        public async Task Should_Return_Null_For_Invalid_Tax()
        {
            _taxService = new Mock<ITaxService>();
            _taxService.Setup(d => d.GetTaxByIdAsync(1))
                .Returns(Task.FromResult(new Tax() { TaxId = 1, TaxType = "tax1", TaxPercentage= 20}));
            var response = await _taxService.Object.GetTaxByIdAsync(2);
            Assert.Null(response);

        }


        [Fact]
        public async Task Should_Return_Value_For_Valid_Tax()
        {
            _taxService = new Mock<ITaxService>();
            _taxService.Setup(d => d.GetTaxByIdAsync(1))
                .Returns(Task.FromResult(new Tax() { TaxId = 1, TaxType = "tax1", TaxPercentage = 20 }));
            var response = await _taxService.Object.GetTaxByIdAsync(1);
            Assert.NotNull(response);
            Assert.Equal(20, response.TaxPercentage);

        }
    }
}
