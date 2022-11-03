using System.ComponentModel.DataAnnotations;

namespace GP3.Common.Entities
{
    public enum IntegrationCallbackReason
    {
        LowestPrice, HighestPrice
    }

    public class IntegrationCallback
    {
        [Key]
        public Guid Id { get; set; }
        public IntegrationCallbackReason CallbackReason { get; set; }
        public string CallbackUrl { get; set; } = null!;
    }
}
