using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Interfaces;
using Library.ViewModels.Patrons;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private readonly IPatron patron;

        public PatronController(IPatron Patron)
        {
            patron = Patron;
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
    }
}
