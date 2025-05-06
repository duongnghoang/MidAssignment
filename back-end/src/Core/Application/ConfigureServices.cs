using Application.Services.Authentication;
using Application.Services.BookBorrowingRequests;
using Application.Services.Books;
using Application.Services.Categories;
using Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add Application
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBookBorrowingRequestService, BookBorrowingRequestService>();

        return services;
    }
}