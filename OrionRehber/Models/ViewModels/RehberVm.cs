namespace OrionRehber.Models.ViewModels
{
    public class RehberVm
    {
        public int? Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Telefon { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int? KaydedenKullaniciId { get; set; }
    }
}
