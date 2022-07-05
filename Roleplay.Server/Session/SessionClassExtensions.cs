using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Enums;
using Roleplay.Shared;
using Roleplay.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roleplay.Server.Session
{
    public partial class Session
    {
#region Fields
        public Ped Ped => Source.Character;
        public int CharId => GetGlobalData<int>("Character.CharID");
        public AdminLevel AdminLevel 
        {
            get => GetLocalData("User.PermissionLevel", AdminLevel.User);
            set => SetLocalData("User.PermissionLevel", value);
        }
        public string FirstName => GetGlobalData("Character.FirstName", "");
        public string LastName => GetGlobalData("Character.LastName", "");

        public Vector3 Position
        {
            get
            {
                try
                {
                    //return Ped.Position;
                    if (Source != null && Ped != null)
                    {
                        return Ped.Position;
                    }
                }
                catch (Exception e)
                {
                    //Log.Error(e);
                    //Log.ToClient("LOG", $"There was an error obtaining some of your data on the server. To fix this issue please relog as soon as possible", Color.DarkViolet, Source);
                }

                return Vector3.Zero;
            }
        }

        public Dictionary<string, dynamic> CharacterSettings
        {
            get => getCharacterSettings();
            set => setCharacterSettings(value);
        }

        public Dictionary<string, dynamic> PlayerSettings
        {
            get => getPlayerSettings();
            set => setPlayerSettings(value);
        }

        public List<string> Permissions
        {
            get => getPermissions();
            set => setPermissions(value);
        }

        public Inventory Inventory => new PlayerInventory(this.GetGlobalData("Character.Inventory", ""), this);

        public int Instance
        {
            get => GetGlobalData("Character.Instance", 0);
            set => SetGlobalData("Character.Instance", value);
        }
#endregion


#region Methods
        private Dictionary<string, dynamic> getCharacterSettings()
        {
            var settings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(this.GetGlobalData("Character.Settings", ""));
            return settings ?? new Dictionary<string, dynamic>();
        }

        private void setCharacterSettings(Dictionary<string, dynamic> settings)
        {
            this.SetGlobalData("Character.Settings", JsonConvert.SerializeObject(settings));
        }

        private Dictionary<string, dynamic> getPlayerSettings()
        {
            var settings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(this.GetLocalData("User.Settings", ""));
            return settings ?? new Dictionary<string, dynamic>();
        }

        private void setPlayerSettings(Dictionary<string, dynamic> settings)
        {
            this.SetLocalData("User.Settings", JsonConvert.SerializeObject(settings));
        }

        private List<string> getPermissions()
        {
            var settings = CharacterSettings;

            return settings.ContainsKey("Permissions") ? ((string[])settings["Permissions"].Split(';')).ToList() : new List<string>();
        }

        private void setPermissions(List<string> permsList)
        {
            var charSettings = CharacterSettings;

            charSettings["Permissions"] = string.Join(";", permsList);

            CharacterSettings = charSettings;
        }
#endregion
    }
}
