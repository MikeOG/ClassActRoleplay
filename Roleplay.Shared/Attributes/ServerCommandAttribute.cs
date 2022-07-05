using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if CLIENT

#elif SERVER
using Roleplay.Server.Enums;
#endif

using Roleplay.Shared.Enums;

namespace Roleplay.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    class ServerCommandAttribute : Attribute
    {
        public string Command;
        public bool IsRestriced;
        public bool SkipDutyCheck = false;
        public JobType JobType = (JobType)(-1);

        public ServerCommandAttribute(string commandName, JobType usage, bool skipDutyCheck = false)
        {
            Command = commandName;
            JobType = usage;
            IsRestriced = true;
            SkipDutyCheck = skipDutyCheck;
        }

        public ServerCommandAttribute(string commandName, bool isRestriced = false)
        {
            Command = commandName;
            IsRestriced = isRestriced;
        }
#if SERVER
        public AdminLevel AdminLevel = (AdminLevel)(-1);

        public ServerCommandAttribute(string commandName, AdminLevel usage)
        {
            Command = commandName;
            AdminLevel = usage;
            IsRestriced = true;
        }
#endif
    }
}
