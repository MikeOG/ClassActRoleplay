using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Session;
using Roleplay.Client.UI;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using MenuFramework;
using Newtonsoft.Json;

namespace Roleplay.Client.Player.Clothing
{
    internal class TattooModel
    {
        public string collection;
        public string tattooName;
        public string displayName;
        public string maleHashName;
        public string femaleHashName;
        public string zone;
    }
    internal class Tattoos : ClientAccessor
    {
        List<TattooModel> tattooModels = new List<TattooModel>()
        {
            new TattooModel {
                collection = "mpairraces_overlays",
                tattooName = "TAT_AR_000",
                displayName = "Turbulence",
                maleHashName = "MP_Airraces_Tattoo_000_M",
                femaleHashName = "MP_Airraces_Tattoo_000_F",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpairraces_overlays",
                tattooName = "TAT_AR_001",
                displayName = "Pilot Skull",
                maleHashName = "MP_Airraces_Tattoo_001_M",
                femaleHashName = "MP_Airraces_Tattoo_001_F",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpairraces_overlays",
                tattooName = "TAT_AR_002",
                displayName = "Winged Bombshell",
                maleHashName = "MP_Airraces_Tattoo_002_M",
                femaleHashName = "MP_Airraces_Tattoo_002_F",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpairraces_overlays",
                tattooName = "TAT_AR_003",
                displayName = "Toxic Trails",
                maleHashName = "MP_Airraces_Tattoo_003_M",
                femaleHashName = "MP_Airraces_Tattoo_003_F",
                zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
                collection = "mpairraces_overlays",
                tattooName = "TAT_AR_004",
                displayName = "Balloon Pioneer",
                maleHashName = "MP_Airraces_Tattoo_004_M",
                femaleHashName = "MP_Airraces_Tattoo_004_F",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpairraces_overlays",
                tattooName = "TAT_AR_005",
                displayName = "Parachute Belle",
                maleHashName = "MP_Airraces_Tattoo_005_M",
                femaleHashName = "MP_Airraces_Tattoo_005_F",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpairraces_overlays",
                tattooName = "TAT_AR_006",
                displayName = "Bombs Away",
                maleHashName = "MP_Airraces_Tattoo_006_M",
                femaleHashName = "MP_Airraces_Tattoo_006_F",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpairraces_overlays",
                tattooName = "TAT_AR_007",
                displayName = "Eagle Eyes",
                maleHashName = "MP_Airraces_Tattoo_007_M",
                femaleHashName = "MP_Airraces_Tattoo_007_F",
                zone = "ZONE_TORSO"
            },

            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_018",
                displayName = "Ship Arms",
                maleHashName = "MP_Bea_M_Back_000",
                femaleHashName = "",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_019",
                displayName = "Tribal Hammerhead",
                maleHashName = "MP_Bea_M_Chest_000",
                femaleHashName = "",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_020",
                displayName = "Tribal Shark",
                maleHashName = "MP_Bea_M_Chest_001",
                femaleHashName = "",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_021",
                displayName = "Pirate Skull",
                maleHashName = "MP_Bea_M_Head_000",
                femaleHashName = "",
                zone = "ZONE_HEAD"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_022",
                displayName = "Surf LS",
                maleHashName = "MP_Bea_M_Head_001",
                femaleHashName = "",
                zone = "ZONE_HEAD"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_031",
                displayName = "Shark",
                maleHashName = "MP_Bea_M_Head_002",
                femaleHashName = "",
                zone = "ZONE_HEAD"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_027",
                displayName = "Tribal Star",
                maleHashName = "MP_Bea_M_Lleg_000",
                femaleHashName = "",
                zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_025",
                displayName = "Tribal Tiki Tower",
                maleHashName = "MP_Bea_M_Rleg_000",
                femaleHashName = "",
                zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
                collection = "mpsmuggler_overlays",
                tattooName = "TAT_BB_026",
                displayName = "Tribal Sun",
                maleHashName = "MP_Smuggler_Tattoo_000_M",
                femaleHashName = "",
                zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_026",
                displayName = "Tribal Sun",
                maleHashName = "MP_Bea_M_RArm_000",
                femaleHashName = "",
                zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_024",
                displayName = "Tiki Tower",
                maleHashName = "MP_Bea_M_LArm_000",
                femaleHashName = "",
                zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_017",
                displayName = "Mermaid L.S.",
                maleHashName = "MP_Bea_M_LArm_001",
                femaleHashName = "",
                zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_028",
                displayName = "Little Fish",
                maleHashName = "MP_Bea_M_Neck_000",
                femaleHashName = "",
                zone = "ZONE_HEAD"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_029",
                displayName = "Surfs Up",
                maleHashName = "MP_Bea_M_Neck_001",
                femaleHashName = "",
                zone = "ZONE_HEAD"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_030",
                displayName = "Vespucci Beauty",
                maleHashName = "MP_Bea_M_RArm_001",
                femaleHashName = "",
                zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_023",
                displayName = "Swordfish",
                maleHashName = "MP_Bea_M_Stom_000",
                femaleHashName = "",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_032",
                displayName = "Wheel",
                maleHashName = "MP_Bea_M_Stom_001",
                femaleHashName = "",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_003",
                displayName = "Rock Solid",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Back_000",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_001",
                displayName = "Hibiscus Flower Duo",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Back_001",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_005",
                displayName = "Shrimp",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Back_002",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_012",
                displayName = "Anchor",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Chest_000",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_013",
                displayName = "Anchor",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Chest_001",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_000",
                displayName = "Los Santos Wreath",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Chest_002",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_006",
                displayName = "Love Dagger",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_RSide_000",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_007",
                displayName = "School of Fish",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_RLeg_000",
                zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_015",
                displayName = "Tribal Fish",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_RArm_001",
                zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_008",
                displayName = "Tribal Butterfly",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Neck_000",
                zone = "ZONE_HEAD"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_011",
                displayName = "Sea Horses",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Should_000",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_004",
                displayName = "Catfish",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Should_001",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_014",
                displayName = "Swallow",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Stom_000",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_009",
                displayName = "Hibiscus Flower",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Stom_001",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_010",
                displayName = "Dolphin",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_Stom_002",
                zone = "ZONE_TORSO"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_002",
                displayName = "Tribal Flower",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_LArm_000",
                zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
                collection = "mpbeach_overlays",
                tattooName = "TAT_BB_016",
                displayName = "Parrot",
                maleHashName = "",
                femaleHashName = "MP_Bea_F_LArm_001",
                zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_000",
            displayName = "Demon Rider",
            maleHashName = "MP_MP_Biker_Tat_000_M",
            femaleHashName = "MP_MP_Biker_Tat_000_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_001",
            displayName = "Both Barrels",
            maleHashName = "MP_MP_Biker_Tat_001_M",
            femaleHashName = "MP_MP_Biker_Tat_001_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_002",
            displayName = "Rose Tribute",
            maleHashName = "MP_MP_Biker_Tat_002_M",
            femaleHashName = "MP_MP_Biker_Tat_002_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_003",
            displayName = "Web Rider",
            maleHashName = "MP_MP_Biker_Tat_003_M",
            femaleHashName = "MP_MP_Biker_Tat_003_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_004",
            displayName = "Dragon's Fury",
            maleHashName = "MP_MP_Biker_Tat_004_M",
            femaleHashName = "MP_MP_Biker_Tat_004_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_005",
            displayName = "Made In America",
            maleHashName = "MP_MP_Biker_Tat_005_M",
            femaleHashName = "MP_MP_Biker_Tat_005_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_006",
            displayName = "Chopper Freedom",
            maleHashName = "MP_MP_Biker_Tat_006_M",
            femaleHashName = "MP_MP_Biker_Tat_006_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_007",
            displayName = "Swooping Eagle",
            maleHashName = "MP_MP_Biker_Tat_007_M",
            femaleHashName = "MP_MP_Biker_Tat_007_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_008",
            displayName = "Freedom Wheels",
            maleHashName = "MP_MP_Biker_Tat_008_M",
            femaleHashName = "MP_MP_Biker_Tat_008_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_009",
            displayName = "Morbid Arachnid",
            maleHashName = "MP_MP_Biker_Tat_009_M",
            femaleHashName = "MP_MP_Biker_Tat_009_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_010",
            displayName = "Skull Of Taurus",
            maleHashName = "MP_MP_Biker_Tat_010_M",
            femaleHashName = "MP_MP_Biker_Tat_010_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_011",
            displayName = "R.I.P. My Brothers",
            maleHashName = "MP_MP_Biker_Tat_011_M",
            femaleHashName = "MP_MP_Biker_Tat_011_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_012",
            displayName = "Urban Stunter",
            maleHashName = "MP_MP_Biker_Tat_012_M",
            femaleHashName = "MP_MP_Biker_Tat_012_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_013",
            displayName = "Demon Crossbones",
            maleHashName = "MP_MP_Biker_Tat_013_M",
            femaleHashName = "MP_MP_Biker_Tat_013_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_014",
            displayName = "Lady Mortality",
            maleHashName = "MP_MP_Biker_Tat_014_M",
            femaleHashName = "MP_MP_Biker_Tat_014_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_015",
            displayName = "Ride or Die",
            maleHashName = "MP_MP_Biker_Tat_015_M",
            femaleHashName = "MP_MP_Biker_Tat_015_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_016",
            displayName = "Macabre Tree",
            maleHashName = "MP_MP_Biker_Tat_016_M",
            femaleHashName = "MP_MP_Biker_Tat_016_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_017",
            displayName = "Clawed Beast",
            maleHashName = "MP_MP_Biker_Tat_017_M",
            femaleHashName = "MP_MP_Biker_Tat_017_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_018",
            displayName = "Skeletal Chopper",
            maleHashName = "MP_MP_Biker_Tat_018_M",
            femaleHashName = "MP_MP_Biker_Tat_018_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_019",
            displayName = "Gruesome Talons",
            maleHashName = "MP_MP_Biker_Tat_019_M",
            femaleHashName = "MP_MP_Biker_Tat_019_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_020",
            displayName = "Cranial Rose",
            maleHashName = "MP_MP_Biker_Tat_020_M",
            femaleHashName = "MP_MP_Biker_Tat_020_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_021",
            displayName = "Flaming Reaper",
            maleHashName = "MP_MP_Biker_Tat_021_M",
            femaleHashName = "MP_MP_Biker_Tat_021_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_022",
            displayName = "Western Insignia",
            maleHashName = "MP_MP_Biker_Tat_022_M",
            femaleHashName = "MP_MP_Biker_Tat_022_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_023",
            displayName = "Western MC",
            maleHashName = "MP_MP_Biker_Tat_023_M",
            femaleHashName = "MP_MP_Biker_Tat_023_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_024",
            displayName = "Live to Ride",
            maleHashName = "MP_MP_Biker_Tat_024_M",
            femaleHashName = "MP_MP_Biker_Tat_024_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_025",
            displayName = "Good Luck",
            maleHashName = "MP_MP_Biker_Tat_025_M",
            femaleHashName = "MP_MP_Biker_Tat_025_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_026",
            displayName = "American Dream",
            maleHashName = "MP_MP_Biker_Tat_026_M",
            femaleHashName = "MP_MP_Biker_Tat_026_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_027",
            displayName = "Bad Luck",
            maleHashName = "MP_MP_Biker_Tat_027_M",
            femaleHashName = "MP_MP_Biker_Tat_027_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_028",
            displayName = "Dusk Rider",
            maleHashName = "MP_MP_Biker_Tat_028_M",
            femaleHashName = "MP_MP_Biker_Tat_028_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_029",
            displayName = "Bone Wrench",
            maleHashName = "MP_MP_Biker_Tat_029_M",
            femaleHashName = "MP_MP_Biker_Tat_029_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_030",
            displayName = "Brothers For Life",
            maleHashName = "MP_MP_Biker_Tat_030_M",
            femaleHashName = "MP_MP_Biker_Tat_030_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_031",
            displayName = "Gear Head",
            maleHashName = "MP_MP_Biker_Tat_031_M",
            femaleHashName = "MP_MP_Biker_Tat_031_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_032",
            displayName = "Western Eagle",
            maleHashName = "MP_MP_Biker_Tat_032_M",
            femaleHashName = "MP_MP_Biker_Tat_032_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_033",
            displayName = "Eagle Emblem",
            maleHashName = "MP_MP_Biker_Tat_033_M",
            femaleHashName = "MP_MP_Biker_Tat_033_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_034",
            displayName = "Brotherhood of Bikes",
            maleHashName = "MP_MP_Biker_Tat_034_M",
            femaleHashName = "MP_MP_Biker_Tat_034_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_035",
            displayName = "Chain Fist",
            maleHashName = "MP_MP_Biker_Tat_035_M",
            femaleHashName = "MP_MP_Biker_Tat_035_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_036",
            displayName = "Engulfed Skull",
            maleHashName = "MP_MP_Biker_Tat_036_M",
            femaleHashName = "MP_MP_Biker_Tat_036_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_037",
            displayName = "Scorched Soul",
            maleHashName = "MP_MP_Biker_Tat_037_M",
            femaleHashName = "MP_MP_Biker_Tat_037_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_038",
            displayName = "FTW",
            maleHashName = "MP_MP_Biker_Tat_038_M",
            femaleHashName = "MP_MP_Biker_Tat_038_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_039",
            displayName = "Gas Guzzler",
            maleHashName = "MP_MP_Biker_Tat_039_M",
            femaleHashName = "MP_MP_Biker_Tat_039_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_040",
            displayName = "American Made",
            maleHashName = "MP_MP_Biker_Tat_040_M",
            femaleHashName = "MP_MP_Biker_Tat_040_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_041",
            displayName = "No Regrets",
            maleHashName = "MP_MP_Biker_Tat_041_M",
            femaleHashName = "MP_MP_Biker_Tat_041_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_042",
            displayName = "Grim Rider",
            maleHashName = "MP_MP_Biker_Tat_042_M",
            femaleHashName = "MP_MP_Biker_Tat_042_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_043",
            displayName = "Ride Forever",
            maleHashName = "MP_MP_Biker_Tat_043_M",
            femaleHashName = "MP_MP_Biker_Tat_043_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_044",
            displayName = "Ride Free",
            maleHashName = "MP_MP_Biker_Tat_044_M",
            femaleHashName = "MP_MP_Biker_Tat_044_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_045",
            displayName = "Ride Hard Die Fast",
            maleHashName = "MP_MP_Biker_Tat_045_M",
            femaleHashName = "MP_MP_Biker_Tat_045_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_046",
            displayName = "Skull Chain",
            maleHashName = "MP_MP_Biker_Tat_046_M",
            femaleHashName = "MP_MP_Biker_Tat_046_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_047",
            displayName = "Snake Bike",
            maleHashName = "MP_MP_Biker_Tat_047_M",
            femaleHashName = "MP_MP_Biker_Tat_047_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_048",
            displayName = "STFU",
            maleHashName = "MP_MP_Biker_Tat_048_M",
            femaleHashName = "MP_MP_Biker_Tat_048_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_049",
            displayName = "These Colors Don't Run",
            maleHashName = "MP_MP_Biker_Tat_049_M",
            femaleHashName = "MP_MP_Biker_Tat_049_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_050",
            displayName = "Unforgiven",
            maleHashName = "MP_MP_Biker_Tat_050_M",
            femaleHashName = "MP_MP_Biker_Tat_050_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_051",
            displayName = "Western Stylized",
            maleHashName = "MP_MP_Biker_Tat_051_M",
            femaleHashName = "MP_MP_Biker_Tat_051_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_052",
            displayName = "Biker Mount",
            maleHashName = "MP_MP_Biker_Tat_052_M",
            femaleHashName = "MP_MP_Biker_Tat_052_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_053",
            displayName = "Muffler Helmet",
            maleHashName = "MP_MP_Biker_Tat_053_M",
            femaleHashName = "MP_MP_Biker_Tat_053_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_054",
            displayName = "Mum",
            maleHashName = "MP_MP_Biker_Tat_054_M",
            femaleHashName = "MP_MP_Biker_Tat_054_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_055",
            displayName = "Poison Scorpion",
            maleHashName = "MP_MP_Biker_Tat_055_M",
            femaleHashName = "MP_MP_Biker_Tat_055_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_056",
            displayName = "Bone Cruiser",
            maleHashName = "MP_MP_Biker_Tat_056_M",
            femaleHashName = "MP_MP_Biker_Tat_056_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_057",
            displayName = "Laughing Skull",
            maleHashName = "MP_MP_Biker_Tat_057_M",
            femaleHashName = "MP_MP_Biker_Tat_057_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_058",
            displayName = "Reaper Vulture",
            maleHashName = "MP_MP_Biker_Tat_058_M",
            femaleHashName = "MP_MP_Biker_Tat_058_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_059",
            displayName = "Faggio",
            maleHashName = "MP_MP_Biker_Tat_059_M",
            femaleHashName = "MP_MP_Biker_Tat_059_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbiker_overlays",
            tattooName = "TAT_BI_060",
            displayName = "We Are The Mods!",
            maleHashName = "MP_MP_Biker_Tat_060_M",
            femaleHashName = "MP_MP_Biker_Tat_060_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_005",
            displayName = "Cash is King",
            maleHashName = "MP_Buis_M_Neck_000",
            femaleHashName = "",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_006",
            displayName = "Bold Dollar Sign",
            maleHashName = "MP_Buis_M_Neck_001",
            femaleHashName = "",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_007",
            displayName = "Script Dollar Sign",
            maleHashName = "MP_Buis_M_Neck_002",
            femaleHashName = "",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_008",
            displayName = "$100",
            maleHashName = "MP_Buis_M_Neck_003",
            femaleHashName = "",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_003",
            displayName = "$100 Bill",
            maleHashName = "MP_Buis_M_LeftArm_000",
            femaleHashName = "",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_004",
            displayName = "All-Seeing Eye",
            maleHashName = "MP_Buis_M_LeftArm_001",
            femaleHashName = "",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_009",
            displayName = "Dollar Skull",
            maleHashName = "MP_Buis_M_RightArm_000",
            femaleHashName = "",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_010",
            displayName = "Green",
            maleHashName = "MP_Buis_M_RightArm_001",
            femaleHashName = "",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_011",
            displayName = "Refined Hustler",
            maleHashName = "MP_Buis_M_Stomach_000",
            femaleHashName = "",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_001",
            displayName = "Rich",
            maleHashName = "MP_Buis_M_Chest_000",
            femaleHashName = "",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_002",
            displayName = "$$$",
            maleHashName = "MP_Buis_M_Chest_001",
            femaleHashName = "",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_000",
            displayName = "Makin' Paper",
            maleHashName = "MP_Buis_M_Back_000",
            femaleHashName = "",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_002",
            displayName = "High Roller",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Chest_000",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_003",
            displayName = "Makin' Money",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Chest_001",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_004",
            displayName = "Love Money",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Chest_002",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_011",
            displayName = "Diamond Back",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Stom_000",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_012",
            displayName = "Santo Capra Logo",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Stom_001",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_013",
            displayName = "Money Bag",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Stom_002",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_000",
            displayName = "Respect",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Back_000",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_001",
            displayName = "Gold Digger",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Back_001",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_007",
            displayName = "Val-de-Grace Logo",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Neck_000",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_008",
            displayName = "Money Rose",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_Neck_001",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_009",
            displayName = "Dollar Sign",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_RArm_000",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_005",
            displayName = "Greed is Good",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_LArm_000",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_006",
            displayName = "Single",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_LLeg_000",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpbusiness_overlays",
            tattooName = "TAT_BUS_F_010",
            displayName = "Diamond Crown",
            maleHashName = "",
            femaleHashName = "MP_Buis_F_RLeg_000",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_000",
            displayName = "Thor & Goblin",
            maleHashName = "MP_Christmas2017_Tattoo_000_M",
            femaleHashName = "MP_Christmas2017_Tattoo_000_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_001",
            displayName = "Viking Warrior",
            maleHashName = "MP_Christmas2017_Tattoo_001_M",
            femaleHashName = "MP_Christmas2017_Tattoo_001_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_002",
            displayName = "Kabuto",
            maleHashName = "MP_Christmas2017_Tattoo_002_M",
            femaleHashName = "MP_Christmas2017_Tattoo_002_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_003",
            displayName = "Native Warrior",
            maleHashName = "MP_Christmas2017_Tattoo_003_M",
            femaleHashName = "MP_Christmas2017_Tattoo_003_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_004",
            displayName = "Tiger & Mask",
            maleHashName = "MP_Christmas2017_Tattoo_004_M",
            femaleHashName = "MP_Christmas2017_Tattoo_004_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_005",
            displayName = "Ghost Dragon",
            maleHashName = "MP_Christmas2017_Tattoo_005_M",
            femaleHashName = "MP_Christmas2017_Tattoo_005_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_006",
            displayName = "Medusa",
            maleHashName = "MP_Christmas2017_Tattoo_006_M",
            femaleHashName = "MP_Christmas2017_Tattoo_006_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_007",
            displayName = "Spartan Combat",
            maleHashName = "MP_Christmas2017_Tattoo_007_M",
            femaleHashName = "MP_Christmas2017_Tattoo_007_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_008",
            displayName = "Spartan Warrior",
            maleHashName = "MP_Christmas2017_Tattoo_008_M",
            femaleHashName = "MP_Christmas2017_Tattoo_008_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_009",
            displayName = "Norse Rune",
            maleHashName = "MP_Christmas2017_Tattoo_009_M",
            femaleHashName = "MP_Christmas2017_Tattoo_009_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_010",
            displayName = "Spartan Shield",
            maleHashName = "MP_Christmas2017_Tattoo_010_M",
            femaleHashName = "MP_Christmas2017_Tattoo_010_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_011",
            displayName = "Weathered Skull",
            maleHashName = "MP_Christmas2017_Tattoo_011_M",
            femaleHashName = "MP_Christmas2017_Tattoo_011_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_012",
            displayName = "Tiger Headdress",
            maleHashName = "MP_Christmas2017_Tattoo_012_M",
            femaleHashName = "MP_Christmas2017_Tattoo_012_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_013",
            displayName = "Katana",
            maleHashName = "MP_Christmas2017_Tattoo_013_M",
            femaleHashName = "MP_Christmas2017_Tattoo_013_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_014",
            displayName = "Celtic Band",
            maleHashName = "MP_Christmas2017_Tattoo_014_M",
            femaleHashName = "MP_Christmas2017_Tattoo_014_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_015",
            displayName = "Samurai Combat",
            maleHashName = "MP_Christmas2017_Tattoo_015_M",
            femaleHashName = "MP_Christmas2017_Tattoo_015_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_016",
            displayName = "Odin & Raven",
            maleHashName = "MP_Christmas2017_Tattoo_016_M",
            femaleHashName = "MP_Christmas2017_Tattoo_016_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_017",
            displayName = "Feather Sleeve",
            maleHashName = "MP_Christmas2017_Tattoo_017_M",
            femaleHashName = "MP_Christmas2017_Tattoo_017_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_018",
            displayName = "Muscle Tear",
            maleHashName = "MP_Christmas2017_Tattoo_018_M",
            femaleHashName = "MP_Christmas2017_Tattoo_018_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_019",
            displayName = "Strike Force",
            maleHashName = "MP_Christmas2017_Tattoo_019_M",
            femaleHashName = "MP_Christmas2017_Tattoo_019_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_020",
            displayName = "Medusa's Gaze",
            maleHashName = "MP_Christmas2017_Tattoo_020_M",
            femaleHashName = "MP_Christmas2017_Tattoo_020_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_021",
            displayName = "Spartan & Lion",
            maleHashName = "MP_Christmas2017_Tattoo_021_M",
            femaleHashName = "MP_Christmas2017_Tattoo_021_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_022",
            displayName = "Spartan & Horse",
            maleHashName = "MP_Christmas2017_Tattoo_022_M",
            femaleHashName = "MP_Christmas2017_Tattoo_022_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_023",
            displayName = "Samurai Tallship",
            maleHashName = "MP_Christmas2017_Tattoo_023_M",
            femaleHashName = "MP_Christmas2017_Tattoo_023_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_024",
            displayName = "Dragon Slayer",
            maleHashName = "MP_Christmas2017_Tattoo_024_M",
            femaleHashName = "MP_Christmas2017_Tattoo_024_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_025",
            displayName = "Winged Serpent",
            maleHashName = "MP_Christmas2017_Tattoo_025_M",
            femaleHashName = "MP_Christmas2017_Tattoo_025_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_026",
            displayName = "Spartan Skull",
            maleHashName = "MP_Christmas2017_Tattoo_026_M",
            femaleHashName = "MP_Christmas2017_Tattoo_026_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_027",
            displayName = "Molon Labe",
            maleHashName = "MP_Christmas2017_Tattoo_027_M",
            femaleHashName = "MP_Christmas2017_Tattoo_027_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_028",
            displayName = "Spartan Mural",
            maleHashName = "MP_Christmas2017_Tattoo_028_M",
            femaleHashName = "MP_Christmas2017_Tattoo_028_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2017_overlays",
            tattooName = "TAT_H27_029",
            displayName = "Cerberus",
            maleHashName = "MP_Christmas2017_Tattoo_029_M",
            femaleHashName = "MP_Christmas2017_Tattoo_029_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_000",
            displayName = "Skull Rider",
            maleHashName = "MP_Xmas2_M_Tat_000",
            femaleHashName = "MP_Xmas2_F_Tat_000",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_001",
            displayName = "Spider Outline",
            maleHashName = "MP_Xmas2_M_Tat_001",
            femaleHashName = "MP_Xmas2_F_Tat_001",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_002",
            displayName = "Spider Color",
            maleHashName = "MP_Xmas2_M_Tat_002",
            femaleHashName = "MP_Xmas2_F_Tat_002",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_003",
            displayName = "Snake Outline",
            maleHashName = "MP_Xmas2_M_Tat_003",
            femaleHashName = "MP_Xmas2_F_Tat_003",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_004",
            displayName = "Snake Shaded",
            maleHashName = "MP_Xmas2_M_Tat_004",
            femaleHashName = "MP_Xmas2_F_Tat_004",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_005",
            displayName = "Carp Outline",
            maleHashName = "MP_Xmas2_M_Tat_005",
            femaleHashName = "MP_Xmas2_F_Tat_005",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_006",
            displayName = "Carp Shaded",
            maleHashName = "MP_Xmas2_M_Tat_006",
            femaleHashName = "MP_Xmas2_F_Tat_006",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_007",
            displayName = "Los Muertos",
            maleHashName = "MP_Xmas2_M_Tat_007",
            femaleHashName = "MP_Xmas2_F_Tat_007",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_008",
            displayName = "Death Before Dishonor",
            maleHashName = "MP_Xmas2_M_Tat_008",
            femaleHashName = "MP_Xmas2_F_Tat_008",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_009",
            displayName = "Time To Die",
            maleHashName = "MP_Xmas2_M_Tat_009",
            femaleHashName = "MP_Xmas2_F_Tat_009",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_010",
            displayName = "Electric Snake",
            maleHashName = "MP_Xmas2_M_Tat_010",
            femaleHashName = "MP_Xmas2_F_Tat_010",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_011",
            displayName = "Roaring Tiger",
            maleHashName = "MP_Xmas2_M_Tat_011",
            femaleHashName = "MP_Xmas2_F_Tat_011",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_012",
            displayName = "8 Ball Skull",
            maleHashName = "MP_Xmas2_M_Tat_012",
            femaleHashName = "MP_Xmas2_F_Tat_012",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_013",
            displayName = "Lizard",
            maleHashName = "MP_Xmas2_M_Tat_013",
            femaleHashName = "MP_Xmas2_F_Tat_013",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_014",
            displayName = "Floral Dagger",
            maleHashName = "MP_Xmas2_M_Tat_014",
            femaleHashName = "MP_Xmas2_F_Tat_014",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_015",
            displayName = "Japanese Warrior",
            maleHashName = "MP_Xmas2_M_Tat_015",
            femaleHashName = "MP_Xmas2_F_Tat_015",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_016",
            displayName = "Loose Lips Outline",
            maleHashName = "MP_Xmas2_M_Tat_016",
            femaleHashName = "MP_Xmas2_F_Tat_016",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_017",
            displayName = "Loose Lips Color",
            maleHashName = "MP_Xmas2_M_Tat_017",
            femaleHashName = "MP_Xmas2_F_Tat_017",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_018",
            displayName = "Royal Dagger Outline",
            maleHashName = "MP_Xmas2_M_Tat_018",
            femaleHashName = "MP_Xmas2_F_Tat_018",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_019",
            displayName = "Royal Dagger Color",
            maleHashName = "MP_Xmas2_M_Tat_019",
            femaleHashName = "MP_Xmas2_F_Tat_019",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_020",
            displayName = "Time's Up Outline",
            maleHashName = "MP_Xmas2_M_Tat_020",
            femaleHashName = "MP_Xmas2_F_Tat_020",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_021",
            displayName = "Time's Up Color",
            maleHashName = "MP_Xmas2_M_Tat_021",
            femaleHashName = "MP_Xmas2_F_Tat_021",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_022",
            displayName = "You're Next Outline",
            maleHashName = "MP_Xmas2_M_Tat_022",
            femaleHashName = "MP_Xmas2_F_Tat_022",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_023",
            displayName = "You're Next Color",
            maleHashName = "MP_Xmas2_M_Tat_023",
            femaleHashName = "MP_Xmas2_F_Tat_023",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_024",
            displayName = "Snake Head Outline",
            maleHashName = "MP_Xmas2_M_Tat_024",
            femaleHashName = "MP_Xmas2_F_Tat_024",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_025",
            displayName = "Snake Head Color",
            maleHashName = "MP_Xmas2_M_Tat_025",
            femaleHashName = "MP_Xmas2_F_Tat_025",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_026",
            displayName = "Fuck Luck Outline",
            maleHashName = "MP_Xmas2_M_Tat_026",
            femaleHashName = "MP_Xmas2_F_Tat_026",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_027",
            displayName = "Fuck Luck Color",
            maleHashName = "MP_Xmas2_M_Tat_027",
            femaleHashName = "MP_Xmas2_F_Tat_027",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_028",
            displayName = "Executioner",
            maleHashName = "MP_Xmas2_M_Tat_028",
            femaleHashName = "MP_Xmas2_F_Tat_028",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpchristmas2_overlays",
            tattooName = "TAT_X2_029",
            displayName = "Beautiful Death",
            maleHashName = "MP_Xmas2_M_Tat_029",
            femaleHashName = "MP_Xmas2_F_Tat_029",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_000",
            displayName = "Bullet Proof",
            maleHashName = "MP_Gunrunning_Tattoo_000_M",
            femaleHashName = "MP_Gunrunning_Tattoo_000_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_001",
            displayName = "Crossed Weapons",
            maleHashName = "MP_Gunrunning_Tattoo_001_M",
            femaleHashName = "MP_Gunrunning_Tattoo_001_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_002",
            displayName = "Grenade",
            maleHashName = "MP_Gunrunning_Tattoo_002_M",
            femaleHashName = "MP_Gunrunning_Tattoo_002_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_003",
            displayName = "Lock & Load",
            maleHashName = "MP_Gunrunning_Tattoo_003_M",
            femaleHashName = "MP_Gunrunning_Tattoo_003_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_004",
            displayName = "Sidearm",
            maleHashName = "MP_Gunrunning_Tattoo_004_M",
            femaleHashName = "MP_Gunrunning_Tattoo_004_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_005",
            displayName = "Patriot Skull",
            maleHashName = "MP_Gunrunning_Tattoo_005_M",
            femaleHashName = "MP_Gunrunning_Tattoo_005_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_006",
            displayName = "Combat Skull",
            maleHashName = "MP_Gunrunning_Tattoo_006_M",
            femaleHashName = "MP_Gunrunning_Tattoo_006_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_007",
            displayName = "Stylized Tiger",
            maleHashName = "MP_Gunrunning_Tattoo_007_M",
            femaleHashName = "MP_Gunrunning_Tattoo_007_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_008",
            displayName = "Bandolier",
            maleHashName = "MP_Gunrunning_Tattoo_008_M",
            femaleHashName = "MP_Gunrunning_Tattoo_008_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_009",
            displayName = "Butterfly Knife",
            maleHashName = "MP_Gunrunning_Tattoo_009_M",
            femaleHashName = "MP_Gunrunning_Tattoo_009_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_010",
            displayName = "Cash Money",
            maleHashName = "MP_Gunrunning_Tattoo_010_M",
            femaleHashName = "MP_Gunrunning_Tattoo_010_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_011",
            displayName = "Death Skull",
            maleHashName = "MP_Gunrunning_Tattoo_011_M",
            femaleHashName = "MP_Gunrunning_Tattoo_011_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_012",
            displayName = "Dollar Daggers",
            maleHashName = "MP_Gunrunning_Tattoo_012_M",
            femaleHashName = "MP_Gunrunning_Tattoo_012_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_013",
            displayName = "Wolf Insignia",
            maleHashName = "MP_Gunrunning_Tattoo_013_M",
            femaleHashName = "MP_Gunrunning_Tattoo_013_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_014",
            displayName = "Backstabber",
            maleHashName = "MP_Gunrunning_Tattoo_014_M",
            femaleHashName = "MP_Gunrunning_Tattoo_014_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_015",
            displayName = "Spiked Skull",
            maleHashName = "MP_Gunrunning_Tattoo_015_M",
            femaleHashName = "MP_Gunrunning_Tattoo_015_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_016",
            displayName = "Blood Money",
            maleHashName = "MP_Gunrunning_Tattoo_016_M",
            femaleHashName = "MP_Gunrunning_Tattoo_016_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_017",
            displayName = "Dog Tags",
            maleHashName = "MP_Gunrunning_Tattoo_017_M",
            femaleHashName = "MP_Gunrunning_Tattoo_017_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_018",
            displayName = "Dual Wield Skull",
            maleHashName = "MP_Gunrunning_Tattoo_018_M",
            femaleHashName = "MP_Gunrunning_Tattoo_018_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_019",
            displayName = "Pistol Wings",
            maleHashName = "MP_Gunrunning_Tattoo_019_M",
            femaleHashName = "MP_Gunrunning_Tattoo_019_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_020",
            displayName = "Crowned Weapons",
            maleHashName = "MP_Gunrunning_Tattoo_020_M",
            femaleHashName = "MP_Gunrunning_Tattoo_020_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_021",
            displayName = "Have a Nice Day",
            maleHashName = "MP_Gunrunning_Tattoo_021_M",
            femaleHashName = "MP_Gunrunning_Tattoo_021_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_022",
            displayName = "Explosive Heart",
            maleHashName = "MP_Gunrunning_Tattoo_022_M",
            femaleHashName = "MP_Gunrunning_Tattoo_022_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_023",
            displayName = "Rose Revolver",
            maleHashName = "MP_Gunrunning_Tattoo_023_M",
            femaleHashName = "MP_Gunrunning_Tattoo_023_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_024",
            displayName = "Combat Reaper",
            maleHashName = "MP_Gunrunning_Tattoo_024_M",
            femaleHashName = "MP_Gunrunning_Tattoo_024_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_025",
            displayName = "Praying Skull",
            maleHashName = "MP_Gunrunning_Tattoo_025_M",
            femaleHashName = "MP_Gunrunning_Tattoo_025_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_026",
            displayName = "Restless Skull",
            maleHashName = "MP_Gunrunning_Tattoo_026_M",
            femaleHashName = "MP_Gunrunning_Tattoo_026_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_027",
            displayName = "Serpent Revolver",
            maleHashName = "MP_Gunrunning_Tattoo_027_M",
            femaleHashName = "MP_Gunrunning_Tattoo_027_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_028",
            displayName = "Micro SMG Chain",
            maleHashName = "MP_Gunrunning_Tattoo_028_M",
            femaleHashName = "MP_Gunrunning_Tattoo_028_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_029",
            displayName = "Win Some Lose Some",
            maleHashName = "MP_Gunrunning_Tattoo_029_M",
            femaleHashName = "MP_Gunrunning_Tattoo_029_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpgunrunning_overlays",
            tattooName = "TAT_GR_030",
            displayName = "Pistol Ace",
            maleHashName = "MP_Gunrunning_Tattoo_030_M",
            femaleHashName = "MP_Gunrunning_Tattoo_030_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_000",
            displayName = "Crossed Arrows",
            maleHashName = "FM_Hip_M_Tat_000",
            femaleHashName = "FM_Hip_F_Tat_000",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_001",
            displayName = "Single Arrow",
            maleHashName = "FM_Hip_M_Tat_001",
            femaleHashName = "FM_Hip_F_Tat_001",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_002",
            displayName = "Chemistry",
            maleHashName = "FM_Hip_M_Tat_002",
            femaleHashName = "FM_Hip_F_Tat_002",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_003",
            displayName = "Diamond Sparkle",
            maleHashName = "FM_Hip_M_Tat_003",
            femaleHashName = "FM_Hip_F_Tat_003",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_004",
            displayName = "Bone",
            maleHashName = "FM_Hip_M_Tat_004",
            femaleHashName = "FM_Hip_F_Tat_004",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_005",
            displayName = "Beautiful Eye",
            maleHashName = "FM_Hip_M_Tat_005",
            femaleHashName = "FM_Hip_F_Tat_005",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_006",
            displayName = "Feather Birds",
            maleHashName = "FM_Hip_M_Tat_006",
            femaleHashName = "FM_Hip_F_Tat_006",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_007",
            displayName = "Bricks",
            maleHashName = "FM_Hip_M_Tat_007",
            femaleHashName = "FM_Hip_F_Tat_007",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_008",
            displayName = "Cube",
            maleHashName = "FM_Hip_M_Tat_008",
            femaleHashName = "FM_Hip_F_Tat_008",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_009",
            displayName = "Squares",
            maleHashName = "FM_Hip_M_Tat_009",
            femaleHashName = "FM_Hip_F_Tat_009",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_010",
            displayName = "Horseshoe",
            maleHashName = "FM_Hip_M_Tat_010",
            femaleHashName = "FM_Hip_F_Tat_010",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_011",
            displayName = "Infinity",
            maleHashName = "FM_Hip_M_Tat_011",
            femaleHashName = "FM_Hip_F_Tat_011",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_012",
            displayName = "Antlers",
            maleHashName = "FM_Hip_M_Tat_012",
            femaleHashName = "FM_Hip_F_Tat_012",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_013",
            displayName = "Boombox",
            maleHashName = "FM_Hip_M_Tat_013",
            femaleHashName = "FM_Hip_F_Tat_013",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_014",
            displayName = "Spray Can",
            maleHashName = "FM_Hip_M_Tat_014",
            femaleHashName = "FM_Hip_F_Tat_014",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_015",
            displayName = "Mustache",
            maleHashName = "FM_Hip_M_Tat_015",
            femaleHashName = "FM_Hip_F_Tat_015",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_016",
            displayName = "Lightning Bolt",
            maleHashName = "FM_Hip_M_Tat_016",
            femaleHashName = "FM_Hip_F_Tat_016",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_017",
            displayName = "Eye Triangle",
            maleHashName = "FM_Hip_M_Tat_017",
            femaleHashName = "FM_Hip_F_Tat_017",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_018",
            displayName = "Origami",
            maleHashName = "FM_Hip_M_Tat_018",
            femaleHashName = "FM_Hip_F_Tat_018",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_019",
            displayName = "Charm",
            maleHashName = "FM_Hip_M_Tat_019",
            femaleHashName = "FM_Hip_F_Tat_019",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_020",
            displayName = "Geo Pattern",
            maleHashName = "FM_Hip_M_Tat_020",
            femaleHashName = "FM_Hip_F_Tat_020",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_021",
            displayName = "Geo Fox",
            maleHashName = "FM_Hip_M_Tat_021",
            femaleHashName = "FM_Hip_F_Tat_021",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_022",
            displayName = "Pencil",
            maleHashName = "FM_Hip_M_Tat_022",
            femaleHashName = "FM_Hip_F_Tat_022",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_023",
            displayName = "Smiley",
            maleHashName = "FM_Hip_M_Tat_023",
            femaleHashName = "FM_Hip_F_Tat_023",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_024",
            displayName = "Pyramid",
            maleHashName = "FM_Hip_M_Tat_024",
            femaleHashName = "FM_Hip_F_Tat_024",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_025",
            displayName = "Watch Your Step",
            maleHashName = "FM_Hip_M_Tat_025",
            femaleHashName = "FM_Hip_F_Tat_025",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_026",
            displayName = "Pizza",
            maleHashName = "FM_Hip_M_Tat_026",
            femaleHashName = "FM_Hip_F_Tat_026",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_027",
            displayName = "Padlock",
            maleHashName = "FM_Hip_M_Tat_027",
            femaleHashName = "FM_Hip_F_Tat_027",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_028",
            displayName = "Thorny Rose",
            maleHashName = "FM_Hip_M_Tat_028",
            femaleHashName = "FM_Hip_F_Tat_028",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_029",
            displayName = "Sad",
            maleHashName = "FM_Hip_M_Tat_029",
            femaleHashName = "FM_Hip_F_Tat_029",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_030",
            displayName = "Shark Fin",
            maleHashName = "FM_Hip_M_Tat_030",
            femaleHashName = "FM_Hip_F_Tat_030",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_031",
            displayName = "Skateboard",
            maleHashName = "FM_Hip_M_Tat_031",
            femaleHashName = "FM_Hip_F_Tat_031",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_032",
            displayName = "Paper Plane",
            maleHashName = "FM_Hip_M_Tat_032",
            femaleHashName = "FM_Hip_F_Tat_032",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_033",
            displayName = "Stag",
            maleHashName = "FM_Hip_M_Tat_033",
            femaleHashName = "FM_Hip_F_Tat_033",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_034",
            displayName = "Stop",
            maleHashName = "FM_Hip_M_Tat_034",
            femaleHashName = "FM_Hip_F_Tat_034",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_035",
            displayName = "Sewn Heart",
            maleHashName = "FM_Hip_M_Tat_035",
            femaleHashName = "FM_Hip_F_Tat_035",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_036",
            displayName = "Shapes",
            maleHashName = "FM_Hip_M_Tat_036",
            femaleHashName = "FM_Hip_F_Tat_036",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_037",
            displayName = "Sunrise",
            maleHashName = "FM_Hip_M_Tat_037",
            femaleHashName = "FM_Hip_F_Tat_037",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_038",
            displayName = "Grub",
            maleHashName = "FM_Hip_M_Tat_038",
            femaleHashName = "FM_Hip_F_Tat_038",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_039",
            displayName = "Sleeve",
            maleHashName = "FM_Hip_M_Tat_039",
            femaleHashName = "FM_Hip_F_Tat_039",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_040",
            displayName = "Black Anchor",
            maleHashName = "FM_Hip_M_Tat_040",
            femaleHashName = "FM_Hip_F_Tat_040",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_041",
            displayName = "Tooth",
            maleHashName = "FM_Hip_M_Tat_041",
            femaleHashName = "FM_Hip_F_Tat_041",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_042",
            displayName = "Sparkplug",
            maleHashName = "FM_Hip_M_Tat_042",
            femaleHashName = "FM_Hip_F_Tat_042",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_043",
            displayName = "Triangle White",
            maleHashName = "FM_Hip_M_Tat_043",
            femaleHashName = "FM_Hip_F_Tat_043",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_044",
            displayName = "Triangle Black",
            maleHashName = "FM_Hip_M_Tat_044",
            femaleHashName = "FM_Hip_F_Tat_044",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_045",
            displayName = "Mesh Band",
            maleHashName = "FM_Hip_M_Tat_045",
            femaleHashName = "FM_Hip_F_Tat_045",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_046",
            displayName = "Triangles",
            maleHashName = "FM_Hip_M_Tat_046",
            femaleHashName = "FM_Hip_F_Tat_046",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_047",
            displayName = "Cassette",
            maleHashName = "FM_Hip_M_Tat_047",
            femaleHashName = "FM_Hip_F_Tat_047",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_048",
            displayName = "Peace",
            maleHashName = "FM_Hip_M_Tat_048",
            femaleHashName = "FM_Hip_F_Tat_048",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_000",
            displayName = "Block Back",
            maleHashName = "MP_MP_ImportExport_Tat_000_M",
            femaleHashName = "MP_MP_ImportExport_Tat_000_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_001",
            displayName = "Power Plant",
            maleHashName = "MP_MP_ImportExport_Tat_001_M",
            femaleHashName = "MP_MP_ImportExport_Tat_001_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_002",
            displayName = "Tuned to Death",
            maleHashName = "MP_MP_ImportExport_Tat_002_M",
            femaleHashName = "MP_MP_ImportExport_Tat_002_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_003",
            displayName = "Mechanical Sleeve",
            maleHashName = "MP_MP_ImportExport_Tat_003_M",
            femaleHashName = "MP_MP_ImportExport_Tat_003_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_004",
            displayName = "Piston Sleeve",
            maleHashName = "MP_MP_ImportExport_Tat_004_M",
            femaleHashName = "MP_MP_ImportExport_Tat_004_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_005",
            displayName = "Dialed In",
            maleHashName = "MP_MP_ImportExport_Tat_005_M",
            femaleHashName = "MP_MP_ImportExport_Tat_005_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_006",
            displayName = "Engulfed Block",
            maleHashName = "MP_MP_ImportExport_Tat_006_M",
            femaleHashName = "MP_MP_ImportExport_Tat_006_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_007",
            displayName = "Drive Forever",
            maleHashName = "MP_MP_ImportExport_Tat_007_M",
            femaleHashName = "MP_MP_ImportExport_Tat_007_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_008",
            displayName = "Scarlett",
            maleHashName = "MP_MP_ImportExport_Tat_008_M",
            femaleHashName = "MP_MP_ImportExport_Tat_008_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_009",
            displayName = "Serpents of Destruction",
            maleHashName = "MP_MP_ImportExport_Tat_009_M",
            femaleHashName = "MP_MP_ImportExport_Tat_009_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_010",
            displayName = "Take the Wheel",
            maleHashName = "MP_MP_ImportExport_Tat_010_M",
            femaleHashName = "MP_MP_ImportExport_Tat_010_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpimportexport_overlays",
            tattooName = "TAT_IE_011",
            displayName = "Talk Shit Get Hit",
            maleHashName = "MP_MP_ImportExport_Tat_011_M",
            femaleHashName = "MP_MP_ImportExport_Tat_011_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mphipster_overlays",
            tattooName = "TAT_HP_048",
            displayName = "Peace",
            maleHashName = "FM_Hip_M_Tat_048",
            femaleHashName = "FM_Hip_F_Tat_048",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_000",
            displayName = "SA Assault",
            maleHashName = "MP_LR_Tat_000_M",
            femaleHashName = "MP_LR_Tat_000_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_003",
            displayName = "Lady Vamp",
            maleHashName = "MP_LR_Tat_003_M",
            femaleHashName = "MP_LR_Tat_003_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_006",
            displayName = "Love Hustle",
            maleHashName = "MP_LR_Tat_006_M",
            femaleHashName = "MP_LR_Tat_006_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_008",
            displayName = "Love the Game",
            maleHashName = "MP_LR_Tat_008_M",
            femaleHashName = "MP_LR_Tat_008_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_011",
            displayName = "Lady Liberty",
            maleHashName = "MP_LR_Tat_011_M",
            femaleHashName = "MP_LR_Tat_011_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_012",
            displayName = "Royal Kiss",
            maleHashName = "MP_LR_Tat_012_M",
            femaleHashName = "MP_LR_Tat_012_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_016",
            displayName = "Two Face",
            maleHashName = "MP_LR_Tat_016_M",
            femaleHashName = "MP_LR_Tat_016_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_018",
            displayName = "Skeleton Party",
            maleHashName = "MP_LR_Tat_018_M",
            femaleHashName = "MP_LR_Tat_018_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_019",
            displayName = "Death Behind",
            maleHashName = "MP_LR_Tat_019_M",
            femaleHashName = "MP_LR_Tat_019_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_022",
            displayName = "My Crazy Life",
            maleHashName = "MP_LR_Tat_022_M",
            femaleHashName = "MP_LR_Tat_022_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_028",
            displayName = "Loving Los Muertos",
            maleHashName = "MP_LR_Tat_028_M",
            femaleHashName = "MP_LR_Tat_028_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_029",
            displayName = "Death Us Do Part",
            maleHashName = "MP_LR_Tat_029_M",
            femaleHashName = "MP_LR_Tat_029_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_030",
            displayName = "San Andreas Prayer",
            maleHashName = "MP_LR_Tat_030_M",
            femaleHashName = "MP_LR_Tat_030_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_031",
            displayName = "Dead Pretty",
            maleHashName = "MP_LR_Tat_031_M",
            femaleHashName = "MP_LR_Tat_031_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_032",
            displayName = "Reign Over",
            maleHashName = "MP_LR_Tat_032_M",
            femaleHashName = "MP_LR_Tat_032_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider2_overlays",
            tattooName = "TAT_S2_035",
            displayName = "Black Tears",
            maleHashName = "MP_LR_Tat_035_M",
            femaleHashName = "MP_LR_Tat_035_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_001",
            displayName = "King Fight",
            maleHashName = "MP_LR_Tat_001_M",
            femaleHashName = "MP_LR_Tat_001_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_002",
            displayName = "Holy Mary",
            maleHashName = "MP_LR_Tat_002_M",
            femaleHashName = "MP_LR_Tat_002_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_004",
            displayName = "Gun Mic",
            maleHashName = "MP_LR_Tat_004_M",
            femaleHashName = "MP_LR_Tat_004_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_005",
            displayName = "Love the Game",
            maleHashName = "MP_LR_Tat_008_M",
            femaleHashName = "MP_LR_Tat_008_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_007",
            displayName = "LS Serpent",
            maleHashName = "MP_LR_Tat_007_M",
            femaleHashName = "MP_LR_Tat_007_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_009",
            displayName = "Amazon",
            maleHashName = "MP_LR_Tat_009_M",
            femaleHashName = "MP_LR_Tat_009_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_010",
            displayName = "Bad Angel",
            maleHashName = "MP_LR_Tat_010_M",
            femaleHashName = "MP_LR_Tat_010_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_013",
            displayName = "Love Gamble",
            maleHashName = "MP_LR_Tat_013_M",
            femaleHashName = "MP_LR_Tat_013_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_014",
            displayName = "Love is Blind",
            maleHashName = "MP_LR_Tat_014_M",
            femaleHashName = "MP_LR_Tat_014_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_015",
            displayName = "Seductress",
            maleHashName = "MP_LR_Tat_015_M",
            femaleHashName = "MP_LR_Tat_015_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_017",
            displayName = "Ink Me",
            maleHashName = "MP_LR_Tat_017_M",
            femaleHashName = "MP_LR_Tat_017_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_020",
            displayName = "Presidents",
            maleHashName = "MP_LR_Tat_020_M",
            femaleHashName = "MP_LR_Tat_020_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_021",
            displayName = "Sad Angel",
            maleHashName = "MP_LR_Tat_021_M",
            femaleHashName = "MP_LR_Tat_021_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_023",
            displayName = "Dance of Hearts",
            maleHashName = "MP_LR_Tat_023_M",
            femaleHashName = "MP_LR_Tat_023_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_026",
            displayName = "Royal Takeover",
            maleHashName = "MP_LR_Tat_026_M",
            femaleHashName = "MP_LR_Tat_026_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_027",
            displayName = "Los Santos Life",
            maleHashName = "MP_LR_Tat_027_M",
            femaleHashName = "MP_LR_Tat_027_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mplowrider_overlays",
            tattooName = "TAT_S1_033",
            displayName = "City Sorrow",
            maleHashName = "MP_LR_Tat_033_M",
            femaleHashName = "MP_LR_Tat_033_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_002",
            displayName = "The Howler",
            maleHashName = "MP_LUXE_TAT_002_M",
            femaleHashName = "MP_LUXE_TAT_002_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_005",
            displayName = "Fatal Dagger",
            maleHashName = "MP_LUXE_TAT_005_M",
            femaleHashName = "MP_LUXE_TAT_005_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_010",
            displayName = "Intrometric",
            maleHashName = "MP_LUXE_TAT_010_M",
            femaleHashName = "MP_LUXE_TAT_010_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_011",
            displayName = "Cross of Roses",
            maleHashName = "MP_LUXE_TAT_011_M",
            femaleHashName = "MP_LUXE_TAT_011_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_012",
            displayName = "Geometric Galaxy",
            maleHashName = "MP_LUXE_TAT_012_M",
            femaleHashName = "MP_LUXE_TAT_012_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_016",
            displayName = "Egyptian Mural",
            maleHashName = "MP_LUXE_TAT_016_M",
            femaleHashName = "MP_LUXE_TAT_016_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_S1_010",
            displayName = "Bad Angel",
            maleHashName = "MP_LR_Tat_010_M",
            femaleHashName = "MP_LR_Tat_010_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_017",
            displayName = "Heavenly Deity",
            maleHashName = "MP_LUXE_TAT_017_M",
            femaleHashName = "MP_LUXE_TAT_017_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_018",
            displayName = "Divine Goddess",
            maleHashName = "MP_LUXE_TAT_018_M",
            femaleHashName = "MP_LUXE_TAT_018_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_022",
            displayName = "Cloaked Angel",
            maleHashName = "MP_LUXE_TAT_022_M",
            femaleHashName = "MP_LUXE_TAT_022_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_023",
            displayName = "Starmetric",
            maleHashName = "MP_LUXE_TAT_023_M",
            femaleHashName = "MP_LUXE_TAT_023_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_025",
            displayName = "Reaper Sway",
            maleHashName = "MP_LUXE_TAT_025_M",
            femaleHashName = "MP_LUXE_TAT_025_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_026",
            displayName = "Floral Print",
            maleHashName = "MP_LUXE_TAT_026_M",
            femaleHashName = "MP_LUXE_TAT_026_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_027",
            displayName = "Cobra Dawn",
            maleHashName = "MP_LUXE_TAT_027_M",
            femaleHashName = "MP_LUXE_TAT_027_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_028",
            displayName = "Python Skull",
            maleHashName = "MP_LUXE_TAT_028_M",
            femaleHashName = "MP_LUXE_TAT_028_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_029",
            displayName = "Geometric Design",
            maleHashName = "MP_LUXE_TAT_029_M",
            femaleHashName = "MP_LUXE_TAT_029_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_030",
            displayName = "Geometric Design",
            maleHashName = "MP_LUXE_TAT_030_M",
            femaleHashName = "MP_LUXE_TAT_030_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe2_overlays",
            tattooName = "TAT_L2_031",
            displayName = "Geometric Design",
            maleHashName = "MP_LUXE_TAT_031_M",
            femaleHashName = "MP_LUXE_TAT_031_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_008",
            displayName = "Skull",
            maleHashName = "FM_Tat_Award_M_000",
            femaleHashName = "FM_Tat_Award_F_000",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_009",
            displayName = "Burning Heart",
            maleHashName = "FM_Tat_Award_M_001",
            femaleHashName = "FM_Tat_Award_F_001",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_010",
            displayName = "Grim Reaper Smoking Gun",
            maleHashName = "FM_Tat_Award_M_002",
            femaleHashName = "FM_Tat_Award_F_002",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_011",
            displayName = "Blackjack",
            maleHashName = "FM_Tat_Award_M_003",
            femaleHashName = "FM_Tat_Award_F_003",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_012",
            displayName = "Hustler",
            maleHashName = "FM_Tat_Award_M_004",
            femaleHashName = "FM_Tat_Award_F_004",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_013",
            displayName = "Angel",
            maleHashName = "FM_Tat_Award_M_005",
            femaleHashName = "FM_Tat_Award_F_005",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_014",
            displayName = "Skull and Sword",
            maleHashName = "FM_Tat_Award_M_006",
            femaleHashName = "FM_Tat_Award_F_006",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_015",
            displayName = "Racing Blonde",
            maleHashName = "FM_Tat_Award_M_007",
            femaleHashName = "FM_Tat_Award_F_007",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_016",
            displayName = "Los Santos Customs",
            maleHashName = "FM_Tat_Award_M_008",
            femaleHashName = "FM_Tat_Award_F_008",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_017",
            displayName = "Dragon and Dagger",
            maleHashName = "FM_Tat_Award_M_009",
            femaleHashName = "FM_Tat_Award_F_009",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_018",
            displayName = "Ride or Die",
            maleHashName = "FM_Tat_Award_M_010",
            femaleHashName = "FM_Tat_Award_F_010",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_019",
            displayName = "Blank Scroll",
            maleHashName = "FM_Tat_Award_M_011",
            femaleHashName = "FM_Tat_Award_F_011",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_020",
            displayName = "Embellished Scroll",
            maleHashName = "FM_Tat_Award_M_012",
            femaleHashName = "FM_Tat_Award_F_012",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_021",
            displayName = "Seven Deadly Sins",
            maleHashName = "FM_Tat_Award_M_013",
            femaleHashName = "FM_Tat_Award_F_013",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_022",
            displayName = "Trust No One",
            maleHashName = "FM_Tat_Award_M_014",
            femaleHashName = "FM_Tat_Award_F_014",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_023",
            displayName = "Racing Brunette",
            maleHashName = "FM_Tat_Award_M_015",
            femaleHashName = "FM_Tat_Award_F_015",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_024",
            displayName = "Clown",
            maleHashName = "FM_Tat_Award_M_016",
            femaleHashName = "FM_Tat_Award_F_016",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_025",
            displayName = "Clown and Gun",
            maleHashName = "FM_Tat_Award_M_017",
            femaleHashName = "FM_Tat_Award_F_017",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_026",
            displayName = "Clown Dual Wield",
            maleHashName = "FM_Tat_Award_M_018",
            femaleHashName = "FM_Tat_Award_F_018",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_027",
            displayName = "Clown Dual Wield Dollars",
            maleHashName = "FM_Tat_Award_M_019",
            femaleHashName = "FM_Tat_Award_F_019",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_204",
            displayName = "Brotherhood",
            maleHashName = "FM_Tat_M_000",
            femaleHashName = "FM_Tat_F_000",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_205",
            displayName = "Dragons",
            maleHashName = "FM_Tat_M_001",
            femaleHashName = "FM_Tat_F_001",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_209",
            displayName = "Melting Skull",
            maleHashName = "FM_Tat_M_002",
            femaleHashName = "FM_Tat_F_002",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_206",
            displayName = "Dragons and Skull",
            maleHashName = "FM_Tat_M_003",
            femaleHashName = "FM_Tat_F_003",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_219",
            displayName = "Faith",
            maleHashName = "FM_Tat_M_004",
            femaleHashName = "FM_Tat_F_004",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_201",
            displayName = "Serpents",
            maleHashName = "FM_Tat_M_005",
            femaleHashName = "FM_Tat_F_005",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_202",
            displayName = "Oriental Mural",
            maleHashName = "FM_Tat_M_006",
            femaleHashName = "FM_Tat_F_006",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_210",
            displayName = "The Warrior",
            maleHashName = "FM_Tat_M_007",
            femaleHashName = "FM_Tat_F_007",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_211",
            displayName = "Dragon Mural",
            maleHashName = "FM_Tat_M_008",
            femaleHashName = "FM_Tat_F_008",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_213",
            displayName = "Skull on the Cross",
            maleHashName = "FM_Tat_M_009",
            femaleHashName = "FM_Tat_F_009",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_218",
            displayName = "LS Flames",
            maleHashName = "FM_Tat_M_010",
            femaleHashName = "FM_Tat_F_010",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_214",
            displayName = "LS Script",
            maleHashName = "FM_Tat_M_011",
            femaleHashName = "FM_Tat_F_011",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_220",
            displayName = "Los Santos Bills",
            maleHashName = "FM_Tat_M_012",
            femaleHashName = "FM_Tat_F_012",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_215",
            displayName = "Eagle and Serpent",
            maleHashName = "FM_Tat_M_013",
            femaleHashName = "FM_Tat_F_013",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_207",
            displayName = "Flower Mural",
            maleHashName = "FM_Tat_M_014",
            femaleHashName = "FM_Tat_F_014",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_203",
            displayName = "Zodiac Skull",
            maleHashName = "FM_Tat_M_015",
            femaleHashName = "FM_Tat_F_015",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_216",
            displayName = "Evil Clown",
            maleHashName = "FM_Tat_M_016",
            femaleHashName = "FM_Tat_F_016",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_212",
            displayName = "Tribal",
            maleHashName = "FM_Tat_M_017",
            femaleHashName = "FM_Tat_F_017",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_208",
            displayName = "Serpent Skull",
            maleHashName = "FM_Tat_M_018",
            femaleHashName = "FM_Tat_F_018",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_217",
            displayName = "The Wages of Sin",
            maleHashName = "FM_Tat_M_019",
            femaleHashName = "FM_Tat_F_019",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_221",
            displayName = "Dragon",
            maleHashName = "FM_Tat_M_020",
            femaleHashName = "FM_Tat_F_020",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_222",
            displayName = "Serpent Skull",
            maleHashName = "FM_Tat_M_021",
            femaleHashName = "FM_Tat_F_021",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_223",
            displayName = "Fiery Dragon",
            maleHashName = "FM_Tat_M_022",
            femaleHashName = "FM_Tat_F_022",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_224",
            displayName = "Hottie",
            maleHashName = "FM_Tat_M_023",
            femaleHashName = "FM_Tat_F_023",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_225",
            displayName = "Flaming Cross",
            maleHashName = "FM_Tat_M_024",
            femaleHashName = "FM_Tat_F_024",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_226",
            displayName = "LS Bold",
            maleHashName = "FM_Tat_M_025",
            femaleHashName = "FM_Tat_F_025",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_227",
            displayName = "Smoking Dagger",
            maleHashName = "FM_Tat_M_026",
            femaleHashName = "FM_Tat_F_026",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_228",
            displayName = "Virgin Mary",
            maleHashName = "FM_Tat_M_027",
            femaleHashName = "FM_Tat_F_027",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_229",
            displayName = "Mermaid",
            maleHashName = "FM_Tat_M_028",
            femaleHashName = "FM_Tat_F_028",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_230",
            displayName = "Trinity Knot",
            maleHashName = "FM_Tat_M_029",
            femaleHashName = "FM_Tat_F_029",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_231",
            displayName = "Lucky Celtic Dogs",
            maleHashName = "FM_Tat_M_030",
            femaleHashName = "FM_Tat_F_030",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_232",
            displayName = "Lady M",
            maleHashName = "FM_Tat_M_031",
            femaleHashName = "FM_Tat_F_031",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_233",
            displayName = "Faith",
            maleHashName = "FM_Tat_M_032",
            femaleHashName = "FM_Tat_F_032",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_234",
            displayName = "Chinese Dragon",
            maleHashName = "FM_Tat_M_033",
            femaleHashName = "FM_Tat_F_033",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_235",
            displayName = "Flaming Shamrock",
            maleHashName = "FM_Tat_M_034",
            femaleHashName = "FM_Tat_F_034",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_236",
            displayName = "Dragon",
            maleHashName = "FM_Tat_M_035",
            femaleHashName = "FM_Tat_F_035",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_237",
            displayName = "Way of the Gun",
            maleHashName = "FM_Tat_M_036",
            femaleHashName = "FM_Tat_F_036",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_238",
            displayName = "Grim Reaper",
            maleHashName = "FM_Tat_M_037",
            femaleHashName = "FM_Tat_F_037",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_239",
            displayName = "Dagger",
            maleHashName = "FM_Tat_M_038",
            femaleHashName = "FM_Tat_F_038",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_240",
            displayName = "Broken Skull",
            maleHashName = "FM_Tat_M_039",
            femaleHashName = "FM_Tat_F_039",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_241",
            displayName = "Flaming Skull",
            maleHashName = "FM_Tat_M_040",
            femaleHashName = "FM_Tat_F_040",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_242",
            displayName = "Dope Skull",
            maleHashName = "FM_Tat_M_041",
            femaleHashName = "FM_Tat_F_041",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_243",
            displayName = "Flaming Scorpion",
            maleHashName = "FM_Tat_M_042",
            femaleHashName = "FM_Tat_F_042",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_244",
            displayName = "Indian Ram",
            maleHashName = "FM_Tat_M_043",
            femaleHashName = "FM_Tat_F_043",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_245",
            displayName = "Stone Cross",
            maleHashName = "FM_Tat_M_044",
            femaleHashName = "FM_Tat_F_044",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_246",
            displayName = "Skulls and Rose",
            maleHashName = "FM_Tat_M_045",
            femaleHashName = "FM_Tat_F_045",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "multiplayer_overlays",
            tattooName = "TAT_FM_247",
            displayName = "Lion",
            maleHashName = "FM_Tat_M_047",
            femaleHashName = "FM_Tat_F_047",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_000",
            displayName = "Serpent of Death",
            maleHashName = "MP_LUXE_TAT_000_M",
            femaleHashName = "MP_LUXE_TAT_000_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_001",
            displayName = "Elaborate Los Muertos",
            maleHashName = "MP_LUXE_TAT_001_M",
            femaleHashName = "MP_LUXE_TAT_001_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_003",
            displayName = "Abstract Skull",
            maleHashName = "MP_LUXE_TAT_003_M",
            femaleHashName = "MP_LUXE_TAT_003_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_L2_011",
            displayName = "Cross of Roses",
            maleHashName = "MP_LUXE_TAT_011_M",
            femaleHashName = "MP_LUXE_TAT_011_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_004",
            displayName = "Floral Raven",
            maleHashName = "MP_LUXE_TAT_004_M",
            femaleHashName = "MP_LUXE_TAT_004_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_006",
            displayName = "Adorned Wolf",
            maleHashName = "MP_LUXE_TAT_006_M",
            femaleHashName = "MP_LUXE_TAT_006_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_S1_010",
            displayName = "Bad Angel",
            maleHashName = "MP_LR_Tat_010_M",
            femaleHashName = "MP_LR_Tat_010_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_007",
            displayName = "Eye of the Griffin",
            maleHashName = "MP_LUXE_TAT_007_M",
            femaleHashName = "MP_LUXE_TAT_007_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_008",
            displayName = "Flying Eye",
            maleHashName = "MP_LUXE_TAT_008_M",
            femaleHashName = "MP_LUXE_TAT_008_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_009",
            displayName = "Floral Symmetry",
            maleHashName = "MP_LUXE_TAT_009_M",
            femaleHashName = "MP_LUXE_TAT_009_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_013",
            displayName = "Mermaid Harpist",
            maleHashName = "MP_LUXE_TAT_013_M",
            femaleHashName = "MP_LUXE_TAT_013_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_014",
            displayName = "Ancient Queen",
            maleHashName = "MP_LUXE_TAT_014_M",
            femaleHashName = "MP_LUXE_TAT_014_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_015",
            displayName = "Smoking Sisters",
            maleHashName = "MP_LUXE_TAT_015_M",
            femaleHashName = "MP_LUXE_TAT_015_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_019",
            displayName = "Geisha Bloom",
            maleHashName = "MP_LUXE_TAT_019_M",
            femaleHashName = "MP_LUXE_TAT_019_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_020",
            displayName = "Archangel & Mary",
            maleHashName = "MP_LUXE_TAT_020_M",
            femaleHashName = "MP_LUXE_TAT_020_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_021",
            displayName = "Gabriel",
            maleHashName = "MP_LUXE_TAT_021_M",
            femaleHashName = "MP_LUXE_TAT_021_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpluxe_overlays",
            tattooName = "TAT_LX_024",
            displayName = "Feather Mural",
            maleHashName = "MP_LUXE_TAT_024_M",
            femaleHashName = "MP_LUXE_TAT_024_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_000",
            displayName = "Stunt Skull",
            maleHashName = "MP_MP_Stunt_Tat_000_M",
            femaleHashName = "MP_MP_Stunt_Tat_000_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_001",
            displayName = "8 Eyed Skull",
            maleHashName = "MP_MP_Stunt_tat_001_M",
            femaleHashName = "MP_MP_Stunt_tat_001_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_002",
            displayName = "Big Cat",
            maleHashName = "MP_MP_Stunt_tat_002_M",
            femaleHashName = "MP_MP_Stunt_tat_002_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_003",
            displayName = "Poison Wrench",
            maleHashName = "MP_MP_Stunt_tat_003_M",
            femaleHashName = "MP_MP_Stunt_tat_003_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_004",
            displayName = "Scorpion",
            maleHashName = "MP_MP_Stunt_tat_004_M",
            femaleHashName = "MP_MP_Stunt_tat_004_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_005",
            displayName = "Demon Spark Plug",
            maleHashName = "MP_MP_Stunt_tat_005_M",
            femaleHashName = "MP_MP_Stunt_tat_005_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_006",
            displayName = "Toxic Spider",
            maleHashName = "MP_MP_Stunt_tat_006_M",
            femaleHashName = "MP_MP_Stunt_tat_006_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_007",
            displayName = "Dagger Devil",
            maleHashName = "MP_MP_Stunt_tat_007_M",
            femaleHashName = "MP_MP_Stunt_tat_007_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_008",
            displayName = "Moonlight Ride",
            maleHashName = "MP_MP_Stunt_tat_008_M",
            femaleHashName = "MP_MP_Stunt_tat_008_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_009",
            displayName = "Arachnid of Death",
            maleHashName = "MP_MP_Stunt_tat_009_M",
            femaleHashName = "MP_MP_Stunt_tat_009_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_010",
            displayName = "Grave Vulture",
            maleHashName = "MP_MP_Stunt_tat_010_M",
            femaleHashName = "MP_MP_Stunt_tat_010_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_011",
            displayName = "Wheels of Death",
            maleHashName = "MP_MP_Stunt_tat_011_M",
            femaleHashName = "MP_MP_Stunt_tat_011_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_012",
            displayName = "Punk Biker",
            maleHashName = "MP_MP_Stunt_tat_012_M",
            femaleHashName = "MP_MP_Stunt_tat_012_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_013",
            displayName = "Dirt Track Hero",
            maleHashName = "MP_MP_Stunt_tat_013_M",
            femaleHashName = "MP_MP_Stunt_tat_013_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_014",
            displayName = "Bat Cat of Spades",
            maleHashName = "MP_MP_Stunt_tat_014_M",
            femaleHashName = "MP_MP_Stunt_tat_014_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_015",
            displayName = "Praying Gloves",
            maleHashName = "MP_MP_Stunt_tat_015_M",
            femaleHashName = "MP_MP_Stunt_tat_015_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_016",
            displayName = "Coffin Racer",
            maleHashName = "MP_MP_Stunt_tat_016_M",
            femaleHashName = "MP_MP_Stunt_tat_016_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_017",
            displayName = "Bat Wheel",
            maleHashName = "MP_MP_Stunt_tat_017_M",
            femaleHashName = "MP_MP_Stunt_tat_017_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_018",
            displayName = "Vintage Bully",
            maleHashName = "MP_MP_Stunt_tat_018_M",
            femaleHashName = "MP_MP_Stunt_tat_018_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_019",
            displayName = "Engine Heart",
            maleHashName = "MP_MP_Stunt_tat_019_M",
            femaleHashName = "MP_MP_Stunt_tat_019_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_020",
            displayName = "Piston Angel",
            maleHashName = "MP_MP_Stunt_tat_020_M",
            femaleHashName = "MP_MP_Stunt_tat_020_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_021",
            displayName = "Golden Cobra",
            maleHashName = "MP_MP_Stunt_tat_021_M",
            femaleHashName = "MP_MP_Stunt_tat_021_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_022",
            displayName = "Piston Head",
            maleHashName = "MP_MP_Stunt_tat_022_M",
            femaleHashName = "MP_MP_Stunt_tat_022_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_023",
            displayName = "Tanked",
            maleHashName = "MP_MP_Stunt_tat_023_M",
            femaleHashName = "MP_MP_Stunt_tat_023_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_024",
            displayName = "Road Kill",
            maleHashName = "MP_MP_Stunt_tat_024_M",
            femaleHashName = "MP_MP_Stunt_tat_024_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_025",
            displayName = "Speed Freak",
            maleHashName = "MP_MP_Stunt_tat_025_M",
            femaleHashName = "MP_MP_Stunt_tat_025_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_026",
            displayName = "Winged Wheel",
            maleHashName = "MP_MP_Stunt_tat_026_M",
            femaleHashName = "MP_MP_Stunt_tat_026_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_027",
            displayName = "Punk Road Hog",
            maleHashName = "MP_MP_Stunt_tat_027_M",
            femaleHashName = "MP_MP_Stunt_tat_027_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_028",
            displayName = "Quad Goblin",
            maleHashName = "MP_MP_Stunt_tat_028_M",
            femaleHashName = "MP_MP_Stunt_tat_028_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_029",
            displayName = "Majestic Finish",
            maleHashName = "MP_MP_Stunt_tat_029_M",
            femaleHashName = "MP_MP_Stunt_tat_029_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_030",
            displayName = "Man's Ruin",
            maleHashName = "MP_MP_Stunt_tat_030_M",
            femaleHashName = "MP_MP_Stunt_tat_030_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_031",
            displayName = "Stunt Jesus",
            maleHashName = "MP_MP_Stunt_tat_031_M",
            femaleHashName = "MP_MP_Stunt_tat_031_F",
            zone = "ZONE_LEFT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_032",
            displayName = "Wheelie Mouse",
            maleHashName = "MP_MP_Stunt_tat_032_M",
            femaleHashName = "MP_MP_Stunt_tat_032_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_033",
            displayName = "Sugar Skull Trucker",
            maleHashName = "MP_MP_Stunt_tat_033_M",
            femaleHashName = "MP_MP_Stunt_tat_033_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_034",
            displayName = "Feather Road Kill",
            maleHashName = "MP_MP_Stunt_tat_034_M",
            femaleHashName = "MP_MP_Stunt_tat_034_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_035",
            displayName = "Stuntman's End",
            maleHashName = "MP_MP_Stunt_tat_035_M",
            femaleHashName = "MP_MP_Stunt_tat_035_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_036",
            displayName = "Biker Stallion",
            maleHashName = "MP_MP_Stunt_tat_036_M",
            femaleHashName = "MP_MP_Stunt_tat_036_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_037",
            displayName = "Big Grills",
            maleHashName = "MP_MP_Stunt_tat_037_M",
            femaleHashName = "MP_MP_Stunt_tat_037_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_038",
            displayName = "One Down Five Up",
            maleHashName = "MP_MP_Stunt_tat_038_M",
            femaleHashName = "MP_MP_Stunt_tat_038_F",
            zone = "ZONE_RIGHT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_039",
            displayName = "Kaboom",
            maleHashName = "MP_MP_Stunt_tat_039_M",
            femaleHashName = "MP_MP_Stunt_tat_039_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_040",
            displayName = "Monkey Chopper",
            maleHashName = "MP_MP_Stunt_tat_040_M",
            femaleHashName = "MP_MP_Stunt_tat_040_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_041",
            displayName = "Brapp",
            maleHashName = "MP_MP_Stunt_tat_041_M",
            femaleHashName = "MP_MP_Stunt_tat_041_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_042",
            displayName = "Flaming Quad",
            maleHashName = "MP_MP_Stunt_tat_042_M",
            femaleHashName = "MP_MP_Stunt_tat_042_F",
            zone = "ZONE_HEAD"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_043",
            displayName = "Engine Arm",
            maleHashName = "MP_MP_Stunt_tat_043_M",
            femaleHashName = "MP_MP_Stunt_tat_043_F",
            zone = "ZONE_LEFT_ARM"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_044",
            displayName = "Ram Skull",
            maleHashName = "MP_MP_Stunt_tat_044_M",
            femaleHashName = "MP_MP_Stunt_tat_044_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_045",
            displayName = "Severed Hand",
            maleHashName = "MP_MP_Stunt_tat_045_M",
            femaleHashName = "MP_MP_Stunt_tat_045_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_046",
            displayName = "Full Throttle",
            maleHashName = "MP_MP_Stunt_tat_046_M",
            femaleHashName = "MP_MP_Stunt_tat_046_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_047",
            displayName = "Brake Knife",
            maleHashName = "MP_MP_Stunt_tat_047_M",
            femaleHashName = "MP_MP_Stunt_tat_047_F",
            zone = "ZONE_RIGHT_LEG"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_048",
            displayName = "Racing Doll",
            maleHashName = "MP_MP_Stunt_tat_048_M",
            femaleHashName = "MP_MP_Stunt_tat_048_F",
            zone = "ZONE_TORSO"
            },
            new TattooModel {
            collection = "mpstunt_overlays",
            tattooName = "TAT_ST_049",
            displayName = "Seductive Mechanic",
            maleHashName = "MP_MP_Stunt_tat_049_M",
            femaleHashName = "MP_MP_Stunt_tat_049_F",
            zone = "ZONE_RIGHT_ARM"
            },
        };

