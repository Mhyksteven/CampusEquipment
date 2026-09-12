using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Infrastructure.Data;
using CampusEquipment.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusEquipment.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Description = d.Description
                })
                .ToListAsync();
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Where(d => d.DepartmentId == id)
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Description = d.Description
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DepartmentDto> AddAsync(DepartmentDto dto)
        {
            var department = new Department
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(department.DepartmentId))!;
        }

        public async Task<bool> UpdateAsync(int id, DepartmentDto dto)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return false;
            }

            department.Name = dto.Name;
            department.Description = dto.Description;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return false;
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentId == id);
        }
    }
}