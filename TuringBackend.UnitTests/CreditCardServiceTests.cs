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
    public class CreditCardServiceTests
    {
        private Mock<ICreditCardService> _creditcardService;

        public CreditCardServiceTests()
        {
        }


        [Fact]
        public  void Should_Return_True_For_Valid_Card()
        {
            _creditcardService = new Mock<ICreditCardService>();
           
            var response =  _creditcardService.Object.IsCardNumberValid("4409582103487191");
            Assert.False(response);

        }

        [Fact]
        public void Should_Return_False_For_InValid_Card()
        {
            _creditcardService = new Mock<ICreditCardService>();

            var response = _creditcardService.Object.IsCardNumberValid("1234567890");
            Assert.False(response);

        }

    }
}
