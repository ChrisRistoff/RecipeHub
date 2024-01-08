using Dapper;
using Npgsql;
using OpenSourceRecipes.Models;

namespace OpenSourceRecipes.Services;
public class CommentRepository
{
    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;

    public CommentRepository(IConfiguration configuration)
    {
        this._configuration = configuration;

        string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        if (env == "Testing")
        {
            _connectionString = "TestConnection";
        }
        else if (env == "Production")
        {
            _connectionString = "ProductionConnection";
        }
        else
        {
            _connectionString = "DefaultConnection";
        }
    }

    public async Task<GetCommentDto?> CreateComment(CreateCommentDto comment)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        string query = $"INSERT INTO \"RecipeComment\" " +
                        "(\"RecipeId\", \"UserId\", \"Author\", \"Comment\") " +
                        "VALUES (@RecipeId, @UserId, @Author, @Comment) " +
                        "RETURNING *;";

        return await connection.QueryFirstOrDefaultAsync<GetCommentDto>(query, comment);
    }

    public async Task<IEnumerable<GetCommentDto>> GetCommentsByRecipeId(int recipeId)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        string recipeQuery = $"SELECT * FROM \"Recipe\" WHERE \"RecipeId\" = @RecipeId;";

        var recipe = await connection.QueryFirstOrDefaultAsync<GetRecipeByIdDto>(recipeQuery, new { RecipeId = recipeId });

        if (recipe == null)
        {
            throw new Exception("Recipe does not exist");
        }

        string query = $"SELECT * FROM \"RecipeComment\" WHERE \"RecipeId\" = @RecipeId;";

        return await connection.QueryAsync<GetCommentDto>(query, new { RecipeId = recipeId });
    }

    public async Task DeleteComment(int commentId)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        string query = $"DELETE FROM \"RecipeComment\" WHERE \"CommentId\" = @CommentId;";

        await connection.ExecuteAsync(query, new { CommentId = commentId });

        return;
    }

    public async Task<GetCommentDto> GetCommentById(int commentId)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        string query = $"SELECT * FROM \"RecipeComment\" WHERE \"CommentId\" = @CommentId;";

        return await connection.QueryFirstOrDefaultAsync<GetCommentDto>(query, new { CommentId = commentId });
    }
}