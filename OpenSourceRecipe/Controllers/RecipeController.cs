using Microsoft.AspNetCore.Mvc;
using OpenSourceRecipes.Models;
using OpenSourceRecipes.Services;

namespace OpenSourceRecipe.Controllers;

[ApiController]
public class RecipeController(RecipeRepository recipeRepository) : ControllerBase
{
    [HttpPost("api/recipes")]
    public async Task<ActionResult<GetRecipeDto>> CreateRecipe(CreateRecipeDto recipe)
    {
        try
        {
            return await recipeRepository.CreateRecipe(recipe);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("api/recipes")]
    public async Task<ActionResult<IEnumerable<GetRecipeDto>>> GetAllRecipes()
    {
        try
        {
            return Ok(await recipeRepository.GetAllRecipes());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("api/recipes/{recipeId}")]
    public async Task<ActionResult<GetRecipeByIdDto>> GetRecipeById(int recipeId)
    {
        try
        {
            GetRecipeByIdDto? recipe = await recipeRepository.GetRecipeById(recipeId);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
