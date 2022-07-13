using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Client.Interfaces
{
    interface IJob
    {
        void StartJob();
        void EndJob();
        void GiveJobPayment();
        Task JobTick();
    }
}
