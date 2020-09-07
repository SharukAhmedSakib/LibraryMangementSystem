using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Interfaces;
using Library.Models;
using Library.ViewModels.Patrons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private readonly IPatron patron;
        private readonly ILibraryBranch libraryBranch;

        public PatronController(IPatron Patron, ILibraryBranch libraryBranch)
        {
            patron = Patron;
            this.libraryBranch = libraryBranch;
        }

        public IActionResult Index()
        {
            var allPatrons = patron.GetAll();
            var patronModels = allPatrons.Select(p => new PatronDetailModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                LibraryCardId = p.LibraryCard.Id,
                OverdueFees = p.LibraryCard.Fees,
                HomeLibrary = p.HomeLibraryBranch.Name
            }).ToList();

            var model = new PatronIndexModel()
            {
                Patrons = patronModels
            };
            return View(model);

        }

        public IActionResult Detail(int id)
        {
            var patronModel = patron.Get(id);

            var model = new PatronDetailModel
            {
                Id = patronModel.Id,
                LastName = patronModel.LastName ?? "No Last Name Provided",
                FirstName = patronModel.FirstName ?? "No First Name Provided",
                Address = patronModel.Address ?? "No Address Provided",
                HomeLibrary = patronModel.HomeLibraryBranch?.Name ?? "No Home Library",
                MemberSince = patronModel.LibraryCard?.Created,
                OverdueFees = patronModel.LibraryCard?.Fees,
                LibraryCardId = patronModel.LibraryCard?.Id,
                Telephone = string.IsNullOrEmpty(patronModel.TelephoneNumber) ? "No Telephone Number Provided" : patronModel.TelephoneNumber,
                AssetsCheckedOut = patron.GetCheckouts(id).ToList(),
                CheckoutHistory = patron.GetCheckoutHistory(id),
                Holds = patron.GetHolds(id)
            };

            return View(model);
        }

        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //    return View();
        //}


        //[HttpPost]
        //public IActionResult Edit(int id)
        //{
        //    return View();
        //}

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var removed = patron.RemovePatron(id);

            if (removed)
            {
                ViewBag.msg = "Success";
                return RedirectToAction("Index");
            }
            ViewBag.msg = "Error";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult RegisterPatron(int id)
        {
            List<LibraryBranch> libraryList = new List<LibraryBranch>();

            libraryList = libraryBranch.GetAll().ToList();


            ViewBag.listOfBranches = libraryList;

            var model = patron.Get(id);
            var newModel = new PatronRegisterModel
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                Address = model.Address,
                TelephoneNumber = model.TelephoneNumber,
                HomeLibraryBranchId = model.HomeLibraryBranchId,
                Fees = model.LibraryCard.Fees,
                DateOfBirth = model.DateOfBirth
            };
            return PartialView("_CreatePatron", newModel);


        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult RegisterPatron(PatronRegisterModel model)
        {
            var newPatron = patron.CreatePatron(model);
            if (newPatron && model.Id !=0)
            {
                ViewBag.msg = "Success";
                return RedirectToAction("Index");
            }

            if (newPatron && model.Id == 0)
            {
                ViewBag.msg = "Success";
                return RedirectToAction("Register", "Account");
            }
            ViewBag.msg = "Error";
            return RedirectToAction("Register", "Account");
        }
    }
}
