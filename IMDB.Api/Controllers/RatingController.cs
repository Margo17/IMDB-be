using IMDB.Api.Auth;
using IMDB.Api.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;
using IMDB.Contracts.Requests;
using IMDB.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.Api.Controllers;

[ApiController]
public class RatingController(IRatingService _ratingService) : ControllerBase
{
    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token)
    {
        Guid? userId = HttpContext.GetUserId();
        
        bool result = await _ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);

        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
    {
        Guid? userId = HttpContext.GetUserId();

        bool result = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);

        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    [ProducesResponseType(typeof(IEnumerable<MovieRatingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token)
    {
        Guid? userId = HttpContext.GetUserId();

        IEnumerable<MovieRating> result = await _ratingService.GetRatingsForUserAsync(userId!.Value, token);

        return Ok(result.MapToResponse());
    }
}