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
    public class LibraryAssetServices : ILibraryAsset
    {
        private readonly ApplicationDbContext context;

        public LibraryAssetServices(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(LibraryAsset newAsset)
        {
            context.Add(newAsset);
            save();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return context.LibraryAssets.Include(a => a.Status).Include(a => a.Location);
        }

        public string GetAuthorOrDirector(int id)
        {
            if (context.LibraryAssets.OfType<Book>().Where(asset=>asset.Id==id).Any())
            {
                return context.Books.Where(book => book.Id == id).Select(a => a.Author).FirstOrDefault();
            }
            if (context.LibraryAssets.OfType<Video>().Where(asset => asset.Id == id).Any())
            {
                return context.Videos.Where(video => video.Id == id).Select(d => d.Director).FirstOrDefault();
            }
            else
            {
                return "Unknown";
            }
                
        }

        public LibraryAsset GetById(int id)
        {
            return context.LibraryAssets.Where(a => a.Id == id).Include(a => a.Status).Include(a => a.Location).FirstOrDefault();
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return context.LibraryAssets.Where(a => a.Id == id).Select(a => a.Location).FirstOrDefault();
        }

        public string GetDeweyIndex(int id)
        {
            if (context.Books.Any(book=>book.Id==id))
            {
                return context.Books.Where(book => book.Id == id).Select(c => c.DeweyIndex).FirstOrDefault();
            }
            else
            {
                return "";
            }
        }

        public string GetIsbn(int id)
        {
            if (context.Books.Any(book => book.Id == id))
            {
                return context.Books.Where(book => book.Id == id).Select(c => c.ISBN).FirstOrDefault();
            }
            else
            {
                return "";
            }
        }

        public string GetTitle(int id)
        {
            if (context.LibraryAssets.Any(la => la.Id == id))
            {
                return context.LibraryAssets.Where(c => c.Id == id).Select(i => i.Title).FirstOrDefault();
            }
            else
            {
                return "";
            }
        }

        public string GetType(int id)
        {
            var type = context.LibraryAssets.OfType<Book>().Where(t => t.Id == id);

            return type.Any() ? "Book" : "Video";
        }

        public void save()
        {
            context.SaveChanges();
        }
    }
}
