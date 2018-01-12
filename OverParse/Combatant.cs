using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace OverParse
{
    // Handles the combat data assignments
    public class Combatant
    {
        // General Variables
        private const float maxBGopacity = 0.6f;
        public List<Attack> Attacks;
        public string ID, isTemporary;
        public string Name { get; set; }
        public int ActiveTime;
        public float PercentDPS, PercentReadDPS, AllyPct, DBPct, LswPct, PwpPct, AisPct, RidePct;        

        // Static Variables
        public static float maxShare = 0;
        public static string Log;

        // Hero Finish Attack IDs
        public static string[] FinishAttackIDs      = new string[] { "2268332858"  , // Hero Time Sword slashes
                                                                     "170999070"   , // Hero Time Sword finish
                                                                     "2268332813"  , // Hero Time Sword finish hard hit
                                                                     "1266101764"  , // Hero Time Talis pull-in
                                                                     "11556353"    , // Hero Time Talis slashes
                                                                     "1233721870"  , // Hero Time Talis slashes while switched to Sword
                                                                     "1233722348"  , // Hero Time Talis slashes while switched to TMG
                                                                     "3480338695" }; // Hero Time TMG burst
        // Photon Weaponry Attack IDs
        public static string[] PhotonAttackIDs      = new string[] { "2414748436"  , // Facility Cannon
                                                                     "1954812953"  , // Photon Cannon (Uncharged)
                                                                     "2822784832"  , // Photon Cannon (Charged)
                                                                     "3339644659"  , // Photon Particle Turret
                                                                     "2676260123"  , // Photon Laser Cannon
                                                                     "224805109"  }; // Photon Punisher
        // A.I.S. Weapon Attack IDs
        public static string[] AISAttackIDs         = new string[] { "119505187"   , // A.I.S rifle (Solid Vulcan)
                                                                     "79965782"    , // A.I.S melee first attack (Photon Saber)
                                                                     "79965783"    , // A.I.S melee second attack (Photon Saber)
                                                                     "79965784"    , // A.I.S melee third attack (Photon Saber)
                                                                     "80047171"    , // A.I.S dash melee (Photon Saber)
                                                                     "434705298"   , // A.I.S rockets (Photon Grenade)
                                                                     "79964675"    , // A.I.S gap closer PA attack (Photon Rush)
                                                                     "1460054769"  , // A.I.S cannon (Photon Blaster)
                                                                     "4081218683"  , // A.I.S mob freezing attack (Photon Blizzard)
                                                                     "3298256598"  , // A.I.S Weak Bullet
                                                                     "2826401717" }; // A.I.S Area Heal
        // Rideroid Weapon Attack IDs
        public static string[] RideAttackIDs        = new string[] { "3491866260"  , // Rideroid throw
                                                                     "2056025809"  , // Rideroid hit forward slow
                                                                     "2534881408"  , // Rideroid hit forward stop
                                                                     "2600476838"  , // Rideroid hit dodge
                                                                     "1247666429"  , // Rideroid hit forward fast
                                                                     "3750571080"  , // Big UFO outer control unit hit?
                                                                     "3642240295"  , // Big UFO Core hit?
                                                                     "651750924"   , // Big UFO hit?
                                                                     "2452463220"  , // Something relating to big ufo and rideroid
                                                                     "1732461796"  , // Something relating to big ufo and rideroid
                                                                     "3809261131"  , // Something relating to big ufo and rideroid
                                                                     "1876785244"  , // Rideroid auto-attack (Mother phase 1)
                                                                     "3765765641"  , // Rideroid rockets (Mother phase 1)
                                                                     "3642969286"  , // Rideroid barrel roll (Mother phase 1)
                                                                     "1258041436" }; // Rideroid Mother's wall spun back (Mother phase 1)
        // Dark Blast Attack IDs
        public static string[] DBAttackIDs          = new string[] { "267911699"   , // Dark Blast first hit
                                                                     "262346668"   , // Dark Blast second
                                                                     "265285249"   , // Dark Blast third
                                                                     "264996390"   , // Dark Blast fourth (kick)
                                                                     "311089933"   , // Dark Blast fifth (launcher)
                                                                     "3988916155"  , // Dark Blast sixth (pummel)
                                                                     "265781051"   , // Dark Blast seventh (pummel pt2)
                                                                     "3141577094"  , // Dark Blast Step Attack
                                                                     "2289473436"  , // Dark Blast Violence Step
                                                                     "517914866"   , // Physical Dash melee
                                                                     "517914869"   , // Physical Dash melee wide range
                                                                     "1117313539"  , // Punishment Knuckle (uncharged)
                                                                     "1611279117"  , // Punishment Knuckle (charged)
                                                                     "3283361988"  , // Ultimate Impact
                                                                     "1117313602"  , // Infinity Rush hits (uncharged)
                                                                     "395090797"   , // Infinity Rush finish (uncharged)
                                                                     "2429416220"  , // Infinity Rush hits charged
                                                                     "1697271546"  , // Infinity Rush finish charged
                                                                     "1117313924" }; // Tyrant Strike
        // Laconium Sword (& Cannons) Attack IDs
        public static string[] LaconiumAttackIDs    = new string[] { "1913897098"  , // Rapid-Fire Mana Gun
                                                                     "2235773608"  , // Laconium Sword air second normal attack 
                                                                     "2235773610"  , // Laconium Sword air first normal attack 
                                                                     "2235773611"  , // Laconium Sword air third normal attack
                                                                     "2235773818"  , // Buster Divide (Laconium Sword uncharged)
                                                                     "2235773926"  , // Laconium Sword second normal attack
                                                                     "2235773927"  , // Laconium Sword first normal attack
                                                                     "2235773944"  , // Laconium Sword third normal attack
                                                                     "2618804663"  , // Buster Divide (Laconium Sword charged)
                                                                     "2619614461"  , // Laconium Sword Step Attack
                                                                     "3607718359" }; // Laconium Sword slash
        // List of the above attack IDs combined
        public static string[] NonAllyAttackIDs     = PhotonAttackIDs.Concat(AISAttackIDs).Concat(RideAttackIDs).Concat(DBAttackIDs).Concat(LaconiumAttackIDs).ToArray();

        public int Damage => Attacks.Sum(x => x.Damage);

        public int Damaged => Attacks.Sum(x => x.Dmgd);

        public string ReadDamaged => Attacks.Sum(x => x.Dmgd).ToString("N0");

        public int GetZanverseDamage => Attacks.Where(a => a.ID == "2106601422").Sum(x => x.Damage);

        public int GetFinishDamage => Attacks.Where(a => FinishAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public string WJAPercent => ((Attacks.Average(a => a.JA)) * 100).ToString("00.00");

        public string WCRIPercent => ((Attacks.Average(a => a.Cri)) * 100).ToString("00.00");

        public double DPS => Damage / ActiveTime;

        public double ReadDPS => Math.Round(ReadDamage / (double)ActiveTime);

        public string StringDPS => ReadDPS.ToString("N0");

        public bool IsAIS => (isTemporary == "AIS");

        public bool IsRide => (isTemporary == "Ride");

        public bool IsZanverse => (isTemporary == "Zanverse");

        public bool IsPwp => (isTemporary == "Pwp");

        public bool IsFinish => (isTemporary == "HTF Attacks");

        public bool IsDB => (isTemporary == "DB");

        public string MaxHitdmg => MaxHitAttack.Damage.ToString("N0");

        public bool IsYou => (ID == Hacks.currentPlayerID);

        public int MaxHitNum => MaxHitAttack.Damage;

        public string MaxHitID => MaxHitAttack.ID;

        public string DPSReadout => PercentReadDPSReadout;

        /* Player-chan - A.R.K.S. Guardian / Protaganist / Matoi's Love Interest 
         * GET Data Properties
         */

        public int      AllyDamage       => GetDamageDealt(GetAttackID(NonAllyAttackIDs, true)); // Ally Total Damage
        public Attack   AllyMaxHit       => GetMaxHit(NonAllyAttackIDs, true);                   // Ally Max Hit Damage
        public string   AllyAtkName      => GetAttackName(AllyMaxHit);                           // Ally Max Hit Attack Name
        public string   AllyDPS          => CalculateDPS(AllyDamage);                            // Ally DPS
        public string   AllyJAPct        => GetJAValue(GetAttackID(NonAllyAttackIDs, true));     // Ally JA Percentage (%)
        public string   AllyCriPct       => GetCritValue(GetAttackID(NonAllyAttackIDs, true));   // Ally Critical Percentage (%)

        public string   AllyReadPct      => AllyPct.ToString("N2");            // Read Ally on MPA contribution (%)
        public string   AllyReadDamage   => AllyDamage.ToString("N0");         // Read Ally on damage dealt
        public string   AllyMaxHitdmg    => AllyMaxHit.Damage.ToString("N0");  // Read Ally on Max Hit for damage

        /* Photon Weaponry (PwP) - A.R.K.S. Supplied Tools of "Massive Destruction" (Photon Punisher etc.)
         * GET Data Properties
         */

        public int      PwpDamage       => GetDamageDealt(GetAttackID(PhotonAttackIDs)); // PwP Total Damage
        public Attack   PwpMaxHit       => GetMaxHit(PhotonAttackIDs);                   // PwP Max Hit Damage
        public string   PwpAtkName      => GetAttackName(PwpMaxHit);                     // PwP Max Hit Attack Name
        public string   PwpDPS          => CalculateDPS(PwpDamage);                      // PwP DPS
        public string   PwpJAPct        => GetJAValue(GetAttackID(PhotonAttackIDs));     // PwP JA Percentage (%)
        public string   PwpCriPct       => GetCritValue(GetAttackID(PhotonAttackIDs));   // PwP Critical Percentage (%)

        public string   PwpReadPct      => PwpPct.ToString("N2");            // Read PwP on MPA contribution (%)
        public string   PwpReadDamage   => PwpDamage.ToString("N0");         // Read PwP on damage dealt
        public string   PwpMaxHitdmg    => PwpMaxHit.Damage.ToString("N0");  // Read PwP on Max Hit for damage

        /* A.R.K.S. Interception Silhouette (AIS) - WE ARE BORDERLESS (Yamato boat fights, Magatsu bonus etc)
         * GET Data Properties
         */

        public int      AisDamage       => GetDamageDealt(GetAttackID(AISAttackIDs)); // AIS Total Damage
        public Attack   AisMaxHit       => GetMaxHit(AISAttackIDs);                   // AIS Max Hit Damage
        public string   AisAtkName      => GetAttackName(AisMaxHit);                  // AIS Max Hit Attack Name
        public string   AisDPS          => CalculateDPS(AisDamage);                   // AIS DPS
        public string   AisJAPct        => GetJAValue(GetAttackID(AISAttackIDs));     // AIS JA Percentage (%)
        public string   AisCriPct       => GetCritValue(GetAttackID(AISAttackIDs));   // AIS Critical Percentage (%)

        public string   AisReadPct      => AisPct.ToString("N2");            // Read AIS on MPA contribution (%)
        public string   AisReadDamage   => AisDamage.ToString("N0");         // Read AIS on damage dealt
        public string   AisMaxHitdmg    => AisMaxHit.Damage.ToString("N0");  // Read AIS on Max Hit for damage

        /* Rideroid (Ride) - A.R.K.S. version of Sonic the Hedgehod & Up-skirt reasons (Mother EQ etc.)
         * GET Data Properties
         */

        public int      RideDamage      => GetDamageDealt(GetAttackID(RideAttackIDs)); // Ride Total Damage
        public Attack   RideMaxHit      => GetMaxHit(RideAttackIDs);                   // Ride Max Hit Damage
        public string   RideAtkName     => GetAttackName(RideMaxHit);                  // Ride Max Hit Attack Name
        public string   RideDPS         => CalculateDPS(RideDamage);                   // Ride DPS
        public string   RideJAPct       => GetJAValue(GetAttackID(RideAttackIDs));     // Ride JA Percentage (%)
        public string   RideCriPct      => GetCritValue(GetAttackID(RideAttackIDs));   // Ride Critical Percentage (%)

        public string   RideReadPct     => RidePct.ToString("N2");            // Read Ride on MPA contribution (%)
        public string   RideReadDamage  => RideDamage.ToString("N0");         // Read Ride on damage dealt
        public string   RideMaxHitdmg   => RideMaxHit.Damage.ToString("N0");  // Read Ride on Max Hit for damage

        /* Dark Blast (DaB) - Ultimate Dark Falz Player Shenanigans (Ultimate IMPACT etc.)
         * GET Data Properties
         */ 

        public int      DBDamage        => GetDamageDealt(GetAttackID(DBAttackIDs));    // DaB Total Damage
        public Attack   DBMaxHit        => GetMaxHit(DBAttackIDs);                      // DaB Max Hit Damage
        public string   DBAtkName       => GetAttackName(DBMaxHit);                     // DaB Max Hit Attack Name
        public string   DBDPS           => CalculateDPS(DBDamage);                      // DaB DPS
        public string   DBJAPct         => GetJAValue(GetAttackID(DBAttackIDs));        // DaB JA Percentage (%)
        public string   DBCriPct        => GetCritValue(GetAttackID(DBAttackIDs));      // DaB Critical Percentage (%)

        public string   DBReadPct       => DBPct.ToString("N2");            // Read DaB on MPA contribution (%)
        public string   DBReadDamage    => DBDamage.ToString("N0");         // Read DaB on damage dealt
        public string   DBMaxHitdmg     => DBMaxHit.Damage.ToString("N0");  // Read DaB on Max Hit for damage

        /* Laconium Sword | Mana Cannons (LwS) - Feminized Gilgamesh VS Giant Winged Fire-breathing Lizard
         * GET Data Properties
         */ 
        
        public int      LswDamage       => GetDamageDealt(GetAttackID(LaconiumAttackIDs)); // LwS Total Damage
        public Attack   LswMaxHit       => GetMaxHit(LaconiumAttackIDs);                   // LwS Max Hit Damage
        public string   LswAtkName      => GetAttackName(LswMaxHit);                       // LwS Max Hit Attack Name
        public string   LswDPS          => CalculateDPS(LswDamage);                        // LwS DPS
        public string   LswJAPct        => GetJAValue(GetAttackID(LaconiumAttackIDs));     // LwS JA Percentage (%)
        public string   LswCriPct       => GetCritValue(GetAttackID(LaconiumAttackIDs));   // LwS Critical Percentage (%)

        public string   LswReadPct      => LswPct.ToString("N2");            // Read LwS on MPA contribution (%)
        public string   LswReadDamage   => LswDamage.ToString("N0");         // Read LwS on damage dealt
        public string   LswMaxHitdmg    => LswMaxHit.Damage.ToString("N0");  // Read LwS on Max Hit for damage

        public string FDPSReadout
        {
            get
            {
                if (Properties.Settings.Default.DPSformat)
                {
                    return FormatNumber(ReadDPS);
                } else {
                    return StringDPS;
                }
            }
        }

        public string JAPercent
        {
            get {
                try
                {
                    if (Properties.Settings.Default.Nodecimal)
                    {
                        return ((Attacks.Where(a => !MainWindow.ignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N0");
                    } else {
                        return ((Attacks.Where(a => !MainWindow.ignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N2");
                    }
                }
                catch { return "Error"; }
            }
        }

        public string CRIPercent
        {
            get {
                try
                {
                    if (Properties.Settings.Default.Nodecimal)
                    {
                        return ((Attacks.Average(a => a.Cri)) * 100).ToString("N0");
                    } else {
                        return ((Attacks.Average(a => a.Cri)) * 100).ToString("N2");
                    }
                }
                catch { return "Error"; }
            }
        }

        public string DamageReadout => ReadDamage.ToString("N0");

        public int ReadDamage
        {
            get
            {
                if (IsZanverse || IsFinish || IsAIS || IsPwp || IsDB || IsRide)
                    return Damage;

                int temp = Damage;
                if (Properties.Settings.Default.SeparateZanverse)
                    temp -= GetZanverseDamage;
                if (Properties.Settings.Default.SeparateFinish)
                    temp -= GetFinishDamage;
                if (Properties.Settings.Default.SeparatePwp)
                    temp -= PwpDamage;
                if (Properties.Settings.Default.SeparateAIS)
                    temp -= AisDamage;
                if (Properties.Settings.Default.SeparateDB)
                    temp -= DBDamage;
                if (Properties.Settings.Default.SeparateRide)
                    temp -= RideDamage;
                return temp;
            }
        }

        public string DisplayName
        {
            get
            {
                if (Properties.Settings.Default.AnonymizeNames && IsAlly) { return AnonymousName(); }
                return Name;
            }
        }

        public Brush Brush
        {
            get
            {
                if (Properties.Settings.Default.ShowDamageGraph && (IsAlly))
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 128, 128), Color.FromArgb(128, 30, 30, 30));
                } else {
                    if (IsYou && Properties.Settings.Default.HighlightYourDamage)
                        return new SolidColorBrush(Color.FromArgb(128, 0, 255, 255));
                    return new SolidColorBrush(Color.FromArgb(127, 30, 30, 30));
                }
            }
        }

        public Brush Brush2
        {
            get
            {
                if (Properties.Settings.Default.ShowDamageGraph && (IsAlly && !IsZanverse))
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 64, 64), Color.FromArgb(0, 0, 0, 0));
                } else {
                    if (IsYou && Properties.Settings.Default.HighlightYourDamage)
                        return new SolidColorBrush(Color.FromArgb(128, 0, 64,64));
                    return new SolidColorBrush(new Color());
                }
            }
        }

        public bool IsAlly
        {
            get
            {
                if (int.Parse(ID) >= 10000000 && !IsZanverse && !IsFinish)
                    return true;
                return false;
            }
        }

        public Attack MaxHitAttack
        {
            get
            {
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                return Attacks.FirstOrDefault();
            }
        }

        public string MaxHit
        {
            get
            {
                if (MaxHitAttack == null)
                    return "--";
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(MaxHitID))
                {
                    attack = MainWindow.skillDict[MaxHitID];
                }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }

        public string PercentReadDPSReadout
        {
            get
            {
                if (PercentReadDPS < -.5)
                {
                    return "--";
                }
                else
                {
                    return $"{PercentReadDPS:0.00}";
                }
            }
        }

        /* CLASS FUNCTIONS */ 

        // Censors other players' name except the user
        public string AnonymousName()
        {
            if (IsYou)
                return Name;
            else
                return "----";
        }

        // Draw method for generating the damage graph
        LinearGradientBrush GenerateBarBrush(Color c, Color c2)
        {
            if (!Properties.Settings.Default.ShowDamageGraph)
                c = new Color();

            if (IsYou && Properties.Settings.Default.HighlightYourDamage)
                c = Color.FromArgb(128, 0, 255, 255);

            LinearGradientBrush lgb = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(1, 0)
            };
            lgb.GradientStops.Add(new GradientStop(c, 0));
            lgb.GradientStops.Add(new GradientStop(c, ReadDamage / maxShare));
            lgb.GradientStops.Add(new GradientStop(c2, ReadDamage / maxShare));
            lgb.GradientStops.Add(new GradientStop(c2, 1));
            lgb.SpreadMethod = GradientSpreadMethod.Repeat;
            return lgb;
        }

        /* HELPER FUNCTIONS */

        // Formating numbers to either K (1,000) or M (1,000,000)
        private String FormatNumber(double value)
        {
            int num = (int)Math.Round(value);

            if (value >= 100000000)
                return (value / 1000000).ToString("#,0") + "M";
            if (value >= 1000000)
                return (value / 1000000D).ToString("0.0") + "M";
            if (value >= 100000)
                return (value / 1000).ToString("#,0") + "K";
            if (value >= 1000)
                return (value / 1000D).ToString("0.0") + "K";
            return value.ToString("#,0");
        }

        // Fetch the attack ID
        private IEnumerable<OverParse.Attack> GetAttackID (string[] attackID, bool ally = false) 
        {
            if (ally) 
            {
                return Attacks.Where(a => !attackID.Contains(a.ID));
            } 
            else 
            {
                return Attacks.Where(a => attackID.Contains(a.ID));
            }
        }

        // Fetch the Max Damage Hit that the player did
        private Attack GetMaxHit (string[] attackID, bool ally = false) 
        {
            if (ally) 
            {
                Attacks.RemoveAll(a => attackID.Contains(a.ID));
            } 
            else 
            {
                Attacks.RemoveAll(a => !attackID.Contains(a.ID));
            }

            Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));

            if (Attacks != null)
            {
                return Attacks.FirstOrDefault();
            } 
            else 
            {
                return null;
            }
        }

        // Fetch the Attack Name that achieved Max Damage Hit
        private string GetAttackName (Attack maxHit) 
        {
            if (maxHit == null) { return "--"; }
            string attack = "Unknown";

            if (MainWindow.skillDict.ContainsKey(maxHit.ID)) { attack = MainWindow.skillDict[maxHit.ID]; }
            return MaxHitAttack.Damage.ToString(attack);
        }

        // Calclate the actual Damage Per Second / DPS
        private string CalculateDPS (int damageDealt) 
        {
            return Math.Round(damageDealt / (double)ActiveTime).ToString("N0");
        }

        // Fetch the total Damage Dealt value [ Use after (GetAttackID) function ]
        private int GetDamageDealt (IEnumerable<OverParse.Attack> attackID, bool ally = false) 
        {
            if (ally) 
            {
                int player = Damage;
                int notplayer = attackID.Sum(x => x.Damage);

                player -= notplayer;

                if (Properties.Settings.Default.SeparateZanverse) { player -= GetZanverseDamage; }
                if (Properties.Settings.Default.SeparateFinish) { player -= GetFinishDamage; }

                return player;
            } 
            else 
            {
                return attackID.Sum(x => x.Damage);
            }
        }

        // Fetch the Just Attack value [ Use after (GetAttackID) function ]
        private string GetJAValue (IEnumerable<OverParse.Attack> attackID, bool nodecimal = false) 
        {
            if (nodecimal) 
            {
                return (attackID.Average(x => x.JA) * 100).ToString("N0");
            }
            else 
            {
                return (attackID.Average(x => x.JA) * 100).ToString("N2");
            }
        }

        // Fetch the Critical Attack value [ Use after (GetAttackID) function ]
        private string GetCritValue (IEnumerable<OverParse.Attack> attackID, bool nodecimal = false) 
        {
            if (nodecimal) 
            {
                return (attackID.Average(x => x.Cri) * 100).ToString("N0");    
            }
            else 
            {
                return (attackID.Average(x => x.Cri) * 100).ToString("N2");    
            }
        }

        // Constructor #1
        public Combatant(string id, string name)
        {
            ID = id;
            Name = name;
            PercentDPS = -1;
            Attacks = new List<Attack>();
            isTemporary = "no";
            PercentReadDPS = 0;
            ActiveTime = 0;
        }

        // Constructor #2
        public Combatant(string id, string name, string temp)
        {
            ID = id;
            Name = name;
            PercentDPS = -1;
            Attacks = new List<Attack>();
            isTemporary = temp;
            PercentReadDPS = 0;
            ActiveTime = 0;
        }
    }

    // Tyrone-sama's Hacks! - kyaaa
    static class Hacks
    {
        public static string currentPlayerID;
        public static bool DontAsk = false;
        public static string targetID = "";
    }

    // Attack class properties
    public class Attack
    {
        public string ID;
        public int Damage;
        public int Timestamp;
        public int JA;
        public int Cri;
        public int Dmgd;

        public Attack(string initID, int initDamage, int initTimestamp, int justAttack, int critical, int damaged)
        {
            ID = initID;
            Damage = initDamage;
            Timestamp = initTimestamp;
            JA = justAttack;
            Cri = critical;
            Dmgd = damaged;
        }
    }
}
