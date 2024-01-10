using Dapper;
using Npgsql;

namespace OpenSourceRecipes.Seeds
{
    public class MyRecipeObject
    {
    public int RecipeId { get; set; } = 0;
    public string RecipeTitle { get; set; } = "";
    public string TagLine { get; set; } = "";
    public int Difficulty { get; set; } = 0;
    public int TimeToPrepare { get; set; } = 0;
    public string RecipeMethod { get; set; } = "";
    public string PostedOn { get; set; } = "";
    public int? ForkedFromId { get; set; } = null;
    public string Cuisine { get; set; } = "";
    public string RecipeImg { get; set; } = "";
    public int UserId { get; set; } = 0;
    public int CuisineId { get; set; } = 0;
    public int? OriginalRecipeId { get; set; } = null;
    }

    public class SeedRecipeData(IConfiguration configuration)
    {
        public async Task<IEnumerable<MyRecipeObject>> InsertIntoRecipe(string connectionStringName)
        {
            await using var connection = new NpgsqlConnection(configuration.GetConnectionString(connectionStringName));

            var recipesObject = new RecipesData();
            var recipeArr = recipesObject.GetRecipes();

            var ingredients = recipesObject.ingredients();
            var quantity = recipesObject.quantities();

            Console.WriteLine(recipeArr.Length);
            Console.WriteLine(ingredients.Length);
            Console.WriteLine(quantity.Length);

            List<MyRecipeObject> insertedRecipes = new List<MyRecipeObject>();

            Console.WriteLine("Inserting Recipes");
            Console.WriteLine("------------------------");

            for (int i = 0; i < recipeArr.Length; i++)
            {
                try
                {
                    MyRecipeObject recipe = recipeArr[i];
                    string query = $"INSERT INTO \"Recipe\" " +
                                    "(\"RecipeTitle\", \"TagLine\", \"Difficulty\", \"TimeToPrepare\", \"RecipeMethod\", \"Cuisine\", \"RecipeImg\", \"ForkedFromId\", \"OriginalRecipeId\", \"UserId\", \"CuisineId\") " +
                                    "VALUES (@RecipeTitle, @TagLine, @Difficulty, @TimeToPrepare, @RecipeMethod, @Cuisine, @RecipeImg, @ForkedFromId, @OriginalRecipeId, @UserId, @CuisineId) " +
                                    "RETURNING *;";
                    var insertedRecipe = await connection.QueryFirstOrDefaultAsync<MyRecipeObject>(query, recipe);
                    if (insertedRecipe != null) insertedRecipes.Add(insertedRecipe);

                    for (int j = 0; j < ingredients[i].Length; j++)
                    {
                        Console.WriteLine($"RecipeId: {insertedRecipe?.RecipeId}, IngredientId: {ingredients[i][j]}, Quantity: {quantity[i][j]}");
                        Console.WriteLine("--------------------------------------------------------------------------------------------------------");
                        var parameters = new { Quantity = quantity[i][j], RecipeId = insertedRecipe?.RecipeId, IngredientId = ingredients[i][j] };
                        string ingredientQuery = $"INSERT INTO \"RecipeIngredient\" " +
                                        "(\"Quantity\", \"RecipeId\", \"IngredientId\") " +
                                        $"VALUES (@Quantity, @RecipeId, @IngredientId) " +
                                        "RETURNING *;";


                        var insertedRecipeIngredient = await connection.QueryFirstOrDefaultAsync<MyRecipeObject>(ingredientQuery, parameters);
                        if (insertedRecipeIngredient != null) insertedRecipes.Add(insertedRecipeIngredient);
                    }
                }
                catch (Exception ex)
                {
                     Console.WriteLine($"Error inserting recipe: {ex}");
                    throw;
                }
            }
            Console.WriteLine("------------------------");
            Console.WriteLine("Successfully inserted Recipes");
            return insertedRecipes;
        }
    }
}
