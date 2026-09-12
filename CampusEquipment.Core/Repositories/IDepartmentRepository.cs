using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto?> GetByIdAsync(int id);
        Task<DepartmentDto> AddAsync(DepartmentDto dto);
        Task<bool> UpdateAsync(int id, DepartmentDto dto);
        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}