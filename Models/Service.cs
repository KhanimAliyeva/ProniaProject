using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Service : BaseEntity
{


    public string Title { get; set; }


    public string Description { get; set; }


    public string IconUrl { get; set; }
}
