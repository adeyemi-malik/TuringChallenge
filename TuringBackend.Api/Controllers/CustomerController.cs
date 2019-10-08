using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TuringBackend.Models.Data;
using TuringBackend.Api.Core;
using TuringBackend.Api.Services;
using TuringBackend.Models;

namespace TuringBackend.Api.Controllers
{
    /// <summary>
    ///     Everything about Customer
    /// </summary>
    [Route("customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICreditCardService _creditCardService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerController> _logger;


        public CustomerController(ICustomerService customerService, 
            IAuthenticationService authenticationService,
            ICreditCardService creditCardService,
            IMapper mapper, ILogger<CustomerController> logger)
        {
            _authenticationService = authenticationService;
            _creditCardService = creditCardService;
            _mapper = mapper;
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        ///     Update a customer
        /// </summary>
        /// <returns>Return a Object of Customer with auth credentials.</returns>
        /// <param name="name">Customer name</param>
        /// <param name="email">Customer email</param>
        /// <param name="password">Customer password</param>
        /// <param name="day_phone">Customer day phone</param>
        /// <param name="eve_phone">Customer eve phone</param>
        /// <param name="mob_phone">Customer mob phone</param>
        /// <response code="200">An array of object Category</response>
        /// <response code="400">Return a error object</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut("~/customer")]
        [Authorize]
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> Update(
            [Required] [FromForm] string name,
            [Required] [FromForm] string email,
            string password,
            string day_phone,
            string eve_phone,
            string mob_phone
        )
        {
            try
            {
                var userEmail = _authenticationService.GetUserId(User);
                var customer = await _customerService.GetCustomerByEmailAsync(email);
                if (customer == null)
                {
                    var error = new Error(400, "USR_01", "Email or Password is invalid", "email,password");
                    return NotFound(error);
                }
                else
                {
                    var updatedCustomer = await _customerService.UpdateCustomerPasswordAsync(userEmail, email,name, password, day_phone, eve_phone, mob_phone);
                    updatedCustomer.CreditCard = _authenticationService.CreditCardMask(updatedCustomer.CreditCard);

                    return Ok(updatedCustomer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error updating customer");
                var error = new Error(400, "USR_10", "error updating customer", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Customer by ID
        /// </summary>
        /// <returns>Return a Object of Customer with auth credentials.</returns>
        /// <response code="200">A customer</response>
        /// <response code="400">Return a error object</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("~/customer")]
        [Authorize]
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Error),404)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var email = _authenticationService.GetUserId(User);
                var customer = await _customerService.GetCustomerByEmailAsync(email);
                if (customer == null)
                {
                    var error = new Error(400, "USR_01", "Email or Password is invalid", "email,password");
                    return NotFound(error);
                }
                else
                {
                    customer.CreditCard = _authenticationService.CreditCardMask(customer.CreditCard);
                    return Ok(customer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error getting customer");
                var error = new Error(400, "USR_10", "error getting customer", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Register a Customer
        /// </summary>
        /// <param name="name">Customer name</param>
        /// <param name="email">Email of User.</param>
        /// <param name="password">Password of User</param>
        /// <response code="201">Return a Object of Customer with auth credentials</response>
        /// <response code="400">Return a error object</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Customer), 201)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> Post(
            [Required] [FromForm] string name,
            [Required] [FromForm] string email,
            [Required] [FromForm] string password
        )
        {
            try
            {
                if (!_customerService.EmailValid(email))
                {
                    var error = new Error(400, "USR_03", "The email is invalid", "email");
                    return BadRequest(error);
                }

                if (await _customerService.CustomerEmailExists(email))
                {
                    var error = new Error(400, "USR_04", "The email already exists.", "email");
                    return BadRequest(error);
                }
                var customer = await _customerService.RegisterCustomerAsync(name, email, password);
                customer.CreditCard = _authenticationService.CreditCardMask(customer.CreditCard);
                var token = await _authenticationService.CreateAccessTokenAsync(customer.Email, password);

                return Created("",new CustomerRegister{AccessToken = token.Token.Token, Customer = customer, ExpiresIn = token.Token.Expiration.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error registering", name, email);
                var error = new Error(400, "USR_10", "error registering customer", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Sign in in the Shopping.
        /// </summary>
        /// <param name="email">Email of User</param>
        /// <param name="password">Password of User</param>
        /// <response code="200">Return a Object of Customer with auth credentials</response>
        /// <response code="400">Return a error object</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CustomerRegister), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> Login(
            [Required] [FromForm] string email,
            [Required] [FromForm] string password
        )
        {
            try
            {
                if (!_customerService.EmailValid(email))
                {
                    var error = new Error(400, "USR_03", "The email is invalid", "email");
                    return BadRequest(error);
                }

                if (!await _customerService.CustomerEmailExists(email))
                {
                    var error = new Error(400, "USR_05", "The email doesn't exist", "email");
                    return BadRequest(error);
                }

                var tokenResponse = await _authenticationService.CreateAccessTokenAsync(email, password);
                if (tokenResponse.Success)
                {
                    var customer = await _customerService.GetCustomerByEmailAsync(email);
                    customer.CreditCard = _authenticationService.CreditCardMask(customer.CreditCard);
                    return Ok(new CustomerRegister { AccessToken = tokenResponse.Token.Token, Customer = customer, ExpiresIn = tokenResponse.Token.Expiration.ToString() });
                }
                else
                {
                    var error = new Error(400, "USR_01", "Email or Password is invalid", "email,password");
                    return BadRequest(error);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error login", email);
                var error = new Error(400, "USR_10", "error login customer", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Sign in with a facebook login token.
        /// </summary>
        /// <returns>Token generated from your facebook login</returns>
        /// <response code="200">Return a Object of Customer with auth credentials</response>
        /// <response code="400">Return a error object</response>
        [HttpPost("facebook")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> FaceBookLogin(
            [Required] [FromForm] string access_token
        )
        {
            await Task.CompletedTask;
            return Ok();
        }

        /// <response code="200">Return a Object of Customer with auth credentials</response>
        /// <response code="400">Return a error object</response>
        [HttpPut("address")]
        [Authorize]
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> UpdateAddress(
            [Required] [FromForm] string address_1,
                       [FromForm] string address_2,
            [Required] [FromForm] string city,
            [Required] [FromForm] string region,
            [Required] [FromForm] string postal_code,
            [Required] [FromForm] string country,
            [Required] [FromForm] int shipping_region_id
        )
        {
            try
            {
                var email = _authenticationService.GetUserId(User);
                var customer = await _customerService.GetCustomerByEmailAsync(email);
                if (customer == null)
                {
                    var error = new Error(400, "USR_01", "Email or Password is invalid", "email,password");
                    return NotFound(error);
                }
                else
                {
                    var updatedCustomer = await _customerService.UpdateCustomerAsync(email, address_1, address_2, city,
                        region, postal_code, country, shipping_region_id);
                    updatedCustomer.CreditCard = _authenticationService.CreditCardMask(updatedCustomer.CreditCard);
                    return Ok(updatedCustomer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error updating customer");
                var error = new Error(400, "USR_10", "error updating customer", "");
                return BadRequest(error);
            }
        }

        /// <response code="200">Return a Object of Customer with auth credentials</response>
        /// <response code="400">Return a error object</response>
        [HttpPut("creditCard")]
        [Authorize]
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> UpdateCreditCard(
            [Required] [FromForm] string credit_card
        )
        {
            try
            {
                if (!_creditCardService.IsCardNumberValid(credit_card))
                {
                    var error = new Error(400, "USR_10", "invalid credit card", "credit_card");
                    return BadRequest(error);
                }
                var email = _authenticationService.GetUserId(User);
                var customer = await _customerService.GetCustomerByEmailAsync(email);
                if (customer == null)
                {
                    var error = new Error(400, "USR_01", "Email or Password is invalid", "email,password");
                    return NotFound(error);
                }
                else
                {
                    var updatedCustomer = await _customerService.UpdateCustomerCreditCardAsync(email, credit_card);
                    updatedCustomer.CreditCard = _authenticationService.CreditCardMask(credit_card);
                    return Ok(updatedCustomer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error updating customer credit card");
                var error = new Error(400, "USR_10", "error updating customer credit card", "");
                return BadRequest(error);
            }
        }

    }
}