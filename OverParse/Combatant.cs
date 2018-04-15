using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace OverParse
{
    // Handles the combat data assignments
    public class Combatant
    {
        // Static Variables
        public static float maxShare = 0;
        public static string Log;

        // Hero Finish Attack IDs
        public static string[] FinishAttackIDs   = new string[] { "2268332858"  , // Hero Time Sword slashes
                                                                  "170999070"   , // Hero Time Sword finish
                                                                  "2268332813"  , // Hero Time Sword finish hard hit
                                                                  "1266101764"  , // Hero Time Talis pull-in
                                                                  "11556353"    , // Hero Time Talis slashes
                                                                  "1233721870"  , // Hero Time Talis slashes while switched to Sword
                                                                  "1233722348"  , // Hero Time Talis slashes while switched to TMG
                                                                  "3480338695" }; // Hero Time TMG burst
        // Photon Weaponry Attack IDs
        public static string[] PhotonAttackIDs   = new string[] { "2414748436"  , // Facility Cannon
                                                                  "1954812953"  , // Photon Cannon (Uncharged)
                                                                  "2822784832"  , // Photon Cannon (Charged)
                                                                  "3339644659"  , // Photon Particle Turret
                                                                  "2676260123"  , // Photon Laser Cannon
                                                                  "224805109"  }; // Photon Punisher
        // A.I.S. Weapon Attack IDs
        public static string[] AISAttackIDs      = new string[] { "119505187"   , // A.I.S rifle (Solid Vulcan)
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
        public static string[] RideAttackIDs     = new string[] { "3491866260"  , // Rideroid throw
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
        public static string[] DBAttackIDs       = new string[] { "267911699"   , // Dark Blast (Elder) first hit
                                                                  "262346668"   , // Dark Blast (Elder) second
                                                                  "265285249"   , // Dark Blast (Elder) third
                                                                  "264996390"   , // Dark Blast (Elder) fourth (kick)
                                                                  "311089933"   , // Dark Blast (Elder) fifth (launcher)
                                                                  "3988916155"  , // Dark Blast (Elder) sixth (pummel)
                                                                  "265781051"   , // Dark Blast (Elder) seventh (pummel pt2)
                                                                  "3141577094"  , // Dark Blast (Elder) Step Attack
                                                                  "2289473436"  , // Dark Blast (Elder) Violence Step
                                                                  "517914866"   , // Physical Dash melee
                                                                  "517914869"   , // Physical Dash melee wide range
                                                                  "1117313539"  , // Punishment Knuckle (uncharged)
                                                                  "1611279117"  , // Punishment Knuckle (charged)
                                                                  "3283361988"  , // Ultimate Impact
                                                                  "1117313602"  , // Infinity Rush hits (uncharged)
                                                                  "395090797"   , // Infinity Rush finish (uncharged)
                                                                  "2429416220"  , // Infinity Rush hits charged
                                                                  "1697271546"  , // Infinity Rush finish charged
                                                                  "1117313924"  , // Tyrant Strike
                                                                  "2743071591"  , // Dark Blast (Loser) hit
                                                                  "1783571383"  , // Ortho Sabarta (1-4?)
                                                                  "2928504078"  , // Ortho Sabarta (1-4?)
                                                                  "1783571188"  , // Convergent Ray uncharged
                                                                  "2849190450"  , // Convergent Ray charged
                                                                  "1223455602"  , // Gamma Burst
                                                                  "651603449"   , // Wisdom Force
                                                                  "2970658149"  , // Dive Assault
                                                                  "2191939386"  , // Counter Step
                                                                  "2091027507"  , // Special,Diffusion Ray
                                                                  "4078260742"  , // Sharp Glide
                                                                  "2743062721" }; // Attack Advance (Loser) 
        // Laconium Sword (& Cannons) Attack IDs
        public static string[] LaconiumAttackIDs = new string[] { "1913897098"  , // Rapid-Fire Mana Gun
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
        public static string[] NonAllyAttackIDs = PhotonAttackIDs.Concat(AISAttackIDs).Concat(RideAttackIDs).Concat(DBAttackIDs).Concat(LaconiumAttackIDs).ToArray();

        // General Variables
        private const float maxBGopacity = 0.6f;
        public List<Attack> Attacks, ZvsAttacks, HTFAttacks, DBAttacks, LswAttacks, PwpAttacks, AisAttacks, RideAttacks;
        public string ID, isTemporary;
        public string Name { get; set; }
        public float PercentDPS, PercentReadDPS;
        public int ActiveTime;

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
            Damaged = 0;
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
            Damaged = 0;
        }

        /* Common GET Data Properties */

        public int Damaged;   // Remon's fixes
        public int ZvsDamage  => GetDamageDealt(GetZanverseID());                // Zanverse total damage
        public int HTFDamage  => GetDamageDealt(GetAttackID(FinishAttackIDs));   // Hero Time Finish total damage
        public int PwpDamage  => GetDamageDealt(GetAttackID(PhotonAttackIDs));   // PwP Total Damage
        public int AisDamage  => GetDamageDealt(GetAttackID(AISAttackIDs));      // AIS Total Damage
        public int RideDamage => GetDamageDealt(GetAttackID(RideAttackIDs));     // Ride Total Damage
        public int DBDamage   => GetDamageDealt(GetAttackID(DBAttackIDs));       // DaB Total Damage
        public int LswDamage  => GetDamageDealt(GetAttackID(LaconiumAttackIDs)); // LwS Total Damage

        public int Damage     => GetGeneralDamage();  // General damage dealt
        public int MaxHitNum  => MaxHitAttack.Damage; // Max Hit damage
        public int ReadDamage => GetReadingDamage();  // Filtered damage dealt

        public Attack MaxHitAttack => GetGeneralMaxHitAttack(); // General max hit damage number

        public double DPS     => GetGeneralDPS(); // General DPS
        public double ReadDPS => GetReadingDPS(); // Filtered DPS

        public string DisplayName => GetDisplayName(); // Get player OR anon names

        public string DamageReadout => ReadDamage.ToString("N0"); // Damage dealt stringified
        public string ReadDamaged   => Damaged.ToString("N0");    // Damage taken stringified

        public string StringDPS             => ReadDPS.ToString("N0"); // DPS numbers stringified
        public string PercentReadDPSReadout => GetPercentReadDPS();    // DPS numbers percentified
        public string FDPSReadout           => GetDPSReadout();        // Formated DPS numbers
        public string DPSReadout            => PercentReadDPSReadout;  // Formated DPS (Percent)

        public string MaxHit    => GetGeneralMaxHit();                 // Max hit numbers
        public string MaxHitID  => MaxHitAttack.ID;                    // Max hit attack ID
        public string MaxHitdmg => MaxHitAttack.Damage.ToString("N0"); // Max hit numbers stringified 
        
        public string JAPercent  => GetJAPercent();  // Just Attack % in decimal point of 0|2
        public string WJAPercent => GetWJAPercent(); // Just Attack % in format of 00.00

        public string CRIPercent  => GetCRIPercent();  // Critical Rate % in decimal point of 0|2
        public string WCRIPercent => GetWCRIPercent(); // Critical Rate % in format of 00.00

        public bool IsYou      => CheckIsYou();               // Player-chan running
        public bool IsAlly     => CheckIsAlly();              // Other players running
        public bool IsZanverse => CheckIsType("Zanverse");    // Zanverse being cast
        public bool IsPwp      => CheckIsType("Pwp");         // Photon weapons using
        public bool IsAIS      => CheckIsType("AIS");         // A.I.S. mode running
        public bool IsRide     => CheckIsType("Ride");        // Rideroid mode running
        public bool IsFinish   => CheckIsType("HTF Attacks"); // Hero Time Finish executing
        public bool IsDB       => CheckIsType("DB");          // Dark Blast running
        public bool IsLsw      => CheckIsType("Lsw");         // Laconium Sword or Mana cannon running

        public Brush Brush  => GetBrushPrimary();   // Player-chan damage graph
        public Brush Brush2 => GetBrushSecondary(); // Other players damage graph

        /* HELPER FUNCTIONS */

        // Censors other players' name except the user
        private string AnonymousName()
        {
            if (IsYou)
                return Name;
            else
                return "----";
        }

        // Draw method for generating the damage graph
        private LinearGradientBrush GenerateBarBrush(Color c, Color c2)
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

        // Fetch the technique "Zanverse" ID
        private IEnumerable<OverParse.Attack> GetZanverseID() 
        {
            return Attacks.Where(a => a.ID == "2106601422"); // Zanverse
        }

        // Fetch the attack ID
        private IEnumerable<OverParse.Attack> GetAttackID(string[] attackID) 
        {
            return Attacks.Where(a => attackID.Contains(a.ID));
        }
        
        // Fetch the total Damage Dealt value [ Use after (GetAttackID) function ]
        private int GetDamageDealt(IEnumerable<OverParse.Attack> attackID) 
        {
            return attackID.Sum(x => x.Damage);
        }

        // Returns the general damage dealt
        private int GetGeneralDamage() 
        { 
            return Attacks.Sum(x => x.Damage); 
        }

        // Returns the damage dealt that has been filtered
        private int GetReadingDamage()
        {
            if (IsZanverse || IsFinish || IsAIS || IsPwp || IsDB || IsRide || IsLsw)
                return Damage;

            int temp = Damage;
            if (Properties.Settings.Default.SeparateZanverse)
                temp -= ZvsDamage;
            if (Properties.Settings.Default.SeparateFinish)
                temp -= HTFDamage;
            if (Properties.Settings.Default.SeparatePwp)
                temp -= PwpDamage;
            if (Properties.Settings.Default.SeparateAIS)
                temp -= AisDamage;
            if (Properties.Settings.Default.SeparateDB)
                temp -= DBDamage;
            if (Properties.Settings.Default.SeparateRide)
                temp -= RideDamage;
            if (Properties.Settings.Default.SeparateLsw)
                temp -= LswDamage;
            return temp;
        }

        // Returns the max damage hit done
        private Attack GetGeneralMaxHitAttack()
        {
            Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
            return Attacks.FirstOrDefault();
        }

        // Returns the general DPS
        private double GetGeneralDPS() 
        { 
            if (ActiveTime == 0)
            {
                return Damage;
            }
            else
            {
                return Damage / ActiveTime;
            }
        }

        // Returns the DPS that has been filtered
        private double GetReadingDPS() 
        { 
            if (ActiveTime == 0)
            {
                return ReadDamage;
            }
            else
            {
                return Math.Round(ReadDamage / (double)ActiveTime); 
            }
        }
        
        // Returns the display naming choices (Name or Anon)
        private string GetDisplayName()
        {
            if (Properties.Settings.Default.AnonymizeNames && IsAlly) { return AnonymousName(); }
            return Name;
        }


        // Percentifies the DPS numbers
        private string GetPercentReadDPS()
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

        // Stringifies the DPS numbers
        private string GetDPSReadout()
        {
            if (Properties.Settings.Default.DPSformat)
            {
                return FormatNumber(ReadDPS);
            } 
            else 
            {
                return StringDPS;
            }
        }

        // Returns the attack name that achieved max damage hit
        private string GetGeneralMaxHit()
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

        // Returns the Just Attack Percentage
        private string GetJAPercent()
        {
            try
            {
                if (Properties.Settings.Default.Nodecimal)
                {
                    return ((Attacks.Where(a => !MainWindow.ignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N0");
                } 
                else 
                {
                    return ((Attacks.Where(a => !MainWindow.ignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N2");
                }
            }
            catch { return "Error"; }
        }

        // Returns the Just Attack Percentage in "00.00"
        private string GetWJAPercent() 
        { 
            return ((Attacks.Where(a => !MainWindow.ignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("00.00"); 
        }

        // Returns the Critical Rate Percentange
        private string GetCRIPercent()
        {
            try
            {
                if (Properties.Settings.Default.Nodecimal)
                {
                    return ((Attacks.Average(a => a.Cri)) * 100).ToString("N0");
                } 
                else 
                {
                    return ((Attacks.Average(a => a.Cri)) * 100).ToString("N2");
                }
            }
            catch { return "Error"; }
        }

        // Returns the Critical Rate Percentage in "00.00"
        private string GetWCRIPercent() 
        { 
            return ((Attacks.Average(a => a.Cri)) * 100).ToString("00.00"); 
        }

        // Checks if this is the user
        private bool CheckIsYou() 
        { 
            return (ID == Hacks.currentPlayerID); 
        }

        // Checks if this is a player
        private bool CheckIsAlly()
        {
            if (int.Parse(ID) >= 10000000 && !IsZanverse && !IsFinish)
            {
                return true;
            } 
            else 
            {
                return false;
            }
        }

        // Checks if this is using other modes of attack
        private bool CheckIsType(string currType) 
        { 
            return (isTemporary == currType); 
        }

        // Generates the damage bar graph for the user
        private Brush GetBrushPrimary()
        {
            if (Properties.Settings.Default.ShowDamageGraph && (IsAlly))
            {
                return GenerateBarBrush(Color.FromArgb(128, 0, 128, 128), Color.FromArgb(128, 30, 30, 30));
            } 
            else 
            {
                if (IsYou && Properties.Settings.Default.HighlightYourDamage)
                {
                    return new SolidColorBrush(Color.FromArgb(128, 0, 255, 255));
                }

                return new SolidColorBrush(Color.FromArgb(127, 30, 30, 30));
            }
        }

        // Generates the damage bar graph for other players
        private Brush GetBrushSecondary()
        {
            if (Properties.Settings.Default.ShowDamageGraph && (IsAlly && !IsZanverse))
            {
                return GenerateBarBrush(Color.FromArgb(128, 0, 64, 64), Color.FromArgb(0, 0, 0, 0));
            } 
            else 
            {
                if (IsYou && Properties.Settings.Default.HighlightYourDamage) 
                {
                    return new SolidColorBrush(Color.FromArgb(128, 0, 64,64));
                }

                return new SolidColorBrush(new Color());
            }
        }
        

        /* NOT USED - for now (Will think of a way to add the tabs back in more efficient method)

        // Fetch the Max Damage Hit that the player did
        private Attack GetMaxHit(string[] attackID) 
        {
            Attacks.RemoveAll(a => !attackID.Contains(a.ID));

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
        private string GetAttackName(Attack maxHit) 
        {
            if (maxHit == null) { return "--"; }
            string attack = "Unknown";

            if (MainWindow.skillDict.ContainsKey(maxHit.ID)) { attack = MainWindow.skillDict[maxHit.ID]; }
            return MaxHitAttack.Damage.ToString(attack);
        }

        // Calclate the actual Damage Per Second / DPS
        private string CalculateDPS(int damageDealt) 
        {
            return Math.Round(damageDealt / (double)ActiveTime).ToString("N0");
        }

        // Fetch the Just Attack value [ Use after (GetAttackID) function ]
        private string GetJAValue(IEnumerable<OverParse.Attack> attackID) 
        {
            return (attackID.Average(x => x.JA) * 100).ToString("N2");
        }

        // Fetch the Critical Attack value [ Use after (GetAttackID) function ]
        private string GetCritValue(IEnumerable<OverParse.Attack> attackID) 
        {
            return (attackID.Average(x => x.Cri) * 100).ToString("N2");    
        }

        */
    }

    // Tyrone-sama's Hacks! - kyaaa (Used for finding current player ID)
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
        public int Damage, JA, Cri, Timestamp;

        public Attack(string initID, int initDamage, int justAttack, int critical, int initTimestamp)
        {
            ID = initID;
            Damage = initDamage;
            JA = justAttack;
            Cri = critical;
            Timestamp = initTimestamp;
        }
    }
}
