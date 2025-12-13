using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;          
using OrionRehber.Data;
using OrionRehber.Models;
using System.Security.Claims;

namespace OrionRehber.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        
        //    LOGIN SAYFASI
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Rehber");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string user, string password, bool rememberMe)
        {
            //Kullanıcıyı kullanıcı adı veya e-posta ile bul
            var kullanici = _context.Kullanici
                .FirstOrDefault(x =>
                    x.KullaniciAdi == user || x.Eposta == user);

            if (kullanici == null)
            {
                ViewBag.Hata = "Kullanıcı adı veya şifre hatalı.";
                return View();
            }

            // Hashli şifreyi doğrula
            var hasher = new PasswordHasher<Kullanici>();
            var result = hasher.VerifyHashedPassword(kullanici, kullanici.Sifre, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Hata = "Kullanıcı adı veya şifre hatalı.";
                return View();
            }

            // 3) Yetkilendirme Bilgileri (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, kullanici.Isim),               // ekranda gözüken isim
                new Claim(ClaimTypes.Email, kullanici.Eposta),
                new Claim("UserId", kullanici.Id.ToString())              // kendi Id'n
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = DateTime.UtcNow.AddDays(5)
                }
            );

            return RedirectToAction("Index", "Rehber");
        } 

        //    ÇIKIŞ
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

     
        //    REGISTER
        
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Rehber");

            return View();
        }

        [HttpPost]
        public IActionResult Register(Kullanici model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Hata = "Form hatalı.";
                return View(model);
            }

            // kullanıcı kontrolü
            bool exists = _context.Kullanici
                .Any(x => x.KullaniciAdi == model.KullaniciAdi || x.Eposta == model.Eposta);

            if (exists)
            {
                ViewBag.Hata = "Bu kullanıcı adı veya e-posta zaten kullanılıyor.";
                return View(model);
            }

            model.KayitTarihi = DateTime.Now;

            // Şifreyi hashle
            var hasher = new PasswordHasher<Kullanici>();
            string hashedPassword = hasher.HashPassword(model, model.Sifre);
            model.Sifre = hashedPassword;

            // Veritabanına kaydet
            _context.Kullanici.Add(model);
            _context.SaveChanges();

            ViewBag.Basarili = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }
    }
}
