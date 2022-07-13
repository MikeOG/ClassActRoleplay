using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Permissions
{
    // TODO finish permissions tree stuff sooner or later
    public class PermissionTree
    {
        public string Name { get; }
        public List<string> Permissions = new List<string>();

        public PermissionTree(List<string> permissions, string permissionBase)
        {
            Name = permissionBase.ToLower().FirstLetterToUpper();

            Log.Debug($"Creating permissions tree {Name}");
            foreach (var perm in permissions)
            {
                if (perm.Contains(permissionBase))
                {
                    Permissions.Add(perm);

                    Log.Debug($"Adding permission {perm} to permissions tree {Name}");
                }
            }
        }

        /// <summary>
        /// Creates all correspodning <see cref="PermissionTree"/> objects from a list of permissions
        /// </summary>
        /// <param name="permissions">List of permissions the trees want to be created from</param>
        /// <returns>All <see cref="PermissionTree"/> objects that can be created from the permissions list</returns>
        public static List<PermissionTree> CreateTrees(List<string> permissions)
        {
            var permissionBases = new List<string>();
            var treeList = new List<PermissionTree>();

            foreach (var perm in permissions)
            {
                var permSplit = perm.Split('.');

                var baseIndex = permSplit[0] == "business" ? 1 : 0;

                if (!permissionBases.Contains(permSplit[baseIndex]))
                {
                    permissionBases.Add(permSplit[baseIndex]);
                }
            }

            foreach(var permBase in permissionBases)
            {
                treeList.Add(new PermissionTree(permissions, permBase));
            }

            return treeList;
        }
    }
}
