using System.ComponentModel.DataAnnotations;

namespace PharmacyApi.Models;

public class Drug
{
    public int Id {get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Number is required")]
    public int Number { get; set; }

    [Required(ErrorMessage = "Stock is required")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public float Price { get; set; }
}