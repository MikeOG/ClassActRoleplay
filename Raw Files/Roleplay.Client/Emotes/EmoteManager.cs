using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Roleplay.Client.Enums;
using Roleplay.Client.Player.Controls;
using Roleplay.Client.Emotes;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Jobs.EmergencyServices.Police;

namespace Roleplay.Client.Emotes
{
    public class EmoteManager : ClientAccessor
    {
        public static WeaponHash currentWeapon = WeaponHash.Unarmed;
        private string weaponDictionary = "reaction@intimidation@1h";
        private string weaponAnim = "intro";
        public static string CurrentEmote = "";
        public static bool isPlayingEmote;
        private List<WeaponHash> nonHolsteredWeapons = new List<WeaponHash>()
        {
            WeaponHash.SwitchBlade,
            WeaponHash.Knife,
            WeaponHash.Machete,
            WeaponHash.PetrolCan,
            WeaponHash.Flashlight,
            WeaponHash.Flare,
            WeaponHash.FireExtinguisher,
            WeaponHash.Crowbar,
            WeaponHash.Dagger,
            WeaponHash.GolfClub,
            WeaponHash.Bat
        };
        private Animation HandsupAnimation = new Animation("missminuteman_1ig_2", "", "handsup_enter", "", "Handsup", new AnimationOptions
        {
            LoopEnableMovement = true
        });
        private Animation HandsOverHead = new Animation("busted", "", "idle_a", "", "Hands over head", new AnimationOptions
        {
            LoopDoLoop = true,
            LoopEnableMovement = true
        });

