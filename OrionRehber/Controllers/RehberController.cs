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

       
        // LİSTE – sadece kendi kayıtları
       

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


        // ===========================
        // TEK KAYIT GETİR (Edit için) – sadece kullanıcının kendi kaydı
        // ===========================
        [HttpGet]
        public IActionResult Get(int id)
        {
            var currentUserId = GetCurrentUserId();

            var kisi = _context.Rehber
                .FirstOrDefault(x => x.Id == id && x.KaydedenKullaniciId == currentUserId);

            if (kisi == null)
                return NotFound();

            return Json(kisi);
        }

        // ===========================
        // YENİ KAYIT (AJAX)
        // ===========================
        [HttpPost]
        public IActionResult Add([FromForm] RehberVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Form hatalı"
                });
            }

            var currentUserId = GetCurrentUserId();

            var entity = new Rehber
            {
                Ad = model.Ad,
                Soyad = model.Soyad,
                Telefon = model.Telefon,
                KayitTarihi = DateTime.Now,
                IsDeleted = false,
                KaydedenKullaniciId = currentUserId   // sabit 1 değil, gerçek kullanıcı
            };

            _context.Rehber.Add(entity);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Kayıt başarıyla eklendi"
            });
        }

        // ===========================
        // GÜNCELLE (AJAX) – sadece kendi kaydı
        // ===========================
        [HttpPost]
        public IActionResult Edit([FromForm] RehberVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Form hatalı"
                });
            }

            var currentUserId = GetCurrentUserId();

            var kisi = _context.Rehber
                .FirstOrDefault(x => x.Id == model.Id && x.KaydedenKullaniciId == currentUserId);

            if (kisi == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kayıt bulunamadı"
                });
            }

            kisi.Ad = model.Ad;
            kisi.Soyad = model.Soyad;
            kisi.Telefon = model.Telefon;

            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Kayıt başarıyla güncellendi"
            });
        }

        // 
        // SİLME (AJAX) – sadece kendi kaydı
        // ===========================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var currentUserId = GetCurrentUserId();

            var kisi = _context.Rehber
                .FirstOrDefault(x => x.Id == id && x.KaydedenKullaniciId == currentUserId);

            if (kisi == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kayıt bulunamadı"
                });
            }

            kisi.IsDeleted = true; 
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Kayıt başarıyla silindi"
            });
        }
    }
}
