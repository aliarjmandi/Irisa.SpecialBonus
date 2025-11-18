using Irisa.SpecialBonus.Api.Configuration;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Services;
using Irisa.SpecialBonus.Persistence.Identity;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Identity + JWT
builder.Services.AddApplicationIdentity(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDapperServices();

// Swagger با JWT
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// اجرای Seeder
await IdentitySeeder.SeedAsync(app.Services);

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Irisa Special Bonus API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
