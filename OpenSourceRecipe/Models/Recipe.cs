using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OpenSourceRecipes.Models;

public class Recipe
{
    // Primary Key
    public int RecipeId { get; set; }

    [MaxLength(255)]
    public string? RecipeTitle { get; set; }

    [MaxLength(255)]
    public string? TagLine { get; set; }

    // Difficulty is a number between 1 and 3
    [Range(1, 3)]
    public int Difficulty { get; set; }

    // TimeToPrepare is a number in minutes
    [Range(1, 1000)]
    public int TimeToPrepare { get; set; }

    // Method is a string of text describing the recipe
    public string? RecipeMethod { get; set; }

    // Date the recipe was posted defaults to the current date
    public string PostedOn { get; set; } = DateTime.Now.ToString("MM/dd/yyyy");

    // Image
    public string? RecipeImg { get; set; }

    // Cuisine name
    public string? Cuisine { get; set; }

    // ForkedFromId is the id of the recipe this recipe was forked from
    public int? ForkedFromId { get; set; } = null;
    // OriginalRecipeId is the id of the original recipe this recipe was forked from
    public int? OriginalRecipeId { get; set; } = null;

    // Foreign Key to the User table
    public int UserId { get; set; }

    // Foreign Key to the Cuisine table
    public int CuisineId { get; set; }
}

public class CreateRecipeDto
{
    [Required]
    public string? RecipeTitle { get; set; }
    [Required]
    public string? TagLine { get; set; }
    [Required]
    public int Difficulty { get; set; }
    [Required]
    public int TimeToPrepare { get; set; }
    [Required]
    public string? RecipeMethod { get; set; }
    [Required]
    public string? RecipeImg { get; set; }
    [Required]
    public string? Cuisine { get; set; }
    public int UserId { get; set; }
    public int? ForkedFromId { get; set; } = null;
    public int? OriginalRecipeId { get; set; } = null;
    [Required]
    public int CuisineId { get; set; }
}

public class GetRecipesDto
{
    public int RecipeId { get; set; }
    public string? RecipeTitle { get; set; }
    public string? TagLine { get; set; }
    public int Difficulty { get; set; }
    public int TimeToPrepare { get; set; }
    public string? RecipeMethod { get; set; }
    public string? PostedOn { get; set; }
    public string? RecipeImg { get; set; }
    public string? Cuisine { get; set; }
    public int? ForkedFromId { get; set; } = null;
    public int? OriginalRecipeId { get; set; } = null;
    public int UserId { get; set; }
    public int CuisineId { get; set; }
    public int ForkCount { get; set; }
    public int DirectForkCount { get; set; }
    public int RatingCount { get; set; } = 0;
    public double AverageRating { get; set; } = 0;
}

public class GetForkedRecipesDto
{
    public int RecipeId { get; set; }
    public string? RecipeTitle { get; set; }
    public string? TagLine { get; set; }
    public int Difficulty { get; set; }
    public int TimeToPrepare { get; set; }
    public string? RecipeMethod { get; set; }
    public string? PostedOn { get; set; }
    public string? RecipeImg { get; set; }
    public string? Cuisine { get; set; }
    public int? ForkedFromId { get; set; } = null;
    public int? OriginalRecipeId { get; set; } = null;
    public int UserId { get; set; }
    public int CuisineId { get; set; }
    public int DirectForkCount { get; set; }
    public int RatingCount { get; set; } = 0;
    public double AverageRating { get; set; } = 0;
}

public class GetRecipeByIdDto
{
    public int RecipeId { get; set; }
    public string? RecipeTitle { get; set; }
    public string? TagLine { get; set; }
    public int Difficulty { get; set; }
    public int TimeToPrepare { get; set; }
    public string? RecipeMethod { get; set; }
    public string? PostedOn { get; set; }
    public string? RecipeImg { get; set; }
    public string? Cuisine { get; set; }
    public int? ForkedFromId { get; set; } = null;
    public int? OriginalRecipeId { get; set; } = null;
    public int UserId { get; set; }
    public string? Username { get; set; }
    public int CuisineId { get; set; }
    public int ForkCount { get; set; }
    public int DirectForkCount { get; set; }
    public int RatingCount { get; set; } = 0;
    public double AverageRating { get; set; } = 0;
    public List<IngredientRecipeDto>? RecipeIngredients { get; set; }
}

public class PatchRecipeDto
{
    public int RecipeId { get; set; }
    public string? RecipeTitle { get; set; }
    public string? TagLine { get; set; }
    public int? Difficulty { get; set; }
    public int? TimeToPrepare { get; set; }
    public string? RecipeMethod { get; set; }
    public string? PostedOn { get; set; }
    public string? RecipeImg { get; set; }
    public string? Cuisine { get; set; }
    public int UserId { get; set; }
    public int? CuisineId { get; set; }
    public int? ForkCount { get; set; }
    public int? DirectForkCount { get; set; }
    public int? RatingCount { get; set; } = 0;
    public double? AverageRating { get; set; } = 0;
    public List<IngredientRecipeDto>? RecipeIngredients { get; set; }
}

public class SearchRecipeDto
{
    public string? SearchTerm { get; set; }
}

public class GetRelevantRecipesDto
{
    public int RecipeId { get; set; }
    public string? RecipeTitle { get; set; }
    public string? RecipeImg { get; set; }
    public double AverageRating { get; set; } = 0;
}
