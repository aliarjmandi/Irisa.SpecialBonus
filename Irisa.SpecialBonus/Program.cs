using Irisa.SpecialBonus.Api.Configuration;
using Irisa.SpecialBonus.Persistence.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Identity (EF فقط برای امنیت)
builder.Services.AddApplicationIdentity(builder.Configuration);

// JWT Authentication (بر اساس تنظیماتی که قبلاً ساختیم)
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();
await IdentitySeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
