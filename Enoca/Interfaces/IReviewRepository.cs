using Enoca.Models;

namespace Enoca.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReview(int reviewId);
        ICollection<Review> GetReviewsOfPokemon(int pokeId);
        bool ReviewExists(int reviewId);
    }
}
