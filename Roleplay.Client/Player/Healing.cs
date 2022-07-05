using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Emotes;

namespace Roleplay.Client.Player
{
    public class Healing : ClientAccessor
    {
        public Healing(Client client) : base(client)
        {
            client.RegisterEventHandler("Player.DoHeal", new Action<string>(OnDoHeal));
        }

        public async void FirstAidHeal()
        {
            var currentHealth = Game.PlayerPed.Health;
            EmoteManager.playerAnimations["healthkit"].PlayFullAnim();

            for (var i = 0; i < 15; i++)
            {
                if (currentHealth >= Game.PlayerPed.MaxHealth) return;

                currentHealth++;
                Game.PlayerPed.Health = currentHealth;
                await BaseScript.Delay(125);
            }
        }

        public async void BandageHeal()
        {
            var currentHealth = Game.PlayerPed.Health;
            EmoteManager.playerAnimations["healthkit"].PlayFullAnim();

            for (var i = 0; i < 10; i++)
            {
                if (currentHealth >= Game.PlayerPed.MaxHealth) return;

                currentHealth++;
                Game.PlayerPed.Health = currentHealth;
                await BaseScript.Delay(125);
            }
        }

        public async void FoodHeal()
        {
            var currentHealth = Game.PlayerPed.Health;
            EmoteManager.playerAnimations["eat"].PlayFullAnim();

            for (var i = 0; i < 5; i++)
            {
                if (currentHealth >= Game.PlayerPed.MaxHealth) return;

                currentHealth++;
                Game.PlayerPed.Health = currentHealth;
                await BaseScript.Delay(125);
            }
        }

        private void OnDoHeal(string healType)
        {
            if (healType == "FirstAid")
                FirstAidHeal();
            else if (healType == "Bandage")
                BandageHeal();
            else
                FoodHeal();
        }
    }
}
