using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LookBook.Models;

namespace LookBook.Controllers
{
    public class LibraryController : Controller
    {


        // GET: Biblioteca
        public ActionResult ViewAll()
        {
            LibraryDataBase db = new LibraryDataBase();
            List<Book> books = db.Books.OrderBy(book => book.Title).ToList();
            return View(books);
        }

        public ActionResult Edit(int id)
        {
            LibraryDataBase db = new LibraryDataBase();
            Book book = db.Books.Find(id);
            
            SimpleBook sBook = new SimpleBook
            {
                Title = book.Title,
                Author = book.Author.AuthorName,
                GenreType = book.Genre.GenreType,
                Publisher = book.Publisher.PublisherName,
                BookRef = book.ID
            };
            ViewBag.Description = "Editar";
            return View(sBook);
        }

        [HttpPost]
        public ActionResult Edit(SimpleBook sBook)
        {
            LibraryDataBase db = new LibraryDataBase();
            Book book = db.Books.Find(sBook.BookRef);

            ModifyAuthor(book, sBook, db);
            ModifyGenre(book, sBook, db);
            ModifyPublisher(book, sBook, db);
            ModifyTitle(book, sBook);

            db.Entry(book).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            FreeUnusedMemory(db);
            return RedirectToAction("ViewAll");
        }

        public ActionResult Delete(int id)
        {
            LibraryDataBase db = new LibraryDataBase();
            Book book = db.Books.Find(id);
            book.Author.QuantityBooks--;
            book.Publisher.QuantityBooks--;
            db.Books.Remove(book);
            db.SaveChanges();

            FreeUnusedMemory(db);
            return RedirectToAction("ViewAll");
        }

        public ActionResult AddNew()
        {
            LibraryDataBase db = new LibraryDataBase();

            SimpleBook sBook = NewEmptySimpleBook(db);
            ViewBag.Description = "Agregar Nuevo";
            return View("Edit",sBook);
        }

        [HttpPost]
        public ActionResult AddNew(SimpleBook sBook)
        {
            LibraryDataBase db = new LibraryDataBase();
            Book book = new Book();
            book.Author = new Author { AuthorName = "" };
            book.Publisher = new Publisher { PublisherName = "" };

            ModifyAuthor(book, sBook, db);
            ModifyGenre(book, sBook, db);
            ModifyPublisher(book, sBook, db);
            ModifyTitle(book, sBook);

            db.Books.Add(book);
            db.SaveChanges();

            FreeUnusedMemory(db);
            return RedirectToAction("ViewAll");
        }

        public ActionResult Search()
        {
            LibraryDataBase db = new LibraryDataBase();
            SimpleBook sBook = NewEmptySimpleBook(db);

            return View(sBook);
        }

        [HttpPost]
        public ActionResult Search(SimpleBook sBook)
        {
            LibraryDataBase db = new LibraryDataBase();
            List<Book> bookList = db.Books.ToList();

            List<Book> filteredList = FilterByTitle(sBook, bookList);
            filteredList = FilterByGenre(sBook, filteredList);
            filteredList = FilterByPublisher(sBook, filteredList);
            filteredList = FilterByAuthor(sBook, filteredList);

            return View("ViewAll", filteredList.OrderBy(book => book.Title));
        }



        private void ModifyAuthor(Book book, SimpleBook sBook, LibraryDataBase db)
        {
            if (sBook.Author == book.Author.AuthorName) { return; }

            if (book.Author.AuthorName != "") // Si no es un libro nuevo
            {
                book.Author.QuantityBooks--; // Le resto uno a la cant de libros del autor anterior
            }


            bool existingAuthor = false;
            foreach (Author a in db.Authors)
            {
                if (a.AuthorName == sBook.Author)
                {
                    existingAuthor = true;
                    book.Author = a;
                    a.QuantityBooks++;
                    break;
                }
            }
            if (!existingAuthor)
            {
                Author newAuthor = new Author(); //Creo el autor nuevo
                newAuthor.AuthorName = sBook.Author; //Le cambio el Name
                newAuthor.QuantityBooks = 1; //Le asigno la cant de libros del autor actual
                db.Authors.Add(newAuthor); //Lo agrego a la tabla de Authors
                book.Author = newAuthor;
            }



        }

