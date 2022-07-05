using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if CLIENT
namespace Roleplay.Client.Models
#elif SERVER
namespace Roleplay.Server.Models
#endif
{
    public static partial class Settings
    {

    }
}
