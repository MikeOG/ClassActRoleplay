using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Session;
using Roleplay.Shared;

namespace Roleplay.Server.Permissions
{
    public static class Permissions
    {
        /// <summary>
        /// Checks if the specified <see cref="Session"/> has the specified permissions
        /// </summary>
        /// <param name="playerSession">Specified session of target player</param>
        /// <param name="permissions">String of job permissions split by ;</param>
        /// <returns></returns>
        public static bool HasPermissions(Session.Session playerSession, string permissions)
        {
            return HasPermissions(playerSession, permissions.Split(';').ToList());
        }

        /// <summary>
        /// Checks if the specified <see cref="Session"/> has the specified permissions
        /// </summary>
        /// <param name="playerSession">Specified session of target player</param>
        /// <param name="permissions">List of permissions to check</param>
        /// <returns></returns>
        public static bool HasPermissions(Session.Session playerSession, List<string> permissions)
        {
            var playerPerms = playerSession.Permissions;

            return permissions.Contains("***") || permissions.All(playerPerms.Contains);
        }

        /// <summary>
        /// Lists all permissions the player currently has. Splitting them up by their respective branches
        /// </summary>
        /// <param name="playerSession"></param>
        public static void ListPermissions(this Session.Session playerSession)
        {
            var perms = playerSession.Permissions;

            var playerPermTrees = PermissionTree.CreateTrees(perms);

            foreach (var tree in playerPermTrees)
            {
                Log.Info($"Permission tree - {tree.Name} start");
                foreach (var perm in tree.Permissions)
                {
                    Log.Info(perm);
                }
                Log.Info($"Permission tree - {tree.Name} end");
            }
        }
    }
}
