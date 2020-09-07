using Library.Models;
using Library.ViewModels.Patrons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Interfaces
{
    public interface IPatron
    {
        IEnumerable<Patron> GetAll();
        Patron Get(int id);
        bool CreatePatron(PatronRegisterModel model);
        bool RemovePatron(int id);
        bool EditPatron(int id);
        void Add(Patron newBook);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId);
        IEnumerable<Hold> GetHolds(int patronId);
        IEnumerable<Checkout> GetCheckouts(int id);
    }
}
