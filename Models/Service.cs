using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Service : BaseEntity
{


    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    [MaxLength(256)]
    public string Description { get; set; }

    [Required]
    [MaxLength(256)]
    public string IconUrl { get; set; }
}
