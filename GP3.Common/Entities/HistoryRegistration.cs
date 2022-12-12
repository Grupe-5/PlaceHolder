using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GP3.Common.Entities
{
    public enum ProviderSelection
    {
        Eso, Ignitis, Perlas
    };

    public class HistoryRegistration
    {
        public HistoryRegistration (string user, string token, ProviderSelection provider)
        {
            User = user;
            Token = token;
            Provider = provider;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string User { get; set; } = null!;
        public string Token { get; set; } = null!;
        public ProviderSelection Provider { get; set; }
    }
}
