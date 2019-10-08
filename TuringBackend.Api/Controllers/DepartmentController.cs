using System;
using System.Net;
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
    ///     Everything about Department
    /// </summary>
    [Route("departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDepartmentService departmentService, IMapper mapper, ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        ///     Get Departments
        /// </summary>
        /// <returns>Return a list of departments.</returns>
        /// <response code="200">An array of object Department</response>
        /// <response code="400">Return a error object</response>
        [HttpGet]
        [ProducesResponseType(typeof(Department), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var departments = await _departmentService.GetDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error retrieving department list");
                var error = new Error(400, "DEP_03", "error retrieving department list", "");
                return BadRequest(error);
            }
        }

        /// <summary>
        ///     Get Department by ID
        /// </summary>
        /// <param name="department_id"></param>
        /// <returns>Return a department by ID.</returns>
        /// <response code="200">An  object of Department</response>
        /// <response code="404">Return an error for not found</response>
        /// <response code="400">Return a error object</response>
        [HttpGet("{department_id:int}")]
        [ProducesResponseType(typeof(Department), 200)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 400)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Get(int department_id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(department_id);
                if (department != null)
                {
                    return Ok(department);
                }

                return NotFound(new Error(404, "DEP_02", " department with this ID does not exist", "department_id"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error retrieving department", department_id);
                var error = new Error(400, "DEP_03", "error retrieving department", "");
                return BadRequest(error);
            }
        }
    }
}