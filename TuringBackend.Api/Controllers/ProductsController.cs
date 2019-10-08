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
    ///     Everything about Product
    /// </summary>
    [Route("products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IDepartmentService _departmentService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductController> _logger;


        public ProductController(IProductService productService, ICategoryService categoryService, IDepartmentService departmentService, IAuthenticationService authenticationService, IMapper mapper, ILogger<ProductController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _departmentService = departmentService;
            _authenticationService = authenticationService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        ///     Get Products
        /// </summary>
        /// <returns>Return a list of products.</returns>
        /// <response code="200">An array of object Product</response>
        /// <response code="400">Return a error object</response>
        [HttpGet]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Get(string order, int page = 1, int limit = 20)
        {
            try
            {
                var products = await _productService.GetProductsASync(page,limit,order);
                var totalPages = (products.TotalCount + limit - 1) / limit;
                var pagingMetaData = new PaginationMeta(page, limit, totalPages, products.TotalCount);
                var response = new {pagingMetaData, rows = products.Rows };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving products list");
                var error = new Error(400, "PRD_03", "error retrieving products list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Product by ID
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns>Return a product by ID.</returns>
        /// <response code="200">An  object of Product</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("{product_id:int}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> Get(int product_id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(product_id);
                if (product != null)
                {
                    return Ok(product);
                }

                return NotFound(new Error(404, "PRD_02", " product with this ID does not exist", "product_id"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving product", product_id);
                var error = new Error(400, "PRD_03", "error retrieving product", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Search Products
        /// </summary>
        /// <param name="query_string"></param>
        /// <param name="all_words"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="description_length"></param>
        /// <returns>Search Products.</returns>
        /// <response code="200">An  object of Product</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> Search(string query_string, string all_words = "on", int page = 1,
            int limit = 20, int description_length = 200)
        {
            try
            {
                var products = await _productService.GetProductsASync(query_string, all_words,page, limit, description_length);
                var totalPages = (products.TotalCount + limit - 1) / limit;
                var pagingMetaData = new PaginationMeta(page, limit, totalPages, products.TotalCount);
                var response = new { pagingMetaData, rows = products.Rows };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving products list");
                var error = new Error(400, "PRD_03", "error retrieving products list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Search Products of Categories
        /// </summary>
        /// <param name="category_id"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="description_length"></param>
        /// <returns>Search Products of Categories.</returns>
        /// <response code="200">An  object of Product</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("inCategory/{category_id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> ProductsByCategory(int category_id, int page = 1, int limit = 20,
            int description_length = 200)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIAsync(category_id);
                if (category == null)
                {
                    return NotFound(new Error(404, "CAT_01", " category with this ID does not exist", "category_id"));
                }
                var products = await _productService.GetProductsByCategoryAsync(category_id, page, limit, description_length);
                var totalPages = (products.TotalCount + limit - 1) / limit;
                var pagingMetaData = new PaginationMeta(page, limit, totalPages, products.TotalCount);
                var response = new { pagingMetaData, rows = products.Rows };
                return Ok(response);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving product list by category id", category_id);
                var error = new Error(400, "PRD_03", "error retrieving product list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Search Products of Categories
        /// </summary>
        /// <param name="department_id"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="description_length"></param>
        /// <returns>Search Products on Department.</returns>
        /// <response code="200">An  object of Product</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("inDepartment/{department_id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> ProductsByDepartment(int department_id, int page = 1, int limit = 20,
            int description_length = 200)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(department_id);
                if (department == null)
                {
                    return NotFound(new Error(404, "DEP_02", " department with this ID does not exist", "department_id"));
                }
                var products = await _productService.GetProductsByDepartmentAsync(department_id, page, limit, description_length);
                var totalPages = (products.TotalCount + limit - 1) / limit;
                var pagingMetaData = new PaginationMeta(page, limit, totalPages, products.TotalCount);
                var response = new { pagingMetaData, rows = products.Rows };
                return Ok(response);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving product list by department id", department_id);
                var error = new Error(400, "PRD_03", "error retrieving product list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get details of a Product
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns>Get details of a Product.</returns>
        /// <response code="200">An  object of Product</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("{product_id:int}/details")]
        [ProducesResponseType(typeof(ProductDetail), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> GetProductDetails(int product_id)
        {
            try
            {
                var product = await _productService.GetProductDetailsAsync(product_id);
                if (product != null)
                {
                    return Ok(product);
                }

                return NotFound(new Error(404, "PRD_02", " product with this ID does not exist", "product_id"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving product", product_id);
                var error = new Error(400, "PRD_03", "error retrieving product", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get locations of a
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns>Get locations of a Product.</returns>
        /// <response code="200">An  array of Product</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("{product_id:int}/locations")]
        [ProducesResponseType(typeof(ProductLocations), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> GetProductLocations(int product_id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(product_id);
                if (product == null)
                {
                    return NotFound(new Error(404, "PRD_02", " product with this ID does not exist", "product_id"));
                }
                var productLocations = await _productService.GetProductLocationsAsync(product_id);
                return Ok(productLocations);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving product location with id", product_id);
                var error = new Error(400, "PRD_03", "error retrieving product location", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get reviews of a Product
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns>Get reviews of a Product.</returns>
        /// <response code="200">An  array of Product</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("{product_id:int}/reviews")]
        [ProducesResponseType(typeof(CustomerReview), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> GetProductReviews(int product_id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(product_id);
                if (product == null)
                {
                    return NotFound(new Error(404, "PRD_02", " product with this ID does not exist", "product_id"));
                }
                var reviews = await _productService.GetProductReviewsAsync(product_id);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving product reviews with id", product_id);
                var error = new Error(400, "PRD_03", "error retrieving product reviews", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     save reviews of a Product
        /// </summary>
        /// <param name="product_id">ProductID</param>
        /// <param name="review">Review Text of the Product</param>
        /// <param name="rating">Rating of Product</param>
        /// <returns>Get reviews of a Product.</returns>
        /// <response code="200">No data.</response>
        /// <response code="400">Return a error object</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Return an error for not found</response>
        [HttpPost("{product_id:int}/reviews")]
        [Authorize]
        [ProducesResponseType(typeof(CustomerReview), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> PostProductReviews(
            int product_id,
            [FromForm] [Required] string review,
            [FromForm] [Required] short rating)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(product_id);
                if (product == null)
                {
                    return NotFound(new Error(404, "PRD_02", " product with this ID does not exist", "product_id"));
                }

                var userEmail = _authenticationService.GetUserId(User);
                var response = await _productService.PostProductReviewAsync(userEmail,product_id, review, rating);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error posting  product reviews with id", product_id);
                var error = new Error(400, "PRD_03", "error posting product reviews", "");
                return BadRequest(error);
            }
        }
    }


}