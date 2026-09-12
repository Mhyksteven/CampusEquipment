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
    }
}