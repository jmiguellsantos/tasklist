using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tasklist.Controllers;
using tasklist.Data;
using tasklist.Models;
using Xunit;

namespace tasklist.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexReturnsViewResultWithCorrectModel()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Categorias.Add(new Categoria { CategoriaId = "1", Nome = "Categoria Teste" });
                context.Statuses.Add(new Status { StatusId = "aberto", Nome = "Aberto" });
                context.Tarefas.Add(new Tarefa { Id = 1, Descricao = "Tarefa de teste", CategoriaId = "1", StatusId = "aberto", DataDeVencimento = DateTime.Today });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new HomeController(context);

                var result = controller.Index(null) as ViewResult;
                var model = result.ViewData.Model as List<Tarefa>;

                Assert.NotNull(result);
                Assert.NotNull(result.ViewData["Filtros"]);
                Assert.NotNull(result.ViewData["Categorias"]);
                Assert.NotNull(result.ViewData["Status"]);
                Assert.NotNull(result.ViewData["VencimentoValores"]);
                Assert.NotNull(model);
                Assert.True(model.Count > 0);
            }
        }

        [Fact]
        public void AdicionarReturnsViewResultWithCorrectModel()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Categorias.Add(new Categoria { CategoriaId = "1", Nome = "Categoria Teste" });
                context.Statuses.Add(new Status { StatusId = "aberto", Nome = "Aberto" });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new HomeController(context);
                var result = controller.Adicionar() as ViewResult;

                Assert.NotNull(result);
                Assert.NotNull(result.ViewData["Categorias"]);
                Assert.NotNull(result.ViewData["Status"]);
                Assert.NotNull(result.Model);
                Assert.IsType<Tarefa>(result.Model);
            }
        }


        [Fact]
        public void FiltrarRedirectsToActionIndex()
        {
            var controller = new HomeController(null);
            var filtro = new string[] { "filtro1", "filtro2" };

            var result = controller.Filtrar(filtro);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("filtro1-filtro2", redirectToActionResult.RouteValues["ID"]);
        }

        [Fact]
        public void MarcarCompleto_ReturnsRedirectToAction_Index()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Tarefas.Add(new Tarefa { Id = 1, CategoriaId = "1", Descricao = "Descrição da Tarefa", DataDeVencimento = DateTime.Today, StatusId = "aberto" });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new HomeController(context);

                var result = controller.MarcarCompleto("1", new Tarefa { Id = 1, CategoriaId = "1", Descricao = "Descrição da Tarefa", DataDeVencimento = DateTime.Today, StatusId = "aberto" });

                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
                Assert.True(redirectToActionResult.RouteValues.ContainsKey("ID"));

                var tarefa = context.Tarefas.FirstOrDefault(t => t.Id == 1);
                Assert.NotNull(tarefa);
                Assert.Equal("completo", tarefa.StatusId);
            }
        }


        [Fact]
        public void AdicionarReturnsRedirectToActionIndexWhenModelStateIsValid()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Categorias.Add(new Categoria { CategoriaId = "1", Nome = "Categoria Teste" });
                context.Statuses.Add(new Status { StatusId = "aberto", Nome = "Aberto" });
                context.SaveChanges();
            }

            var tarefa = new Tarefa { Descricao = "Descrição da Tarefa", DataDeVencimento = DateTime.Today, CategoriaId = "1", StatusId = "aberto" };

            using (var context = new AppDbContext(options))
            {
                var controller = new HomeController(context);

                var result = controller.Adicionar(tarefa);

                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                var tarefaInDatabase = context.Tarefas.FirstOrDefault(t => t.Descricao == "Descrição da Tarefa");

                Assert.NotNull(redirectToActionResult);
                Assert.Equal("Index", redirectToActionResult.ActionName);
                Assert.NotNull(tarefaInDatabase);
            }
        }


        [Fact]
        public void Adicionar_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Categorias.Add(new Categoria { CategoriaId = "1", Nome = "Categoria Teste" });
                context.Statuses.Add(new Status { StatusId = "aberto", Nome = "Aberto" });
                context.SaveChanges();

                var tarefa = new Tarefa { Descricao = "", DataDeVencimento = DateTime.Today, CategoriaId = "1", StatusId = "aberto" };

                var controller = new HomeController(context);
                controller.ModelState.AddModelError("Descricao", "Preencha a descrição!");

                var result = controller.Adicionar(tarefa);
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsType<Tarefa>(viewResult.Model);
                Assert.Equal(tarefa, model);
                Assert.NotNull(viewResult.ViewData["Categorias"]);
                Assert.NotNull(viewResult.ViewData["Status"]);
            }
        }

        [Fact]
        public void DeletarCompletos_ReturnsRedirectToAction_Index()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Tarefas.AddRange(
                    new Tarefa { Id = 1, StatusId = "completo", CategoriaId = "1", Descricao = "Descrição da Tarefa 1", DataDeVencimento = DateTime.Today.AddDays(-1) },
                    new Tarefa { Id = 2, StatusId = "aberto", CategoriaId = "2", Descricao = "Descrição da Tarefa 2", DataDeVencimento = DateTime.Today }
                );
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new HomeController(context);

                var result = controller.DeletarCompletos("1");

                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
                Assert.Equal("1", redirectToActionResult.RouteValues["ID"]);

                var tarefasInDatabase = context.Tarefas.ToList();
                Assert.Single(tarefasInDatabase);
                Assert.Equal(2, tarefasInDatabase.First().Id);
            }
        }
    }
}
