namespace CampusEquipment.Core.DTOs
{
    public class UpdateEquipmentDto
    {
        public string AssetCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public DateOnly? PurchaseDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
    }
}