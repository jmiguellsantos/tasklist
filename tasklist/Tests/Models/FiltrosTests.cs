using tasklist.Models;
using Xunit;

namespace tasklist.Tests.Models
{
    public class FiltrosTests
    {
        [Theory]
        [InlineData(null, "todos", "todos", "todos")]
        [InlineData("1-futuro-aberto", "1", "futuro", "aberto")]
        public void Constructor_ShouldInitializeProperties(string filtroString, string expectedCategoriaId, string expectedVencimento, string expectedStatusId)
        {
            var filtros = new Filtros(filtroString);

            Assert.Equal(expectedCategoriaId, filtros.CategoriaId);
            Assert.Equal(expectedVencimento, filtros.Vencimento);
            Assert.Equal(expectedStatusId, filtros.StatusId);
        }

        [Theory]
        [InlineData("1-futuro-aberto", true, true, true)]
        [InlineData("todos-futuro-aberto", false, true, true)]
        [InlineData("1-todos-aberto", true, false, true)]
        [InlineData("1-futuro-todos", true, true, false)]
        public void Property_TemCategoria_TemVencimento_TemStatus_ShouldReturnCorrectValues(string filtroString, bool expectedTemCategoria, bool expectedTemVencimento, bool expectedTemStatus)
        {
            var filtros = new Filtros(filtroString);

            Assert.Equal(expectedTemCategoria, filtros.TemCategoria);
            Assert.Equal(expectedTemVencimento, filtros.TemVencimento);
            Assert.Equal(expectedTemStatus, filtros.TemStatus);
        }

        [Theory]
        [InlineData("futuro", false, true, false)]
        [InlineData("passado", true, false, false)]
        [InlineData("hoje", false, false, true)]
        public void Property_EPassado_EFuturo_EHoje_ShouldReturnCorrectValues(string vencimento, bool expectedEPassado, bool expectedEFuturo, bool expectedEHoje)
        {
            var filtroString = "1-" + vencimento + "-aberto";
            var filtros = new Filtros(filtroString);

            Assert.Equal(expectedEPassado, filtros.EPassado);
            Assert.Equal(expectedEFuturo, filtros.EFuturo);
            Assert.Equal(expectedEHoje, filtros.EHoje);
        }


        [Fact]
        public void VencimentoValoresFiltro_ShouldContainExpectedValues()
        {
            var expectedValues = new Dictionary<string, string>
            {
                {"futuro", "Futuro" },
                {"passado", "Passado" },
                {"hoje", "Hoje" }
            };

            var actualValues = Filtros.VencimentoValoresFiltro;

            Assert.Equal(expectedValues, actualValues);
        }
    }
}
