using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountManagerController : Controller
    {
        UserManager<IdentityUser> userManager;
        RoleManager<IdentityRole> roleManager;
        public AccountManagerController(UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AssignRole()
        {
            var users = userManager.Users.ToList();
            var roles = roleManager.Roles.ToList();
            List<string> listOfRoles = new List<string>();
            foreach (var item in roles)
            {
                listOfRoles.Add(item.Name);
            }

            List<AssignRoleViewModel> list = new List<AssignRoleViewModel>();
            foreach (var item in users)
            {
                AssignRoleViewModel model = new AssignRoleViewModel();
                model.UserId = item.Id;
                model.UserName = item.UserName;
                model.Email = item.Email;
                model.RoleName = userManager.GetRolesAsync(item).Result.Count != 0 ? userManager.GetRolesAsync(item).Result[0] : "";
                model.Roles = listOfRoles;
                list.Add(model);
            }

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string UserId, string RoleName)
        {
            if (ModelState.IsValid)
            {

                //var user = await userManager.FindByIdAsync(UserId);
                //both are correct
                var user = userManager.FindByIdAsync(UserId).Result;



                //var roles = roleManager.Roles.ToList();
                //List<string> listOfRoles = new List<string>();
                //foreach (var item in roles)
                //{
                //    listOfRoles.Add(item.Name);
                //}
                //    var Results = await userManager.RemoveFromRolesAsync(user, listOfRoles);



                var roles = await userManager.GetRolesAsync(user);
                var result1 = await userManager.RemoveFromRolesAsync(user, roles);


                var Result = await userManager.AddToRoleAsync(user, RoleName);
                return RedirectToAction("AssignRole");
            }
            return View();
        }
    }
}
