using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Infrastructure.Data;
using CampusEquipment.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusEquipment.Infrastructure.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly AppDbContext _context;

        public EquipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllAsync()
        {
            return await _context.Equipment
                .Include(e => e.Department)
                .Select(e => new EquipmentDto
                {
                    EquipmentId = e.EquipmentId,
                    AssetCode = e.AssetCode,
                    Name = e.Name,
                    Category = e.Category,
                    Brand = e.Brand,
                    Model = e.Model,
                    PurchaseDate = e.PurchaseDate,
                    Status = e.Status,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department.Name
                })
                .ToListAsync();
        }

        public async Task<EquipmentDto?> GetByIdAsync(int id)
        {
            return await _context.Equipment
                .Include(e => e.Department)
                .Where(e => e.EquipmentId == id)
                .Select(e => new EquipmentDto
                {
                    EquipmentId = e.EquipmentId,
                    AssetCode = e.AssetCode,
                    Name = e.Name,
                    Category = e.Category,
                    Brand = e.Brand,
                    Model = e.Model,
                    PurchaseDate = e.PurchaseDate,
                    Status = e.Status,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EquipmentDto> AddAsync(CreateEquipmentDto dto)
        {
            var equipment = new Equipment
            {
                AssetCode = dto.AssetCode,
                Name = dto.Name,
                Category = dto.Category,
                Brand = dto.Brand,
                Model = dto.Model,
                PurchaseDate = dto.PurchaseDate,
                Status = dto.Status,
                DepartmentId = dto.DepartmentId
            };

            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(equipment.EquipmentId))!;
        }

        public async Task<bool> UpdateAsync(int id, UpdateEquipmentDto dto)
        {
            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.EquipmentId == id);

            if (equipment == null)
            {
                return false;
            }

            equipment.AssetCode = dto.AssetCode;
            equipment.Name = dto.Name;
            equipment.Category = dto.Category;
            equipment.Brand = dto.Brand;
            equipment.Model = dto.Model;
            equipment.PurchaseDate = dto.PurchaseDate;
            equipment.Status = dto.Status;
            equipment.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.EquipmentId == id);

            if (equipment == null)
            {
                return false;
            }

            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssetCodeExistsAsync(
            string assetCode,
            int? excludeEquipmentId = null)
        {
            var query = _context.Equipment
                .Where(e => e.AssetCode == assetCode);

            if (excludeEquipmentId.HasValue)
            {
                query = query.Where(
                    e => e.EquipmentId != excludeEquipmentId.Value);
            }

            return await query.AnyAsync();
        }
    }
}