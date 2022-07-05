using CitizenFX.Core;
using Roleplay.Client.Jobs;
using Roleplay.Client.Session;

namespace Roleplay.Client
{
    public abstract class ClientAccessor : BaseScript
    {
        protected Client Client { get; }
        protected Session.Session LocalSession => Client.LocalSession;
        //protected PlayerList Players => Client.PlayerList;

        private SessionManager sessionManager;
        public SessionManager Sessions => sessionManager ?? (sessionManager = Client.Get<SessionManager>());

        private JobHandler jobHandler;
        public JobHandler JobHandler => jobHandler ?? (jobHandler = Client.Get<JobHandler>());

        protected ClientAccessor(Client client)
        {
            Client = client;
        }
    }
}
