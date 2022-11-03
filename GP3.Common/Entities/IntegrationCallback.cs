namespace GP3.Common.Entities
{
    public enum IntegrationCallbackReason
    {
        LowestPrice, HighestPrice
    }

    public class IntegrationCallback
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IntegrationCallbackReason CallbackReason { get; set; }
        public string CallbackUrl { get; set; } = null!;
    }
}
