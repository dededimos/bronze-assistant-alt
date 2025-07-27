using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassesOrdersModels.Models.SubModels
{
    public class GlassesOrderSmall
    {
        public string OrderId { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Undefined;
        public int GlassesCount { get; set; }
        public int CabinsCount { get; set; }
        public int PA0Count { get; set; }
        public DateTime Created { get; set; } = DateTime.MinValue;
    }
}
