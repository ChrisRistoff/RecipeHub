using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenSourceRecipes.Models;
using OpenSourceRecipes.Services;

namespace OpenSourceRecipe.Controllers;

[ApiController]
public class CommentController(CommentRepository commentRepository) : ControllerBase
{

    [HttpPost("api/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<ActionResult<GetCommentDto?>> CreateComment(CreateCommentDto comment)
    {
        try
        {
            string? username = User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;

            if (username != comment.Author)
            {
                return Unauthorized();
            }

            GetCommentDto? newComment = await commentRepository.CreateComment(comment);

            return CreatedAtAction(nameof(CreateComment), newComment);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("api/comments/{recipeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetCommentDto>>> GetCommentsByRecipeId(int recipeId, int userId)
    {
        try
        {
            return Ok(await commentRepository.GetCommentsByRecipeId(recipeId, userId));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("api/comments/{commentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult> DeleteComment(int commentId)
    {
        try
        {
            GetCommentDto? comment = await commentRepository.GetCommentById(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            string? userIdFromToken = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (userIdFromToken != comment.UserId.ToString())
            {
                return Unauthorized();
            }

            await commentRepository.DeleteComment(commentId);

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpPatch("api/comments/{commentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<GetCommentDto?>> UpdateComment(int commentId, EditCommentBodyDto comment)
    {
        try
        {
            GetCommentDto? commentFromDb = await commentRepository.GetCommentById(commentId);

            if (commentFromDb == null)
            {
                return NotFound();
            }

            string? userIdFromToken = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (userIdFromToken != commentFromDb.UserId.ToString())
            {
                return Unauthorized();
            }

            if (comment.Comment.IsNullOrEmpty())
            {
                return BadRequest("Comment cannot empty");
            }

            return Ok(await commentRepository.UpdateComment(commentId, comment));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
