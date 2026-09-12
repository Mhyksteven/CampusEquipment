using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusEquipment.API.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(
            IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var equipment =
                await _equipmentService.GetAllAsync();

            return Ok(
                new ApiResponse<IEnumerable<EquipmentDto>>
                {
                    Success = true,
                    Message =
                        "Equipment retrieved successfully.",
                    Data = equipment
                });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var equipment =
                await _equipmentService.GetByIdAsync(id);

            if (equipment == null)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Equipment not found.",
                        Data = null
                    });
            }

            return Ok(
                new ApiResponse<EquipmentDto>
                {
                    Success = true,
                    Message =
                        "Equipment retrieved successfully.",
                    Data = equipment
                });
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateEquipmentDto dto)
        {
            try
            {
                var equipment =
                    await _equipmentService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = equipment.EquipmentId },
                    new ApiResponse<EquipmentDto>
                    {
                        Success = true,
                        Message =
                            "Equipment created successfully.",
                        Data = equipment
                    });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains(
                    "already exists",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(
                        new ApiResponse<object>
                        {
                            Success = false,
                            Message = ex.Message,
                            Data = null
                        });
                }

                return BadRequest(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateEquipmentDto dto)
        {
            try
            {
                var updated =
                    await _equipmentService.UpdateAsync(
                        id,
                        dto);

                if (!updated)
                {
                    return NotFound(
                        new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Equipment not found.",
                            Data = null
                        });
                }

                var equipment =
                    await _equipmentService.GetByIdAsync(id);

                return Ok(
                    new ApiResponse<EquipmentDto?>
                    {
                        Success = true,
                        Message =
                            "Equipment updated successfully.",
                        Data = equipment
                    });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains(
                    "already exists",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(
                        new ApiResponse<object>
                        {
                            Success = false,
                            Message = ex.Message,
                            Data = null
                        });
                }

                return BadRequest(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var retired =
                await _equipmentService.DeleteAsync(id);

            if (!retired)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Equipment not found.",
                        Data = null
                    });
            }

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Equipment retired successfully.",
                    Data = null
                });
        }
    }
}