        public static bool IsPlayingAnim = false;
        public static readonly Dictionary<string, PlayableAnimation> playerAnimations = new Dictionary<string, PlayableAnimation>
        {
            ["healthkit"] = new Animation("anim@amb@board_room@supervising@", "", "dissaproval_01_lo_amy_skater_01", "", "Health Kit", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 2500,
            }),
            ["drink"] = new Animation("mp_player_inteat@pnq", "", "loop", "", "Drink", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 2500,
            }),
            ["beast"] = new Animation("anim@mp_fm_event@intro", "", "beast_transform", "", "Beast", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 5000,
            }),
            ["chill"] = new Animation("switch@trevor@scares_tramp", "", "trev_scares_tramp_idle_tramp", "", "Chill", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["cloudgaze"] = new Animation("switch@trevor@annoys_sunbathers", "", "trev_annoys_sunbathers_loop_girl", "", "Cloudgaze", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["cloudgaze2"] = new Animation("switch@trevor@annoys_sunbathers", "", "trev_annoys_sunbathers_loop_guy", "", "Cloudgaze 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["prone"] = new Animation("missfbi3_sniping", "", "prone_dave", "", "Prone", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["pullover"] = new Animation("misscarsteal3pullover", "", "pull_over_right", "", "Pullover", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 1300,
            }),
            ["idle"] = new Animation("anim@heists@heist_corona@team_idles@male_a", "", "idle", "", "Idle", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idle8"] = new Animation("amb@world_human_hang_out_street@male_b@idle_a", "", "idle_b", "", "Idle 8"),
            ["idle9"] = new Animation("friends@fra@ig_1", "", "base_idle", "", "Idle 9", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idle10"] = new Animation("mp_move@prostitute@m@french", "", "idle", "", "Idle 10", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["idle11"] = new Animation("random@countrysiderobbery", "", "idle_a", "", "Idle 11", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idle2"] = new Animation("anim@heists@heist_corona@team_idles@female_a", "", "idle", "", "Idle 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idle3"] = new Animation("anim@heists@humane_labs@finale@strip_club", "ped_b_celebrate_intro", "ped_b_celebrate_loop", "", "Idle 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idle4"] = new Animation("anim@mp_celebration@idles@female", "", "celebration_idle_f_a", "", "Idle 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idle5"] = new Animation("anim@mp_corona_idles@female_b@idle_a", "", "idle_a", "", "Idle 5", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idle6"] = new Animation("anim@mp_corona_idles@male_c@idle_a", "", "idle_a", "", "Idle 6", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idle7"] = new Animation("anim@mp_corona_idles@male_d@idle_a", "", "idle_a", "", "Idle 7", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["wait3"] = new Animation("amb@world_human_hang_out_street@female_hold_arm@idle_a", "", "idle_a", "", "Wait 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idledrunk"] = new Animation("random@drunk_driver_1", "", "drunk_driver_stand_loop_dd1", "", "Idle Drunk", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idledrunk2"] = new Animation("random@drunk_driver_1", "", "drunk_driver_stand_loop_dd2", "", "Idle Drunk 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["idledrunk3"] = new Animation("missarmenian2", "", "standing_idle_loop_drunk", "", "Idle Drunk 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["airguitar"] = new Animation("anim@mp_player_intcelebrationfemale@air_guitar", "", "air_guitar", "", "Air Guitar"),
            ["airsynth"] = new Animation("anim@mp_player_intcelebrationfemale@air_synth", "", "air_synth", "", "Air Synth"),
            ["argue"] = new Animation("misscarsteal4@actor", "", "actor_berating_loop", "", "Argue", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["argue2"] = new Animation("oddjobs@assassinate@vice@hooker", "", "argue_a", "", "Argue 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["atm"] = new Scenario("PROP_HUMAN_ATM", "ATM"),
            ["leafblower"] = new Scenario("WORLD_HUMAN_GARDENER_LEAF_BLOWER", "Leafblower"),
            ["bartender"] = new Animation("anim@amb@clubhouse@bar@drink@idle_a", "", "idle_a_bartender", "", "Bartender", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["beg"] = new Scenario("WORLD_HUMAN_BUM_FREEWAY", "Beg"),
            ["blowkiss"] = new Animation("anim@mp_player_intcelebrationfemale@blow_kiss", "", "blow_kiss", "", "Blow Kiss"),
            ["blowkiss2"] = new Animation("anim@mp_player_intselfieblow_kiss", "", "exit", "", "Blow Kiss 2", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 2000
            }),
            ["curtsy"] = new Animation("anim@mp_player_intcelebrationpaired@f_f_sarcastic", "sarcastic_left", "", "", "Curtsy"),
            ["bringiton"] = new Animation("misscommon@response", "", "bring_it_on", "", "Bring It On", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 3000
            }),
            ["comeatmebro"] = new Animation("mini@triathlon", "", "want_some_of_this", "", "Come at me bro", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 2000
            }),
            ["bumsleep"] = new Scenario("WORLD_HUMAN_BUM_SLUMPED", "Bum Sleep"),
            ["puddle"] = new Scenario("WORLD_HUMAN_BUM_WASH", "Puddle"),
            ["bumbin"] = new Scenario("PROP_HUMAN_BUM_BIN", "Bum Bin"),
            ["camera2"] = new Scenario("WORLD_HUMAN_PAPARAZZI", "Camera 2"),
            ["chinup"] = new Scenario("PROP_HUMAN_MUSCLE_CHIN_UPS", "Chinup"),
            ["cop"] = new Scenario("WORLD_HUMAN_COP_IDLES", "Cop"),
            ["cop2"] = new Animation("anim@amb@nightclub@peds@", "", "rcmme_amanda1_stand_loop_cop", "", "Cop 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["cop3"] = new Animation("amb@code_human_police_investigate@idle_a", "", "idle_b", "", "Cop 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["traffic"] = new Animation("amb@world_human_car_park_attendant@male@idle_a", "", "idle_c", "", "Traffic", new AnimationOptions
            {
                Prop = ObjectHash.prop_parking_wand_01,
                PropBone = Bone.PH_R_Hand,
                PropOffset = new Vector3(0.00f, 0.030f, -0.030f),
                PropRotation = new Vector3(0, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = false,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["crossarms"] = new Animation("amb@world_human_hang_out_street@female_arms_crossed@idle_a", "", "idle_a", "", "Crossarms", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["crossarms2"] = new Animation("amb@world_human_hang_out_street@male_c@idle_a", "", "idle_b", "", "Crossarms 2", new AnimationOptions
            {
                LoopEnableMovement = true,
            }),
            ["crossarms3"] = new Animation("anim@heists@heist_corona@single_team", "", "single_team_loop_boss", "", "Crossarms 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["crossarms4"] = new Animation("random@street_race", "", "_car_b_lookout", "", "Crossarms 4", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["crossarms5"] = new Animation("anim@amb@nightclub@peds@", "", "rcmme_amanda1_stand_loop_cop", "", "Crossarms 5", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["crossarms6"] = new Animation("random@shop_gunstore", "", "_idle", "", "Crossarms 6", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["crossarms7"] = new Animation("anim@amb@business@bgen@bgen_no_work@", "", "stand_phone_phoneputdown_idle_nowork", "", "Crossarms 7", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["crossarms8"] = new Animation("mini@hookers_sp", "", "idle_reject_", "", "Crossarms 8", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["crossarmsside"] = new Animation("rcmnigel1a_band_groupies", "", "base_m2", "", "Crossarms Side", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["lookout"] = new Scenario("CODE_HUMAN_CROSS_ROAD_WAIT", "Lookout"),
            ["damn"] = new Animation("gestures@m@standing@casual", "", "gesture_damn", "", "Damn", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 1000
            }),
            ["damn2"] = new Animation("anim@am_hold_up@male", "", "shoplift_mid", "", "Damn 2", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 1000
            }),
            ["pointdown"] = new Animation("gestures@f@standing@casual", "", "gesture_hand_down", "", "Point Down", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 1000
            }),
            ["droptoknees"] = new Animation("random@arrests@busted", "", "idle_a", "", "Drop to Knees", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["facepalm2"] = new Animation("anim@mp_player_intcelebrationfemale@face_palm", "", "face_palm", "", "Facepalm 2", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 8000
            }),
            ["facepalm"] = new Animation("random@car_thief@agitated@idle_a", "", "agitated_idle_a", "", "Facepalm", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 8000
            }),
            ["facepalm3"] = new Animation("missminuteman_1ig_2", "", "tasered_2", "", "Facepalm 3", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 8000
            }),
            ["facepalm4"] = new Animation("anim@mp_player_intupperface_palm", "", "idle_a", "", "Facepalm 4", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDoLoop = true,
            }),
            ["fallover"] = new Animation("random@drunk_driver_1", "drunk_fall_over", "", "", "Fall Over"),
            ["fallover2"] = new Animation("mp_suicide", "pistol", "", "", "Fall Over 2"),
            ["fallover3"] = new Animation("mp_suicide", "pill", "", "", "Fall Over 3"),
            ["fallover4"] = new Animation("friends@frf@ig_2", "", "knockout_plyr", "", "Fall Over 4"),
            ["fallover5"] = new Animation("anim@gangops@hostage@", "", "victim_fail", "", "Fall Over 5"),
            ["fallasleep"] = new Animation("mp_sleep", "", "sleep_loop", "", "Fall Asleep", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDoLoop = true,
            }),
            ["fightme"] = new Animation("anim@deathmatch_intros@unarmed", "intro_male_unarmed_c", "", "", "Fight Me"),
            ["fightme2"] = new Animation("anim@deathmatch_intros@unarmed", "intro_male_unarmed_e", "", "", "Fight Me 2"),
            ["record"] = new Scenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", "Record"),
            ["finger"] = new Animation("anim@mp_player_intselfiethe_bird", "", "idle_a", "", "Finger", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["finger2"] = new Animation("anim@mp_player_intupperfinger", "", "idle_a_fp", "", "Finger 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["flex"] = new Scenario("WORLD_HUMAN_MUSCLE_FLEX", "Flex"),
            ["guard"] = new Scenario("WORLD_HUMAN_GUARD_STAND", "Guard"),
            ["hammer"] = new Scenario("WORLD_HUMAN_HAMMERING", "Hammer"),
            ["handshake"] = new Animation("mp_ped_interaction", "", "handshake_guy_a", "", "Handshake", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 8000
            }),
            ["handshake2"] = new Animation("mp_ped_interaction", "", "handshake_guy_b", "", "Handshake 2", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 8000
            }),
            ["hangout"] = new Scenario("WORLD_HUMAN_HANG_OUT_STREET", "Hangout"),
            ["wait4"] = new Animation("amb@world_human_hang_out_street@Female_arm_side@idle_a", "", "idle_a", "", "Wait 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["wait5"] = new Animation("missclothing", "", "idle_storeclerk", "", "Wait 5", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait6"] = new Animation("timetable@amanda@ig_2", "", "ig_2_base_amanda", "", "Wait 6", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait7"] = new Animation("rcmnigel1cnmt_1c", "", "base", "", "Wait 7", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait8"] = new Animation("rcmjosh1", "", "idle", "", "Wait 8", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait9"] = new Animation("rcmjosh2", "", "josh_2_intp1_base", "", "Wait 9", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait10"] = new Animation("timetable@amanda@ig_3", "", "ig_3_base_tracy", "", "Wait 10", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait11"] = new Animation("misshair_shop@hair_dressers", "", "keeper_base", "", "Wait 11", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["hiking"] = new Animation("move_m@hiking", "", "idle", "", "Hiking", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["hug"] = new Animation("mp_ped_interaction", "", "kisses_guy_a", "", "Hug"),
            ["hug2"] = new Animation("mp_ped_interaction", "", "kisses_guy_b", "", "Hug 2"),
            ["hug3"] = new Animation("mp_ped_interaction", "", "hugs_guy_a", "", "Hug 3"),
            ["impatient"] = new Scenario("WORLD_HUMAN_STAND_IMPATIENT", "Impatient"),
            ["inspect"] = new Animation("random@train_tracks", "", "idle_e", "", "Inspect"),
            ["janitor"] = new Scenario("WORLD_HUMAN_JANITOR", "Janitor"),
            ["jazzhands"] = new Animation("anim@mp_player_intcelebrationfemale@jazz_hands", "", "jazz_hands", "", "Jazzhands", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 6000,
            }),
            ["jog"] = new Scenario("WORLD_HUMAN_JOG_STANDING", "Jog"),
            ["jog2"] = new Animation("amb@world_human_jog_standing@male@idle_a", "", "idle_a", "", "Jog 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["jog3"] = new Animation("amb@world_human_jog_standing@female@idle_a", "", "idle_a", "", "Jog 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["jog4"] = new Animation("amb@world_human_power_walker@female@idle_a", "", "idle_a", "", "Jog 4", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["jog5"] = new Animation("move_m@joy@a", "", "walk", "", "Jog 5", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["jumpingjacks"] = new Animation("timetable@reunited@ig_2", "", "jimmy_getknocked", "", "Jumping Jacks", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["kneel"] = new Scenario("CODE_HUMAN_MEDIC_KNEEL", "Kneel"),
            ["kneel2"] = new Animation("rcmextreme3", "", "idle", "", "Kneel 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["kneel3"] = new Animation("amb@world_human_bum_wash@male@low@idle_a", "", "idle_a", "", "Kneel 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["knock"] = new Animation("timetable@jimmy@doorknock@", "", "knockdoor_idle", "", "Knock", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDoLoop = true,
            }),
            ["knock2"] = new Animation("missheistfbi3b_ig7", "", "lift_fibagent_loop", "", "Knock 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["knucklecrunch"] = new Animation("anim@mp_player_intcelebrationfemale@knuckle_crunch", "", "knuckle_crunch", "", "Knuckle Crunch", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["lapdance"] = new Animation("mp_safehouse", "lap_dance_girl", "", "", "Lapdance"),
            ["lean"] = new Scenario("WORLD_HUMAN_LEANING", "Lean"),
            ["lean2"] = new Animation("amb@world_human_leaning@female@wall@back@hand_up@idle_a", "", "idle_a", "", "Lean 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["lean3"] = new Animation("amb@world_human_leaning@female@wall@back@holding_elbow@idle_a", "", "idle_a", "", "Lean 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["lean4"] = new Animation("amb@world_human_leaning@male@wall@back@foot_up@idle_a", "", "idle_a", "", "Lean 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["lean5"] = new Animation("amb@world_human_leaning@male@wall@back@hands_together@idle_b", "", "idle_b", "", "Lean 5", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["leanflirt"] = new Animation("random@street_race", "", "_car_a_flirt_girl", "", "Lean Flirt", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["leanbar"] = new Scenario("PROP_HUMAN_BUM_SHOPPING_CART", "Lean Bar"),
            ["leanbar2"] = new Animation("amb@prop_human_bum_shopping_cart@male@idle_a", "", "idle_c", "", "Lean Bar 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["leanbar3"] = new Animation("anim@amb@nightclub@lazlow@ig1_vip@", "", "clubvip_base_laz", "", "Lean Bar 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["leanbar4"] = new Animation("anim@heists@prison_heist", "", "ped_b_loop_a", "", "Lean Bar 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["leanhigh"] = new Animation("anim@mp_ferris_wheel", "", "idle_a_player_one", "", "Lean High", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["leanhigh2"] = new Animation("anim@mp_ferris_wheel", "", "idle_a_player_two", "", "Lean High 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["leanside"] = new Animation("timetable@mime@01_gc", "", "idle_a", "", "Leanside", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["leanside2"] = new Animation("misscarstealfinale", "", "packer_idle_1_trevor", "", "Leanside 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["leanside3"] = new Animation("misscarstealfinalecar_5_ig_1", "", "waitloop_lamar", "", "Leanside 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["leanside4"] = new Animation("misscarstealfinalecar_5_ig_1", "", "waitloop_lamar", "", "Leanside 4", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = false,
            }),
            ["leanside5"] = new Animation("rcmjosh2", "", "josh_2_intp1_base", "", "Leanside 5", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = false,
            }),
            ["maid"] = new Scenario("WORLD_HUMAN_MAID_CLEAN", "Maid"),
            ["map"] = new Scenario("WORLD_HUMAN_TOURIST_MAP", "Map"),
            ["me"] = new Animation("gestures@f@standing@casual", "", "gesture_me_hard", "", "Me", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 1000
            }),
            ["mechanic"] = new Animation("mini@repair", "", "fixing_a_ped", "", "Mechanic", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["mechanic2"] = new Animation("amb@world_human_vehicle_mechanic@male@base", "", "idle_a", "", "Mechanic 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["mechanic3"] = new Animation("anim@amb@clubhouse@tutorial@bkr_tut_ig3@", "", "machinic_loop_mechandplayer", "", "Mechanic 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["mechanic4"] = new Animation("anim@amb@clubhouse@tutorial@bkr_tut_ig3@", "", "machinic_loop_mechandplayer", "", "Mechanic 4", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["medic"] = new Scenario("CODE_HUMAN_MEDIC_TEND_TO_DEAD", "Medic"),
            ["medic2"] = new Animation("amb@medic@standing@tendtodead@base", "", "base", "", "Medic 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["meditate"] = new Animation("rcmcollect_paperleadinout@", "", "meditiate_idle", "", "Meditiate", new AnimationOptions // CHANGE ME
            {
                LoopDoLoop = true,
            }),
            ["meditate2"] = new Animation("rcmepsilonism3", "", "ep_3_rcm_marnie_meditating", "", "Meditiate 2", new AnimationOptions // CHANGE ME
            {
                LoopDoLoop = true,
            }),
            ["meditate3"] = new Animation("rcmepsilonism3", "", "base_loop", "", "Meditiate 3", new AnimationOptions // CHANGE ME
            {
                LoopDoLoop = true,
            }),
            ["metal"] = new Animation("anim@mp_player_intincarrockstd@ps@", "enter", "idle_a", "", "Metal", new AnimationOptions // CHANGE ME
            {
                StartEnableMovement = true,
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["musician"] = new Scenario("WORLD_HUMAN_MUSICIAN", "Musician"),
            ["no"] = new Animation("anim@heists@ornate_bank@chat_manager", "", "fail", "", "No", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["no2"] = new Animation("mp_player_int_upper_nod", "", "mp_player_int_nod_no", "", "No 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["nosepick"] = new Animation("anim@mp_player_intcelebrationfemale@nose_pick", "", "nose_pick", "", "Nose Pick", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["notepad2"] = new Scenario("CODE_HUMAN_MEDIC_TIME_OF_DEATH", "Notepad 2"),
            ["noway"] = new Animation("gestures@m@standing@casual", "", "gesture_no_way", "", "No Way", new AnimationOptions
            {
                LoopDuration = 1500,
                LoopEnableMovement = true,
            }),
            ["ok"] = new Animation("anim@mp_player_intselfiedock", "", "idle_a", "", "OK", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["outofbreath"] = new Animation("re@construction", "", "out_of_breath", "", "Out of Breath", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["parkingmeter"] = new Scenario("PROP_HUMAN_PARKING_METER", "Parking Meter"),
            ["party"] = new Scenario("WORLD_HUMAN_PARTYING", "Party"),
            ["phone3"] = new Scenario("WORLD_HUMAN_STAND_MOBILE", "Phone 3"),
            ["pickup"] = new Animation("random@domestic", "pickup_low", "", "", "Pickup"),
            ["push"] = new Animation("missfinale_c2ig_11", "", "pushcar_offcliff_f", "", "Push", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["push2"] = new Animation("missfinale_c2ig_11", "", "pushcar_offcliff_m", "", "Push 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["point"] = new Animation("gestures@f@standing@casual", "", "gesture_point", "", "Point", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["prosthigh"] = new Scenario("WORLD_HUMAN_PROSTITUTE_HIGH_CLASS", "Prostitue High"),
            ["prostlow"] = new Scenario("WORLD_HUMAN_PROSTITUTE_LOW_CLASS", "Prostitue Low"),
            ["pushup"] = new Animation("amb@world_human_push_ups@male@idle_a", "", "idle_d", "", "Pushup", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["countdown"] = new Animation("random@street_race", "", "grid_girl_race_start", "", "Countdown", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["pointright"] = new Animation("mp_gun_shop_tut", "", "indicate_right", "", "Point Right", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["salute"] = new Animation("anim@mp_player_intincarsalutestd@ds@", "", "idle_a", "", "Salute", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["salute2"] = new Animation("anim@mp_player_intincarsalutestd@ps@", "", "idle_a", "", "Salute 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["salute3"] = new Animation("anim@mp_player_intuppersalute", "", "idle_a", "", "Salute 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["scared"] = new Animation("random@domestic", "", "f_distressed_loop", "", "Scared", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["scared2"] = new Animation("random@homelandsecurity", "", "knees_loop_girl", "", "Scared 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["screwyou"] = new Animation("misscommon@response", "", "screw_you", "", "Screw You", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["shakeoff"] = new Animation("move_m@_idles@shake_off", "", "shakeoff_1", "", "Shake Off", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 3500,
            }),
            ["shot"] = new Animation("random@dealgonewrong", "", "idle_a", "", "Shot", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sleep"] = new Animation("timetable@tracy@sleep@", "", "idle_c", "", "Sleep", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["shrug"] = new Animation("gestures@f@standing@casual", "", "gesture_shrug_hard", "", "Shrug", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 1000,
            }),
            ["shrug2"] = new Animation("gestures@m@standing@casual", "", "gesture_shrug_hard", "", "Shrug 2", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 1000,
            }),
            ["sit"] = new Animation("anim@amb@business@bgen@bgen_no_work@", "", "sit_phone_phoneputdown_idle_nowork", "", "Sit", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sit2"] = new Animation("rcm_barry3", "", "barry_3_sit_loop", "", "Sit 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sit3"] = new Animation("amb@world_human_picnic@male@idle_a", "", "idle_a", "", "Sit 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sit4"] = new Animation("amb@world_human_picnic@female@idle_a", "", "idle_a", "", "Sit 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sit5"] = new Animation("anim@heists@fleeca_bank@ig_7_jetski_owner", "", "owner_idle", "", "Sit 5", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sit6"] = new Animation("timetable@jimmy@mics3_ig_15@", "", "idle_a_jimmy", "", "Sit 6", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sit7"] = new Animation("anim@amb@nightclub@lazlow@lo_alone@", "", "lowalone_base_laz", "", "Sit 7", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sit8"] = new Animation("timetable@jimmy@mics3_ig_15@", "", "mics3_15_base_jimmy", "", "Sit 8", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sit9"] = new Animation("amb@world_human_stupor@male@idle_a", "", "idle_a", "", "Sit 9", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitlean"] = new Animation("timetable@tracy@ig_14@", "", "ig_14_base_tracy", "", "Sit Lean", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitsad"] = new Animation("anim@amb@business@bgen@bgen_no_work@", "", "sit_phone_phoneputdown_sleeping-noworkfemale", "", "Sit Sad", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitscared"] = new Animation("anim@heists@ornate_bank@hostages@hit", "", "hit_loop_ped_b", "", "Sit Scared", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitscared2"] = new Animation("anim@heists@ornate_bank@hostages@ped_c@", "", "flinch_loop", "", "Sit Scared 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitscared3"] = new Animation("anim@heists@ornate_bank@hostages@ped_e@", "", "flinch_loop", "", "Sit Scared 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitdrunk"] = new Animation("timetable@amanda@drunk@base", "", "base", "", "Sit Drunk", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitchair"] = new Scenario("PROP_HUMAN_SEAT_CHAIR_MP_PLAYER", "Sit Chair"),
            ["sitchair2"] = new Animation("timetable@ron@ig_5_p3", "", "ig_5_p3_base", "", "Sit Chair 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitchair3"] = new Animation("timetable@reunited@ig_10", "", "base_amanda", "", "Sit Chair 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitchair4"] = new Animation("timetable@ron@ig_3_couch", "", "base", "", "Sit Chair 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitchair5"] = new Animation("timetable@jimmy@mics3_ig_15@", "", "mics3_15_base_tracy", "", "Sit Chair 5", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitchair6"] = new Animation("timetable@maid@couch@", "", "base", "", "Sit Chair 6", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sitchairside"] = new Animation("timetable@ron@ron_ig_2_alt1", "", "ig_2_alt1_base", "", "Sit Chair Side", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["situp"] = new Animation("amb@world_human_sit_ups@male@idle_a", "", "idle_a", "", "Sit Up", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["clapangry"] = new Animation("anim@arena@celeb@flat@solo@no_props@", "", "angry_clap_a_player_a", "", "Clap Angry", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["slowclap3"] = new Animation("anim@mp_player_intupperslow_clap", "", "idle_a", "", "Slow Clap 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["clap"] = new Animation("amb@world_human_cheering@male_a", "", "base", "", "Clap", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["slowclap"] = new Animation("anim@mp_player_intcelebrationfemale@slow_clap", "", "slow_clap", "", "Slow Clap", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["slowclap2"] = new Animation("anim@mp_player_intcelebrationmale@slow_clap", "", "slow_clap", "", "Slow Clap 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["smell"] = new Animation("move_p_m_two_idles@generic", "", "fidget_sniff_fingers", "", "Smell", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["smoke3"] = new Scenario("WORLD_HUMAN_DRUG_DEALER", "Smoke 3"),
            ["smokeleanf"] = new Animation("amb@world_human_leaning@female@smoke@idle_a", "", "idle_a", "", "Smoke Lean F", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["stickup"] = new Animation("random@countryside_gang_fight", "", "biker_02_stickup_loop", "", "Stick Up", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["stumble"] = new Animation("misscarsteal4@actor", "stumble", "", "", "Stumble", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["stunned"] = new Animation("stungun@standing", "", "damage", "", "Stunned", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sunbathe"] = new Animation("amb@world_human_sunbahte@male@back@base", "", "base", "", "Sunbathe", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sunbathe2"] = new Animation("amb@world_human_sunbahte@female@back@base", "", "base", "", "Sunbathe 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["sunbathe3"] = new Scenario("WORLD_HUMAN_SUNBATHE", "Sunbathe 3"),
            ["sunbatheback"] = new Scenario("WORLD_HUMAN_SUNBATHE_BACK", "Sunbathe Back"),
            ["t"] = new Animation("missfam5_yoga", "", "a2_pose", "", "T", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["t2"] = new Animation("mp_sleep", "", "bind_pose_180", "", "T 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["think4"] = new Animation("mp_cp_welcome_tutthink", "", "b_think", "", "Think 4", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 2000,
            }),
            ["think"] = new Animation("misscarsteal4@aliens", "", "rehearsal_base_idle_director", "", "Think", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["think3"] = new Animation("timetable@tracy@ig_8@base", "", "base", "", "Think 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["think2"] = new Animation("missheist_jewelleadinout", "", "jh_int_outro_loop_a", "", "Think 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["thumbsup3"] = new Animation("anim@mp_player_intincarthumbs_uplow@ds@", "", "enter", "", "Thumbs Up 3", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 3000,
            }),
            ["thumbsup2"] = new Animation("anim@mp_player_intselfiethumbs_up", "", "idle_a", "", "Thumbs Up 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["thumbsup"] = new Animation("anim@mp_player_intupperthumbs_up", "", "idle_a", "", "Thumbs Up", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["type"] = new Animation("anim@heists@prison_heiststation@cop_reactions", "", "cop_b_idle", "", "Type", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["type2"] = new Animation("anim@heists@prison_heistig1_p1_guard_checks_bus", "", "loop", "", "Type 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["type3"] = new Animation("mp_prison_break", "", "hack_loop", "", "Type 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["type4"] = new Animation("mp_fbi_heist", "intro", "loop", "outro", "Type 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["warmth"] = new Animation("amb@world_human_stand_fire@male@idle_a", "", "idle_a", "", "Warmth", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wave4"] = new Animation("random@mugging5", "", "001445_01_gangintimidation_1_female_idle_b", "", "Wave 4", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 3000,
            }),
            ["wave2"] = new Animation("anim@mp_player_intcelebrationfemale@wave", "", "wave", "", "Wave 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wave3"] = new Animation("friends@fra@ig_1", "", "over_here_idle_a", "", "Wave 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wave"] = new Animation("friends@frj@ig_1", "", "wave_a", "", "Wave", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wave5"] = new Animation("friends@frj@ig_1", "", "wave_b", "", "Wave 5", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wave6"] = new Animation("friends@frj@ig_1", "", "wave_c", "", "Wave 6", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wave7"] = new Animation("friends@frj@ig_1", "", "wave_d", "", "Wave 7", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wave8"] = new Animation("friends@frj@ig_1", "", "wave_e", "", "Wave 8", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wave9"] = new Animation("gestures@m@standing@casual", "", "gesture_hello", "", "Wave 9", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["weights"] = new Scenario("WORLD_HUMAN_MUSCLE_FLEX", "Weights"),
            ["weld"] = new Scenario("WORLD_HUMAN_WELDING", "Weld"),
            ["whistle"] = new Animation("taxi_hail", "", "hail_taxi", "", "Whistle", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 2000,
            }),
            ["whistle2"] = new Animation("rcmnigel1c", "", "hailing_whistle_waive_a", "", "Whistle 2", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 2000,
            }),
            ["windowshop"] = new Scenario("WORLD_HUMAN_WINDOW_SHOP_BROWSE", "Window Shop"),
            ["yeah"] = new Animation("anim@mp_player_intupperair_shagging", "", "idle_a", "", "Yeah", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["yoga"] = new Scenario("WORLD_HUMAN_YOGA", "Yoga"),
            ["lift"] = new Animation("random@hitch_lift", "", "idle_f", "", "Lift", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["lol"] = new Animation("anim@arena@celeb@flat@paired@no_props@", "", "laugh_a_player_b", "", "LOL", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["lol2"] = new Animation("anim@arena@celeb@flat@solo@no_props@", "", "giggle_a_player_b", "", "LOL 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["statue"] = new Animation("amb@world_human_statue@idle_a", "", "idle_a", "", "Statue", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["statue2"] = new Animation("fra_0_int-1", "", "cs_lamardavis_dual-1", "", "Statue 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["statue3"] = new Animation("club_intro2-0", "", "csb_englishdave_dual-0", "", "Statue 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["gangsign"] = new Animation("mp_player_int_uppergang_sign_a", "", "mp_player_int_gang_sign_a", "", "Gang Sign", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["gangsign2"] = new Animation("mp_player_int_uppergang_sign_b", "", "mp_player_int_gang_sign_b", "", "Gang Sign 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["passout"] = new Animation("missarmenian2", "", "drunk_loop", "", "Passout", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["passout2"] = new Animation("missarmenian2", "", "corpse_search_exit_ped", "", "Passout 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["passout3"] = new Animation("anim@gangops@morgue@table@", "", "body_search", "", "Passout 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["passout4"] = new Animation("mini@cpr@char_b@cpr_def", "", "cpr_pumpchest_idle", "", "Passout 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["passout5"] = new Animation("random@mugging4", "", "flee_backward_loop_shopkeeper", "", "Passout 5", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["petting"] = new Animation("creatures@rottweiler@tricks@", "", "petting_franklin", "", "Petting", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancef"] = new Animation("anim@amb@nightclub@dancers@solomun_entourage@", "", "mi_dance_facedj_17_v1_female^1", "", "Dance F", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancef2"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "", "high_center", "", "Dance F2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancef3"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "", "high_center_up", "", "Dance F3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancef4"] = new Animation("anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity", "", "hi_dance_facedj_09_v2_female^1", "", "Dance F4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancef5"] = new Animation("anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity", "", "hi_dance_facedj_09_v2_female^3", "", "Dance F5", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancef6"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "", "high_center_up", "", "Dance F6", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancefslow"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "", "low_center", "", "Dance F Slow", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancefslow2"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "", "low_center_down", "", "Dance F Slow 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dancefslow3"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "", "low_center", "", "Dance F Slow 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance"] = new Animation("anim@amb@nightclub@dancers@podium_dancers@", "", "hi_dance_facedj_17_v2_male^5", "", "Dance", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance2"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@", "", "high_center_down", "", "Dance 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance3"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@male@var_a@", "", "high_center", "", "Dance 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance4"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@", "", "high_center_up", "", "Dance 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance5"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "", "high_center", "", "Dance 5", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["dance6"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "", "high_center_up", "", "Dance 6", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["danceshy"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@male@var_a@", "", "low_center", "", "Dance Shy", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["danceshy2"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "", "low_center_down", "", "Dance Shy 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["danceslow"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@", "", "low_center", "", "Dance Slow", new AnimationOptions
            {
                LoopDoLoop = true,
            }),

            ["dancehorse"] = new Animation("anim@amb@nightclub@lazlow@hi_dancefloor@", "", "dancecrowd_li_15_handup_laz", "", "Dance dancehorse", new AnimationOptions
            {
                Prop = ObjectHash.ba_prop_battle_hobby_horse,
                PropBone = Bone.PH_R_Hand,
                PropOffset = new Vector3(0.0f, 0.0f, 0.0f),
                //PropRotation = new Vector3(-65f, -18f, -120f),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),

            ["dancehorse2"] = new Animation("anim@amb@nightclub@lazlow@hi_dancefloor@", "", "crowddance_hi_11_handup_laz", "", "Dance dancehorse2", new AnimationOptions
            {
                Prop = ObjectHash.ba_prop_battle_hobby_horse,
                PropBone = Bone.PH_R_Hand,
                PropOffset = new Vector3(0.0f, 0.0f, 0.0f),
                //PropRotation = new Vector3(-65f, -18f, -120f),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),

            ["dancehorse"] = new Animation("anim@amb@nightclub@lazlow@hi_dancefloor@", "", "dancecrowd_li_11_hu_shimmy_laz", "", "Dance dancehorse3", new AnimationOptions
            {
                Prop = ObjectHash.ba_prop_battle_hobby_horse,
                PropBone = Bone.PH_R_Hand,
                PropOffset = new Vector3(0.0f, 0.0f, 0.0f),
                //PropRotation = new Vector3(-65f, -18f, -120f),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),

            ["crawl"] = new Animation("move_injured_ground", "", "front_loop", "", "Crawl", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["flip2"] = new Animation("anim@arena@celeb@flat@solo@no_props@", "", "cap_a_player_a", "", "Flip 2"),
            ["flip"] = new Animation("anim@arena@celeb@flat@solo@no_props@", "", "flip_a_player_a", "", "Flip"),
            ["slide"] = new Animation("anim@arena@celeb@flat@solo@no_props@", "", "slide_a_player_a", "", "Slide"),
            ["slide2"] = new Animation("anim@arena@celeb@flat@solo@no_props@", "", "slide_b_player_a", "", "Slide 2"),
            ["slide3"] = new Animation("anim@arena@celeb@flat@solo@no_props@", "", "slide_c_player_a", "", "Slide 3"),
            ["slugger"] = new Animation("anim@arena@celeb@flat@solo@no_props@", "", "slugger_a_player_a", "", "Slugger"),
            ["flipoff"] = new Animation("anim@arena@celeb@podium@no_prop@", "", "flip_off_a_1st", "", "Flip Off", new AnimationOptions
            {
                LoopEnableMovement = true,
            }),
            ["flipoff2"] = new Animation("anim@arena@celeb@podium@no_prop@", "", "flip_off_c_1st", "", "Flip Off 2", new AnimationOptions
            {
                LoopEnableMovement = true,
            }),
            ["bow"] = new Animation("anim@arena@celeb@podium@no_prop@", "", "regal_c_1st", "", "Bow", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["bow2"] = new Animation("anim@arena@celeb@podium@no_prop@", "", "regal_a_1st", "", "Bow 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["dance7"] = new Animation("rcmnigel1bnmt_1b", "", "dance_loop_tyler", "", "Dance 7", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance8"] = new Animation("misschinese2_crystalmazemcs1_cs", "", "dance_loop_tao", "", "Dance 8", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance9"] = new Animation("misschinese2_crystalmazemcs1_ig", "", "dance_loop_tao", "", "Dance 9", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance10"] = new Animation("missfbi3_sniping", "", "dance_m_default", "", "Dance 10", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance11"] = new Animation("special_ped@mountain_dancer@monologue_3@monologue_3a", "", "mnt_dnc_buttwag", "", "Dance 11", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance12"] = new Animation("move_clown@p_m_zero_idles@", "", "fidget_short_dance", "", "Dance 12", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance13"] = new Animation("move_clown@p_m_two_idles@", "", "fidget_short_dance", "", "Dance 13", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance14"] = new Animation("anim@amb@nightclub@lazlow@hi_podium@", "", "danceidle_hi_11_buttwiggle_b_laz", "", "Dance 14", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance15"] = new Animation("timetable@tracy@ig_5@idle_a", "", "idle_a", "", "Dance 15", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance16"] = new Animation("timetable@tracy@ig_8@idle_b", "", "idle_d", "", "Dance 16", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["dance17"] = new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "", "med_center_up", "", "Dance 17", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9bark"] = new Animation("creatures@rottweiler@amb@world_dog_barking@idle_a", "", "idle_a", "", "zk9bark", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9barksit"] = new Animation("misschop_vehicle@back_of_van", "", "chop_bark", "", "zk9barksit", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9growl"] = new Animation("misschop_vehicle@back_of_van", "", "chop_growl", "", "zk9growl", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9barkup"] = new Animation("creatures@rottweiler@amb@world_dog_barking@idle_a", "", "idle_b", "", "zk9barkup", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9shake"] = new Animation("creatures@rottweiler@amb@world_dog_barking@idle_a", "", "idle_c", "", "zk9shake", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9beg"] = new Animation("creatures@rottweiler@tricks@", "", "beg_loop", "", "zk9beg", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9paw"] = new Animation("creatures@rottweiler@tricks@", "", "paw_loop", "", "zk9paw", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9pet"] = new Animation("creatures@rottweiler@tricks@", "", "petting_chop", "", "zk9pet", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9focus"] = new Animation("missfra0_chop_shared", "", "chop_bark_at_carriage", "", "zk9focus", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9focus2"] = new Animation("creatures@rottweiler@indication@", "", "indicate_high", "", "zk9focus2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9sit"] = new Animation("creatures@retriever@amb@world_dog_sitting@idle_a", "", "idle_b", "", "zk9sit", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9sitscratch"] = new Animation("creatures@retriever@amb@world_dog_sitting@idle_a", "", "idle_a", "", "zk9sitscratch", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9tail"] = new Animation("creatures@retriever@amb@world_dog_barking@idle_a", "", "idle_c", "", "zk9tail", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["blahblah"] = new Animation("facials@creatures@retriever@bark", "", "bark_facial", "", "Blahblah", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9wag"] = new Animation("es_4_rcm_p1-3", "", "a_c_retriever_dual-3", "", "zk9wag", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["k9sleep"] = new Animation("creatures@rottweiler@amb@sleep_in_kennel@", "", "sleep_in_kennel", "", "zk9sleep", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["keyfob"] = new Animation("anim@mp_player_intmenu@key_fob@", "", "fob_click", "", "Key Fob", new AnimationOptions
            {
                LoopDoLoop = false,
                LoopEnableMovement = true,
                LoopDuration = 1000,
            }),
            ["golfswing"] = new Animation("rcmnigel1d", "", "swing_a_mark", "", "Golf Swing"),
            ["eat"] = new Animation("mp_player_inteat@burger", "", "mp_player_int_eat_burger", "", "Eat", new AnimationOptions
            {
                LoopEnableMovement = true,
                LoopDuration = 3000,
            }),
            ["reaching"] = new Animation("move_m@intimidation@cop@unarmed", "", "idle", "", "Reaching", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait"] = new Animation("random@shop_tattoo", "", "_idle_a", "", "Wait", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait2"] = new Animation("missbigscore2aig_3", "", "wait_for_van_c", "", "Wait 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait12"] = new Animation("rcmjosh1", "", "idle", "", "Wait 12", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["wait13"] = new Animation("rcmnigel1a", "", "base", "", "Wait 13", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["lapdance2"] = new Animation("mini@strip_club@private_dance@idle", "", "priv_dance_idle", "", "Lapdance 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["lapdance3"] = new Animation("mini@strip_club@private_dance@part2", "", "priv_dance_p2", "", "Lapdance 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["lapdance3"] = new Animation("mini@strip_club@private_dance@part3", "", "priv_dance_p3", "", "Lapdance 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["twerk"] = new Animation("switch@trevor@mocks_lapdance", "", "001443_01_trvs_28_idle_stripper", "", "Twerk", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["slap"] = new Animation("melee@unarmed@streamed_variations", "", "plyr_takedown_front_slap", "", "Slap", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["headbutt"] = new Animation("melee@unarmed@streamed_variations", "", "plyr_takedown_front_headbutt", "", "Headbutt"),
            ["fishdance"] = new Animation("anim@mp_player_intupperfind_the_fish", "", "idle_a", "", "Fish Dance", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["peace"] = new Animation("mp_player_int_upperpeace_sign", "", "mp_player_int_peace_sign", "", "Peace", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["peace2"] = new Animation("anim@mp_player_intupperpeace", "", "idle_a", "", "Peace 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["cpr"] = new Animation("mini@cpr@char_a@cpr_str", "", "cpr_pumpchest", "", "CPR", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["cpr2"] = new Animation("mini@cpr@char_a@cpr_str", "", "cpr_pumpchest", "", "CPR 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["ledge"] = new Animation("missfbi1", "", "ledge_loop", "", "Ledge", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["airplane"] = new Animation("missfbi1", "", "ledge_loop", "", "Air Plane", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["peek"] = new Animation("random@paparazzi@peek", "", "left_peek_a", "", "Peek", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["cough"] = new Animation("timetable@gardener@smoking_joint", "", "idle_cough", "", "Cough", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["stretch"] = new Animation("mini@triathlon", "", "idle_e", "", "Stretch", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["stretch2"] = new Animation("mini@triathlon", "", "idle_f", "", "Stretch 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["stretch3"] = new Animation("mini@triathlon", "", "idle_d", "", "Stretch 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["stretch4"] = new Animation("rcmfanatic1maryann_stretchidle_b", "", "idle_e", "", "Stretch 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["celebrate"] = new Animation("rcmfanatic1celebrate", "", "celebrate", "", "Celebrate", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["punching"] = new Animation("rcmextreme2", "", "loop_punching", "", "Punching", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["superhero"] = new Animation("rcmbarry", "", "base", "", "Superhero", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["superhero2"] = new Animation("rcmbarry", "", "base", "", "Superhero 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["mindcontrol"] = new Animation("rcmbarry", "", "mind_control_b_loop", "", "Mind Control", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["mindcontrol2"] = new Animation("rcmbarry", "", "bar_1_attack_idle_aln", "", "Mind Control 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["clown"] = new Animation("rcm_barry2", "", "clown_idle_0", "", "Clown", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["clown2"] = new Animation("rcm_barry2", "", "clown_idle_1", "", "Clown 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["clown3"] = new Animation("rcm_barry2", "", "clown_idle_2", "", "Clown 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["clown4"] = new Animation("rcm_barry2", "", "clown_idle_3", "", "Clown 4", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["clown5"] = new Animation("rcm_barry2", "", "clown_idle_6", "", "Clown 5", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["tryclothes"] = new Animation("mp_clothing@female@shoes", "", "try_trousers_neutral_a", "", "Try Clothes", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["tryclothes2"] = new Animation("mp_clothing@female@shoes", "", "try_shirt_positive_a", "", "Try Clothes 2", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["tryclothes3"] = new Animation("mp_clothing@female@shoes", "", "try_shoes_positive_a", "", "Try Clothes 3", new AnimationOptions
            {
                LoopDoLoop = true,
            }),
            ["nervous2"] = new Animation("mp_missheist_countrybank@nervous", "", "nervous_idle", "", "Nervous 2", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["nervous"] = new Animation("amb@world_human_bum_standing@twitchy@idle_a", "", "idle_c", "", "Nervous", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["nervous3"] = new Animation("rcmme_tracey1", "", "nervous_loop", "", "Nervous 3", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["uncuff"] = new Animation("mp_arresting", "", "a_uncuff", "", "Uncuff", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["namaste"] = new Animation("timetable@amanda@ig_4", "", "ig_4_base", "", "Namaste", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["dj"] = new Animation("anim@amb@nightclub@djs@dixon@", "", "dixn_dance_cntr_open_dix", "", "DJ", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["threaten"] = new Animation("random@atmrobberygen", "", "b_atm_mugging", "", "Threaten", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["radio"] = new Animation("random@arrests", "", "generic_radio_chatter", "", "Radio", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["pull"] = new Animation("random@mugging4", "", "struggle_loop_b_thief", "", "Pull", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["bird"] = new Animation("random@peyote@bird", "", "wakeup", "", "Bird"),
            ["chicken"] = new Animation("random@peyote@chicken", "", "wakeup", "", "Chicken", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
            }),
            ["bark"] = new Animation("random@peyote@dog", "", "wakeup", "", "Bark"),
            ["pee"] = new Animation("misscarsteal2peeing", "peeing_intro", "peeing_loop", "", "Pee", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
                StartEnableMovement = true,
            }),
            ["rabbit"] = new Animation("random@peyote@rabbit", "", "wakeup", "", "Rabbit"),
            ["book"] = new Animation("cellphone@", "", "cellphone_text_read_base", "", "Book", new AnimationOptions
            {
                Prop = ObjectHash.prop_novel_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.1000f, 0.0300f, -0.0600f),
                PropRotation = new Vector3(0, -160, -80),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["spray2"] = new Animation("switch@franklin@lamar_tagging_wall", "", "lamar_tagging_wall_loop_lamar", "", "Spray 2", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_spray_can,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0300f, -0.0100f, -0.0900f),
                PropRotation = new Vector3(130, 150, -20),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["spray"] = new Animation("switch@franklin@lamar_tagging_wall", "", "lamar_tagging_exit_loop_lamar", "", "Spray", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_spray_can,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0300f, -0.0100f, -0.0900f),
                PropRotation = new Vector3(130, 150, -20),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["pasta"] = new Animation("impexp_int-0", "", "mp_m_waremech_01_dual-0", "", "Pasta", new AnimationOptions
            {
                Prop = ObjectHash.v_res_foodjarc,
                PropBone = Bone.SKEL_Spine2,
                PropOffset = new Vector3(-0.2500f, 0.3500f, -0.0100f),
                PropRotation = new Vector3(0, 90, 10),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["basketball"] = new Animation("impexp_int-0", "", "mp_m_waremech_01_dual-0", "", "Basketball", new AnimationOptions
            {
                Prop = ObjectHash.prop_bskball_01,
                PropBone = Bone.SKEL_Spine2,
                PropOffset = new Vector3(-0.1000f, 0.3700f, -0.0100f),
                PropRotation = new Vector3(80, 30, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["bouquet"] = new Animation("impexp_int-0", "", "mp_m_waremech_01_dual-0", "", "Bouquet", new AnimationOptions
            {
                Prop = ObjectHash.prop_snow_flower_02,
                PropBone = Bone.SKEL_Spine2,
                PropOffset = new Vector3(-0.2900f, 0.4000f, -0.0200f),
                PropRotation = new Vector3(0, -90, -90),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["teddy"] = new Animation("impexp_int-0", "", "mp_m_waremech_01_dual-0", "", "Teddy", new AnimationOptions
            {
                Prop = ObjectHash.v_ilev_mr_rasberryclean,
                PropBone = Bone.SKEL_Spine2,
                PropOffset = new Vector3(-0.2040f, 0.4610f, -0.0160f),
                PropRotation = new Vector3(-180, 90, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["yeehaw"] = new Animation("amb@world_human_cop_idles@female@idle_a", "", "idle_a", "", "Yeehaw", new AnimationOptions
            {
                Prop = ObjectHash.prop_ld_hat_01,
                PropBone = Bone.SKEL_Head,
                PropOffset = new Vector3(0.1260f, -0.0230f, 0.0000f),
                PropRotation = new Vector3(0, 90, -9),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["box"] = new Animation("anim@heists@box_carry@", "", "idle", "", "Box", new AnimationOptions
            {
                Prop = ObjectHash.hei_prop_heist_box,
                PropBone = Bone.SKEL_Spine3,
                PropOffset = new Vector3(-0.0900f, 0.4500f, 0.0100f),
                PropRotation = new Vector3(-3, 89, -1),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["backpack"] = new Animation("move_p_m_zero_rucksack", "", "idle", "", "Backpack", new AnimationOptions
            {
                Prop = ObjectHash.p_michael_backpack_s,
                PropBone = Bone.SKEL_Spine3,
                PropOffset = new Vector3(0.0700f, -0.1100f, -0.0500f),
                PropRotation = new Vector3(175, 104, -3),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["newspaper"] = new Animation("missmic3", "", "newspaper_idle_loop_dave", "", "Newspaper", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_newspaper,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0590f, 0.1000f, -0.3900f),
                PropRotation = new Vector3(8, -78, -23),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["trash"] = new Animation("anim@heists@narcotics@trash", "", "idle", "", "Trash", new AnimationOptions
            {
                Prop = ObjectHash.hei_prop_heist_binbag,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0200f, -0.0200f, -0.0300f),
                PropRotation = new Vector3(-89, -24, -1),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["rose"] = new Animation("anim@heists@humane_labs@finale@keycards", "", "ped_a_enter_loop", "", "Rose", new AnimationOptions
            {
                Prop = ObjectHash.prop_single_rose,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0700f, 0.0200f, 0.0200f),
                PropRotation = new Vector3(-103, 2, -24),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["umbrella"] = new Animation("amb@world_human_drinking@coffee@male@base", "", "base", "", "Umbrella", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_brolly_01,
                PropBone = Bone.PH_R_Hand,
                PropOffset = new Vector3(0.00f, 0.030f, -0.030f),
                PropRotation = new Vector3(0, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["umbrella2"] = new Animation("rcmnigel1d", "", "base_club_shoulder", "", "Umbrella 2", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_brolly_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0700f, 0.0100f, 0.0200f),
                PropRotation = new Vector3(-107.500f, -5.4f, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["umbrella3"] = new Animation("anim@heists@humane_labs@finale@keycards", "", "ped_a_enter_loop", "", "Umbrella 3", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_brolly_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0300f, -0.0100f, 0.0300f),
                PropRotation = new Vector3(-108f, 8f, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["umbrellaout"] = new Animation("missmic4premiere", "", "interview_short_lazlow", "", "Umbrella Out", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_brolly_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0400f, -0.0200f, -0.0300f),
                PropRotation = new Vector3(-73, -1, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["notepad"] = new Animation("missheistdockssetup1clipboard@base", "", "base", "", "Notepad", new AnimationOptions
            {
                Prop = ObjectHash.prop_notepad_01, // CHANGE ME
                PropBone = Bone.PH_L_Hand,
                PropOffset = new Vector3(-0.080f, 0.020f, -0.020f),
                PropRotation = new Vector3(86, -4, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["textlean"] = new Animation("amb@world_human_leaning@female@wall@back@texting@idle_a", "", "idle_a", "", "Text Lean", new AnimationOptions
            {
                Prop = ObjectHash.prop_amb_phone, // CHANGE ME
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.07f, 0.025f, -0.032f),
                PropRotation = new Vector3(-90, 0, 0),
                LoopDoLoop = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["textlean2"] = new Animation("amb@world_human_leaning@male@wall@back@texting@idle_a", "", "idle_b", "", "Text Lean 2", new AnimationOptions
            {
                Prop = ObjectHash.prop_amb_phone, // CHANGE ME
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.07f, 0.025f, -0.032f),
                PropRotation = new Vector3(-90, 0, 0),
                LoopDoLoop = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["beerlean"] = new Animation("amb@world_human_leaning@male@wall@back@beer@idle_a", "", "idle_b", "", "Beer Lean", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_beer_bot_01, // CHANGE ME
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.07f, 0.025f, -0.032f),
                PropRotation = new Vector3(-90, 0, 0),
                LoopDoLoop = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["clipboard3"] = new Animation("anim@amb@business@bgen@bgen_inspecting@", "", "inspecting_low_confused_inspector", "", "Clipboard 3", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_clipboard_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.12f, 0.000f, 0.11f),
                PropRotation = new Vector3(-30, -99, -141),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["clipboard2"] = new Animation("amb@world_human_clipboard@male@idle_a", "", "idle_c", "", "Clipboard 2", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_clipboard_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.11f, 0.020f, 0.11f),
                PropRotation = new Vector3(-30, -120, -140),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["clipboard"] = new Animation("missfam4", "", "base", "", "Clipboard", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_clipboard_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.11f, 0.020f, 0.11f),
                PropRotation = new Vector3(-30, -120, -140),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["coffee"] = new Animation("amb@world_human_aa_coffee@base", "", "base", "", "Coffee", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_coffeecup_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.07f, 0.025f, -0.032f),
                PropRotation = new Vector3(-90, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["tablet"] = new Animation("amb@world_human_tourist_map@male@base", "", "base", "", "Tablet", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_tablet,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.1200f, 0.0300f, -0.1200f),
                PropRotation = new Vector3(152, 27, 52),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["tablet2"] = new Animation("anim@arena@amb@seat_drone_tablet@male@var_a@", "", "tablet_idle_c", "", "Tablet 2", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_tablet,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.1200f, -0.0100f, -0.1000f),
                PropRotation = new Vector3(143, 22, 18),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["coffee2"] = new Animation("amb@world_human_aa_coffee@idle_a", "", "idle_a", "", "Coffee 2", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_coffeecup_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.07f, 0.025f, -0.032f),
                PropRotation = new Vector3(-90, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["guitar"] = new Animation("amb@world_human_musician@guitar@male@idle_a", "", "idle_b", "", "Guitar", new AnimationOptions
            {
                Prop = ObjectHash.prop_acc_guitar_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(-0.2302f, -0.2409f, 0.0463f),
                PropRotation = new Vector3(-33, 97, 60),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["guitar2"] = new Animation("amb@world_human_musician@guitar@male@idle_a", "", "idle_b", "", "Guitar 2", new AnimationOptions
            {
                Prop = ObjectHash.prop_el_guitar_03,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(-0.2302f, -0.2409f, 0.0463f),
                PropRotation = new Vector3(-33, 97, 60),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["guitar3"] = new Animation("amb@world_human_musician@guitar@male@idle_a", "", "idle_b", "", "Guitar 3", new AnimationOptions
            {
                Prop = ObjectHash.prop_el_guitar_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(-0.2302f, -0.2409f, 0.0463f),
                PropRotation = new Vector3(-33, 97, 60),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["arguephone"] = new Animation("missmic4premiere", "", "prem_producer_argue_a", "", "Argue Phone", new AnimationOptions
            {
                Prop = ObjectHash.prop_npc_phone_02,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0900f, 0.0400f, -0.0000f),
                PropRotation = new Vector3(70, 50, 80),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["call"] = new Animation("anim@cellphone@in_car@ps", "", "cellphone_call_listen_base", "", "Call", new AnimationOptions
            {
                Prop = ObjectHash.prop_npc_phone_02,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0635f, 0.0207f, -0.0207f),
                PropRotation = new Vector3(110, 110, 4.7f),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["call2"] = new Animation("amb@world_human_stand_mobile_fat@male@standing@call@idle_a", "", "idle_b", "", "Call 2", new AnimationOptions
            {
                Prop = ObjectHash.prop_npc_phone_02,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0635f, 0.0207f, -0.0207f),
                PropRotation = new Vector3(110, 110, 4.7f),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["smoke"] = new Animation("amb@world_human_smoking@male@male_b@idle_a", "", "idle_a", "", "Smoke", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_ciggy_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.1220f, 0.0435f, -0.0200f),
                PropRotation = new Vector3(0, 1, 102),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["smoke2"] = new Animation("amb@world_human_smoking@female@idle_a", "", "idle_b", "", "Smoke 2", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_ciggy_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.1140f, 0.0030f, -0.0050f),
                PropRotation = new Vector3(0, 50, -30),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["smokepot"] = new Animation("amb@world_human_smoking_pot@male@base", "", "base", "", "Smoke Pot", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_ciggy_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0800f, 0.0400f, 0.0400f),
                PropRotation = new Vector3(0, 0, 80),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["phone"] = new Animation("amb@world_human_stand_mobile@male@text@idle_a", "", "idle_a", "", "Phone", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_phone_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0900f, 0.0300f, -0.0600f),
                PropRotation = new Vector3(20, 30, -140),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["phone2"] = new Animation("cellphone@", "", "cellphone_text_read_base", "", "Phone 2", new AnimationOptions
            {
                Prop = ObjectHash.p_amb_phone_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0800f, 0.0100f, -0.0200f),
                PropRotation = new Vector3(80, -20, 120),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["wine"] = new Animation("anim@heists@humane_labs@finale@keycards", "", "ped_a_enter_loop", "", "Wine", new AnimationOptions
            {
                Prop = ObjectHash.prop_drink_redwine,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0500f, -0.0400f, 0.0500f),
                PropRotation = new Vector3(-110, 10, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["flute"] = new Animation("anim@heists@humane_labs@finale@keycards", "", "ped_a_enter_loop", "", "Flute", new AnimationOptions
            {
                Prop = ObjectHash.prop_champ_flute,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0500f, -0.0400f, 0.0500f),
                PropRotation = new Vector3(-110, 10, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["champagne"] = new Animation("anim@heists@humane_labs@finale@keycards", "", "ped_a_enter_loop", "", "Champagne", new AnimationOptions
            {
                Prop = ObjectHash.prop_drink_champ,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0376f, -0.0402f, 0.0538f),
                PropRotation = new Vector3(5, -110, -99),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["clean"] = new Animation("timetable@floyd@clean_kitchen@base", "", "base", "", "Clean", new AnimationOptions
            {
                Prop = ObjectHash.prop_sponge_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0800f, 0.0100f, -0.0350f),
                PropRotation = new Vector3(110, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["clean2"] = new Animation("amb@world_human_maid_clean@", "", "base", "", "Clean 2", new AnimationOptions
            {
                Prop = ObjectHash.prop_sponge_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0800f, 0.0100f, -0.0350f),
                PropRotation = new Vector3(110, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["clean3"] = new Animation("amb@world_human_maid_clean@", "", "idle_d", "", "Clean 3", new AnimationOptions
            {
                Prop = ObjectHash.prop_sponge_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0800f, 0.0100f, -0.0350f),
                PropRotation = new Vector3(110, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["clean4"] = new Animation("timetable@patricia@pat_ig_1", "", "base", "", "Clean 4", new AnimationOptions
            {
                Prop = ObjectHash.prop_sponge_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0800f, 0.0100f, -0.0350f),
                PropRotation = new Vector3(110, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["torch"] = new Animation("amb@world_human_security_shine_torch@male@base", "", "base", "", "Torch", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_police_torch_02,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0500f, 0.0100f, 0.0300f),
                PropRotation = new Vector3(24, 0, 105),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["bagel"] = new Animation("mp_player_inteat@burger", "", "mp_player_int_eat_burger", "", "Bagel", new AnimationOptions
            {
                Prop = ObjectHash.p_ing_bagel_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.1040f, 0.0300f, 0.0300f),
                PropRotation = new Vector3(18, 46, 170),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["burger"] = new Animation("mp_player_inteat@burger", "", "mp_player_int_eat_burger", "", "Burger", new AnimationOptions
            {
                Prop = ObjectHash.prop_cs_burger_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0860f, 0.0500f, 0.0300f),
                PropRotation = new Vector3(20, 149, -42),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["sandwich"] = new Animation("mp_player_inteat@burger", "", "mp_player_int_eat_burger", "", "Sandwich", new AnimationOptions
            {
                Prop = ObjectHash.prop_sandwich_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.1000f, 0.0500f, 0.0400f),
                PropRotation = new Vector3(-57, 39, 90),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["taco"] = new Animation("mp_player_inteat@burger", "", "mp_player_int_eat_burger", "", "Taco", new AnimationOptions
            {
                Prop = ObjectHash.prop_taco_01,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0900f, 0.0500f, 0.0300f),
                PropRotation = new Vector3(-12, 70, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["water"] = new Animation("mp_player_inteat@pnq", "", "loop", "", "Water", new AnimationOptions
            {
                Prop = ObjectHash.prop_ld_flow_bottle,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0700f, -0.0100f, 0.0300f),
                PropRotation = new Vector3(-104, 25, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["beer"] = new Animation("mp_player_inteat@pnq", "", "loop", "", "Beer", new AnimationOptions
            {
                Prop = ObjectHash.prop_amb_beer_bottle,
                PropBone = Bone.IK_L_Hand,
                PropOffset = new Vector3(0.0700f, -0.0300f, 0.0300f),
                PropRotation = new Vector3(-95, 24, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["cup"] = new Animation("amb@world_human_drinking@coffee@male@idle_a", "", "idle_c", "", "Cup", new AnimationOptions
            {
                Prop = ObjectHash.prop_plastic_cup_02,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0700f, -0.0400f, -0.0300f),
                PropRotation = new Vector3(-89, 31, 20),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["mugshot"] = new Animation("mp_character_creation@lineup@male_a", "", "loop_raised", "", "Mugshot", new AnimationOptions
            {
                Prop = ObjectHash.prop_police_id_board,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.1500f, 0.0900f, -0.2300f),
                PropRotation = new Vector3(0, 71, 70),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["camera"] = new Animation("amb@world_human_paparazzi@male@base", "", "base", "", "Camera", new AnimationOptions
            {
                Prop = ObjectHash.prop_pap_camera_01,
                PropBone = Bone.IK_R_Hand,
                PropOffset = new Vector3(0.0660f, 0.0020f, -0.0210f),
                PropRotation = new Vector3(-13, 80, 49),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["camera2"] = new Animation("missfbi3_camcrew", "", "base_cman", "", "Camera2", new AnimationOptions
            {
                Prop = ObjectHash.prop_v_cam_01,
                PropBone = Bone.PH_R_Hand,
                PropOffset = new Vector3(0.0660f, 0.0020f, -0.0210f),
                PropRotation = new Vector3(0, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["bmic"] = new Animation("missfbi3_camcrew", "", "base_sman", "", "bmic", new AnimationOptions
            {
                Prop = ObjectHash.prop_v_bmike_01,
                PropBone = Bone.PH_R_Hand,
                PropRotation = new Vector3(-65f, 0f, -120f),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["mic"] = new Animation("amb@world_human_drinking@coffee@male@base", "", "base", "", "mic", new AnimationOptions
            {
                Prop = ObjectHash.prop_microphone_02,
                PropBone = Bone.PH_R_Hand,
                PropRotation = new Vector3(0, 0, 0),
                LoopDoLoop = true,
                LoopEnableMovement = true,
                AttachPropLoop = true,
                OverrideLoopAnimDict = false
            }),
            ["wait14"] = new Animation("re@stag_do@idle_a", "", "idle_c_ped", "", "wait14", new AnimationOptions
            {
                LoopDoLoop = true,
                LoopEnableMovement = true,
                OverrideLoopAnimDict = false
            }),
        };

        internal static List<string> EmoteList = new List<string>();
        private async Task DisableActionsFunc()
        {
            Game.DisableControlThisFrame(1, Control.SelectWeapon);
            Game.DisableControlThisFrame(1, Control.Aim);
            Game.DisableControlThisFrame(1, Control.Attack);
            Game.DisableControlThisFrame(1, Control.Attack2);
            Game.DisableControlThisFrame(1, Control.MeleeAttack1);
            Game.DisableControlThisFrame(1, Control.MeleeAttack2);
            Game.Player.DisableFiringThisFrame();
        }
        public EmoteManager(Client client) : base(client)
        {
            client.RegisterTickHandler(OnTick);
            client.RegisterEventHandler("Emotes.PlayEmote", new Action<string>(async emote =>
            {
                await playerAnimations[emote].PlayFullAnim();
                if (emote.ToLower() == "cpr")
                    await Game.PlayerPed.Task.PlayAnimation("missheistfbi3b_ig8_2", "cpr_loop_paramedic", 1.0f, 1.0f, -1, (AnimationFlags)52, 0.0f);
            }));
            client.RegisterEventHandler("Emotes.ShowAll", new Action(() =>
            {
                string animString = "";
                foreach (string anim in playerAnimations.Keys)
                    animString = animString + anim + ", ";
                Log.ToChat(animString);
            }));
            client.RegisterEventHandler("Emotes.AttemptEmote", new Action<string>(async emote =>
            {
                if (playerAnimations.ContainsKey(emote))
                    await playerAnimations[emote].PlayFullAnim();
                if (emote.ToLower() == "cpr")
                    await Game.PlayerPed.Task.PlayAnimation("missheistfbi3b_ig8_2", "cpr_loop_paramedic", 1.0f, 1.0f, -1, (AnimationFlags)52, 0.0f);
            }));
            CommandRegister.RegisterCommand("emote|e", new Action<Command>(cmd =>
            {
                if (cmd.GetArgAs(0, "") == "help")
                    BaseScript.TriggerEvent("Emotes.ShowAll");
                else
                    BaseScript.TriggerEvent("Emotes.AttemptEmote", cmd.GetArgAs(0, ""));
            }));
            EntityDecoration.RegisterProperty("Player.AnimState", DecorationType.Int);

            HandsupAnimation.OnAnimEnd = () =>
            {
                if (LocalSession.GetGlobalData("Character.HasHandsUp", false))
                {
                    LocalSession.UpdateGlobalData(JsonConvert.SerializeObject(new Dictionary<string, bool> { { "Character.HasHandsUp", false } }));
                    Client.TriggerServerEvent("Session.SetPlayerStatus", "Character.HasHandsUp", false);
                }
            };

            HandsOverHead.OnAnimEnd = () =>
            {
                if (LocalSession.GetGlobalData("Character.HasHandsOverHead", false))
                {
                    LocalSession.UpdateGlobalData(JsonConvert.SerializeObject(new Dictionary<string, bool> { { "Character.HasHandsOverHead", false } }));
                    Client.TriggerServerEvent("Session.SetPlayerStatus", "Character.HasHandsOverHead", false);
                }
            };
        }
        public static void PlayAnimation(string emote)
        {
            if (playerAnimations.ContainsKey(emote))
            {
                playerAnimations[emote].PlayFullAnim();
            }
        }

        private async Task OnTick()
        {
            if (LocalSession == null) return;
            var playerPed = Game.PlayerPed;
            var onDutyAsPolice = Client.Instances.Jobs.OnDutyAsJob(JobType.Police, LocalSession);
            var waitTime = onDutyAsPolice ? 1000 : 1900;
            weaponDictionary = onDutyAsPolice ? "reaction@intimidation@cop@unarmed" : "reaction@intimidation@1h";
            var newWeapon = playerPed.Weapons.Current.Hash;
            if (!playerPed.IsInVehicle() && !IsEntityPlayingAnim(PlayerPedId(), weaponDictionary, weaponAnim, 3))
            {
                if (newWeapon != currentWeapon && Enum.IsDefined(typeof(WeaponHash), newWeapon))
                {
                    Client.RegisterTickHandler(DisableActionsFunc);
                    await BaseScript.Delay(0);
                    var prevWeapon = currentWeapon;
                    currentWeapon = newWeapon;
                    if (nonHolsteredWeapons.FirstOrDefault(o => o == newWeapon) == default(WeaponHash))
                    {
                        if (currentWeapon == WeaponHash.Unarmed)
                        {
                            weaponAnim = "outro";
                            SetCurrentPedWeapon(playerPed.Handle, (uint)prevWeapon, true);
                            await playerPed.Task.PlayAnimation(weaponDictionary, weaponAnim, 2.0f, 2.0f, waitTime, (AnimationFlags)52, 0.0f);
                            await BaseScript.Delay(waitTime - 600);
                            SetCurrentPedWeapon(playerPed.Handle, (uint)currentWeapon, true);
                        }
                        else
                        {
                            weaponAnim = "intro";
                            SetPedCurrentWeaponVisible(playerPed.Handle, false, false, false, false);
                            await playerPed.Task.PlayAnimation(weaponDictionary, weaponAnim, 2.0f, 2.0f, waitTime, (AnimationFlags)52, 0.0f);
                            await BaseScript.Delay(waitTime - 700);
                            SetPedCurrentWeaponVisible(playerPed.Handle, true, false, false, false);
                        }
                        while (IsEntityPlayingAnim(playerPed.Handle, weaponDictionary, weaponAnim, 3))
                            await BaseScript.Delay(0);
                    }
                    Client.DeregisterTickHandler(DisableActionsFunc);
                }
            }

            if (Input.IsControlJustPressed(Control.ReplayTimelinePickupClip))
            {
                if (playerPed.IsDoingAction()) return;

                if (HandsupAnimation.IsPlayingAnim)
                {
                    HandsupAnimation.End(playerPed);
                }
                else
                {
                    HandsupAnimation.PlayFullAnim();
                    Client.TriggerServerEvent("Session.SetPlayerStatus", "Character.HasHandsUp", true);
                }
            }

            if (Input.IsControlJustPressed(Control.ReplayTimelinePickupClip, true, ControlModifier.Shift))
            {
                if (playerPed.IsDoingAction()) return;

                if (HandsOverHead.IsPlayingAnim)
                {
                    HandsOverHead.End(playerPed);
                }
                else
                {
                    HandsOverHead.PlayFullAnim();
                    Client.TriggerServerEvent("Session.SetPlayerStatus", "Character.HasHandsOverHead", true);
                }
            }
        }
    }
}