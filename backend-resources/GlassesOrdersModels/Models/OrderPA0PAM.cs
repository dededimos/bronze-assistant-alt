using CommonInterfacesBronze;

namespace GlassesOrdersModels.Models
{
    public class OrderPA0PAM
    {
        public string OrderId { get; set; } = string.Empty;
        public List<ICodeable> Items { get; set; } = new();
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

    }


}
