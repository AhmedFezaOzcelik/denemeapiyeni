using Enoca.Models;

namespace Enoca.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemon(int id);
        Pokemon GetPokemon(string name);
        decimal GetPokemonRating(int pokeId);

        bool PokemonExists(int pokeId);
        void AddPokemon(Pokemon pokemon);

        bool PokemonExistsByName(string Name);
        bool UpdatePokemon(Pokemon pokemon);

        bool DeletePokemon(Pokemon pokemon);

        bool Save();
    }
}
