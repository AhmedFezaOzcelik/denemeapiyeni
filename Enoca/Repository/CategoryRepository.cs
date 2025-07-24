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
    }
}
