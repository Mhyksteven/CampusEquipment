using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Repositories
{
    public interface IEquipmentRepository
    {
        Task<IEnumerable<EquipmentDto>> GetAllAsync();
        Task<EquipmentDto?> GetByIdAsync(int id);
        Task<EquipmentDto> AddAsync(CreateEquipmentDto dto);
        Task<bool> UpdateAsync(int id, UpdateEquipmentDto dto);
        Task<bool> DeleteAsync(int id);

        Task<bool> AssetCodeExistsAsync(
            string assetCode,
            int? excludeEquipmentId = null);
    }
}