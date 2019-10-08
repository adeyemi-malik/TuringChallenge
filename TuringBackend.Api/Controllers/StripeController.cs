using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using TuringBackend.Models.Data;
using Microsoft.AspNetCore.Http;
using TuringBackend.Api.Core;
using TuringBackend.Api.Services;
using TuringBackend.Models;
using Order = TuringBackend.Models.Order;

namespace TuringBackend.Api.Controllers
{
    /// <summary>
    ///     Everything about Stripe
    /// </summary>
    [Route("stripe")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailService _emailService;
        private readonly ChargeService _chargeService;
        private readonly IOptions<StripeSettings> _stripeSettings;
        private readonly IOptions<AppOptions> _appOptions;
        private readonly ILogger<StripeController> _logger;

        public StripeController(
            IOptions<StripeSettings> stripeSettings, 
            IOptions<AppOptions> appOptions, 
            ILogger<StripeController> logger,
            IOrderService orderService, 
            IAuthenticationService authenticationService,
            IEmailService emailService, 
            IMapper mapper)
        {
            _orderService = orderService;
            _authenticationService = authenticationService;
            _emailService = emailService;
            _stripeSettings = stripeSettings;
            _appOptions = appOptions;
            _logger = logger;
            _chargeService = new ChargeService(_stripeSettings.Value.SecretKey);

            // From https://stripe.com/docs/charges
            StripeConfiguration.SetApiKey(_stripeSettings.Value.SecretKey);
        }

        /// <summary>
        ///     Get Stripe
        /// </summary>
        /// <param name="stripeToken">
        ///     The API token, you can use this example to get it:
        ///     https://stripe.com/docs/stripe-js/elements/quickstart
        /// </param>
        /// <param name="order_id">The order ID recorded before (Check the Order Documentation)</param>
        /// <param name="description">Description to order.</param>
        /// <param name="amount">Only numbers like: 999</param>
        /// <param name="currency">Default value : USD</param>
        /// <returns>Return a list of stripe.</returns>
        /// <response code="200">Object from Stripe</response>
        /// <response code="400">Return a error object</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> Charge(
            [Required] [FromForm] string stripeToken,
            [Required] [FromForm] int order_id,
            [Required] [FromForm] string description,
            [Required] [FromForm] int amount,
            string currency = "usd"
        )
        {
            try
            {
                var email = _authenticationService.GetUserId(User);
                Dictionary<string, string> Metadata = new Dictionary<string, string>
                {
                    {"order_id", order_id.ToString()}
                };
                var options = new ChargeCreateOptions
                {
                    Amount = amount,
                    Currency = currency,
                    Description = description,
                    SourceId = stripeToken,
                    ReceiptEmail = email,
                    Metadata = Metadata, 
                    CustomerId =  email
                };
                var charge = _chargeService.Create(options);
                return Ok(new {charge, message = "charge successfully created"});
            }
            catch(Exception ex)
            {

                _logger.LogError(ex, "error processing stripe charge");
                var error = new Error(400, "STP03", "error processing stripe charge", "");
                return BadRequest(error);
            }
           
        }

        /// <summary>
        ///     Endpoint that provide a synchronization
        /// </summary>
        /// <response code="200">This endpoint is used by Stripe</response>
        [HttpPost("webhooks")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> WebHooks()
        {
            _logger.LogInformation("WebHook Hit:");
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _stripeSettings.Value.WebhooksKey, throwOnApiVersionMismatch: true);
                var charge = (Charge)stripeEvent.Data.Object;
                switch (charge.Status)
                {
                    case "succeeded":
                        break;
                    case "failed":
                        break;
                }
            }
            catch (Exception e)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}