using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OpenSourceRecipes.Models;

namespace OpenSourceRecipes.Services;
public class UserRepository
{

    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;

    public UserRepository(IConfiguration configuration)
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
    public async Task<GetUserDto?> GetUserById(int userId)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));
        var sql = "SELECT * FROM \"User\" WHERE \"UserId\" = @UserId";

        return await connection.QueryFirstOrDefaultAsync<GetUserDto>(sql, new {UserId = userId});
    }
    public async Task<GetUserDto?> GetUserByUsername(string username)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));
        var sql = "SELECT * FROM \"User\" WHERE \"Username\" = @Username";

        return await connection.QueryFirstOrDefaultAsync<GetUserDto>(sql, new { Username = username });
    }

    public async Task<GetLoggedInUserDto> RegisterUser(User user)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        user.Password = HashPassword(user.Password!);

        var parameters = new DynamicParameters();
        parameters.Add("Username", user.Username);
        parameters.Add("Name", user.Name);
        parameters.Add("ProfileImg", user.ProfileImg);
        parameters.Add("Password", user.Password);
        parameters.Add("Status", user.Status);
        parameters.Add("Bio", user.Bio);

        var sql = "INSERT INTO \"User\" " +
                  "(\"Username\", \"Name\", \"ProfileImg\", \"Password\", \"Status\", \"Bio\") " +
                  "VALUES (@Username, @Name, @ProfileImg, @Password, @Status, @Bio) RETURNING *";

        var newUser = await connection.QueryAsync<User>(sql, parameters);

        if (newUser == null)
        {
            throw new Exception("User not created");
        }

        GetUserDto? userDetails = await GetUserByUsername(user.Username!);

        string token = GenerateJwtToken(userDetails);

        return new GetLoggedInUserDto
        {
            UserId = userDetails?.UserId,
            Username = userDetails?.Username,
            Token = token
        };
    }

    public async Task<GetLoggedInUserDto> SignUserIn(string username, string password)
    {
        using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        User? user = await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM \"User\" WHERE \"Username\" = @Username", new { Username = username });

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (!CheckPassword(password, user.Password!))
        {
            throw new Exception("Invalid password");
        }

        string token = GenerateJwtToken(await GetUserByUsername(username));

        return new GetLoggedInUserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Token = token
        };
    }

    public async Task<GetUserDto?> UpdateUserBio(int userId, string bio)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);
        parameters.Add("Bio", bio);

        var sql = "UPDATE \"User\" SET \"Bio\" = @Bio WHERE \"UserId\" = @UserId RETURNING *";

        return await connection.QueryFirstOrDefaultAsync<GetUserDto>(sql, parameters);
    }

    public async Task<GetUserDto?> UpdateUserImg(int userId, string img)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);
        parameters.Add("ProfileImg", img);

        var sql = "UPDATE \"User\" SET \"ProfileImg\" = @ProfileImg WHERE \"UserId\" = @UserId RETURNING *";

        return await connection.QueryFirstOrDefaultAsync<GetUserDto>(sql, parameters);
    }

    public async Task<GetUserDto?> UpdateUserName(int userId, string name)
    {
        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString(_connectionString!));

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);
        parameters.Add("Name", name);

        var sql = "UPDATE \"User\" SET \"Name\" = @Name WHERE \"UserId\" = @UserId RETURNING *";

        return await connection.QueryFirstOrDefaultAsync<GetUserDto>(sql, parameters);
    }

    // SERVICE METHODS
    private string GenerateJwtToken(GetUserDto? user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", user!.UserId.ToString()!),
            new Claim("Username", user.Username!),

            new Claim(JwtRegisteredClaimNames.Sub, user!.UserId.ToString()!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username!),
            new Claim(JwtRegisteredClaimNames.NameId, user.Name!),
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string HashPassword(string password)
    {
        var hasher = new PasswordHasher<User>();
        return hasher.HashPassword(null!, password);
    }

    private bool CheckPassword(string password, string hashedPassword)
    {
        var hasher = new PasswordHasher<User>();
        return hasher.VerifyHashedPassword(null!, hashedPassword, password) != PasswordVerificationResult.Failed;
    }
}
