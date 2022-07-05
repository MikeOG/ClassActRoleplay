using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Server.Jobs.Criminal.Robbery.Models
{
    public sealed class RobbableStore : RobbableLocation
    {
        private Vector4 vaultLocation;
        private Vector3 vaultVector3;
        private int registersRobbed = 0;
        private bool vaultRobbed;
        private Session.Session robberSession;

        public RobbableStore(string locationName, Vector4 vaultLocation) : base(locationName)
        {
            this.vaultLocation = vaultLocation;
            vaultVector3 = new Vector3(vaultLocation.X, vaultLocation.Y, vaultLocation.Z);
        }

        public override void StartRobbery(Session.Session source)
        {

        }

        public override void EndRobbery()
        {

        }

        public override bool IsRobbable()
        {
            return registersRobbed < 2 && !vaultRobbed;
        }

        public override void StartCooldown()
        {
            
        }

        public override void EndCooldown()
        {
            
        }

        public override string GetLocationType()
        {
            return "store";
        }
    }
}
