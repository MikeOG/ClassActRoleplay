using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.MySQL;
using Roleplay.Server.Players;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Housing;
using Newtonsoft.Json;

namespace Roleplay.Server.Jobs.EmergencyServices.Police
{
    public class CPIC : JobClass
    {
        public CPIC()
        {
            CommandRegister.RegisterJobCommand("cpic|ncic", OnCPICCommand, JobType.Police);
        }

        private void OnCPICCommand(Command cmd)
        {
            var firstName = cmd.GetArgAs(0, "").ToLower();
            var lastName = cmd.GetArgAs(1, "").ToLower();
            var query = new Query<List<dynamic>>("SELECT a.FirstName, a.LastName, a.Bill, (SELECT count(*) from fivem_server_data.player_arrests c WHERE c.game_character_id = a.CharID) as arrests, (SELECT count(*) from fivem_server_data.player_tickets d WHERE d.game_character_id = a.CharID) as tickets, a.CharID, a.Home/*, (SELECT count(*) FROM mdt_data.want_warrant e WHERE e.game_character_id = a.CharID and e.serve_time is null) as warrants, b.cautioncodes*/ FROM character_data a /*left outer join mdt_data.cautioncode b on b.game_character_id = a.CharID*/" /*WHERE lower(FirstName) = @first AND lower(LastName) = @last"*/, new Dictionary<string, dynamic>
            {
                {"lower(FirstName)", firstName},
                {"lower(LastName)", lastName},
            });

            query.Execute(data =>
            {
                if (data.ElementAtOrDefault(0) == null) return;

                var playerData = data[0];

                var warrants = /*playerData.warrants == 0 ?*/ "^2None^0"; //: "^1Refer to MDT^0";

                //Dictionary<string, string> cautionCodes = playerData.cautioncodes == null ? new Dictionary<string, string>() : JsonConvert.DeserializeObject<Dictionary<string, string>>(playerData.cautioncodes);

                var cautionCodeString = "^2None^0";

                /*if (cautionCodes.Count > 0)
                {
                    cautionCodeString = "^1";

                    foreach (var kvp in cautionCodes)
                    {
                        cautionCodeString += $"{kvp.Key},";
                    }

                    cautionCodeString += "^0";

                    if (cautionCodeString == "^1^0") cautionCodeString = "^2None^0";
                }*/

                var home = "No registered property";

                if (playerData.Home != 0)
                {
                    var houseModel = HousingLocations.Locations.FirstOrDefault(o => o.HouseId == playerData.Home);

                    if (houseModel != null)
                    {
                        home = houseModel.HouseAddress;
                    }
                }

                Log.ToClient("[Cpic]", $"CPIC for {playerData.FirstName} {playerData.LastName} : {playerData.tickets} tickets(s), {playerData.arrests} arrest(s). Wants and warrants: {warrants},  Caution codes: {cautionCodeString} | Bill: ${playerData.Bill} | {Server.Get<Phones>().IntToPhoneNumber(playerData.CharID)} | {home}", ConstantColours.Red, cmd.Player);

                Log.Info(playerData.FirstName);
            });
        }
    }
}
