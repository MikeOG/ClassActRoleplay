using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared.Enums;

namespace Roleplay.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DynamicTickAttribute : Attribute
    {
        public TickUsage Usage = TickUsage.All;

        public DynamicTickAttribute(TickUsage usage = TickUsage.All)
        {
            Usage = usage;
        }
    }
}
