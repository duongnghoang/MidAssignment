using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Repositories;

public class BookRepository(ApplicationDbContext context) : BaseRepository<Book>(context), IBookRepository
{
    public async Task<Book?> GetByISBNAsync(string isbn)
    {
        return await _context.Set<Book>()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<bool> IsInCategoryAsync(uint id)
    {
        return await _context.Set<Book>()
            .AnyAsync(b => b.CategoryId == id);
    }
}