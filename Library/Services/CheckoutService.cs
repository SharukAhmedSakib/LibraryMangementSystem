using Library.Data;
using Library.Interfaces;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Services
{
    public class CheckoutService : ICheckout
    {
        private readonly ApplicationDbContext context;

        public CheckoutService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(Checkout newCheckout)
        {
            context.Add(newCheckout);
            context.SaveChanges();
        }

       

        public IEnumerable<Checkout> GetAll()
        {
            return context.checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return context.checkouts.Where(checkout => checkout.Id == checkoutId).FirstOrDefault();
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return context.checkoutHistories.Include(h => h.LibraryAsset).Include(h => h.LibraryCard).Where(h => h.LibraryAsset.Id == id);
        }

        public string GetCurrentHoldPatronName(int id)
        {
            var hold = context.Holds
                .Include(c => c.LibraryAsset)
                .Include(c => c.LibraryCard)
                .FirstOrDefault(c => c.Id == id);

            var cardId = hold?.LibraryCard.Id;

            var patron = context.Patrons.Include(P => P.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;

        }

        public DateTime GetCurrentHoldPlaced(int id)
        {
            return context.Holds
               .Include(c => c.LibraryAsset)
               .Include(c => c.LibraryCard)
               .FirstOrDefault(c => c.Id == id)
               .HoldPlaced;

        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return context.Holds.Include(h => h.LibraryAsset).Where(h => h.LibraryAsset.Id == id);
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;
            var item = context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);

            context.Update(item);

            item.Status = context.Statuses.FirstOrDefault(status => status.Name == "Available");
            UpdateAssetStatus(assetId, "Available");
            RemoveExistingCheckouts(assetId);
            ClosingExistingCheckoutHistory(assetId,now);
            
            
            context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string v)
        {
            var item = context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);

            context.Update(item);

            item.Status = context.Statuses.FirstOrDefault(status => status.Name == v);
        }

        private void ClosingExistingCheckoutHistory(int assetId, DateTime now)
        {
            //close any existing checkout history
            var history = context.checkoutHistories.FirstOrDefault(h => h.LibraryAsset.Id == assetId && h.CheckedIn == null);

            if (history != null)
            {
                context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            //remove any existing checkouts on the item
            var checkout = context.checkouts.FirstOrDefault(co => co.LibraryAsset.Id == assetId);
            if (checkout != null)
            {
                context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");

            context.SaveChanges();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = context.LibraryAssets.Include(a=>a.Status).FirstOrDefault(a => a.Id == assetId);

            var card = context.LibraryCards.FirstOrDefault(c => c.Id == libraryCardId);

            if (asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            context.Add(hold);
            context.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;
            var item = context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);


            //remove any existing checkouts on the item
            RemoveExistingCheckouts(assetId);

            //close any existing checkout history
            ClosingExistingCheckoutHistory(assetId, now);

            //Look for existing holds on the item
            var currentHolds = context.Holds.Include(h => h.LibraryAsset).Include(h => h.LibraryCard).Where(h => h.LibraryAsset.Id == assetId);

            //if there are holds, checkout the item to the
            //Library card with the earliest hold.

            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
                return;
            }
            //otherwise, update the item status to available
            UpdateAssetStatus(assetId, "Available");
            context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliesetHold = currentHolds.OrderBy(holds => holds.HoldPlaced).FirstOrDefault();

            var card = earliesetHold.LibraryCard;

            context.Remove(earliesetHold);
            context.SaveChanges();
            CheckOutItem(assetId, card.Id);
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                return;
            }

            var item = context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);

            UpdateAssetStatus(assetId, "Checked Out");

            var libraryCard = context.LibraryCards.Include(c => c.Checkouts).FirstOrDefault(c => c.Id == libraryCardId);

            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };
            context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            context.Add(checkoutHistory);
            context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public bool IsCheckedOut(int assetId)
        {
            return context.checkouts.Where(c => c.LibraryAsset.Id == assetId).Any();
        }

        public Checkout GetLatestCheckout(int assetId)
        {
            return context.checkouts.Where(c => c.LibraryAsset.Id == assetId).OrderByDescending(c => c.Since).FirstOrDefault();
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetChekoutByAssetId(assetId);

            if (checkout==null)
            {
                return "";
            }
            var cardId = checkout.LibraryCard.Id;

            var patron = context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetChekoutByAssetId(int assetId)
        {
            return context.checkouts
                .Include(c => c.LibraryAsset)
                .Include(c => c.LibraryCard)
                .FirstOrDefault(c => c.LibraryAsset.Id == assetId);
        }
    }
}
