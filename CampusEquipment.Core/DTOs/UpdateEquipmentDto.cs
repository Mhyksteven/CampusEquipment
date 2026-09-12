using System.ComponentModel.DataAnnotations;

namespace CampusEquipment.Core.DTOs
{
    public class UpdateEquipmentDto
    {
        [Required(ErrorMessage = "Asset Code is required.")]
        [StringLength(50)]
        public string AssetCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Equipment Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Brand { get; set; }

        [StringLength(100)]
        public string? Model { get; set; }

        public DateOnly? PurchaseDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }
    }
}