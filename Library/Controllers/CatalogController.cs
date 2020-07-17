using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Interfaces;
using Library.Services;
using Library.ViewModels.Catalog;
using Library.ViewModels.CheckoutModels;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAsset libraryAsset;
        private readonly ICheckout checkout;

        public CatalogController(ILibraryAsset libraryAsset, ICheckout checkout)
        {
            this.libraryAsset = libraryAsset;
            this.checkout = checkout;
        }
        public IActionResult Index()
        {
            var assetModels = libraryAsset.GetAll();

            var listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.Id,
                    ImageUrl = result.ImageUrl,
                    Title = result.Title,
                    Type = libraryAsset.GetType(result.Id),
                    AuthorOrDirector = libraryAsset.GetAuthorOrDirector(result.Id),
                    DeweyCallNumber = libraryAsset.GetDeweyIndex(result.Id)
                });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var asset = libraryAsset.GetById(id);

            var currentHolds = checkout.GetCurrentHolds(id)
                .Select(a => new AssetHoldModel
                {
                    HoldPlaced = checkout.GetCurrentHoldPlaced(a.Id).ToString("d"),
                    PatronName = checkout.GetCurrentHoldPatronName(a.Id)
                });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = libraryAsset.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = libraryAsset.GetAuthorOrDirector(id),
                CurrentLocation = libraryAsset.GetCurrentLocation(id).Name,
                DeweyCallNumber = libraryAsset.GetDeweyIndex(id),
                CheckoutHistory = checkout.GetCheckoutHistory(id),
                ISBN = libraryAsset.GetIsbn(id),
                LatestCheckout = checkout.GetLatestCheckout(id),
                PatronName = checkout.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds
            };

            return View(model);
        }

        public IActionResult Checkout(int id)
        {
            var asset = libraryAsset.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = checkout.IsCheckedOut(id)
            };
            return View(model);
        }

        public IActionResult CheckIn(int id)
        {
            checkout.CheckInItem(id);
            return RedirectToAction("Detail", new { id = id });
        }

        public IActionResult Hold(int id)
        {
            var asset = libraryAsset.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = checkout.IsCheckedOut(id),
                HoldCount = checkout.GetCurrentHolds(id).Count()
            };
            return View(model);
        }


        public IActionResult MarkLost(int assetId)
        {
            checkout.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult MarkFound(int assetId)
        {
            checkout.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }


        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            checkout.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            checkout.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}
