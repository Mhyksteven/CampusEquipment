using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Services
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDto>> GetAllAsync();

        Task<IEnumerable<EquipmentDto>> GetFilteredAsync(
            string? search,
            string? category,
            string? status,
            int? departmentId);

        Task<EquipmentDto?> GetByIdAsync(int id);

        Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto);

        Task<bool> UpdateAsync(int id, UpdateEquipmentDto dto);

        Task<bool> DeleteAsync(int id);
    }
}