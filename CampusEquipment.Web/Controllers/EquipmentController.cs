using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CampusEquipment.Web.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IDepartmentService _departmentService;

        public EquipmentController(
            IEquipmentService equipmentService,
            IDepartmentService departmentService)
        {
            _equipmentService = equipmentService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index(
            string? search,
            string? category,
            string? status,
            int? departmentId)
        {
            var equipment =
                await _equipmentService.GetFilteredAsync(
                    search,
                    category,
                    status,
                    departmentId);

            var allEquipment =
                await _equipmentService.GetAllAsync();

            var departments =
                await _departmentService.GetAllAsync();

            var categories = allEquipment
                .Select(e => e.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            var statuses = new[]
            {
                "Available",
                "Assigned",
                "UnderMaintenance",
                "Retired"
            };

            ViewBag.Search = search;

            ViewBag.Categories =
                new SelectList(categories, category);

            ViewBag.Statuses =
                new SelectList(statuses, status);

            ViewBag.Departments =
                new SelectList(
                    departments,
                    "DepartmentId",
                    "Name",
                    departmentId);

            return View(equipment);
        }

        public async Task<IActionResult> Details(int id)
        {
            var equipment =
                await _equipmentService.GetByIdAsync(id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();

            return View(new CreateEquipmentDto
            {
                Status = "Available"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateEquipmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(dto.DepartmentId);

                return View(dto);
            }

            try
            {
                await _equipmentService.CreateAsync(dto);

                TempData["SuccessMessage"] =
                    "Equipment created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                await LoadDepartmentsAsync(dto.DepartmentId);

                return View(dto);
            }
        }

        // PART 27 - Edit form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var equipment =
                await _equipmentService.GetByIdAsync(id);

            if (equipment == null)
            {
                return NotFound();
            }

            var dto = new UpdateEquipmentDto
            {
                AssetCode = equipment.AssetCode,
                Name = equipment.Name,
                Category = equipment.Category,
                Brand = equipment.Brand,
                Model = equipment.Model,
                PurchaseDate = equipment.PurchaseDate,
                Status = equipment.Status,
                DepartmentId = equipment.DepartmentId
            };

            ViewBag.EquipmentId = id;

            await LoadDepartmentsAsync(
                equipment.DepartmentId);

            return View(dto);
        }

        // PART 27 - Save changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateEquipmentDto dto)
        {
            ViewBag.EquipmentId = id;

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(dto.DepartmentId);

                return View(dto);
            }

            try
            {
                var updated =
                    await _equipmentService.UpdateAsync(
                        id,
                        dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] =
                    "Equipment updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                await LoadDepartmentsAsync(dto.DepartmentId);

                return View(dto);
            }
        }

        private async Task LoadDepartmentsAsync(
            int? selectedDepartmentId = null)
        {
            var departments =
                await _departmentService.GetAllAsync();

            ViewBag.Departments =
                new SelectList(
                    departments,
                    "DepartmentId",
                    "Name",
                    selectedDepartmentId);
        }
    }
}