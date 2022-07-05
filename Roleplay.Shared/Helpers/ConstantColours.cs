using System.Drawing;
using System.Runtime.InteropServices;

namespace Roleplay.Shared.Helpers
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ConstantColours
    {
        public static Color Admin => Color.FromArgb(255, 255, 200);
        public static Color Job => Color.FromArgb(46, 139, 87);
        public static Color Phone => Color.FromArgb(255, 255, 51);
        public static Color Dispatch => Color.FromArgb(46, 139, 87);
        public static Color Bank => Color.FromArgb(50, 205, 50);
        public static Color Log => Color.FromArgb(148, 0, 211);
        public static Color Tattoo => Color.FromArgb(239, 127, 67);
        public static Color Inventory => Color.FromArgb(75, 0, 130);
        public static Color Fuel => Color.FromArgb(255, 127, 80);
        public static Color Jail => Color.FromArgb(139, 0, 139);
        public static Color TalkMarker => Color.FromArgb(105, 0, 50, 255);
        public static Color Store => Color.FromArgb(60, 179, 113);
        public static Color Do => Color.FromArgb(200, 140, 220);
        public static Color Criminal => Color.FromArgb(128, 0, 0);
        public static Color Hunting => Color.FromArgb(145, 55, 76);
        public static Color Help => Color.FromArgb(255, 20, 147);
        public static Color Pooc => Color.FromArgb(0, 255, 235);
        public static Color Advert => Color.FromArgb(0, 255, 200);
        public static Color Housing => Color.FromArgb(0, 191, 255);
        public static Color Radio => Color.FromArgb(235, 152, 0);
        public static Color Red => Color.FromArgb(255, 0, 0);
        public static Color Green => Color.FromArgb(0, 255, 0);
        public static Color Blue => Color.FromArgb(0, 0, 255);
        public static Color Yellow => Color.FromArgb(255, 255, 0);
        public static Color White => Color.FromArgb(255, 255, 255);
        public static Color FatalError => Color.FromArgb(200, 222, 24, 38);
        public static Color Error => Color.FromArgb(200, 232, 144, 5);
        public static Color Warning => Color.FromArgb(200, 241, 212, 0);
        public static Color Info => Color.FromArgb(200, 82, 207, 0);
        public static Color Debug => Color.FromArgb(200, 30, 163, 243);
        public static Color ServerMsg => Color.FromArgb(200, 0, 126, 229);
        public static Color OOC => Color.FromArgb(200, 77, 77, 77);
        public static Color Police => Color.FromArgb(75, 0, 117, 189);
        public static Color BankShop => Color.FromArgb(15, 42, 197, 0);
        public static Color Vehicle => Color.FromArgb(180, 140, 25, 155);
        public static Color Illegal => Color.FromArgb(15, 159, 19, 19);
        public static Color Building => Color.FromArgb(75, 200, 200, 200);
        public static Color JobMission => Color.FromArgb(180, 33, 145, 251);
        public static Color EMS => Color.FromArgb(75, (int)byte.MaxValue, 0, 0);
        public static string FatalErrorHEX => "#DE1826";
        public static string ErrorHEX => "#E89005";
        public static string WarningHEX => "#F1D400";
        public static string InfoHEX => "#52CF00";
        public static string DebugHEX => "#1EA3F3";
        public static string ServerMsgHEX => "#007EE5";
        public static string OOCHEX => "#9A9A9A";
        public static string PoliceHEX => "#0075BD";
        public static string CouncilHEX => "#F1D400";
        public static string BankShopHEX => "#2AC500";
        public static string VehicleHEX => "#8C199B";
        public static string IllegalHEX => "#9F1313";
        public static string BuildingHEX => "#C8C8C8";
        public static string JobMissionHEX => "#2191FB";
        public static string Invalid => "#AAAAAA";
        public static string WeedHex => "#9F1313";
        public static string HelpHEX => "#DB288B";
        public static string StateHEX => "#00e5b1";
        public static string MedicalRedHEX => "#D82525";
        public static string Call911 => "#C31919";
        public static string Reply911 => ConstantColours.PoliceHEX;
        public static string Call311 => "#FFBE5A";
        public static string Reply311 => ConstantColours.PoliceHEX;
        public static string SlashMe => "#CDC3DF";
        public static string CharacterInfo => "#AC96D4";
        public static string TextSend => "#14CAD8";
        public static string TextRecv => "#14D870";
    }
}
