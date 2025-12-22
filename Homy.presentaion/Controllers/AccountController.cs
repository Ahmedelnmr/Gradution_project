using Homy.Application.Dtos.AdminDtos;
using Homy.Domin.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Homy.presentaion.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginDto loginDto, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user != null)
                {
                    // Check if user is active
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "هذا الحساب غير مفعل، يرجى التواصل مع الإدارة");
                        return View(loginDto);
                    }

                    // Check if user is deleted
                    if (user.IsDeleted)
                    {
                        ModelState.AddModelError(string.Empty, "هذا الحساب محذوف");
                        return View(loginDto);
                    }

                    // Verify password and sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);

                    if (result.Succeeded)
                    {
                        // Check roles to ensure it's an admin or authorized personnel if needed
                        // For now we assume any valid login can access, or we can restrict in program.cs policies
                        
                         if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }
                
                ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            return View(loginDto);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
             var user = await _userManager.GetUserAsync(User);
             if(user == null) return NotFound();

             var model = new AdminProfileDto
             {
                 Id = user.Id,
                 FullName = user.FullName,
                 Email = user.Email!,
                 PhoneNumber = user.PhoneNumber,
                 ProfileImageUrl = user.ProfileImageUrl
             };
             return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(AdminProfileDto model)
        {
            if(!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
             if(user == null) return NotFound();

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            if(model.ProfileImage != null)
            {
                // Handle Image Upload
                 string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                 if(!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                 string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                 string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                 using (var fileStream = new FileStream(filePath, FileMode.Create))
                 {
                     await model.ProfileImage.CopyToAsync(fileStream);
                 }

                 user.ProfileImageUrl = "/uploads/profiles/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if(result.Succeeded)
            {
               TempData["SuccessMessage"] = "تم تحديث الملف الشخصي بنجاح";
               model.ProfileImageUrl = user.ProfileImageUrl; // Update view model
               model.Email = user.Email!; // Restore email
               return View(model);
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}
