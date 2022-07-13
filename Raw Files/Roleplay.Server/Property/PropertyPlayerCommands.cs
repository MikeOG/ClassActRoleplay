using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Helpers;
using Roleplay.Server.Realtor;
using Roleplay.Shared;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Property
{
    public class PropertyPlayerCommands : ServerAccessor
    {
        public PropertyPlayerCommands(Server server) : base(server)
        {

        }

        [ServerCommand("property_givekey")]
        private void OnGivePropertyKeys(Command cmd)
        {
            Log.Debug($"{cmd.Session.PlayerName} is attempting to give property keys");

            var property = cmd.Session.GetClosestOwnedProperty(12);
            var closePlayer = cmd.Session.GetClosestPlayer(12);

            Log.Debug($"property is null: {property == null}");
            Log.Debug($"closePlayer is null: {closePlayer == null}");

            if (property == null || closePlayer == null)
            {
                cmd.Session.Message("[Property]", $"You are not near enough to a property and or person to do this", ConstantColours.Housing);
                return;
            }

            Server.Get<PropertyManager>().AddPropertyTenant(property, closePlayer.CharId);
        }

        [ServerCommand("property_addowner")]
        private void OnAddPropertyOwner(Command cmd)
        {
            Log.Debug($"{cmd.Session.PlayerName} is attempting to give property keys and extended ownership");

            var property = cmd.Session.GetClosestOwnedProperty(12);
            var closePlayer = cmd.Session.GetClosestPlayer(12);

            Log.Debug($"property is null: {property == null}");
            Log.Debug($"closePlayer is null: {closePlayer == null}");

            if (property == null || closePlayer == null)
            {
                cmd.Session.Message("[Property]", $"You are not near enough to a property and or person to do this", ConstantColours.Housing);
                return;
            }

            Server.Get<PropertyManager>().RemovePropertyTenant(property, closePlayer.CharId, true); // otherwise won't work properly
            Server.Get<PropertyManager>().AddPropertyTenant(property, closePlayer.CharId, "co-owner");
        }

        [ServerCommand("property_revokekey|revokeowner")]
        private void OnRevokePropertyPermission(Command cmd)
        {
            Log.Debug($"{cmd.Session.PlayerName} is attempting to revoke someones property permissions");

            var property = cmd.Session.GetClosestOwnedProperty(12);
            var closePlayer = cmd.Session.GetClosestPlayer(12);
            var targetId = cmd.GetArgAs(0, 0);

            Log.Debug($"property is null: {property == null}");
            Log.Debug($"closePlayer is null: {closePlayer == null}");

            if (property == null)
            {
                cmd.Session.Message("[Property]", $"You are not near enough to a property to do this", ConstantColours.Housing);
                return;
            }

            if (closePlayer != null)
            {
                targetId = closePlayer.CharId;
            }

            if (targetId == 0)
            {
                cmd.Session.Message("[Property]", $"You are not near anyone or you inputted an invalid characterID", ConstantColours.Housing);
                return;
            }

            Server.Get<PropertyManager>().RemovePropertyTenant(property, targetId);
        }

        [ServerCommand("property_setspawn")]
        private void OnSetPropertySpawn(Command cmd)
        {
            Log.Debug($"{cmd.Session.PlayerName} is attempting to set their spawn to a property");

            var property = cmd.Session.GetClosestPropertyWithTenancy();

            if (property == null)
            {
                cmd.Session.Message("[Property]", $"You do not have the appropiate permissions to set your spawn to this property or you are not near a property", ConstantColours.Housing);
                return;
            }

            Log.Verbose($"Setting {cmd.Session.PlayerName} spawn locaiton to property {property.Address} ({property.PropertyId})");

            var characterSettings = cmd.Session.CharacterSettings;
            characterSettings["SpawnLocation"] = $"home-{property.PropertyId}";
            cmd.Session.CharacterSettings = characterSettings;

            cmd.Session.Message("[Property]", $"Set spawn location to {property.Address}", ConstantColours.Housing);
        }

        [ServerCommand("property_leave")]
        private void OnLeaveProperty(Command cmd)
        {
            var session = cmd.Session;

            if (session.IsInProperty)
            {
                var property = session.PropertyCurrentlyInside;
                var playerPos = session.Position;
                var closestExit = PropertyManager.HouseInteriorLocations.Select(o => o.DistanceToSquared(playerPos)).OrderByDescending(o => o).First();

                if (closestExit < 6.0f || closestExit > 600.0f)
                {
                    Server.Get<PropertyManager>().AttemptLeaveProperty(session);
                }
            }
        }
    }
}
