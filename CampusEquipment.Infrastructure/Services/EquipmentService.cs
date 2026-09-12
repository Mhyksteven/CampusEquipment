using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;
using Microsoft.Extensions.Logging;

namespace CampusEquipment.Infrastructure.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<EquipmentService> _logger;

        public EquipmentService(
            IEquipmentRepository equipmentRepository,
            IDepartmentRepository departmentRepository,
            ILogger<EquipmentService> logger)
        {
            _equipmentRepository = equipmentRepository;
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        // =========================================================
        // GET ALL
        // =========================================================
        public async Task<IEnumerable<EquipmentDto>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all equipment.");

            var equipment = await _equipmentRepository.GetAllAsync();

            _logger.LogInformation(
                "Equipment records retrieved successfully.");

            return equipment;
        }

        // =========================================================
        // SEARCH / FILTER
        // =========================================================
        public async Task<IEnumerable<EquipmentDto>> GetFilteredAsync(
            string? search,
            string? category,
            string? status,
            int? departmentId)
        {
            _logger.LogInformation(
                "Filtering equipment. Search={Search}, Category={Category}, Status={Status}, DepartmentId={DepartmentId}",
                search,
                category,
                status,
                departmentId);

            var equipment = await _equipmentRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                equipment = equipment.Where(e =>
                    (!string.IsNullOrWhiteSpace(e.AssetCode) &&
                     e.AssetCode.Contains(
                         search,
                         StringComparison.OrdinalIgnoreCase))
                    ||
                    (!string.IsNullOrWhiteSpace(e.Name) &&
                     e.Name.Contains(
                         search,
                         StringComparison.OrdinalIgnoreCase))
                    ||
                    (!string.IsNullOrWhiteSpace(e.Brand) &&
                     e.Brand.Contains(
                         search,
                         StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                equipment = equipment.Where(e =>
                    string.Equals(
                        e.Category,
                        category,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                equipment = equipment.Where(e =>
                    string.Equals(
                        e.Status,
                        status,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (departmentId.HasValue)
            {
                equipment = equipment.Where(e =>
                    e.DepartmentId == departmentId.Value);
            }

            return equipment.ToList();
        }

        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<EquipmentDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation(
                "Retrieving Equipment ID {EquipmentId}.",
                id);

            var equipment =
                await _equipmentRepository.GetByIdAsync(id);

            if (equipment == null)
            {
                _logger.LogWarning(
                    "Equipment ID {EquipmentId} was not found.",
                    id);

                return null;
            }

            return equipment;
        }

        // =========================================================
        // CREATE
        // =========================================================
        public async Task<EquipmentDto> CreateAsync(
            CreateEquipmentDto dto)
        {
            _logger.LogInformation(
                "Creating equipment with Asset Code {AssetCode}.",
                dto.AssetCode);

            // Required field checks
            if (string.IsNullOrWhiteSpace(dto.AssetCode))
            {
                _logger.LogWarning(
                    "Create equipment failed because Asset Code is required.");

                throw new InvalidOperationException(
                    "Asset Code is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning(
                    "Create equipment failed because Name is required.");

                throw new InvalidOperationException(
                    "Equipment Name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Category))
            {
                _logger.LogWarning(
                    "Create equipment failed because Category is required.");

                throw new InvalidOperationException(
                    "Category is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                _logger.LogWarning(
                    "Create equipment failed because Status is required.");

                throw new InvalidOperationException(
                    "Status is required.");
            }

            // Duplicate Asset Code check
            var allEquipment =
                await _equipmentRepository.GetAllAsync();

            var duplicate = allEquipment.Any(e =>
                string.Equals(
                    e.AssetCode,
                    dto.AssetCode.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (duplicate)
            {
                _logger.LogWarning(
                    "Create failed because Asset Code {AssetCode} already exists.",
                    dto.AssetCode);

                throw new InvalidOperationException(
                    "An equipment with this Asset Code already exists.");
            }

            // Department validation
            var department =
                await _departmentRepository.GetByIdAsync(
                    dto.DepartmentId);

            if (department == null)
            {
                _logger.LogWarning(
                    "Create failed because Department ID {DepartmentId} does not exist.",
                    dto.DepartmentId);

                throw new InvalidOperationException(
                    "The selected department does not exist.");
            }

            // Clean text values
            dto.AssetCode = dto.AssetCode.Trim();
            dto.Name = dto.Name.Trim();
            dto.Category = dto.Category.Trim();

            if (dto.Brand != null)
            {
                dto.Brand = dto.Brand.Trim();
            }

            if (dto.Model != null)
            {
                dto.Model = dto.Model.Trim();
            }

            dto.Status = dto.Status.Trim();

            // Repository accepts CreateEquipmentDto directly
            var created =
                await _equipmentRepository.AddAsync(dto);

            _logger.LogInformation(
                "Equipment {AssetCode} created successfully.",
                dto.AssetCode);

            return created;
        }

        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateEquipmentDto dto)
        {
            _logger.LogInformation(
                "Updating Equipment ID {EquipmentId}.",
                id);

            var existing =
                await _equipmentRepository.GetByIdAsync(id);

            if (existing == null)
            {
                _logger.LogWarning(
                    "Update failed because Equipment ID {EquipmentId} was not found.",
                    id);

                return false;
            }

            // Required field checks
            if (string.IsNullOrWhiteSpace(dto.AssetCode))
            {
                _logger.LogWarning(
                    "Update failed because Asset Code is required.");

                throw new InvalidOperationException(
                    "Asset Code is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning(
                    "Update failed because Name is required.");

                throw new InvalidOperationException(
                    "Equipment Name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Category))
            {
                _logger.LogWarning(
                    "Update failed because Category is required.");

                throw new InvalidOperationException(
                    "Category is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                _logger.LogWarning(
                    "Update failed because Status is required.");

                throw new InvalidOperationException(
                    "Status is required.");
            }

            // Duplicate Asset Code check
            var allEquipment =
                await _equipmentRepository.GetAllAsync();

            var duplicate = allEquipment.Any(e =>
                e.EquipmentId != id &&
                string.Equals(
                    e.AssetCode,
                    dto.AssetCode.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (duplicate)
            {
                _logger.LogWarning(
                    "Update failed because Asset Code {AssetCode} already exists.",
                    dto.AssetCode);

                throw new InvalidOperationException(
                    "An equipment with this Asset Code already exists.");
            }

            // Department validation
            var department =
                await _departmentRepository.GetByIdAsync(
                    dto.DepartmentId);

            if (department == null)
            {
                _logger.LogWarning(
                    "Update failed because Department ID {DepartmentId} does not exist.",
                    dto.DepartmentId);

                throw new InvalidOperationException(
                    "The selected department does not exist.");
            }

            // =====================================================
            // BUSINESS RULE:
            // Retired equipment cannot become Assigned
            // =====================================================
            if (string.Equals(
                    existing.Status,
                    "Retired",
                    StringComparison.OrdinalIgnoreCase)
                &&
                string.Equals(
                    dto.Status,
                    "Assigned",
                    StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Assignment rejected because Equipment ID {EquipmentId} is retired.",
                    id);

                throw new InvalidOperationException(
                    "Retired equipment cannot be assigned.");
            }

            // =====================================================
            // BUSINESS RULE:
            // Under-maintenance equipment cannot become Assigned
            // =====================================================
            if (string.Equals(
                    existing.Status,
                    "UnderMaintenance",
                    StringComparison.OrdinalIgnoreCase)
                &&
                string.Equals(
                    dto.Status,
                    "Assigned",
                    StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Assignment rejected because Equipment ID {EquipmentId} is under maintenance.",
                    id);

                throw new InvalidOperationException(
                    "Equipment under maintenance cannot be assigned.");
            }

            // Clean text values
            dto.AssetCode = dto.AssetCode.Trim();
            dto.Name = dto.Name.Trim();
            dto.Category = dto.Category.Trim();

            if (dto.Brand != null)
            {
                dto.Brand = dto.Brand.Trim();
            }

            if (dto.Model != null)
            {
                dto.Model = dto.Model.Trim();
            }

            dto.Status = dto.Status.Trim();

            // Repository signature:
            // UpdateAsync(int id, UpdateEquipmentDto dto)
            await _equipmentRepository.UpdateAsync(
                id,
                dto);

            _logger.LogInformation(
                "Equipment ID {EquipmentId} updated successfully.",
                id);

            return true;
        }

        // =========================================================
        // DELETE / RETIRE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation(
                "Retirement requested for Equipment ID {EquipmentId}.",
                id);

            var existing =
                await _equipmentRepository.GetByIdAsync(id);

            if (existing == null)
            {
                _logger.LogWarning(
                    "Retirement failed because Equipment ID {EquipmentId} was not found.",
                    id);

                return false;
            }

            // If already retired, consider operation successful
            if (string.Equals(
                existing.Status,
                "Retired",
                StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation(
                    "Equipment ID {EquipmentId} is already retired.",
                    id);

                return true;
            }

            var updateDto = new UpdateEquipmentDto
            {
                AssetCode = existing.AssetCode,
                Name = existing.Name,
                Category = existing.Category,
                Brand = existing.Brand,
                Model = existing.Model,
                PurchaseDate = existing.PurchaseDate,
                Status = "Retired",
                DepartmentId = existing.DepartmentId
            };

            await _equipmentRepository.UpdateAsync(
                id,
                updateDto);

            _logger.LogInformation(
                "Equipment ID {EquipmentId} retired successfully.",
                id);

            return true;
        }
    }
}