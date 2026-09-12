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

        public async Task<IEnumerable<EquipmentDto>> GetFilteredAsync(
            string? search,
            string? category,
            string? status,
            int? departmentId)
        {
            var equipment =
                await _equipmentRepository.GetAllAsync();

            var query = equipment.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.AssetCode.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) ||

                    e.Name.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) ||

                    (e.Brand != null &&
                     e.Brand.Contains(
                         search,
                         StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e =>
                    string.Equals(
                        e.Category,
                        category,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e =>
                    string.Equals(
                        e.Status,
                        status,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(
                    e => e.DepartmentId == departmentId.Value);
            }

            return query.ToList();
        }

        public async Task<EquipmentDto?> GetByIdAsync(int id)
        {
            return await _equipmentRepository.GetByIdAsync(id);
        }

        public async Task<EquipmentDto> CreateAsync(
            CreateEquipmentDto dto)
        {
            ValidateRequiredFields(
                dto.AssetCode,
                dto.Name,
                dto.Category,
                dto.Status,
                dto.DepartmentId);

            var assetCodeExists =
                await _equipmentRepository.AssetCodeExistsAsync(
                    dto.AssetCode);

            if (assetCodeExists)
            {
                throw new InvalidOperationException(
                    "An equipment with this Asset Code already exists.");
            }

            var departmentExists =
                await _departmentRepository.ExistsAsync(
                    dto.DepartmentId);

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
            var existingEquipment =
                await _equipmentRepository.GetByIdAsync(id);

            if (existingEquipment == null)
            {
                return false;
            }

            ValidateRequiredFields(
                dto.AssetCode,
                dto.Name,
                dto.Category,
                dto.Status,
                dto.DepartmentId);

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
                await _departmentRepository.ExistsAsync(
                    dto.DepartmentId);

            if (!departmentExists)
            {
                throw new InvalidOperationException(
                    "The selected department does not exist.");
            }

            if (string.Equals(
                    dto.Status,
                    "Assigned",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(
                        existingEquipment.Status,
                        "Retired",
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Retired equipment cannot be assigned.");
                }

                if (string.Equals(
                        existingEquipment.Status,
                        "UnderMaintenance",
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Equipment under maintenance cannot be assigned.");
                }
            }

            return await _equipmentRepository.UpdateAsync(
                id,
                dto);
        }

        // PART 28
        // Soft delete: change status to Retired
        // instead of removing the database record.
        public async Task<bool> DeleteAsync(int id)
        {
            var equipment =
                await _equipmentRepository.GetByIdAsync(id);

            if (equipment == null)
            {
                return false;
            }

            // Already retired, no need to change it again.
            if (string.Equals(
                    equipment.Status,
                    "Retired",
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var dto = new UpdateEquipmentDto
            {
                AssetCode = equipment.AssetCode,
                Name = equipment.Name,
                Category = equipment.Category,
                Brand = equipment.Brand,
                Model = equipment.Model,
                PurchaseDate = equipment.PurchaseDate,
                Status = "Retired",
                DepartmentId = equipment.DepartmentId
            };

            return await _equipmentRepository.UpdateAsync(
                id,
                dto);
        }

        private static void ValidateRequiredFields(
            string assetCode,
            string name,
            string category,
            string status,
            int departmentId)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                throw new InvalidOperationException(
                    "Asset Code is required.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException(
                    "Equipment Name is required.");
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                throw new InvalidOperationException(
                    "Category is required.");
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new InvalidOperationException(
                    "Status is required.");
            }

            if (departmentId <= 0)
            {
                throw new InvalidOperationException(
                    "A valid Department is required.");
            }
        }
    }
}