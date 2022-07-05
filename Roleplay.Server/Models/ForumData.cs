using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.HTTP;
using Roleplay.Server.Models;
using Roleplay.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Server.Session
{
    /*public partial class Session
    {
        public ForumData ForumData = null;
        private bool isUpdatingForumData = false;

        public async Task<ForumData> LoadForumData()
        {
            if (isUpdatingForumData) return null;

            isUpdatingForumData = true;
            
            var forumRequest = await new HTTPRequest().Request($"https://forum.Roleplayrp.com/api/providers/steam/{SteamID64}/?api_bypass_permissions=1", "GET", "", new Dictionary<string, string>
            {
                {
                    "Content-Type", "application/x-www-form-urlencoded"
                },
                {
                    "XF-Api-Key", "ssyWvsdxW9fRRNOK3ixeEh6BNVJEN2Rg"
                }
            });

            if (forumRequest.status == HttpStatusCode.OK)
            {
                Log.Debug($"Request to forum was successful continuing");
                JObject responseObject = (dynamic)JsonConvert.DeserializeObject(forumRequest.content); // Keep this as a JObject because i'm dumb and don't know how to deserialize it properly

                if (responseObject.GetValue("user") != null)
                {
                    ForumData = new ForumData((dynamic)responseObject.GetValue("user"));
                }
            }
            else
            {
                Log.Debug($"Recieved an invalid response from API servers. Status code: {forumRequest.status}");
            }

            isUpdatingForumData = false;

            return ForumData;
        }
    }
}

namespace Roleplay.Server.Models
{
    public class ForumData
    {
        public static List<int> EmergencyServiceGroupIds = GetConvar("mg_emergencyServicesGroupIds", "6,7,15,16").Split(',').Select(int.Parse).ToList();
        public static int PriorityRevokedGroupId = GetConvarInt("mg_revokedPriorityGroupId", 21);

        public int ForumId => forumDataObject.GetValue("user_id").ToObject<int>();

        public Dictionary<string, string> ConnectedAccounts => forumDataObject.GetValue("connected_accounts").ToObject<Dictionary<string, string>>();
        public long SteamId => ConnectedAccounts.ContainsKey("steam") ? Convert.ToInt64(ConnectedAccounts["steam"]) : 0;
        public long DiscordId => ConnectedAccounts.ContainsKey("discord") ? Convert.ToInt64(ConnectedAccounts["discord"]) : 0;
        public bool IsFullyLinked => ConnectedAccounts.ContainsKey("steam") && ConnectedAccounts.ContainsKey("discord");//!(SteamId == 0 && DiscordId == 0);

        public List<int> GroupIds => forumDataObject.GetValue("secondary_group_ids").ToObject<List<int>>();
        public bool IsWhitelisted => GroupIds.Contains(GetConvarInt("mg_whitelistGroupId", 17));
        public bool IsEmergencyService => GroupIds.Intersect(EmergencyServiceGroupIds).Any() && !GroupIds.Contains(PriorityRevokedGroupId);
        public bool IsStaff => forumDataObject.GetValue("is_staff").ToObject<bool>() && forumDataObject.GetValue("is_admin").ToObject<bool>();
        public bool IsSuperAdmin => IsStaff && forumDataObject.GetValue("is_super_admin").ToObject<bool>();

        private JObject forumDataObject;

        public ForumData(JObject forumData)
        {
            forumDataObject = forumData;
        }
    }*/
}
