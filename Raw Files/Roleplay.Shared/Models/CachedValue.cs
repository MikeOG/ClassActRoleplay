using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Shared.Models
{
    public abstract class CachedValue<T>
    {
        private DateTime lastUpdate = DateTime.MinValue;
        private readonly long timeoutInterval;

        private T cachedValue;

        protected CachedValue(long timeoutMs)
        {
            timeoutInterval = timeoutMs;
        }

        public T Value
        {
            get
            {
                if ((DateTime.Now - lastUpdate).TotalMilliseconds > timeoutInterval)
                {
                    cachedValue = Update();
                    lastUpdate = DateTime.Now;
                }
                return cachedValue;
            }
        }

        protected abstract T Update();

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
