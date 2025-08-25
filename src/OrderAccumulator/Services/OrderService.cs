using OrderAccumulator.Interfaces.Repositories;
using OrderAccumulator.Interfaces.Service;
using OrderAccumulator.Models;
using OrderAccumulator.Models.Enum;

namespace OrderAccumulator.Services
{
    public class OrderService : IOrderService
    {
        private readonly IExposureRepository _exposureRepository;
        private readonly ILogger<OrderService> _logger;
        private readonly decimal _maxExposureLimit;

        public OrderService(IExposureRepository exposureRepository, ILogger<OrderService> logger, IConfiguration configuration)
        {
            _exposureRepository = exposureRepository;
            _logger = logger;

            _maxExposureLimit = configuration.GetValue<decimal>("MaxExposureLimit", 100_000_000M);
            if (_maxExposureLimit <= 0)
                _maxExposureLimit = 100_000_000M;
        }

        public async Task<OrderProcessingResult> ProcessOrderAsync(Order order)
        {
            try
            {
                decimal notionalOrderValue = order.Quantity * order.Price;
                var currentExposureResult = await _exposureRepository.GetExposureBySymbol(order.Symbol);
                decimal currentExposureAmount = currentExposureResult?.CurrentExposureAmount ?? 0m;

                decimal changeInExposure = 0m;
                string? rejectionReason;

                switch (order.Side)
                {
                    case OrderSide.Buy:
                        changeInExposure = notionalOrderValue;
                        break;
                    case OrderSide.Sell:
                        if (notionalOrderValue > currentExposureAmount)
                        {
                            rejectionReason = $"Valor indisponível. Você tentou vender {notionalOrderValue:N2} mas possui {currentExposureAmount:N2}";
                            _logger.LogWarning($"Venda rejeitada para ClOrdID '{order.ClOrdID}': {rejectionReason}");
                            return new OrderProcessingResult
                            {
                                IsAccepted = false,
                                ClOrdID = order.ClOrdID,
                                Symbol = order.Symbol,
                                Side = order.Side,
                                Quantity = order.Quantity,
                                Price = order.Price,
                                RejectionReason = rejectionReason
                            };
                        }
                        changeInExposure = -notionalOrderValue;
                        break;
                    default: 
                        rejectionReason = "Tipo da ordem inválida.";
                        _logger.LogWarning($"Tipo da ordem ('{order.Side}') não suportada. ClOrdID: {order.ClOrdID}.");
                        return new OrderProcessingResult
                        {
                            IsAccepted = false,
                            ClOrdID = order.ClOrdID,
                            Symbol = order.Symbol,
                            Side = order.Side,
                            Quantity = order.Quantity,
                            Price = order.Price,
                            RejectionReason = rejectionReason
                        };
                }

                decimal projectedExposure = currentExposureAmount + changeInExposure;

                if (projectedExposure > _maxExposureLimit)
                {
                    rejectionReason = $"Limite de exposição ({_maxExposureLimit:N2}) excedido. Exposição atual: {currentExposureAmount:N2}, Proposta: {projectedExposure:N2}.";
                    _logger.LogWarning($"Ordem '{order.ClOrdID}' rejeitada: {rejectionReason}");
                    return new OrderProcessingResult
                    {
                        IsAccepted = false,
                        ClOrdID = order.ClOrdID,
                        Symbol = order.Symbol,
                        Side = order.Side,
                        Quantity = order.Quantity,
                        Price = order.Price,
                        RejectionReason = rejectionReason,
                        NewProjectedExposure = projectedExposure
                    };
                }

                await _exposureRepository.UpdateExposure(order.Symbol, changeInExposure);
                _logger.LogInformation($"Exposição atualizada para {order.Symbol}. Nova exposição: {projectedExposure:N2}.");

                return new OrderProcessingResult
                {
                    IsAccepted = true,
                    ClOrdID = order.ClOrdID,
                    Symbol = order.Symbol,
                    Side = order.Side,
                    Quantity = order.Quantity,
                    Price = order.Price,
                    NewProjectedExposure = projectedExposure
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao processar ordem para ClOrdID '{order.ClOrdID}'.");
                return new OrderProcessingResult
                {
                    IsAccepted = false,
                    ClOrdID = order.ClOrdID,
                    Symbol = order.Symbol,
                    Side = order.Side,
                    Quantity = order.Quantity,
                    Price = order.Price,
                    RejectionReason = "Erro interno do servidor ao processar a ordem.",
                };
            }
        }
    }
}
