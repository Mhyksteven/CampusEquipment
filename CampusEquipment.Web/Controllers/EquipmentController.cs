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

        // PART 22 + PART 23
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
                new SelectList(
                    categories,
                    category);

            ViewBag.Statuses =
                new SelectList(
                    statuses,
                    status);

            ViewBag.Departments =
                new SelectList(
                    departments,
                    "DepartmentId",
                    "Name",
                    departmentId);

            return View(equipment);
        }

        // PART 24
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();

            var dto = new CreateEquipmentDto
            {
                Status = "Available"
            };

            return View(dto);
        }

        // PART 24 + PART 25
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

        // PART 26
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