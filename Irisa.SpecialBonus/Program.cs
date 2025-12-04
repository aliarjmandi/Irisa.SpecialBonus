using Irisa.SpecialBonus.Api.Configuration;
using Irisa.SpecialBonus.Application.Services;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Services;
using Irisa.SpecialBonus.Persistence.Identity;
using Irisa.SpecialBonus.Persistence.Seed;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Identity + JWT
builder.Services.AddApplicationIdentity(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDapperServices();
builder.Services.AddTransient<DatabaseSeeder>();
builder.Services.AddScoped<IUserService, UserService>();

// Swagger با JWT
builder.Services.AddSwaggerWithJwt();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// اجرای Seeder
//await IdentitySeeder.SeedAsync(app.Services);


// اجرای سیدر
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.RunAsync();
}


app.UseSwagger();
/*
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Irisa Special Bonus API v1");
});
*/

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/RewardAPI/swagger/v1/swagger.json", "Irisa Special Bonus API v1");
});



app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
