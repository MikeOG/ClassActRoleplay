using CitizenFX.Core;
using Roleplay.Server.Jobs;
using Roleplay.Server.Session;

namespace Roleplay.Server
{
    public abstract class ServerAccessor : BaseScript
    {
        protected Server Server { get; }
        protected dynamic MySQL => Server.MySQL;
        //protected PlayerList Players => Server.PlayerList;

        private SessionManager sessionManager;
        public SessionManager Sessions => sessionManager ?? (sessionManager = Server.Get<SessionManager>());

        private JobHandler jobHandler;
        public JobHandler JobHandler => jobHandler ?? (jobHandler = Server.Get<JobHandler>());

        protected ServerAccessor(Server server)
        {
            Server = server;
        }
    }
}