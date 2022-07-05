using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Client.Player
{
    public class Phone : ClientAccessor
    {
        public static bool PhoneKeyboardOpen = false;

        public Phone(Client client) : base(client)
        {
            client.RegisterEventHandler("Phones.OnOpenKeyboard", new Action(() =>
            {
                PhoneKeyboardOpen = true;
            }));
            client.RegisterEventHandler("Phones.OnCloseKeyboard", new Action(() =>
            {
                PhoneKeyboardOpen = false;
            }));
        }
    }
}
