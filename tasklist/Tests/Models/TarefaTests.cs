using System.ComponentModel.DataAnnotations;
using tasklist.Models;
using Xunit;

namespace tasklist.Tests.Models
{
    public class TarefaTests
    {
        [Fact]
        public void Tarefa_WithoutDescricao_ShouldReturnValidationError()
        {
            var tarefa = new Tarefa
            {
                DataDeVencimento = DateTime.Today,
                CategoriaId = "1",
                StatusId = "aberto"
            };

            var validationResults = ValidateModel(tarefa);

            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Descricao"));
        }

        [Fact]
        public void Tarefa_WithoutDataDeVencimento_ShouldReturnValidationError()
        {
            var tarefa = new Tarefa
            {
                Descricao = "Descrição da Tarefa",
                CategoriaId = "1",
                StatusId = "aberto"
            };

            var validationResults = ValidateModel(tarefa);

            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("DataDeVencimento"));
        }

        [Fact]
        public void Tarefa_WithoutCategoriaId_ShouldReturnValidationError()
        {
            var tarefa = new Tarefa
            {
                Descricao = "Descrição da Tarefa",
                DataDeVencimento = DateTime.Today,
                StatusId = "aberto"
            };

            var validationResults = ValidateModel(tarefa);

            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("CategoriaId"));
        }

        [Fact]
        public void Tarefa_WithoutStatusId_ShouldReturnValidationError()
        {
            var tarefa = new Tarefa
            {
                Descricao = "Descrição da Tarefa",
                DataDeVencimento = DateTime.Today,
                CategoriaId = "1"
            };

            var validationResults = ValidateModel(tarefa);

            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("StatusId"));
        }

        private static System.Collections.Generic.List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new System.Collections.Generic.List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
    }
}
