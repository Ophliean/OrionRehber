using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrionRehber.Data;
using OrionRehber.Models;
using OrionRehber.Models.ViewModels;
using System;
using System.Linq;

namespace OrionRehber.Controllers
{
    [Authorize]
    public class RehberController : Controller
    {
        private readonly AppDbContext _context;

        public RehberController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var name = User.Identity?.Name ?? string.Empty;

            var user = _context.Kullanici
                .FirstOrDefault(x =>
                       x.KullaniciAdi == name
                    || x.Isim == name
                    || x.Eposta == name);

            if (user != null)
                return user.Id;

            return 1;
        }

       
        // TELEFON: normalize + validate + format
       
        private string NormalizeTelefon(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var digits = new string(input.Where(char.IsDigit).ToArray());

            if (digits.StartsWith("90") && digits.Length >= 12)
                digits = digits.Substring(2);

            if (digits.StartsWith("0") && digits.Length >= 11)
                digits = digits.Substring(1);

            return digits;
        }

        private bool IsValidTrGsm(string normalized10)
        {
            return !string.IsNullOrWhiteSpace(normalized10)
                   && normalized10.Length == 10
                   && normalized10.StartsWith("5");
        }

        private string FormatTelefon(string inputOrNormalized)
        {
            var t = NormalizeTelefon(inputOrNormalized);
            if (!IsValidTrGsm(t)) return inputOrNormalized ?? "";
            return $"+90({t.Substring(0, 3)}){t.Substring(3, 3)}-{t.Substring(6, 2)}-{t.Substring(8, 2)}";
        }

        // LISTE
      
        public IActionResult Index(int page = 1)
        {
            var currentUserId = GetCurrentUserId();
            const int pageSize = 5;

            var query = _context.Rehber
                .Where(x => !x.IsDeleted && x.KaydedenKullaniciId == currentUserId)
                .OrderBy(x => x.Ad);

            var totalCount = query.Count();

            var kisiler = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new RehberListVm
            {
                Kisiler = kisiler,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(vm);
        }

        
        // TEK KAYIT GETİR
    
        [HttpGet]
        public IActionResult Get(int id)
        {
            var currentUserId = GetCurrentUserId();

            var kisi = _context.Rehber
                .FirstOrDefault(x => x.Id == id && x.KaydedenKullaniciId == currentUserId);

            if (kisi == null)
                return NotFound();

            return Json(new
            {
                kisi.Id,
                kisi.Ad,
                kisi.Soyad,
                Telefon = FormatTelefon(kisi.Telefon),
                TelefonRaw = kisi.Telefon
            });
        }

        // YENİ KAYIT
       
        [HttpPost]
        public IActionResult Add([FromForm] RehberVm model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Form hatalı" });

            var normalized = NormalizeTelefon(model.Telefon);
            if (!IsValidTrGsm(normalized))
                return Json(new { success = false, message = "Telefon formatı geçersiz. Örn: 05xx xxx xx xx" });

            var currentUserId = GetCurrentUserId();

            var entity = new Rehber
            {
                Ad = model.Ad?.Trim(),
                Soyad = model.Soyad?.Trim(),
                Telefon = normalized,
                KayitTarihi = DateTime.Now,
                IsDeleted = false,
                KaydedenKullaniciId = currentUserId
            };

            _context.Rehber.Add(entity);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Kayıt başarıyla eklendi",
                data = new
                {
                    id = entity.Id,
                    ad = entity.Ad,
                    soyad = entity.Soyad,
                    telefon = entity.Telefon
                }
            });
        }

     
        // GÜNCELLE
       
        [HttpPost]
        public IActionResult Edit([FromForm] RehberVm model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Form hatalı" });

            var currentUserId = GetCurrentUserId();

            var kisi = _context.Rehber
                .FirstOrDefault(x => x.Id == model.Id && x.KaydedenKullaniciId == currentUserId);

            if (kisi == null)
                return Json(new { success = false, message = "Kayıt bulunamadı" });

            var normalized = NormalizeTelefon(model.Telefon);
            if (!IsValidTrGsm(normalized))
                return Json(new { success = false, message = "Telefon formatı geçersiz. Örn: 05xx xxx xx xx" });

            kisi.Ad = model.Ad?.Trim();
            kisi.Soyad = model.Soyad?.Trim();
            kisi.Telefon = normalized;

            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Kayıt başarıyla güncellendi",
                data = new
                {
                    id = kisi.Id,
                    ad = kisi.Ad,
                    soyad = kisi.Soyad,
                    telefon = kisi.Telefon
                }
            });
        }

        
        // SİLME
        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var currentUserId = GetCurrentUserId();

            var kisi = _context.Rehber
                .FirstOrDefault(x => x.Id == id && x.KaydedenKullaniciId == currentUserId);

            if (kisi == null)
                return Json(new { success = false, message = "Kayıt bulunamadı" });

            kisi.IsDeleted = true;
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Kayıt başarıyla silindi",
                data = new { id = id }
            });
        }
    }
}
