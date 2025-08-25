using System.ComponentModel.DataAnnotations;

namespace OrderUI.Validators
{
    public class PriceOrderAttribute : ValidationAttribute
    {
        private readonly double _min;
        private readonly double _max;

        public PriceOrderAttribute(double min, double max)
        {
            _min = min;
            _max = max;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(ErrorMessage ?? "O campo é obrigatório.");
            }

            if (value is not double doubleValue)
            {
                return ValidationResult.Success;
            }

            if (doubleValue < _min || doubleValue > _max)
                return new ValidationResult(ErrorMessage ?? $"O valor deve estar entre {_min} e {_max}.");

            double decimalValue = Math.Round(doubleValue * 100);
            if (Math.Abs(decimalValue - Math.Truncate(decimalValue)) > 0.0001)
                return new ValidationResult("O valor deve ser um múltiplo de 0.01.");

            return ValidationResult.Success;
        }
    }
}
