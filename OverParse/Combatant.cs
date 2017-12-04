using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace OverParse
{
    public class Combatant
    {
        private const float maxBGopacity = 0.6f;
        public string ID;
        public string Name { get; set; }
        public float PercentDPS;
        public float PercentReadDPS;
        public int ActiveTime;
        public string isTemporary;
        public static string Log;
        public List<Attack> Attacks;
        public static string[] FinishAttackIDs = new string[] { "2268332858", "170999070", "2268332813", "1266101764", "11556353", "1233721870", "1233722348", "3480338695" };
        public static string[] PhotonAttackIDs = new string[] { "2414748436", "1954812953", "2822784832", "3339644659", "2676260123", "224805109" };
        public static string[] MagAttackIDs = new string[] { "2667138551", "3296842070", "546125641", "1810252617", "1509329819", "887429903", "1747262548", "3303318723", "1811866223", "183890233", "3146527639", "2042405421", "195905495", "2373460025", "196570118", "867373469", "1542931456", "3126711672", "1687392375", "649064981", "2364012448", "1049123664", "3760306129", "4075390095", "912617849", "1886571787", "1059126347", "3104220402", "4205203418" };
        public static string[] PBAttackIDs = new string[] { "2004640116", "1792224906", "1075221526", "1817839236", "3826344072", "4211237222", "3903398021", "823402941", "3627456031", "2415309454", "718460570", "498592293", "4274907792", "2379589576", "3512301214", "1846509854", "3625928312", "2811060000", "4277784487", "430308113", "413582417", "4017106933", "380422613", "1161221232", "320287461", "3929437971", "2074769727", "2074769724", "2074769725", "2074769722", "2224628886", "2224628885", "2224628884", "2224628883", "3412523140", "3412523139", "3412523138", "3412523137", "764702094", "764702093", "764702092", "764702091", "2814861617", "2814861618", "2814861619", "2814861772", "2207342095", "2207342092", "2207342093", "2207342090", "4037912096", "4037912097", "4037912102", "4037912103" };
        public static string[] AISAttackIDs = new string[] { "119505187", "79965782", "79965783", "79965784", "80047171", "434705298", "79964675", "1460054769", "4081218683", "3298256598", "2826401717" };
        public static string[] DBAttackIDs = new string[] { "267911699", "262346668", "265285249", "264996390" , "311089933" , "3988916155" , "265781051" , "3141577094" , "2289473436" , "517914866" , "517914869" , "1117313539" , "1611279117" , "3283361988" , "1117313602" , "395090797" , "2429416220" , "1697271546" , "1117313924" };
        public static string[] RideAttackIDs = new string[] { "3491866260", "2056025809", "2534881408", "2600476838", "1247666429", "3750571080", "3642240295", "651750924", "2452463220", "1732461796", "3809261131", "1876785244", "3765765641", "3642969286", "1258041436" };

        public static float maxShare = 0;

        public int Damage => Attacks.Sum(x => x.Damage);

        public int GetZanverseDamage => Attacks.Where(a => a.ID == "2106601422").Sum(x => x.Damage);

        public int GetPhotonDamage => Attacks.Where(a => PhotonAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public int GetFinishDamage => Attacks.Where(a => FinishAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public int GetMagDamage => Attacks.Where(a => MagAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public int GetPBDamage => Attacks.Where(a => PBAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public int AISDamage => Attacks.Where(a => AISAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public int RideDamage => Attacks.Where(a => RideAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public int DBDamage => Attacks.Where(a => DBAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public double DPS => Damage / ActiveTime;

        public double ReadDPS => Math.Round(ReadDamage / (double)ActiveTime);

        public string StringDPS => ReadDPS.ToString("N0");

        public bool IsAIS => (isTemporary == "AIS");

        public bool IsRide => (isTemporary == "Ride");

        public bool IsZanverse => (isTemporary == "Zanverse");

        public bool IsMag => (isTemporary == "Mag Attacks");

        public bool IsPB => (isTemporary == "PB Attacks");

        public bool IsPunisher => (isTemporary == "Photon Attacks");

        public bool IsFinish => (isTemporary == "HTF Attacks");

        public bool IsDB => (isTemporary == "DB");

        public string MaxHitdmg => MaxHitAttack.Damage.ToString("N0");

        public bool IsYou => (ID == Hacks.currentPlayerID);

        public int MaxHitNum => MaxHitAttack.Damage;

        public string MaxHitID => MaxHitAttack.ID;

        public string DPSReadout => PercentReadDPSReadout;

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
                        return ((Attacks.Average(a => a.JA)) * 100).ToString("N0");
                    } else {
                        return ((Attacks.Average(a => a.JA)) * 100).ToString("N2");
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
                if (IsZanverse || IsFinish || IsAIS || IsPunisher || IsDB || IsRide || IsMag || IsPB)
                    return Damage;

                int temp = Damage;
                if (Properties.Settings.Default.SeparateZanverse)
                    temp -= GetZanverseDamage;
                if (Properties.Settings.Default.SeparateFinish)
                    temp -= GetFinishDamage;
                if (Properties.Settings.Default.SeparatePunisher)
                    temp -= GetPhotonDamage;
                if (Properties.Settings.Default.SeparateMag)
                    temp -= GetMagDamage;
                if (Properties.Settings.Default.SeparatePB)
                    temp -= GetPBDamage;
                if (Properties.Settings.Default.SeparateAIS)
                    temp -= AISDamage;
                if (Properties.Settings.Default.SeparateDB)
                    temp -= DBDamage;
                if (Properties.Settings.Default.SeparateRide)
                    temp -= RideDamage;
                return temp;
            }
        }

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

        public string AnonymousName()
        {
            if (IsYou)
                return Name;
            else
                return "----";
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
                if (Properties.Settings.Default.ShowDamageGraph && (IsAlly && !IsZanverse && !IsPunisher))
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 64, 64), Color.FromArgb(0, 0, 0, 0));
                } else {
                    if (IsYou && Properties.Settings.Default.HighlightYourDamage)
                        return new SolidColorBrush(Color.FromArgb(128, 0, 64,64));
                    return new SolidColorBrush(new Color());
                }
            }
        }

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

        public bool IsAlly
        {
            get
            {
                if (int.Parse(ID) >= 10000000 && !IsZanverse && !IsPunisher && !IsFinish && !IsMag && !IsPB)
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

    static class Hacks
    {
        public static string currentPlayerID;
        public static bool DontAsk = false;
        public static string targetID = "";
    }

    public class Attack
    {
        public string ID;
        public int Damage;
        public int Timestamp;
        public int JA;
        public int Cri;

        public Attack(string initID, int initDamage, int initTimestamp, int justAttack, int critical)
        {
            ID = initID;
            Damage = initDamage;
            Timestamp = initTimestamp;
            JA = justAttack;
            Cri = critical;
        }
    }

}
