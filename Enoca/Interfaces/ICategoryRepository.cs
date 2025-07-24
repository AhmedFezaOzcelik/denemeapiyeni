using Enoca.Models;

namespace Enoca.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        ICollection<Pokemon> GetPokemonByCategories(int categoryId);

        bool CategoryExists(int categoryId);
        void AddCategory(Category category);
    }

    
        
 }
