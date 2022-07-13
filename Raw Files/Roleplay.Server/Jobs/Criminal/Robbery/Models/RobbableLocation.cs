using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared.Models;

namespace Roleplay.Server.Jobs.Criminal.Robbery.Models
{
    public abstract class RobbableLocation
    {
        public string LocationName { get; }
        public int Payout => rand.Next(maxPayout / 4, maxPayout);
        public int RequiredPolice => requiredPoliceConvar.Value;
        public int CooldownTime => cooldownTimeConvar.Value * 1000;

        private Random rand = new Random((int)DateTime.Now.Ticks);
        private int maxPayout => maxPayoutConvar.Value;
        private int defaultRequiredPolice => defaultRequiredPoliceConvar.Value;
        private int defaultCooldownTime => defaultCooldownTimeConvar.Value;
        private int defaultMaxPayout => defaultMaxPayoutConvar.Value;
        private CachedConvar<int> requiredPoliceConvar;
        private CachedConvar<int> cooldownTimeConvar;
        private CachedConvar<int> maxPayoutConvar;
        private CachedConvar<int> defaultRequiredPoliceConvar;
        private CachedConvar<int> defaultCooldownTimeConvar;
        private CachedConvar<int> defaultMaxPayoutConvar;

        protected RobbableLocation(string locaitonName)
        {
            LocationName = locaitonName;

            defaultRequiredPoliceConvar = new CachedConvar<int>($"job_robbery_{GetLocationType()}_default_required_police", 1, 60000);
            defaultCooldownTimeConvar = new CachedConvar<int>($"job_robbery_{GetLocationType()}_default_cooldown_time", 2700, 60000);
            defaultMaxPayoutConvar = new CachedConvar<int>($"job_robbery_{GetLocationType()}_default_max_payout", 10000, 60000);
            
            cooldownTimeConvar = new CachedConvar<int>($"job_robbery_{LocationName}_cooldown_time", defaultCooldownTime, 60000);
            maxPayoutConvar = new CachedConvar<int>($"job_robbery_{LocationName}_robbery_payout", defaultMaxPayout, 60000);
            requiredPoliceConvar = new CachedConvar<int>($"job_robbery_{LocationName}_required_police", defaultRequiredPolice, 60000);
        }

        public abstract void StartRobbery(Session.Session source);
        public abstract void EndRobbery();
        public abstract bool IsRobbable();
        public abstract void StartCooldown();
        public abstract void EndCooldown();
        public abstract string GetLocationType();
    }
}
