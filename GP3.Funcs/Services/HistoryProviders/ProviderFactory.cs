using GP3.Common.Entities;
using System;

namespace GP3.Funcs.Services.HistoryProviders
{
    public class ProviderFactory
    {
        private readonly Eso _eso;
        private readonly Ignitis _ignitis;
        private readonly Perlas _perlas;
        public ProviderFactory(Eso eso, Ignitis ignitis, Perlas perlas)
        {
            _eso = eso;
            _ignitis = ignitis;
            _perlas = perlas;
        }

        public IProvider GetProvider(ProviderSelection selection)
        {
            return selection switch
            {
                ProviderSelection.Eso => _eso,
                ProviderSelection.Ignitis => _ignitis,
                ProviderSelection.Perlas => _perlas,
                _ => throw new ArgumentException("Invalid provider!"),
            };
        }
    }
}