        private static Dictionary<string, string> equippedTattoos = new Dictionary<string, string>();
        private Dictionary<string, string> previousTattoos = new Dictionary<string, string>();
        private MenuModel tattooMenu;
        private List<MenuModel> tattooSubMenus;
        private bool inTattooMenu = false;
        private const int tattooPrice = 25;

        public Tattoos(Client client) : base(client)
        {
            Task.Factory.StartNew(async () => {
                var _menuItems = new List<MenuItem>();
                var menuCategories = new Dictionary<string, List<MenuItem>>();
                await tattooModels.ForEachAsync(async o =>
                {
                    var zoneSplit = o.zone.Split('_').ToList();
                    zoneSplit.RemoveAt(0);
                    zoneSplit = zoneSplit.Select(b => b.ToLower().FirstLetterToUpper()).ToList();
                    var zoneName = string.Join(" ", zoneSplit);
                    if (!menuCategories.ContainsKey(zoneName)) menuCategories.Add(zoneName, new List<MenuItem>());
                    menuCategories[zoneName].Add(new MenuItemStandard
                    {
                        Title = o.displayName,
                        OnSelect = item =>
                        {
                            var tattooHash = Game.PlayerPed.Gender == Gender.Male ? o.maleHashName : o.femaleHashName;

                            ((MenuItemStandard)item).Detail = equippedTattoos.ContainsKey(tattooHash) ? "(equipped)" : "";
                        },
                        OnActivate = item =>
                        {
                            var tattooHash = Game.PlayerPed.Gender == Gender.Male ? o.maleHashName : o.femaleHashName;
                            if (!equippedTattoos.ContainsKey(tattooHash))
                            {
                                API.ApplyPedOverlay(Game.PlayerPed.Handle, (uint)Game.GenerateHash(o.collection), (uint)Game.GenerateHash(tattooHash));
                                item.Detail = "(equipped)";
                                equippedTattoos.Add(tattooHash, o.collection);
                            }
                            else
                            {
                                equippedTattoos.Remove(tattooHash);
                                API.ClearPedDecorations(Game.PlayerPed.Handle);
                                equippedTattoos.ToList().ForEach(b =>
                                {
                                    API.ApplyPedOverlay(Game.PlayerPed.Handle, (uint)Game.GenerateHash(b.Value), (uint)Game.GenerateHash(b.Key));
                                });
                                item.Detail = "";
                            }
                        }
                    });
                    await BaseScript.Delay(0);
                });

                tattooSubMenus = new List<MenuModel>();
                menuCategories.ToList().ForEach(o =>
                {
                    tattooSubMenus.Add(new MenuModel
                    {
                        headerTitle = o.Key,
                        menuItems = o.Value
                    });
                    _menuItems.Add(new MenuItemSubMenu
                    {
                        Title = o.Key,
                        SubMenu = tattooSubMenus.Last()
                    });
                });

                tattooMenu = new MenuModel
                {
                    headerTitle = "Tattoos",
                    menuItems = _menuItems
                };

                client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
                {
                    Title = "Tattoos",
                    SubMenu = tattooMenu,
                    OnActivate = item =>
                    {
                        previousTattoos = new Dictionary<string, string>(equippedTattoos);
                        inTattooMenu = true;
                    }
                }, Locations.TattooParlours.Positions.Where(o => o.DistanceToSquared(Game.PlayerPed.Position) < 5.0f).Any, 510);

                Client.RegisterEventHandler("Player.OnLoginComplete", new Action(OnLogin));
                Client.RegisterEventHandler("Player.CheckForInteraciton", new Action(OnInteraction));
                Client.RegisterTickHandler(OnTick);
            });
        }

        public static Dictionary<string, string> CurrentTattoos
        {
            get => equippedTattoos;
            set => equippedTattoos = value;
        }

        private async Task OnTick()
        {
            if (inTattooMenu && InteractionUI.Observer.CurrentMenu != tattooMenu && !tattooSubMenus.Contains(InteractionUI.Observer.CurrentMenu))
            {
                inTattooMenu = false;
                var tattooChanges = equippedTattoos.Count - previousTattoos.Count;
                //Log.ToChat($"tattooChanges {tattooChanges}");
                var tattooCost = tattooChanges * tattooPrice;
                //Log.ToChat(tattooCost.ToString());
                if (tattooCost > 0)
                {
                    var playerSession = Client.Get<SessionManager>().GetPlayer(Game.Player);
                    if (playerSession == null) return;

                    if (await playerSession.CanPayForItem(tattooPrice))
                    {
                        Log.ToChat("[Bank]", $"You paid ${tattooCost} for the tattoo(s)", ConstantColours.Bank);
                        Roleplay.Client.Client.Instance.TriggerServerEvent("Payment.PayForItem", tattooCost, "tattoo shop");
                    }
                    else
                    {
                        Log.ToChat("[Bank]", $"You are not able to pay for these tattoos", ConstantColours.Bank);
                        equippedTattoos = previousTattoos;
                        API.ClearPedDecorations(Game.PlayerPed.Handle);
                        equippedTattoos.ToList().ForEach(o =>
                        {
                            API.ApplyPedOverlay(Game.PlayerPed.Handle, (uint)Game.GenerateHash(o.Value),
                                (uint)Game.GenerateHash(o.Key));
                        });
                    }
                }
                else
                {
                    if (tattooChanges != 0)
                        Log.ToChat("[Tattoo]", $"You removed {tattooChanges * -1} tattoos", ConstantColours.Tattoo);
                }
                CharacterEditorMenu.skinData = new PedData().getSaveableData(true)/*.ToExpando()*/;
                Roleplay.Client.Client.Instance.TriggerServerEvent("Skin.UpdatePlayerSkin", JsonConvert.SerializeObject(CharacterEditorMenu.skinData));
            }
        }

        private async void OnLogin()
        {
            await BlipHandler.AddBlipAsync("Tattoo Parlour", Locations.TattooParlours.Positions, new BlipOptions
            {
                Sprite = BlipSprite.TattooParlor
            });

            await MarkerHandler.AddMarkerAsync(Locations.TattooParlours.Positions, new MarkerOptions
            {
                ScaleFloat = 3.0f
            });
        }

        private void OnInteraction()
        {
            Locations.TattooParlours.Positions.ForEach(o =>
            {
                if (o.DistanceToSquared(Game.PlayerPed.Position) < 5.0f)
                {
                    InteractionUI.Observer.OpenMenu(tattooMenu);
                    equippedTattoos = new Dictionary<string, string>();
                    if (CharacterEditorMenu.skinData.Tattoos.GetType() != typeof(List<object>))
                    {
                        var tatData = (IDictionary<string, object>)CharacterEditorMenu.skinData.Tattoos;
                        tatData.ToList().ForEach(tattooData =>
                        {
                            equippedTattoos.Add(tattooData.Key.ToString(), tattooData.Value.ToString());
                        });
                    }
                    else
                    {
                        ((List<object>)CharacterEditorMenu.skinData.Tattoos).ForEach(m =>
                        {
                            var tatData = (IDictionary<string, object>)m;
                            dynamic tattooData = tatData.ToList().ToDictionary(b => b.Key, b => b.Value).ToExpando();
                            equippedTattoos.Add(tattooData.Key.ToString(), tattooData.Value.ToString());
                        });
                    }
                    previousTattoos = new Dictionary<string, string>(equippedTattoos);
                    inTattooMenu = true;
                }
            });
        }
    }
}
