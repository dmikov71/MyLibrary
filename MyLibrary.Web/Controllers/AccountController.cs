
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyLibrary.Infrastructure.Models;
using MyLibrary.Web.Models;

namespace MyLibrary.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {           
                // Проверка: если пароль требует сброса переходим на форму смены пароля
                if (user is ApplicationUser appUser && appUser.IsPasswordResetRequired)
                {
                    return RedirectToAction(nameof(ChangePasswordFirstLogin), "Account");
                }

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Слишком много неудачных попыток. Попробуйте позже");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangePasswordFirstLogin()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordFirstLogin(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || user is not ApplicationUser appUser)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден");
                return View(model);
            }

            // Проверяем текущий пароль (временный из secret.json)
            var verifyResult = await _signInManager.CheckPasswordSignInAsync(user, model.CurrentPassword, lockoutOnFailure: false);
            if (!verifyResult.Succeeded)
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Неверный текущий пароль");
                return View(model);
            }

            var changeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changeResult.Succeeded)
            {
                foreach (var error in changeResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Снимаем флаг — первый вход завершён
            appUser.IsPasswordResetRequired = false;
            await _userManager.UpdateAsync(appUser);

            TempData["SuccessMessage"] = "Пароль успешно изменён. Теперь вы можете пользоваться системой.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home"); 
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

