using Roleplay.Shared.Enums;

namespace Roleplay.Shared.Helpers
{
    public class CadAlertAIModel
    {
        public int TriggeredByCharID { get; set; }

        public string AIPlate { get; set; }

        public string Code { get; set; }

        public string AlertMessage { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public string Location { get; set; }

        public string Gender { get; set; }

        public string[] SendAlertToPerms { get; set; } = new string[0];

        public JobType SendAlertToGroup { get; set; }
    }
}