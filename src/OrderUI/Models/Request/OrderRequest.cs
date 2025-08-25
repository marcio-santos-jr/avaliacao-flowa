using OrderUI.Models.Enum;
using OrderUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace OrderUI.Models.Request;

public class OrderRequest
{
    [Required(ErrorMessage = "O ativo é obrigatório.")]
    public string? Symbol { get; set; }

    [Required(ErrorMessage = "O lado da ordem é obrigatório.")]
    public OrderSide Side { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(1, 99999, ErrorMessage = "A quantidade deve ser maior que zero e menor que 100.000.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório.")]
    [Range(0.01, 999.99, ErrorMessage = "O preço deve ser maior que zero e menor que 1.000.")]
    public decimal Price { get; set; }

}
