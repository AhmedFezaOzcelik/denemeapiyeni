using Enoca.Data;
using Enoca.Interfaces;
using Enoca.Models;
using System.Diagnostics.Metrics;

namespace Enoca.Repository
{
    public class PokemonRepository:IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext Context) 
        {
            _context = Context;
        }

        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemons.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name==name).FirstOrDefault();
        }   

        public decimal GetPokemonRating(int pokeId)
        {
            var review = _context.Reviews.Where(p=> p.Pokemon.Id == pokeId);
            if(review.Count() <=0)
                return 0;

            return ((decimal)review.Sum(r => r.Rating) / review.Count());



        }

        public ICollection<Pokemon> GetPokemons()
        {
           return _context.Pokemons.OrderBy(p => p.Name).ToList();
        }

        public bool PokemonExists(int pokeId)
        {
            return _context.Pokemons.Any(p => p.Id == pokeId);
        }
        public void AddPokemon(Pokemon pokemon)
        {
            _context.Pokemons.Add(pokemon);
            _context.SaveChanges();
        }

        public bool PokemonExistsByName(string Name)
        {
            return _context.Pokemons.Any(p => p.Name.ToLower().Trim() == Name.ToLower().Trim());
        }

        //Veritabanındaki bir pokemon kaydını güncellenmesini sağlayan metot.
        public bool UpdatePokemon(Pokemon pokemon)
        {
           //_context.Update() metodu entity fraemwork core da bir nesnenin durumunun güncellendi olarak işaretlemesini söyler.
           _context.Update(pokemon);

            return Save();

        }

        public bool Save()
        {
            //_context.SaveChanges() metodu,etkilenen satır sayısını döndürür.
            //Eğer en az bir satır eklendiyse (Yani değişiklik başarılıysa) true döner.
            var saved =_context.SaveChanges();
            return saved > 0;
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            //_context.Remove(), Entity Fraemwork e bu nesnenin durumunun 
            //silindi olarak işaretlenmesini sağlar.
            _context.Remove(pokemon);
            return Save();
        }
    }
}
