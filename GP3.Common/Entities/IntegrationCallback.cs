using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GP3.Common.Entities
{
    public enum IntegrationCallbackReason
    {
        LowestPrice, HighestPrice
    }

    [Index(nameof(User), nameof(CallbackReason), nameof(CallbackUrl), IsUnique = true)]
    public class IntegrationCallback
    {
        public IntegrationCallback(IntegrationCallbackReason callbackReason, string callbackUrl)
        {
            Id = 0;
            User = "";
            CallbackReason = callbackReason;
            CallbackUrl = callbackUrl;
        }

        protected IntegrationCallback(IntegrationCallback other)
        {
            this.Id = other.Id;
            this.User = (string)other.User.Clone();
            this.CallbackReason = other.CallbackReason;
            this.CallbackUrl = (string)other.CallbackUrl.Clone();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string User { get; set; } = null!;
        public IntegrationCallbackReason CallbackReason { get; set; }
        public string CallbackUrl { get; set; } = null!;
    }
}
