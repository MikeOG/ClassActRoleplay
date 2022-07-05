using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Session;

namespace Roleplay.Server.Helpers
{
    public class SessionList : IEnumerable<Session.Session>
    {
        public IEnumerator<Session.Session> GetEnumerator()
        {
            var currentPlayers = Server.Instance.Get<SessionManager>().PlayerList;

            foreach (var player in currentPlayers)
            {
                yield return player;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Session.Session this[int serverId] => this.FirstOrDefault(o => o.ServerID == serverId);
        public Session.Session this[string playerName] => this.FirstOrDefault(o => o.PlayerName == playerName);
    }
}
