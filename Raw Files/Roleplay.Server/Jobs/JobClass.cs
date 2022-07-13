using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Session;

namespace Roleplay.Server.Jobs
{
    public abstract class JobClass //: BaseScript
    {
        protected JobHandler JobHandler => Server.Instance.Instances.Jobs;
        protected SessionManager Sessions => Server.Instance.Instances.Session;
        protected Server Server => Server.Instance;
        protected PlayerList Players => Server.PlayerList;
        protected dynamic MySQL => Server.MySQL;
    }
}
