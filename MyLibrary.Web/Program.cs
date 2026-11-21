using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Application.Services;
using MyLibrary.Domain.Repositories;
using MyLibrary.Infrastructure;
using MyLibrary.Infrastructure.Models;
using MyLibrary.Infrastructure.Repositories;

namespace MyLibrary.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавление сервисов в DI-контейнер.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<MyLibraryDbContext>(options =>
            options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.MigrationsAssembly("MyLibrary.Infrastructure") 
            ));

            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
            builder.Services.AddScoped<IGenreRepository, GenreRepository>();
            builder.Services.AddScoped<ISubgenreRepository, SubgenreRepository>();

            builder.Services.AddHttpClient<IBookLibService, BookLibService>(client =>
            {
                client.BaseAddress = new Uri("https://openlibrary.org/api/");
                client.DefaultRequestHeaders.Add("User-Agent", "MyLibraryApp/1.0"); 
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
                            .AddEntityFrameworkStores<MyLibraryDbContext>();


            var app = builder.Build();

            //Сидирование админа
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                const string adminRole = "Admin";

                // 1. Создаем роль
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
                    if (roleResult.Succeeded)
                        Console.WriteLine($"Роль '{adminRole}' создана.");
                    else
                        Console.Error.WriteLine($"Не удалось создать роль: {string.Join(", ", roleResult.Errors)}");
                }

                // 2. Получаем данные из конфига или берем безопасные дефолты
                // Если в конфиге пусто — используем эти значения. 
                // Они нужны только, чтобы проект запустился у другого разработчика.
                var adminName = app.Configuration["AdminUser:Name"] ?? "Admin";
                var adminPassword = app.Configuration["AdminUser:Password"] ?? "TempPass123!";

                // 3. Создаем пользователя, если его нет
                var existingUser = await userManager.FindByNameAsync(adminName);

                if (existingUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminName,
                        Email = $"{adminName}@local",
                        EmailConfirmed = true,
                        IsPasswordResetRequired = true // ВАЖНО: сразу требуем смену пароля
                    };

                    Console.WriteLine($"Попытка создания админа: {adminName}");

                    var result = await userManager.CreateAsync(admin, adminPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, adminRole);
                        Console.WriteLine("Админ успешно создан. При первом входе потребуется сменить пароль.");
                    }
                    else
                    {
                        Console.Error.WriteLine("Ошибка создания админа:");
                        foreach (var error in result.Errors)
                            Console.Error.WriteLine($" - {error.Description}");
                    }
                }
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
