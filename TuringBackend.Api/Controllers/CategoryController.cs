using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TuringBackend.Api.Core;
using TuringBackend.Api.Services;
using TuringBackend.Models;

namespace TuringBackend.Api.Controllers
{
    [Route("categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, IDepartmentService departmentService, IProductService productService, IMapper mapper, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _departmentService = departmentService;
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        ///     Get Categories
        /// </summary>
        /// <returns>Return a list of categories.</returns>
        /// <response code="200">An array of object Category</response>
        /// <response code="400">Return a error object</response>
        [HttpGet]
        [ProducesResponseType(typeof(Category), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Get(string order, int page = 1, int limit = 20)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync(page,limit,order);
                var totalPages = (categories.TotalCount + limit - 1) / limit;
                var pagingMetaData = new PaginationMeta(page, limit, totalPages, categories.TotalCount);
                var response = new { pagingMetaData, rows = categories.Rows };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving categories list");
                var error = new Error(400, "CAT_02", "error retrieving categories list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Category by ID
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns>Return a category by ID.</returns>
        /// <response code="200">An  object of Category</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("{category_id:int}")]
        [ProducesResponseType(typeof(Category), 200)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Get(int category_id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIAsync(category_id);
                if (category != null)
                {
                    return Ok(category);
                }

                return NotFound(new Error(404, "CAT_01 ", " category with this ID does not exist", "category_id"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving category" , category_id);
                var error = new Error(400, "CAT_02", "error retrieving category", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Department Categories by ID
        /// </summary>
        /// <param name="department_id"></param>
        /// <returns>Returns list category .</returns>
        /// <response code="200">An array of Category</response>
        /// <response code="400">Return a error object</response>
        /// <response code="404">Return an error for not found</response>
        [HttpGet("inDepartment/{department_id:int}")]
        [ProducesResponseType(typeof(Category), 200)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetDepartmentCategory(int department_id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(department_id);
                if (department == null)
                {
                    return NotFound(new Error(404, "DEP_02", " department with this ID does not exist", "department_id"));
                }
                var categories = await _categoryService.GetCategoriesByDepartmentIdAsync(department_id);
                return Ok(categories);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving category list", department_id);
                var error = new Error(400, "CAT_02", "error retrieving department category", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Product Categories by ID
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns>Returns list category .</returns>
        /// <response code="200">An object of Category</response>
        /// <response code="400">Return a error object</response>
        /// <response code="404">Return an error for not found</response>
        [HttpGet("inProduct/{product_id:int}")]
        [ProducesResponseType(typeof(Category), 200)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetProductCategory(int product_id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(product_id);
                if (product == null)
                {
                    return NotFound(new Error(404, "PRD_02", " product with this ID does not exist", "product_id"));
                }

                var categories = await _categoryService.GetCategoriesByProductIdAsync(product_id);
                if (categories != null && categories.Any())
                {
                    return Ok(categories.First());
                }
                return NotFound(new Error(404, "CAT_01 ", " product with this ID does not have any category", "product_id"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving category list", product_id);
                var error = new Error(400, "CAT_02", "error retrieving product category", "");
                return BadRequest(error);
            }
        }
    }
}