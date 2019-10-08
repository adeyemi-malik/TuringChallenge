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
    ///     Everything about Shipping
    /// </summary>
    [Route("shipping")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;
        private readonly IMapper _mapper;
        private readonly ILogger<ShippingController> _logger;


        public ShippingController(IShippingService shippingService, IMapper mapper, ILogger<ShippingController> logger)
        {
            _shippingService = shippingService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        ///     Get Shipping
        /// </summary>
        /// <returns>Return a list of shipping.</returns>
        /// <response code="200">An array of object Shipping</response>
        /// <response code="400">Return a error object</response>
        [HttpGet]
        [ProducesResponseType(typeof(Shipping), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var shipping = await _shippingService.GetShippingAsync();
                return Ok(shipping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving shipping list");
                var error = new Error(400, "SHP_03", "error retrieving shipping list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Shipping by ID
        /// </summary>
        /// <param name="shipping_id"></param>
        /// <returns>Return a shipping by ID.</returns>
        /// <response code="200">An array of object Shipping</response>
        /// <response code="400">Return a error object</response>
        /// <response code="404">Return an error for not found</response>
        [HttpGet("{shipping_id:int}")]
        [ProducesResponseType(typeof(Shipping), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> Get(int shipping_id)
        {
            try
            {
                var shipping = await _shippingService.GetShippingByIdAsync(shipping_id);
                if (shipping != null)
                {
                    return Ok(shipping);
                }

                return NotFound(new Error(404, "SHP_02", " shipping with this ID does not exist", "shipping_id"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving shipping", shipping_id);
                var error = new Error(400, "SHP_03", "error retrieving shipping", "shipping_id");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Shipping Regions
        /// </summary>
        /// <returns>Return a list of shipping region.</returns>
        /// <response code="200">An array of object Shipping Shipping</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("regions")]
        [ProducesResponseType(typeof(ShippingRegion), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetRegions()
        {
            try
            {
                var shipping = await _shippingService.GetShippingRegionAsync();
                return Ok(shipping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving shipping region list");
                var error = new Error(400, "SHP_03", "error retrieving shipping region list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Shipping by ID
        /// </summary>
        /// <param name="shipping_id"></param>
        /// <returns>Return a shipping by region ID.</returns>
        /// <response code="200">An array of object Shipping</response>
        /// <response code="400">Return a error object</response>
        /// <response code="404">Return an error for not found</response>
        [HttpGet("regions/{shipping_region_id}")]
        [ProducesResponseType(typeof(Shipping), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> GetRegionShipping(int shipping_region_id)
        {
            try
            {
                var shippingRegion = await _shippingService.GetShippingRegionIdAsync(shipping_region_id);
                if (shippingRegion == null)
                {
                    return NotFound(new Error(404, "SHR_02", " shipping region  with this ID does not exist", "shipping_region_id"));
                }
                var shipping = await _shippingService.GetShippingByRegionIdAsync(shipping_region_id);
                return Ok(shipping);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving shipping list", shipping_region_id);
                var error = new Error(400, "SHP_03", "error retrieving shipping list by region id", "");
                return BadRequest(error);
            }
        }
    }
}