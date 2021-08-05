using BookStore.Domain.Abstract;
using System.Collections.Generic;
using BookStore.Domain.Entities;

namespace BookStore.Domain.Concrete
{
    public class EFBookRepository : IBookRepository
    {
        EfDbContext context = new EfDbContext();

        public IEnumerable<Book> Books
        {
            get
            {
                return context.Books;

            }
        }

        public void SaveBook(Book book)
        {
           
                Book dbEntity = context.Books.Find(book.ISBN);
                if (dbEntity == null)
                    context.Books.Add(book);
            else
            {
                dbEntity.Title = book.Title;
                dbEntity.Specialization = book.Specialization;
                dbEntity.Price = book.Price;
                dbEntity.Description = book.Description;
                dbEntity.ImageData = book.ImageData;
                dbEntity.ImageMimeType = book.ImageMimeType;

            }

            context.SaveChanges();
        }

        public Book DeleteBook(int ISBN)
        {
            Book dbEntity = context.Books.Find(ISBN);
            if (dbEntity!=null)
            {
                context.Books.Remove(dbEntity);
                context.SaveChanges();

            }


            return dbEntity;
        }
    }
}
