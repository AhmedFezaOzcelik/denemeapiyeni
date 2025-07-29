using Enoca;
using Enoca.Data;
using Enoca.Interfaces;
using Enoca.Repository;
using Microsoft.EntityFrameworkCore;
//JWT kimlik doğrulamasını kullamnak için gerekli sınıfları içerir.
using Microsoft.AspNetCore.Authentication.JwtBearer;
//Jetonları ve anahtarları yönetmek için gerekli sınıfları içerir.
using Microsoft.IdentityModel.Tokens;
//Metin (string) ve byte dizileri arasında dönüşüm yapmak için  gerekli sınıfları içerir.
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
      options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
  });

// --- DÜZELTİLMİŞ BÖLÜM ---
//Kimlik doğrulama servisini projemize ekliyoruz.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //Jwt Bearer şemasını kullanarak kimlik doğrulama işlemlerini yapılandırıyoruz.
    .AddJwtBearer(options =>
    {
        //jeton doğrulama parametrelerini belirliyoruz.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //Gelen jetonun imzasının,bizim gizli anahtarlarımızla doğrulanmasını zorunlu hale getiriyoruz.
            ValidateIssuerSigningKey = true,

            //Gizli anahtarımızı belirtiyoruz. (Yazım hatası düzeltildi)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("AppSettings:Token").Value!)),

            //Jetonu kimin oluşturduğunu (Issuer) doğrulamayı kapatıyoruz.
            ValidateIssuer = false,

            //Jetonun kimin için oluşturulduğunu (Audience) doğrulamayı kapatıyoruz.
            ValidateAudience = false
        };
    });
// --- DÜZELTME BİTTİ ---

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<Enoca.Seed>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSwaggerGen(options =>
{
    //Swagger a bir güvenlik tanımı ekliyoruz.
    //Bu authorize butonunu ve jeton giriş alanını oluşturur.
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer{token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    //Swagger a endpointlere istek gönderirken bu güvenlik şemasını kullanmasını söylüyoruz.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "oauth2"
            }
        },
        new string[] {}
    }
});
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Burada DataContext kullanıyoruz!
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0))
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}

app.UseHttpsRedirection();

//Gelen isteklerde kimlik doğrulama işlemlerini yapılmasını sağlar.
app.UseAuthentication();

//Kimliği doğrulanmış kullanıcının yetkilerini kontrol eder.
app.UseAuthorization(); // Fazladan olan ikinci satır silindi.

app.MapControllers();
app.Run();