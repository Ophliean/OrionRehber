using System.ComponentModel.DataAnnotations;

public class RehberVm
{
    public int? Id { get; set; }

    [Required]
    public string Ad { get; set; }

    [Required]
    public string Soyad { get; set; }

    [Required(ErrorMessage = "Telefon zorunludur")]
    [RegularExpression(@"^[0-9\+\-\s\(\)]+$", ErrorMessage = "Telefon formatı geçersiz")]
    public string Telefon { get; set; }

    public bool IsDeleted { get; set; }
    public int? KaydedenKullaniciId { get; set; }
}
