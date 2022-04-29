using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Todos.API.Filters;
using Todos.API.Middlewares;
using Todos.API.Services;
using Todos.Core.Models;
using Todos.Core.Repositories;
using Todos.Core.Services;
using Todos.Core.Services.Abstract;
using Todos.Data;
using Todos.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);
var ss = new Settings();
builder.Configuration.Bind(ss);
builder.Services.AddSingleton(ss);
builder.Services.AddDbContext<TodosDbContext>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
}, ServiceLifetime.Scoped);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITodoItemsRepository, TodoItemsRepository>();
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op =>
{
    op.SaveToken = true;
    op.RequireHttpsMetadata = false;
    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Secret").Value))
    };
});
builder.Services.AddControllers(op =>
{
    op.Filters.Add<AuthFilter>();
});

builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Hello World!");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodosDbContext>();
    context.Database.EnsureCreated();
}
app.Run();