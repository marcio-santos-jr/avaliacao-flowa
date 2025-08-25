﻿using OrderGenerator.Models.Enum;

namespace OrderGenerator.Models
{
    public class OrderRequest
    {
        public string Symbol { get; set; }
        public OrderSide Side { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
