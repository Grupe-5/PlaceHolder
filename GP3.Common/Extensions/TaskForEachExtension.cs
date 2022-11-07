using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Common.Extensions
{
    public static class TaskForEachExtension
    {
        public static Task ForEachAsync<T>(this IEnumerable<T> sequence, Func<T, Task> action)
        {
            return Task.WhenAll(sequence.Select(action));
        }
    }
}
