using lab1_bochkova.Data;  // Подключаем пространство имен, где находится ваш контекст данных
using Microsoft.AspNetCore.Authentication.JwtBearer;  // Для работы с JWT
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;  // Для настройки токенов
using Microsoft.OpenApi.Models;  // Подключаем пространство имен для Swagger
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Добавляем строку подключения из appsettings.json и регистрируем контекст базы данных с поддержкой повторных попыток
builder.Services.AddDbContext<BarbershopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

// Добавляем аутентификацию с JWT токенами
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "BarbershopAPI",
        ValidAudience = "BarbershopAPI",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecretKeyForJWT1234567890123456")) // Более длинный ключ
    };
});

// Добавляем авторизацию
builder.Services.AddAuthorization();

// Добавляем Swagger для генерации документации API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Barbershop API",
        Version = "v1",
        Description = "API для управления барбершопом, включает CRUD операции для парикмахеров, клиентов и записей"
    });

    // Настраиваем авторизацию для Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Введите токен в формате: Bearer {token}",
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
            new string[] { }
        }
    });
});

// Добавляем контроллеры и представления
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Включаем Swagger и Swagger UI только в режиме разработки
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Barbershop API V1");
        c.RoutePrefix = string.Empty;  // Swagger UI будет доступен по корневому URL
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Добавляем использование аутентификации
app.UseAuthorization();  // Добавляем использование авторизации

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
