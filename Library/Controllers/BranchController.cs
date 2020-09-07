using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Interfaces;
using Library.ViewModels.Branch;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        private readonly ILibraryBranch branch;

        public BranchController(ILibraryBranch branch)
        {
            this.branch = branch;
        }
        public IActionResult Index()
        {
            var branches = branch.GetAll().Select(b => new BranchDetailModel
            {
                Id = b.Id,
                BranchName = b.Name,
                IsOpen = branch.IsBranchOpen(b.Id),
                NumberOfAssets = branch.GetAssetCount(b.Id),
                NumberOfPatrons = branch.GetPatronCount(b.Id)
            });

            var model = new BranchIndexModel()
            {
                Branches = branches
            };
            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var branchInfo = branch.Get(id);
            var model = new BranchDetailModel
            {
                BranchName = branchInfo.Name,
                Description = branchInfo.Description,
                Address = branchInfo.Address,
                Telephone = branchInfo.Telephone,
                BranchOpenedDate = branchInfo.OpenDate.ToString("yyyy-MM-dd"),
                NumberOfPatrons = branch.GetPatronCount(id),
                NumberOfAssets = branch.GetAssetCount(id),
                TotalAssetValue = branch.GetAssetsValue(id),
                ImageUrl = branchInfo.ImageUrl,
                HoursOpen = branch.GetBranchHours(id)
            };

            return View(model);
        }

        
    }
}