        private void ModifyGenre(Book book, SimpleBook sBook, LibraryDataBase db)
        {
            foreach (Genre g in db.Genres)
            {
                if (g.GenreType == sBook.GenreType)
                {
                    book.ID_GenreType = g.ID;
                    break;
                }
            }
        }

        private void ModifyPublisher(Book book, SimpleBook sBook, LibraryDataBase db)
        {
            if (sBook.Publisher == book.Publisher.PublisherName) { return; }
            
            if (book.Publisher.PublisherName != "") // si no es un libro nuevo
            {
                book.Publisher.QuantityBooks--; // Le resto uno a la cant de libros del autor anterior
            }


            bool existingPublisher = false;
            foreach (Publisher p in db.Publishers)
            {
                if (p.PublisherName == sBook.Publisher)
                {
                    existingPublisher = true;
                    book.Publisher = p;
                    p.QuantityBooks++;
                    break;
                }
            }

            if (!existingPublisher)
            {
                Publisher newPublisher = new Publisher(); //Creo el publisher nuevo
                newPublisher.PublisherName = sBook.Publisher; //Le cambio el valor del Name
                newPublisher.QuantityBooks = 1; // Le asigno 1 a la cant de libros del publisher actual
                db.Publishers.Add(newPublisher); //Lo agrego a la tabla publishers
                book.Publisher = newPublisher;
            }
        }

        private void ModifyTitle(Book book, SimpleBook sBook)
        {
            book.Title = sBook.Title;
        }

        private void FreeUnusedMemory(LibraryDataBase db)
        {
            foreach(Author a in db.Authors)
            {
                if (a.QuantityBooks == 0)
                {
                    db.Authors.Remove(a);
                }
            }

            foreach(Publisher p in db.Publishers)
            {
                if (p.QuantityBooks == 0)
                {
                    db.Publishers.Remove(p);
                }
            }
            db.SaveChanges();
        }

        private SimpleBook NewEmptySimpleBook(LibraryDataBase db)
        {

            SimpleBook sBook = new SimpleBook();
            sBook.Title = "";
            sBook.Author = "";
            sBook.GenreType = db.Genres.Find(db.Genres.Max(gen => gen.ID)).GenreType;
            sBook.Publisher = "";
            sBook.BookRef = null;
            return sBook;
        }

        private List<Book> FilterByTitle(SimpleBook sBook, List<Book> list)
        {
            if(list.Count()== 0 || sBook.Title == null) { return list; }
            List<Book> filteredList = new List<Book>();
            if (sBook.Title != null)
            {
                foreach (Book b in list)
                {
                    if (b.Title == sBook.Title) { filteredList.Add(b); }
                }
            }
            return filteredList;
        }
        private List<Book> FilterByGenre(SimpleBook sBook, List<Book> list)
        {
            if (list.Count() == 0 || sBook.GenreType == null) { return list; }
            List<Book> filteredList = new List<Book>();
            if (sBook.GenreType != null)
            {
                foreach (Book b in list)
                {
                    if (b.Genre.GenreType == sBook.GenreType) { filteredList.Add(b); }
                }
            }
            return filteredList;
        }
        private List<Book> FilterByPublisher(SimpleBook sBook, List<Book> list)
        {
            if (list.Count() == 0 || sBook.Publisher == null) { return list; }
            List<Book> filteredList = new List<Book>();
            if (sBook.Publisher != null)
            {
                foreach (Book b in list)
                {
                    if (b.Publisher.PublisherName == sBook.Publisher) { filteredList.Add(b); }
                }
            }
            return filteredList;
        }
        private List<Book> FilterByAuthor(SimpleBook sBook, List<Book> list)
        {
            if (list.Count() == 0 || sBook.Author == null) { return list; }
            List<Book> filteredList = new List<Book>();
            if (sBook.Author != null)
            {
                foreach (Book b in list)
                {
                    if (b.Author.AuthorName == sBook.Author) { filteredList.Add(b); }
                }
            }

            return filteredList;
            
        }


    }
}