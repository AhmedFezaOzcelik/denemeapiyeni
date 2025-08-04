using Enoca.Data;
using Enoca.Interfaces;
using Enoca.Models;

namespace Enoca.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private DataContext _context;
        public CategoryRepository(DataContext context)
        {
           _context= context;
        }
        public bool CategoryExists(int categoryId)
        {
            return _context.Categories.Any(c => c.Id == categoryId);
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(e=> e.Id == id).FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonByCategories(int categoryId)
        {
            return _context.Pokemon_Categories.Where(e=> e.Category.Id == categoryId).Select(e => e.Pokemon).ToList();

        }
        public void AddCategory(Category category) 
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public bool CategoryExistsByName(string Name)
        {
            //Veritabanındaki kategoriler içinde ,adı gönderilen isimle eşleşen var mı kontrol et.
            return _context.Categories.Any(c => c.Name.Trim().ToLower() == Name.Trim().ToLower());
        }

        

        public bool Save()
        {
            //_context.SaveChanges() metodu,etkilenen satır sayısını döndürür.
            //Eğer en az bir satır eklendiyse (Yani değişiklik başarılıysa) true döner.
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool UpdateCategory(Category category)
        {
            //_context.Update() metodu entity fraemwork core da bir nesnenin durumunun güncellendi olarak işaretlemesini söyler.
            _context.Update(category);

            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            //_context.Remove(), Entity Fraemwork e bu nesnenin durumunun 
            //silindi olarak işaretlenmesini sağlar.
            _context.Remove(category);
            return Save();
        }
    }
}
