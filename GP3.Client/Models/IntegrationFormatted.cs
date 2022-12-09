using GP3.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.Models
{
    public class IntegrationFormatted : IntegrationCallback, ICloneable
    {
        public IntegrationFormatted(IntegrationCallback obj) : base(obj) {}
        public object Clone()
        {
            var itg = (IntegrationFormatted) MemberwiseClone();
            itg.CallbackUrl = (string)CallbackUrl.Clone();
            itg.User = (string)User.Clone();
            return itg;
        }
    }
}
