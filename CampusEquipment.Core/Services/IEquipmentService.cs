using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Services
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDto>> GetAllAsync();

        Task<EquipmentDto?> GetByIdAsync(int id);

        Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto);

        Task<bool> UpdateAsync(int id, UpdateEquipmentDto dto);

        Task<bool> DeleteAsync(int id);
    }
}