using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Models
{
    public class BankAccountModel
    {
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public int Balance { get; set; }
        public int OwnerID { get; set; }
        public List<int> AccountEditors { get; set; } = new List<int>();
    }
}
