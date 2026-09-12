using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusEquipment.API.Controllers
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(
            IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments =
                await _departmentService.GetAllAsync();

            return Ok(
                new ApiResponse<IEnumerable<DepartmentDto>>
                {
                    Success = true,
                    Message =
                        "Departments retrieved successfully.",
                    Data = departments
                });
        }
    }
}