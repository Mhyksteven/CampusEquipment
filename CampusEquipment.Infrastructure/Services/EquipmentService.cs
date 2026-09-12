using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;

namespace CampusEquipment.Infrastructure.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public EquipmentService(
            IEquipmentRepository equipmentRepository,
            IDepartmentRepository departmentRepository)
        {
            _equipmentRepository = equipmentRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllAsync()
        {
            return await _equipmentRepository.GetAllAsync();
        }

        public async Task<EquipmentDto?> GetByIdAsync(int id)
        {
            return await _equipmentRepository.GetByIdAsync(id);
        }

        public async Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AssetCode))
            {
                throw new InvalidOperationException(
                    "Asset Code is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new InvalidOperationException(
                    "Equipment Name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Category))
            {
                throw new InvalidOperationException(
                    "Category is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                throw new InvalidOperationException(
                    "Status is required.");
            }

            if (dto.DepartmentId <= 0)
            {
                throw new InvalidOperationException(
                    "A valid Department is required.");
            }

            var assetCodeExists =
                await _equipmentRepository.AssetCodeExistsAsync(dto.AssetCode);

            if (assetCodeExists)
            {
                throw new InvalidOperationException(
                    "An equipment with this Asset Code already exists.");
            }

            var departmentExists =
                await _departmentRepository.ExistsAsync(dto.DepartmentId);

            if (!departmentExists)
            {
                throw new InvalidOperationException(
                    "The selected department does not exist.");
            }

            return await _equipmentRepository.AddAsync(dto);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateEquipmentDto dto)
        {
            var equipment = await _equipmentRepository.GetByIdAsync(id);

            if (equipment == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.AssetCode))
            {
                throw new InvalidOperationException(
                    "Asset Code is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new InvalidOperationException(
                    "Equipment Name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Category))
            {
                throw new InvalidOperationException(
                    "Category is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                throw new InvalidOperationException(
                    "Status is required.");
            }

            if (dto.DepartmentId <= 0)
            {
                throw new InvalidOperationException(
                    "A valid Department is required.");
            }

            var assetCodeExists =
                await _equipmentRepository.AssetCodeExistsAsync(
                    dto.AssetCode,
                    id);

            if (assetCodeExists)
            {
                throw new InvalidOperationException(
                    "An equipment with this Asset Code already exists.");
            }

            var departmentExists =
                await _departmentRepository.ExistsAsync(dto.DepartmentId);

            if (!departmentExists)
            {
                throw new InvalidOperationException(
                    "The selected department does not exist.");
            }

            return await _equipmentRepository.UpdateAsync(id, dto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var equipment = await _equipmentRepository.GetByIdAsync(id);

            if (equipment == null)
            {
                return false;
            }

            return await _equipmentRepository.DeleteAsync(id);
        }
    }
}