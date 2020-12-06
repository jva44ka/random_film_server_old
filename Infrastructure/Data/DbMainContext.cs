using Core.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Data
{
    public class DbMainContext : ApiAuthorizationDbContext<Account>
    {
        public DbMainContext(DbContextOptions<DbMainContext> options, IOptions<OperationalStoreOptions> opts) : base(options, opts)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasMany(c => c.Comments);
            modelBuilder.Entity<Account>().HasMany(c => c.UserFilms);
            modelBuilder.Entity<Account>().HasOne(c => c.CreatedBy);
            modelBuilder.Entity<Account>().HasOne(c => c.ModifiedBy);
            modelBuilder.Entity<Account>().HasOne(c => c.Avatar);
            modelBuilder.Entity<Account>().HasMany(c => c.UserSettings);

            modelBuilder.Entity<Film>().HasMany(x => x.Likes);
            modelBuilder.Entity<Film>().HasMany(x => x.FilmsGenres);
            modelBuilder.Entity<Film>().HasMany(x => x.Likes);
            modelBuilder.Entity<Film>().HasOne(c => c.CreatedBy);
            modelBuilder.Entity<Film>().HasOne(c => c.ModifiedBy);
            modelBuilder.Entity<Film>().HasOne(c => c.Preview);

            modelBuilder.Entity<Genre>();

            modelBuilder.Entity<UserFilm>().HasOne(x => x.User);
            modelBuilder.Entity<UserFilm>().HasOne(x => x.Film);

            modelBuilder.Entity<Comment>().HasOne(x => x.Film);
            modelBuilder.Entity<Comment>().HasOne(x => x.Owner);
            modelBuilder.Entity<Comment>().HasOne(c => c.CreatedBy);
            modelBuilder.Entity<Comment>().HasOne(c => c.ModifiedBy);

            modelBuilder.Entity<FilmGenre>().HasOne(x => x.Film);
            modelBuilder.Entity<FilmGenre>().HasOne(x => x.Genre);

            modelBuilder.Entity<UserSetting>();

            modelBuilder.Entity<Image>();

            modelBuilder.Entity<SelectionList>().HasMany(x => x.FilmSelectionLists);
            modelBuilder.Entity<FilmSelectionList>().HasOne(x => x.SelectionList);
            modelBuilder.Entity<FilmSelectionList>().HasOne(x => x.User);

            base.OnModelCreating(modelBuilder);
        }

    }
}
