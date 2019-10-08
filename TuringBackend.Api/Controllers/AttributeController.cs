using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TuringBackend.Models.Data;
using TuringBackend.Api.Services;
using TuringBackend.Models;
using Attribute = TuringBackend.Models.Attribute;

namespace TuringBackend.Api.Controllers
{
    /// <summary>
    ///     Everything about Attribute
    /// </summary>
    [Route("attributes")]
    [ApiController]
    public class AttributeController : ControllerBase
    {
        private readonly IAttributeService _attributeService;
        private readonly IProductService _productService;

        private readonly IMapper _mapper;
        private readonly ILogger<AttributeController> _logger;

        public AttributeController(IAttributeService attributeService, IProductService productService, IMapper mapper, ILogger<AttributeController> logger)
        {
            _attributeService = attributeService;
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        ///     Get Attributes
        /// </summary>
        /// <returns>Return a list of attributes.</returns>
        /// <response code="200">An array of object Attribute</response>
        /// <response code="400">Return a error object</response>
        [HttpGet]
        [ProducesResponseType(typeof(Attribute), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var attributes = await _attributeService.GetAttributesAsync();
                return Ok(attributes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving attributes list");
                var error = new Error(400, "ATT_02", "error retrieving attributes list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Attribute by ID
        /// </summary>
        /// <param name="attribute_id"></param>
        /// <returns>Return a attribute by ID.</returns>
        /// <response code="200">An object off Attribute</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("{attribute_id:int}")]
        [ProducesResponseType(typeof(Attribute), 200)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> Get(int attribute_id)
        {
            try
            {
                var attribute = await _attributeService.GetAttributeByIdAsync(attribute_id);

                if (attribute != null)
                {
                    return Ok(attribute);
                }

                return NotFound(new Error(404, "ATT_02", " attribute with this ID does not exist", "attribute_id"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving attribute", attribute_id);
                var error = new Error(400, "ATT_03", "error retrieving attribute", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Values Attribute from Atribute
        /// </summary>
        /// <param name="attribute_id"></param>
        /// <returns>Get Values Attribute from Attribute.</returns>
        /// <response code="200">A list of  AttributeValue</response>
        /// <response code="400">Return a error object</response>
        /// <response code="404">Return an error for not found</response>
        [HttpGet("values/{attribute_id:int}")]
        [ProducesResponseType(typeof(Attribute), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> GetAttributeValueByAttributeId(int attribute_id)
        {
            try
            {
                var attribute = await _attributeService.GetAttributeByIdAsync(attribute_id);

                if (attribute == null)
                {
                    return NotFound(new Error(404, "ATT_02", " attribute with this ID does not exist", "attribute_id"));
                }

                var values = await _attributeService.GetAttributeValueByAttributeIdAsync(attribute_id);
                return Ok(values);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving attribute values", attribute_id);
                var error = new Error(400, "ATT_03", "error retrieving attribute values", "");
                return BadRequest(error);
            }
           
        }


        /// <summary>
        ///     Get all Attributes with Product ID
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns>Get all Attributes with Product ID</returns>
        /// <response code="200">A list of  AttributeValue</response>
        /// <response code="400">Return a error object</response>
        /// <response code="404">Return an error for not found</response>
        [HttpGet("inProduct/{product_id:int}")]
        [ProducesResponseType(typeof(Attribute), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> GetAttributeValueByProductId(int product_id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(product_id);

                if (product == null)
                {
                    return NotFound(new Error(404, "PRD_02", " product with this ID does not exist", "product_id"));
                }

                var values = await _attributeService.GetAttributeValueByProductIdAsync(product_id);
                return Ok(values);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving product attribute values", product_id);
                var error = new Error(400, "ATT_03", "error retrieving product attribute values", "");
                return BadRequest(error);
            }
        }
    }
}