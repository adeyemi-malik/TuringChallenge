using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TuringBackend.Models.Data;
using TuringBackend.Api.Core;
using TuringBackend.Api.Services;
using TuringBackend.Models;

namespace TuringBackend.Api.Controllers
{
    /// <summary>
    ///     Everything about Order
    /// </summary>
    [Authorize]
    [Route("orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerService _customerService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, 
            IAuthenticationService authenticationService, 
            ICustomerService customerService,
            IEmailService emailService, 
            IMapper mapper, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _authenticationService = authenticationService;
            _customerService = customerService;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        ///    save Order
        /// </summary>
        /// <param name="cart_id"></param>
        /// <param name="shipping_id"></param>
        /// <param name="tax_id"></param>
        /// <returns>Return a order ID.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 401)]
        public async Task<IActionResult> Post(
            [Required] [FromForm] string cart_id,
            [Required] [FromForm] int shipping_id,
            [Required] [FromForm] int tax_id
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

                var order = await _orderService.SaveOrderAsync(customer.CustomerId, cart_id, shipping_id, tax_id);
                return Created("", new {order_id = order});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error saving order");
                var error = new Error(400, "ORD_03", "error saving order", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Info about Order
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns>Return a order by ID.</returns>
        [HttpGet("{order_id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> Get(int order_id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(order_id);
                if (order != null)
                {
                    return Ok(new {order_id, order_items = order });
                }

                return NotFound(new Error(404, "ORD_02", " order with this ID does not exist", "order_id"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving order", order_id);
                var error = new Error(400, "ORD_03", "error retrieving orer", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get orders by Customer
        /// </summary>
        /// <returns>Return a list of order.</returns>
        /// <response code="200">An array of object Order</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("inCustomer")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> GetOrdersByCustomer()
        {
            try
            {
                var id = await GetCustomerIdAsync();
                var customerOrders = await _orderService.GetOrderByCustomerIdAsync(id);
                return Ok(customerOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving customer orders list");
                var error = new Error(400, "ORD_03", "error retrieving orders list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Info about Order
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns>Return a order by ID.</returns>
        [HttpGet("/orders/shortDetail/{order_id:int}")]
        [ProducesResponseType(typeof(OrderDetail), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]

        public async Task<IActionResult> GetOrderDetail(int order_id)
        {
            try
            {
                var order = await _orderService.GetOrderDetailByIdAsync(order_id);
                if (order != null)
                {
                    return Ok(order);
                }

                return NotFound(new Error(404, "ORD_02", " order with this ID does not exist", "order_id"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving order", order_id);
                var error = new Error(400, "ORD_03", "error retrieving orer", "");
                return BadRequest(error);
            }
        }

        private async Task<int> GetCustomerIdAsync()
        {
            var email = _authenticationService.GetUserId(User);
            var customer =  await _customerService.GetCustomerByEmailAsync(email);
            return customer.CustomerId;
        }
    }
}