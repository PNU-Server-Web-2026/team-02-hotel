using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Features.Reviews;

/// <summary>
/// CRUD-ендпоінти для відгуків. Бізнес-логіки тут немає: лише виклик сервісу і вибір статус-коду.
/// </summary>
[ApiController]
[Route("api/reviews")]
public class ReviewsController(IReviewService reviews) : ControllerBase
{
    /// <summary>GET /api/reviews — список усіх відгуків.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ReviewResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await reviews.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>GET /api/reviews/{id} — один відгук або 404.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var review = await reviews.GetByIdAsync(id, cancellationToken);
        if (review is null)
        {
            return NotFound();
        }

        return review;
    }

    /// <summary>POST /api/reviews — створює відгук, повертає 201 і заголовок Location.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ReviewResponse>> Create(
        CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var review = await reviews.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    /// <summary>PUT /api/reviews/{id} — оновлює відгук: 204 або 404.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, UpdateReviewRequest request, CancellationToken cancellationToken)
    {
        var updated = await reviews.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>DELETE /api/reviews/{id} — видаляє відгук: 204 або 404.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await reviews.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
