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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string User { get; set; } = null!;
        public IntegrationCallbackReason CallbackReason { get; set; }
        public string CallbackUrl { get; set; } = null!;
    }
}
