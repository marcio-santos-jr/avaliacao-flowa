using FluentValidation;
using OrderGenerator.Models;

namespace OrderGenerator.Validators;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("O símbolo é obrigatório.")
            .Must(symbol => new[] { "PETR4", "VALE3", "VIIA4" }.Contains(symbol))
            .WithMessage("Escolha um ativo entre PETR4, VALE3 ou VIIA4.");

        RuleFor(x => x.Side)
            .IsInEnum()
            .WithMessage("O lado da ordem deve ser Compra ou Venda.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.")
            .LessThan(100_000).WithMessage("A quantidade deve ser menor que 100.000.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que 0.")
            .LessThan(1000).WithMessage("O preço deve ser menor que 1000.");
    }
}