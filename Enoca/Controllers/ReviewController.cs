using AutoMapper;
using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Enoca.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ReviewController:Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository,IMapper mapper )
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Enoca.Models.Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
            if (!ModelState.IsValid)

                return BadRequest(ModelState);
            return Ok(reviews);

        }
        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Enoca.Models.Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }
        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Enoca.Models.Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsForAPokemon(int pokeId)
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfPokemon(pokeId));
            if (!ModelState.IsValid)
                return BadRequest();
            return Ok(reviews);
        }
    }
}
