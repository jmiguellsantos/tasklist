using Microsoft.EntityFrameworkCore;
using tasklist.Data;
using tasklist.Models;
using Xunit;

namespace tasklist.Tests.Data
{
    public class AppDbContextTests
    {
        [Fact]
        public void CanSeedData()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Categorias.AddRange(
                    new Categoria { CategoriaId = "1", Nome = "Categoria1" },
                    new Categoria { CategoriaId = "2", Nome = "Categoria2" },
                    new Categoria { CategoriaId = "3", Nome = "Categoria3" },
                    new Categoria { CategoriaId = "4", Nome = "Categoria4" },
                    new Categoria { CategoriaId = "5", Nome = "Categoria5" }
                );
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                Assert.Equal(5, context.Categorias.Count());
            }
        }
    }
}
