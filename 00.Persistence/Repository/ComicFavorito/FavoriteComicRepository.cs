using Domain.Entity;
using Domain.Interface;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Repository.ComicFavorito;

public class FavoriteComicRepository(AppDbContext context) : IFavoriteComicsRepository
{
    public async Task AddFavoriteComic(FavoriteComic comic)
    {
        context.FavoriteComics.Add(comic);
        await context.SaveChangesAsync();
    }

    public async Task RemoveFavoriteComic(int userId, int comicId)
    {
        var favorite = await context.FavoriteComics
            .FirstOrDefaultAsync(fc => fc.UserId == userId && fc.ComicId == comicId);

        if (favorite != null)
        {
            context.FavoriteComics.Remove(favorite);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<FavoriteComic>> GetFavoriteComicsByUserId(int userId)
    {
        return await context.FavoriteComics
            .Include(x => x.NaviUser)
            .Where(fc => fc.UserId == userId)
            .ToListAsync();
    }
}