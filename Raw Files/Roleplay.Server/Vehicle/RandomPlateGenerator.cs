using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;

namespace Roleplay.Server.Vehicle
{
    public class RandomPlateGenerator : ServerAccessor
    {
        public static List<string> PlatesInUse = new List<string>();
        private static Random random = new Random();

        public RandomPlateGenerator(Server server) : base(server)
        {
            loadCurrentPlates();
        }

        public static async Task<string> GenerateRandomPlate()
        {
            var rand = new Random();
            var plateArray = new[]
            {
                rand.Next(1, 9).ToString(),
                rand.Next(1, 9).ToString(),
                GetRandomLetter(),
                GetRandomLetter(),
                GetRandomLetter(),
                rand.Next(1, 9).ToString(),
                rand.Next(1, 9).ToString(),
                rand.Next(1, 9).ToString(),
            };
            var plateText = string.Join("", plateArray);

            do
            {
                plateArray = new[]
                {
                    rand.Next(1, 9).ToString(),
                    rand.Next(1, 9).ToString(),
                    GetRandomLetter(),
                    GetRandomLetter(),
                    GetRandomLetter(),
                    rand.Next(1, 9).ToString(),
                    rand.Next(1, 9).ToString(),
                    rand.Next(1, 9).ToString(),
                };
                plateText = string.Join("", plateArray);
            } while (PlatesInUse.Contains(plateText));
            PlatesInUse.Add(plateText);

            return plateText;
        }

        private static string GetRandomLetter()
        {
            int num = random.Next(0, 26);
            char let = (char)('a' + num);
            return let.ToString().ToUpper();
        }

        private async void loadCurrentPlates()
        {
            await BaseScript.Delay(5000);
            MySQL.execute("SELECT Plate FROM vehicle_data", new Dictionary<string, dynamic>(), new Action<List<object>>(data =>
            {
                foreach (dynamic i in data)
                {
                    PlatesInUse.Add(i.Plate.ToString());
                }
            }));
        }
    }
}
