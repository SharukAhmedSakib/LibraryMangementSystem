using Library.Data;
using Library.Interfaces;
using Library.Models;
using Library.ViewModels.Patrons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Services
{
    public class PatronService : IPatron
    {
        private readonly ApplicationDbContext _context;

        public PatronService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Patron newPatron)
        {
            _context.Add(newPatron);
            _context.SaveChanges();
        }

        public bool CreatePatron(PatronRegisterModel model)
        {
            if (model.Id == 0)
            {
                LibraryCard libraryCard = new LibraryCard
                {
                    Created = DateTime.Now,
                    Fees = model.Fees
                };
                _context.Add(libraryCard);
                _context.SaveChanges();

                model.LibraryCardId = libraryCard.Id;

                Patron newPatron = new Patron
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Address = model.Address,
                    TelephoneNumber = model.TelephoneNumber,
                    HomeLibraryBranchId = model.HomeLibraryBranchId,
                    LibraryCardId = model.LibraryCardId
                };

                _context.Add(newPatron);
            }

            if (model.Id != 0)
            {
                var patron = _context.Patrons.Find(model.Id);


                patron.LastName = model.LastName;
                patron.FirstName = model.FirstName;
                patron.Address = model.Address;
                patron.TelephoneNumber = model.TelephoneNumber;
                patron.HomeLibraryBranchId = model.HomeLibraryBranchId;
                patron.DateOfBirth = model.DateOfBirth;

                _context.Patrons.Update(patron);

                var libraryfees = _context.LibraryCards.Find(patron.LibraryCardId);

                libraryfees.Fees = model.Fees;

                _context.LibraryCards.Update(libraryfees);

            }

            var result = _context.SaveChanges();

            // if the user doesn't change anything the result becomes 0 which makes confliction.
            if (result != 0)
            {
                return true;
            }
            return false;
        }

        public bool EditPatron(int id)
        {
            return true;
        }

        //private int GetLibraryCardId(LibraryCard libraryCard)
        //{
        //    var LibraryCard = _context.LibraryCards.Select(c=>c.)
        //}

        public Patron Get(int id)
        {
            return _context.Patrons
                .Include(a => a.LibraryCard)
                .Include(a => a.HomeLibraryBranch)
                .FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons
                .Include(a => a.LibraryCard)
                .Include(a => a.HomeLibraryBranch);
            // Eager load this data.
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            var cardId = _context.Patrons
                .Include(a => a.LibraryCard)
                .FirstOrDefault(a => a.Id == patronId)?
                .LibraryCard.Id;

            return _context.checkoutHistories
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryAsset)
                .Where(a => a.LibraryCard.Id == cardId)
                .OrderByDescending(a => a.CheckedOut);
        }

        public IEnumerable<Checkout> GetCheckouts(int id)
        {
            var patronCardId = Get(id).LibraryCard.Id;
            return _context.checkouts
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryAsset)
                .Where(v => v.LibraryCard.Id == patronCardId);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            var cardId = _context.Patrons
                .Include(a => a.LibraryCard)
                .FirstOrDefault(a => a.Id == patronId)?
                .LibraryCard.Id;

            return _context.Holds
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryAsset)
                .Where(a => a.LibraryCard.Id == cardId)
                .OrderByDescending(a => a.HoldPlaced);
        }

        public bool RemovePatron(int id)
        {
            var patron = _context.Patrons.Find(id);
            _context.Remove<Patron>(patron);
            var libraryCard = _context.LibraryCards.Find(patron.LibraryCardId);
            //if (libraryCard.Fees == 0)
            //{

            //}
            _context.Remove<LibraryCard>(libraryCard);
            var result= _context.SaveChanges();
            if (result == 2)
            {
                return true;
            }
            return false;
        }
    }
}
