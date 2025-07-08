using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Razor_Final.models;

public class AppUser : IdentityUser
{
    [Column(TypeName = "nvarchar")]
    [StringLength(255)]
    public string? HomeAdress { get; set; }


    [DataType(DataType.DateTime)]
    public DateTime? BirthDate { get; set; }
}