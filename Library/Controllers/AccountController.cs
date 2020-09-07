using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Interfaces;
using Library.Models;
using Library.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        private readonly ILibraryBranch libraryBranch;

        public AccountController(UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signInManager, ILibraryBranch libraryBranch)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            this.libraryBranch = libraryBranch;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {

            //Patron Library dropdown Info

            List<LibraryBranch> libraryList = new List<LibraryBranch>();

            libraryList = libraryBranch.GetAll().ToList();
            ViewBag.listOfBranches = libraryList;

            //end of library info

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            IdentityUser user = new IdentityUser()
            {
                UserName = model.UserName,
                Email = model.Email

            };

            var result = await userManager.CreateAsync(user, model.Password);

            var resultRole = await userManager.AddToRoleAsync(user, "User");

            string confiemationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            string confirmationLink = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, token = confiemationToken }, protocol: HttpContext.Request.Scheme);
            //EmailService.Send(user.Email, "Confirm Your Email", "Click Here To Confirm your Email Address" + confirmationLink);
            System.IO.File.WriteAllText(@"C:\temp\ConfirmEmail.txt", confirmationLink);
            if (result.Succeeded)
            {
                ViewBag.msg = "Success";
                return View();
            }
            ViewBag.msg = "Error";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.Msg = "Email Confirmation Succeeded!";
            }
            else
            {
                ViewBag.Msg = "Email Confirmation Failed!";
            }
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVIewModel model)
        {
            if (ModelState.IsValid)
            {
                var Result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
                if (Result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");

                    ////Code for users Confirmed by login
                    //var user = await userManager.FindByNameAsync(model.UserName);
                    //if (user.EmailConfirmed)
                    //{
                    //    return RedirectToAction("Index", "Home");
                    //}
                    //else
                    //{
                    //    await signInManager.SignOutAsync();
                    //}

                }
            }
           
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.UserEmail);
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            string resetLink = Url.Action("ResetPassword", "Account", new { userid = user.Id, token = resetToken }, protocol: HttpContext.Request.Scheme);
            //EmailService.Send(user.Email, "Password Recovery", "Click here to Recover your Password" + resetLink);
            System.IO.File.WriteAllText(@"C:\temp\ForgotPassword.txt", resetLink);
            //var callbackUrl = .ResetPasswordCallbackLink(user.Id,resetLink, Request.Scheme);
            //SendEmail
            //This comments are not mentioned in the video, wrote it just in case.
            ViewBag.Msg = "Reset Password link Has been Emailed";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            var obj = new ResetPasswordViewModel() { UserId = userId, Token = token };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                ViewBag.Msg = "Password Reset Succeded!";
            }
            else
            {
                ViewBag.Msg = "Password Reset Failed!";
            }
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
