using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TuringBackend.Models.Data;
using TuringBackend.Api.Services;
using TuringBackend.Models;

namespace TuringBackend.Api.Controllers
{
    /// <summary>
    ///     Everything about Tax
    /// </summary>
    [Route("tax")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly ITaxService _taxService;
        private readonly ILogger<TaxController> _logger;

        public TaxController(ITaxService taxService,
            IMapper mapper, ILogger<TaxController> logger)
        {
            _taxService = taxService;
            _logger = logger;
        }

        /// <summary>
        ///     Get Tax
        /// </summary>
        /// <returns>Return a list of tax.</returns>
        /// <response code="200">An array of object Tax</response>
        /// <response code="400">Return a error object</response>
        [HttpGet]
        [ProducesResponseType(typeof(Tax), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var taxes = await _taxService.GetTaxAsync();
                return Ok(taxes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving tax list");
                var error = new Error(400, "TAX_02", "error retrieving tax list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Tax by ID
        /// </summary>
        /// <param name="tax_id"></param>
        /// <returns>Return a tax by ID.</returns>
        /// <response code="200">An array of object Tax</response>
        /// <response code="400">Return a error object</response>
        /// <response code="404">Return an error for not found</response>
        [HttpGet("{tax_id:int}")]
        [ProducesResponseType(typeof(Tax), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> Get(int tax_id)
        {
            try
            {
                var tax = await _taxService.GetTaxByIdAsync(tax_id);
                if (tax != null)
                {
                    return Ok(tax);
                }

                return NotFound(new Error(404, "DEP_02", " department with this ID does not exist", "tax_id"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving tax", tax_id);
                var error = new Error(400, "DEP_03", "error retrieving tax", "tax_id");
                return BadRequest(error);
            }
        }
    }
}