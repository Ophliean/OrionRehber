using System;
namespace OrionRehber.Models { 
    public class Rehber {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; } 
        public string Telefon { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool IsDeleted { get; set; }
        public int KaydedenKullaniciId { get; set; } } }