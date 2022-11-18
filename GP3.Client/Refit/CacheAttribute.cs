using Refit;

namespace GP3.Client.Refit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : PropertyAttribute
    {
        public CacheAttribute()
        {
            Duration = TimeSpan.FromMinutes(5);
            BackgroundUpdate = false;
        }

        public CacheAttribute(double durationInSeconds, bool backgroundUpdate)
        {
            Duration = TimeSpan.FromSeconds(durationInSeconds);
            BackgroundUpdate = backgroundUpdate;
        }

        public new static string Key { get => "RefitCache"; }
        public TimeSpan Duration { get; }
        public bool BackgroundUpdate { get; }
    }
}
