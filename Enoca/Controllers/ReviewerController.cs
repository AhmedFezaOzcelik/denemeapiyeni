using AutoMapper;
using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Enoca.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ReviewerController:Controller
    {
        private readonly IReviewerRepository _reviewerrepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerrepository,IMapper mapper)
        {
            _reviewerrepository = reviewerrepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Enoca.Models.Reviewer>))]
        public IActionResult GetCategories()
        {
            var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerrepository.GetReviewers());
            if (!ModelState.IsValid)

                return BadRequest(ModelState);
            return Ok(reviewers);

        }
        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Enoca.Models.Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerrepository.ReviewerExists(reviewerId))
                return NotFound();
            var reviewer = _mapper.Map<ReviewerDto>(_reviewerrepository.GetReviewer(reviewerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewer);
        }
        [HttpGet("{reviewerId}/reviews")]
        public IActionResult GetReviewsByAReviewer(int reviewerId)
        {
            if (!_reviewerrepository.ReviewerExists(reviewerId))
                return NotFound();
                
            var reviews = _mapper.Map<List < ReviewerDto >>( _reviewerrepository.GetReviewsByReviewer(reviewerId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviews);

        }
    }
}
