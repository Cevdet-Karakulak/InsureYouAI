using InsureYouAI.Dtos;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRegisterDto createUserRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return View(createUserRegisterDto);
            }

            if (!createUserRegisterDto.TermsAccepted)
            {
                ModelState.AddModelError("TermsAccepted", "Kullanım şartlarını kabul etmelisiniz.");
                return View(createUserRegisterDto);
            }

            AppUser appUser = new AppUser()
            {
                Name = createUserRegisterDto.Name,
                Email = createUserRegisterDto.Email,
                Surname = createUserRegisterDto.Surname,
                UserName = createUserRegisterDto.UserName,
                ImageUrl = "Test",
                Description = "Açıklama"
            };

            var result = await _userManager.CreateAsync(appUser, createUserRegisterDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(createUserRegisterDto);
            }

            return RedirectToAction("UserList"); // Başarılı kayıt sonrası yönlendirme
        }
    }
}
