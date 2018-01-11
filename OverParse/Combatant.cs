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
        public static string[] FinishAttackIDs = new string[] { "2268332858", "170999070", "2268332813", "1266101764", "11556353", "1233721870", "1233722348", "3480338695" };
        public static string[] PhotonAttackIDs = new string[] { "2414748436", "1954812953", "2822784832", "3339644659", "2676260123", "224805109" };
        public static string[] LaconiumAttackIDs = new string[] { "1913897098", "2235773608", "2235773610", "2235773611", "2235773818", "2235773926", "2235773927", "2235773944", "2618804663", "2619614461", "3607718359" };
        public static string[] AISAttackIDs = new string[] { "119505187", "79965782", "79965783", "79965784", "80047171", "434705298", "79964675", "1460054769", "4081218683", "3298256598", "2826401717" };
        public static string[] DBAttackIDs = new string[] { "267911699", "262346668", "265285249", "264996390" , "311089933" , "3988916155" , "265781051" , "3141577094" , "2289473436" , "517914866" , "517914869" , "1117313539" , "1611279117" , "3283361988" , "1117313602" , "395090797" , "2429416220" , "1697271546" , "1117313924" };
        public static string[] RideAttackIDs = new string[] { "3491866260", "2056025809", "2534881408", "2600476838", "1247666429", "3750571080", "3642240295", "651750924", "2452463220", "1732461796", "3809261131", "1876785244", "3765765641", "3642969286", "1258041436" };

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

        //Ally Data
        public string AllyReadPct => AllyPct.ToString("N2");
        public int AllyDamage
        {
            get
            {
                int temp = Damage;
                int temp2 = DBDamage + PwpDamage + AisDamage + RideDamage;
                temp -= temp2;
                if (Properties.Settings.Default.SeparateZanverse)
                    temp -= GetZanverseDamage;
                if (Properties.Settings.Default.SeparateFinish)
                    temp -= GetFinishDamage;
                return temp;
            }
        }
        public string AllyReadDamage => AllyDamage.ToString("N0");
        public string AllyDPS => Math.Round(AllyDamage / (double)ActiveTime).ToString("N0");
        public string AllyJAPct => (Attacks.Where(a => !DBAttackIDs.Contains(a.ID) && !PhotonAttackIDs.Contains(a.ID) && !AISAttackIDs.Contains(a.ID) && !RideAttackIDs.Contains(a.ID)).Average(x => x.JA) * 100).ToString("N2");
        public string AllyCriPct => (Attacks.Where(a => !DBAttackIDs.Contains(a.ID) && !PhotonAttackIDs.Contains(a.ID) && !AISAttackIDs.Contains(a.ID) && !RideAttackIDs.Contains(a.ID)).Average(x => x.Cri) * 100).ToString("N2");
        public string AllyMaxHitdmg => AllyMaxHit.Damage.ToString("N0");
        public string AllyAtkName
        {
            get
            {
                if (AllyMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(AllyMaxHit.ID)) { attack = MainWindow.skillDict[AllyMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack AllyMaxHit
        {
            get
            {
                Attacks.RemoveAll(a => (DBAttackIDs.Contains(a.ID) || PhotonAttackIDs.Contains(a.ID) || AISAttackIDs.Contains(a.ID) || RideAttackIDs.Contains(a.ID)));
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return Attacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        /* Dark Blast (DaB) - Ultimate Dark Falz Player Shenanigans
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

        // public int DBDamage => Attacks.Where(a => DBAttackIDs.Contains(a.ID)).Sum(x => x.Damage);
        // public string DBDPS => Math.Round(DBDamage / (double)ActiveTime).ToString("N0");
        // public string DBJAPct => (Attacks.Where(a => DBAttackIDs.Contains(a.ID)).Average(x => x.JA) * 100).ToString("N2");
        // public string DBCriPct => (Attacks.Where(a => DBAttackIDs.Contains(a.ID)).Average(x => x.Cri) * 100).ToString("N2");
        
        /*public string DBAtkName
        {
            get
            {
                if (DBMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(DBMaxHit.ID)) { attack = MainWindow.skillDict[DBMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }*/

        /*public Attack DBMaxHit
        {
            get
            {
                Attacks.RemoveAll(a => !DBAttackIDs.Contains(a.ID));
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return Attacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }*/

        //Laconium sword Data
        public string LswReadPct => LswPct.ToString("N2");
        public int LswDamage => Attacks.Where(a => LaconiumAttackIDs.Contains(a.ID)).Sum(x => x.Damage);
        public string LswReadDamage => LswDamage.ToString("N0");
        public string LswDPS => Math.Round(LswDamage / (double)ActiveTime).ToString("N0");
        public string LswJAPct => (Attacks.Where(a => LaconiumAttackIDs.Contains(a.ID)).Average(x => x.JA) * 100).ToString("N2");
        public string LswCriPct => (Attacks.Where(a => LaconiumAttackIDs.Contains(a.ID)).Average(x => x.Cri) * 100).ToString("N2");
        public string LswMaxHitdmg => LswMaxHit.Damage.ToString("N0");
        public string LswAtkName
        {
            get
            {
                if (LswMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(LswMaxHit.ID)) { attack = MainWindow.skillDict[LswMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack LswMaxHit
        {
            get
            {
                Attacks.RemoveAll(a => !LaconiumAttackIDs.Contains(a.ID));
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return Attacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        //PhotonWeapon Data Properties
        public string PwpReadPct => PwpPct.ToString("N2");
        public int PwpDamage => Attacks.Where(a => PhotonAttackIDs.Contains(a.ID)).Sum(x => x.Damage);
        public string PwpReadDamage => PwpDamage.ToString("N0");
        public string PwpDPS => Math.Round(PwpDamage / (double)ActiveTime).ToString("N0");
        public string PwpJAPct => (Attacks.Where(a => PhotonAttackIDs.Contains(a.ID)).Average(x => x.JA) * 100).ToString("N2");
        public string PwpCriPct => (Attacks.Where(a => PhotonAttackIDs.Contains(a.ID)).Average(x => x.Cri) * 100).ToString("N2");
        public string PwpMaxHitdmg => PwpMaxHit.Damage.ToString("N0");
        public string PwpAtkName
        {
            get
            {
                if (PwpMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(PwpMaxHit.ID)) { attack = MainWindow.skillDict[PwpMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack PwpMaxHit
        {
            get
            {
                Attacks.RemoveAll(a => !PhotonAttackIDs.Contains(a.ID));
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return Attacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        //AIS Data
        public string AisReadPct => AisPct.ToString("N2");
        public int AisDamage => Attacks.Where(a => AISAttackIDs.Contains(a.ID)).Sum(x => x.Damage);
        public string AisReadDamage => PwpDamage.ToString("N0");
        public string AisDPS => Math.Round(PwpDamage / (double)ActiveTime).ToString("N0");
        public string AisJAPct => (Attacks.Where(a => PhotonAttackIDs.Contains(a.ID)).Average(x => x.JA) * 100).ToString("N2");
        public string AisCriPct => (Attacks.Where(a => PhotonAttackIDs.Contains(a.ID)).Average(x => x.Cri) * 100).ToString("N2");
        public string AisMaxHitdmg => AisMaxHit.Damage.ToString("N0");
        public string AisAtkName
        {
            get
            {
                if (AisMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(AisMaxHit.ID)) { attack = MainWindow.skillDict[AisMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack AisMaxHit
        {
            get
            {
                Attacks.RemoveAll(a => !PhotonAttackIDs.Contains(a.ID));
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return Attacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        //Ridroid Data
        public string RideReadPct => RidePct.ToString("N2");
        public int RideDamage => Attacks.Where(a => RideAttackIDs.Contains(a.ID)).Sum(x => x.Damage);
        public string RideReadDamage => RideDamage.ToString("N0");
        public string RideDPS => Math.Round(RideDamage / (double)ActiveTime).ToString("N0");
        public string RideJAPct => (Attacks.Where(a => RideAttackIDs.Contains(a.ID)).Average(x => x.JA) * 100).ToString("N2");
        public string RideCriPct => (Attacks.Where(a => RideAttackIDs.Contains(a.ID)).Average(x => x.Cri) * 100).ToString("N2");
        public string RideMaxHitdmg => RideMaxHit.Damage.ToString("N0");
        public string RideAtkName
        {
            get
            {
                if (RideMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(RideMaxHit.ID)) { attack = MainWindow.skillDict[RideMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack RideMaxHit
        {
            get
            {
                Attacks.RemoveAll(a => !RideAttackIDs.Contains(a.ID));
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return Attacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }


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
        private IEnumerable<OverParse.Attack> GetAttackID (params string[] attackID) 
        {
            return Attacks.Where(a => attackID.Contains(a.ID));
        }

        // Fetch the Max Damage Hit that the player did
        private IEnumerable<OverParse.Attack> GetMaxHit (params string[] attackID) 
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
        private int GetDamageDealt (IEnumerable<OverParse.Attack> attackID) 
        {
            return attackID.Sum(x => x.Damage);
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
