using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Enums;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Housing;
using Newtonsoft.Json;

namespace Roleplay.Server.Admin
{
    public class Bans : ServerAccessor
    {
        private List<string> banTags = new List<string>();

        public Bans(Server server) : base(server)
        {
            CommandRegister.RegisterAdminCommand("ban", OnBanCommand, AdminLevel.Admin);
            loadBanTags();
        }

        public async Task AddBan(Session.Session bannedSession, string banner, string banReason, DateTime banExpires)
        {
            try
            {
                var banTag = await generateBanTag();
                var identifiers = /*JsonConvert.DeserializeObject<List<string>>(bannedSession.GetLocalData("User.Identifiers", bannedSession.SteamIdentifier ?? ""));

                if (identifiers.Count == 0)
                {
                    identifiers = bannedSession.Source.Identifiers.ToList();
                }*/ bannedSession.Source.Identifiers;

                foreach (var iden in identifiers)
                {
                    try
                    { 
                        MySQL.execute("INSERT INTO user_bans (`BannedIdentifier`, `BannedName`, `Banner`, `BanReason`, `BannedExpires`, `BanTag`) VALUES (@iden, @bannedname, @bannername, @reason, @expires, @tag)", new Dictionary<string, dynamic>
                        {
                            {"@iden", iden },
                            {"@bannedname", bannedSession.PlayerName },
                            {"@bannername", banner},
                            {"@reason", banReason },
                            {"@expires", banExpires.ToString("yyyy-MM-dd HH:mm:ss") },
                            {"@tag", banTag },
                        }, new Action<dynamic>(rows =>
                        {
                            Log.Verbose($"Inserted a ban for identifier {iden} with a ban tag of {banTag}");
                        }));
                    }
                    catch { }
                }
            }
            catch { }

        }

        private async void OnBanCommand(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);
            var targetSession = Sessions.GetPlayer(targetId);

            if (targetSession == null) return;

            var banTime = getBanExpireTime(cmd.GetArgAs(1, "1d").ToLower());
            cmd.Args.RemoveAt(0);
            cmd.Args.RemoveAt(0);

            var banReason = string.Join(" ", cmd.Args);
            var banner = cmd.Source == 0 ? "RCON" : cmd.Session.PlayerName;

            await AddBan(targetSession, banner, banReason, banTime);
            targetSession.Source.Drop($"Banned: {banReason}");
        }

        private DateTime getBanExpireTime(string banTime)
        {
            var currentTime = DateTime.UtcNow;
            var epochTime = MiscHelpers.GetEpochTime();
            var banExpire = currentTime;

            try
            {
                if (banTime.Contains("s") || banTime.Contains("m") || banTime.Contains("h") || banTime.Contains("d"))
                {
                    var timeToAdd = TimeSpan.FromTicks(0);

                    if (banTime.Contains("s"))
                    {
                        //timeToAdd = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, Convert.ToInt32(banTime.Replace("s", "")));
                        timeToAdd = TimeSpan.FromSeconds(Convert.ToInt32(banTime.Replace("s", "")));
                    }
                    else if (banTime.Contains("m"))
                    {
                        //timeToAdd = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, Convert.ToInt32(banTime.Replace("m", "")), currentTime.Second);
                        timeToAdd = TimeSpan.FromMinutes(Convert.ToInt32(banTime.Replace("m", "")));
                    }
                    else if (banTime.Contains("h"))
                    {
                        //timeToAdd = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, Convert.ToInt32(banTime.Replace("h", "")), currentTime.Minute, currentTime.Second);
                        timeToAdd = TimeSpan.FromHours(Convert.ToInt32(banTime.Replace("h", "")));
                    }
                    else
                    {
                        //timeToAdd = new DateTime(currentTime.Year, currentTime.Month, Convert.ToInt32(banTime.Replace("d", "")), currentTime.Hour, currentTime.Minute, currentTime.Second);
                        timeToAdd = TimeSpan.FromDays(Convert.ToInt32(banTime.Replace("d", "")));
                    }

                    banExpire = currentTime.Add(timeToAdd);
                }
                else
                {
                    var time = Convert.ToInt32(banTime);

                    if (time == -1)
                        banExpire = DateTime.MaxValue;
                    else
                        banExpire = currentTime.Add(TimeSpan.FromSeconds(time));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return banExpire;
        }

        private async void loadBanTags()
        {
            await BaseScript.Delay(5000);
            MySQL.execute("SELECT BanTag FROM user_bans", new Dictionary<string, dynamic>(),
                new Action<List<dynamic>>(result =>
                {
                    Log.Debug($"Loading ban tags");
                    result.ForEach(tag =>
                    {
                        if(!banTags.Contains(tag.BanTag))
                        {
                            Log.Debug($"Adding ban tag: {tag.BanTag}");
                            banTags.Add(tag.BanTag);
                        }
                    });
                }));
        }

        private async Task<string> generateBanTag()
        {
            var banTag = MiscHelpers.CreateRandomString(6);
            while (banTags.Contains(banTag))
            {
                banTag = MiscHelpers.CreateRandomString(6);
                await BaseScript.Delay(0);
            }
            banTags.Add(banTag);

            return banTag;
        }
    }
}
