namespace Enoca.Dto
{
    public class PokemonCreateDto
    {
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }
        public List<int> OwnerIds { get; set; }

        public List<int> CategoryIds { get; set; }


    }
}
