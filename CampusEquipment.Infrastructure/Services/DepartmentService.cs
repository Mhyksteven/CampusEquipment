using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;

namespace CampusEquipment.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task<DepartmentDto> CreateAsync(DepartmentDto dto)
        {
            return await _departmentRepository.AddAsync(dto);
        }

        public async Task<bool> UpdateAsync(int id, DepartmentDto dto)
        {
            return await _departmentRepository.UpdateAsync(id, dto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _departmentRepository.DeleteAsync(id);
        }
    }
}