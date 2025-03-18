using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantopia.Models;

public class PlantModel
{

    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public int Price { get; set; }

    [Required]
    public string? Description { get; set; }

    public int? PotSize { get; set; }

    public int? PlantHight { get; set; }

    public int? Amount { get; set; }

    public string? ImageName { get; set; }

    [NotMapped]
    [Display(Name = "Upload Image")]
    public IFormFile? ImageFile { get; set; }
}