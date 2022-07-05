using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Shared.Models
{
    public class SessionData
    {
        public int ServerID { get; set; }
        public Dictionary<string, dynamic> GlobalData { get; set; } = new Dictionary<string, dynamic>();
        public Dictionary<string, dynamic> LocalData { get; set; } = new Dictionary<string, dynamic>();
    }
}
