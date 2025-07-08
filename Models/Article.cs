using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASP.NET_Razor_Final.models;

public class Article
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage ="{0} phải nhập!")]
    [Column(TypeName = "nvarchar")]
    [StringLength(100, MinimumLength =5, ErrorMessage =("{0} phải dài từ {2} đến {1}"))]
    [DisplayName("Tiêu đề")]
    public string Title { get; set; }

    [Required(ErrorMessage ="{0} phải nhập!")]
    [DataType(DataType.DateTime)]
    [DisplayName("Ngày tạo")]
    public DateTime Created { get; set; }

    [Column(TypeName = "ntext")]
    [DisplayName("Nội dung")]
    public string? Content { get; set; }
}