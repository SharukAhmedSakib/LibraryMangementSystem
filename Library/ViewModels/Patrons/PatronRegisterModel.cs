using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.ViewModels.Patrons
{
    public class PatronRegisterModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string TelephoneNumber { get; set; }
        public int HomeLibraryBranchId { get; set; }
        public int LibraryCardId { get; set; }
        public decimal Fees { get; set; }



    }
}
