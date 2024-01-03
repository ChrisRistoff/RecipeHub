using System.Reflection;
using System.Text;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenSourceRecipes.Services;
using OpenSourceRecipes.Seeds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo{Title = "Open Source Recipes", Version = "v1"});

    // add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
       Description = "JWT Authorization header using the Bearer scheme.",
       Name = "Authorization",
       In = ParameterLocation.Header,
       Type = SecuritySchemeType.ApiKey,
       Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// add common FluentMigrator services
builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb

    // Add PostgreSQL support to FluentMigrator
    .AddPostgres()

    // Set the connection string
    .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))

    // Define the assembly containing the migrations
    // Assembly is defined in the project file (.csproj)
    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())

    // Enable logging to console in the FluentMigrator way
    .AddLogging(lb => lb.AddFluentMigratorConsole());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate the JWT Issuer (iss) claim
        ValidateIssuer = true,
        // Validate the JWT Audience (aud) claim
        ValidateAudience = true,
        // Validate the token expiry
        ValidateLifetime = true,
        // Validate the JWT Issuer (iss) claim
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // this comes from appsettings.json
        ValidAudience = builder.Configuration["Jwt:Audience"], // this comes from appsettings.json

        // The signing key must match the one used to generate the token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Repositories
builder.Services.AddScoped<UserRepository>();

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
var seedUser = new SeedUserData(builder.Configuration);
seedUser.InsertIntoUser();
var seedFoods = new SeedFoodData(builder.Configuration);
seedFoods.InsertIntoFood();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
