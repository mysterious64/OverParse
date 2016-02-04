﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using NHotkey.Wpf;
using NHotkey;

namespace OverParse
{
    public static class WindowsServices
    {
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }

    public partial class MainWindow : Window
    {
        Log encounterlog;
        public static Dictionary<string, string> skillDict = new Dictionary<string, string>();
        IntPtr hwndcontainer;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            hwndcontainer = hwnd;
        }

        public MainWindow()
        {
            InitializeComponent();

            Directory.CreateDirectory("logs");

            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            AutoEndEncounters.IsChecked = Properties.Settings.Default.AutoEndEncounters;
            SeparateZanverse.IsChecked = Properties.Settings.Default.SeparateZanverse;
            ClickthroughMode.IsChecked = Properties.Settings.Default.ClickthroughEnabled;
            LogToClipboard.IsChecked = Properties.Settings.Default.LogToClipboard;

            HotkeyManager.Current.AddOrReplace("Increment", Key.E, ModifierKeys.Control | ModifierKeys.Shift, EndEncounter_Key);

            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            string[] tmp = File.ReadAllLines("skills.csv");
            foreach (string s in tmp)
            {
                string[] split = s.Split(',');
                skillDict.Add(split[1], split[0]);
            }

            encounterlog = new Log(Properties.Settings.Default.Path);
            Console.WriteLine(Properties.Settings.Default.Path);

            UpdateForm(null, null);
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateForm);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void EndEncounter_Key(object sender, HotkeyEventArgs e)
        {
            EndEncounter_Click(null, null);
            e.Handled = true;
        }

        private void ClickthroughToggle(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ClickthroughEnabled = ClickthroughMode.IsChecked;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = AlwaysOnTop.IsChecked;
            if (Properties.Settings.Default.ClickthroughEnabled)
            {
                int extendedStyle = WindowsServices.GetWindowLong(hwndcontainer, WindowsServices.GWL_EXSTYLE);
                WindowsServices.SetWindowLong(hwndcontainer, WindowsServices.GWL_EXSTYLE, extendedStyle | WindowsServices.WS_EX_TRANSPARENT);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = AlwaysOnTop.IsChecked;
            if (Properties.Settings.Default.ClickthroughEnabled)
            {
                int extendedStyle = WindowsServices.GetWindowLong(hwndcontainer, WindowsServices.GWL_EXSTYLE);
                WindowsServices.SetWindowLong(hwndcontainer, WindowsServices.GWL_EXSTYLE, extendedStyle & ~WindowsServices.WS_EX_TRANSPARENT);
            }
        }


        public void UpdateForm(object sender, EventArgs e)
        {
            encounterlog.UpdateLog(this, null);
            Application.Current.MainWindow.Title = "OverParse WDF Alpha";
            EncounterStatus.Content = encounterlog.logStatus();

            EncounterIndicator.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 100, 100));
            if (encounterlog.valid) { EncounterIndicator.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 100)); }
            if (encounterlog.running) { EncounterIndicator.Fill = new SolidColorBrush(Color.FromArgb(255, 100, 255, 100)); }

            if (encounterlog.running)
            {
                CombatantData.Items.Clear();

                foreach (Combatant c in encounterlog.combatants)
                {
                    if (c.isAlly || FilterPlayers.IsChecked)
                    {
                        CombatantData.Items.Add(c);
                    }
                }
                if (Properties.Settings.Default.AutoEndEncounters) {
                    int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    if ((unixTimestamp - encounterlog.newTimestamp) >= Properties.Settings.Default.EncounterTimeout)
                    {
                        encounterlog = new Log(Properties.Settings.Default.Path);
                    }
                }

            }   
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Select your pso2_bin folder.\n\nThis is the same folder you selected when setting up the Tweaker.\n\nIf you installed to the default location, it will be at \"C:\\PHANTASYSTARONLINE2\\pso2_bin\".", "Help");

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select your pso2_bin folder...";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = Directory.GetCurrentDirectory();

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = Directory.GetCurrentDirectory();
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) { return; }

            var folder = dlg.FileName;
            Console.WriteLine(folder);
            Properties.Settings.Default.Path = folder;
            encounterlog = new Log(folder);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
            encounterlog.WriteLog();
        }

        private void LogToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LogToClipboard = LogToClipboard.IsChecked;
        }


        private void EndEncounter_Click(object sender, RoutedEventArgs e)
        {
            encounterlog.WriteLog();
            if (Properties.Settings.Default.LogToClipboard) { encounterlog.WriteClipboard(); }
            encounterlog = new Log(Properties.Settings.Default.Path);
        }

        private void AutoEndEncounters_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoEndEncounters = AutoEndEncounters.IsChecked;
        }

        private void SeparateZanverse_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateZanverse = SeparateZanverse.IsChecked;
        }

        private void SetEncounterTimeout_Click(object sender, RoutedEventArgs e)
        {
            int x;
            string input = Microsoft.VisualBasic.Interaction.InputBox("How many seconds should the system wait before stopping an encounter?", "Encounter Timeout", Properties.Settings.Default.EncounterTimeout.ToString());
            if (Int32.TryParse(input, out x)) {
                if (x > 0) {
                    Properties.Settings.Default.EncounterTimeout = x;
                } else
                {
                    MessageBox.Show("What.");
                }
            } else
            {
                if (input.Length > 0) { MessageBox.Show("Couldn't parse your input. Enter only a number."); }
            }
        }

        private void Placeholder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This doesn't actually do anything yet.");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show($"OverParse v{version} - WDF Alpha\nAnything and everything may be broken.\n\nPlease use damage information responsibly.", "OverParse");
        }

        private void Website_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.tyronesama.moe/");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GenerateFakeEntries_Click(object sender, RoutedEventArgs e)
        {
            encounterlog.GenerateFakeEntries();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WindowStats_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            result += $"menu bar: {MenuBar.Width.ToString()} width {MenuBar.Height.ToString()} height\n";
            result += $"menu bar: {MenuBar.Padding} padding {MenuBar.Margin} margin\n";
            result += $"menu item: {MenuSystem.Width.ToString()} width {MenuSystem.Height.ToString()} height\n";
            result += $"menu item: {MenuSystem.Padding} padding {MenuSystem.Margin} margin\n";
            result += $"menu item: {AutoEndEncounters.Foreground} fg {AutoEndEncounters.Background} bg\n";
            result += $"menu item: {MenuSystem.FontFamily} {MenuSystem.FontSize} {MenuSystem.FontWeight} {MenuSystem.FontStyle}\n";
            result += $"image: {image.Width} w {image.Height} h {image.Margin} m\n";
            MessageBox.Show(result);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void LogUnmappedAttacks_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter the character name to generate a log for.", "Attack Mapping", Properties.Settings.Default.Username);
            Properties.Settings.Default.Username = input;
            Properties.Settings.Default.Save();
        }
    }

    public class Attack
    {
        public string ID;
        public int Damage;
        public int Timestamp;

        public Attack(string initID, int initDamage, int initTimestamp) {
            ID = initID;
            Damage = initDamage;
            Timestamp = initTimestamp;
            Console.WriteLine($"Attack generated: {ID} - {Damage}");
       }
    }

    public class Combatant
    {
        public int Damage { get; set; }
        public int Healing { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public int MaxHitNum { get; set; }
        public string MaxHitID { get; set; }
        public float DPS { get; set; }
        public float PercentDPS { get; set; }
        public List<Attack> Attacks {get; set;}

        public bool isAlly
        {
            get
            {
                string[] SuAttacks = { "487482498", "2785585589", "639929291" };

                if (Int32.Parse(ID) >= 10000000)
                {
                    return true;
                }

                bool allied = false;
                foreach (Attack a in Attacks)
                {
                    if (SuAttacks.Contains(a.ID)) { allied = true; return allied; }
                }

                return allied;
            }
        }

        public string MaxHit
        {
            get
            {
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(MaxHitID))
                {
                    attack = MainWindow.skillDict[MaxHitID];
                    
                }
                return MaxHitNum.ToString("N0") + $" ({attack})";
            }
        }

        public string DPSReadout
        {
            get
            {
                if (PercentDPS == -1)
                {
                    return "--";
                } else
                {
                    return string.Format("{0:0.0}", PercentDPS) + "%";
                }
            }
        }

        public string DamageReadout
        {
            get { return Damage.ToString("N0"); }
        }

        public Combatant(string id, string name)
        {
            ID = id;
            Name = name;
            Damage = 0;
            Healing = 0;
            MaxHitNum = 0;
            MaxHitID = "none";
            DPS = 0;
            PercentDPS = -1;
            Attacks = new List<Attack>();
        }
    }

    public class Log
    {
        public bool valid;
        public bool running;
        int startTimestamp = 0;
        public int newTimestamp = 0;
        string encounterData;
        StreamReader logreader;
        public List<Combatant> combatants = new List<Combatant>();
        Random random = new Random();

        string FormatNumber(int num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return (num / 1000D).ToString("0.#K");
            }
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.##K");
            }

            return num.ToString("#,0");
        }

        public Log(string attemptDirectory)
        {
            valid = false;
            DirectoryInfo directory = new DirectoryInfo($"{attemptDirectory}\\damagelogs");
            if (!directory.Exists) { Complain(); return; }
            if (directory.GetFiles().Count() == 0) { Complain(); return; }

            valid = true;
            running = false;

            FileInfo log = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
            Console.WriteLine($"Reading from {log.DirectoryName}\\{log.Name}");
            FileStream fileStream = File.Open(log.DirectoryName + "\\" + log.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(0, SeekOrigin.End);
            logreader = new StreamReader(fileStream);
        }

        public void Complain()
        {
            MessageBox.Show("No damage logs were found.\n\nPlease use \"System > Locate damagelogs folder...\" to select your installation directory, and make sure that the Damage Parser plugin is enabled in the Tweaker.", "Error");
        }

        public void WriteClipboard()
        {
            string log = "";

            foreach (Combatant c in combatants)
            {
                if (c.isAlly)
                {
                    string shortname = c.Name;
                    if (c.Name.Length > 4)
                    {
                        shortname = c.Name.Substring(0, 4);
                    }
                    log += $"{shortname} {FormatNumber(c.Damage)} | ";
                }
            }

            try {Clipboard.SetText(log);}
            catch (Exception ex) { }
        }

        public void WriteLog()
        {
            if (combatants.Count != 0)
            {
                int elapsed = newTimestamp - startTimestamp;
                TimeSpan timespan = TimeSpan.FromSeconds(elapsed);
                string timer = timespan.ToString(@"mm\:ss");

                string log = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + " | " + timer + Environment.NewLine + Environment.NewLine;

                foreach (Combatant c in combatants)
                {
                    if (c.isAlly)
                    {
                        log += $"### {c.Name} - {c.Damage} Dmg ({c.DPSReadout}) ### " + Environment.NewLine;

                        List<string> attackTypes = new List<string>();
                        List<int> damageTotals = new List<int>();

                        foreach (Attack a in c.Attacks)
                        {
                            string name = a.ID;

                            if (MainWindow.skillDict.ContainsKey(a.ID))
                            {
                                name = MainWindow.skillDict[a.ID];

                            }

                            if (attackTypes.Contains(name))
                            {
                                int index = attackTypes.IndexOf(name);
                                damageTotals[index] += a.Damage;
                            } else
                            {
                                attackTypes.Add(name);
                                damageTotals.Add(a.Damage);
                            }
                        }

                        int total = damageTotals.Sum();

                        List<Tuple<string, int>> finalAttacks = new List<Tuple<string, int>>();
                        
                        foreach (string str in attackTypes)
                        {
                            finalAttacks.Add(new Tuple<string,int>(str, damageTotals[attackTypes.IndexOf(str)]));
                        }

                        finalAttacks = finalAttacks.OrderBy(x => x.Item2).Reverse().ToList();

                        foreach (Tuple<string, int> t in finalAttacks)
                        {
                            log += $"{t.Item2 * 100 / total}% | {t.Item1} ({t.Item2} dmg)" + Environment.NewLine;
                        }
                        log += Environment.NewLine;
                    }
                }

                foreach (Combatant c in combatants)
                {
                    log += $"{c.Name} | {c.Damage} dmg | {c.DPSReadout} contrib | {c.DPS} DPS | Max: {c.MaxHit}" + Environment.NewLine;
                }

                File.WriteAllText("logs/OverParse Log - " + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".txt", log);

                string result = "";

                foreach (Combatant c in combatants)
                {
                    if (c.Name == Properties.Settings.Default.Username)
                    {
                        Console.WriteLine("found combatant matching name");
                        foreach (Attack a in c.Attacks)
                        {
                            if (!MainWindow.skillDict.ContainsKey(a.ID))
                            {
                                Console.WriteLine("found unmapped attack");
                                result += $"{a.Timestamp / 60}:{a.Timestamp % 60} -- {a.ID} dealing {a.Damage} dmg" + Environment.NewLine;
                            }
                        }

                        File.WriteAllText($"logs/Attack Mapping Debug - " + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".txt", result);
                    }
                }
            }
        }

        public string logStatus()
        {
            if (!valid)
            {
                return "No logs, check \"System > Locate install folder\"!";
            }
            if (!running)
            {
                return "Waiting for combat data...";
            }
            return encounterData;
        }

        public void GenerateFakeEntries()
        {
            for (int i = 0; i <=9 ; i++)
            {
                Combatant temp = new Combatant("1000000" + i.ToString(),"TestPlayer_" + i.ToString());
                temp.PercentDPS = (float)random.Next(0, 10000) / 100;
                temp.DPS = random.Next(0, 1000);
                temp.Damage = random.Next(0, 10000000);
                temp.MaxHitNum = random.Next(0, 1000000);
                temp.MaxHitID = "2368738938";
                combatants.Add(temp);
            }
            for (int i = 0; i <= 9; i++)
            {
                Combatant temp = new Combatant(i.ToString(), "TestEnemy_" + i.ToString());
                temp.PercentDPS = -1;
                temp.DPS = random.Next(0, 1000);
                temp.Damage = random.Next(0, 10000000);
                temp.MaxHitNum = random.Next(0, 1000000);
                temp.MaxHitID = "1612949165";
                combatants.Add(temp);
            }

            valid = true;
            running = true;
        }

        public void UpdateLog(object sender, EventArgs e)
        {
            if (!valid) { return; }
            string newLines = logreader.ReadToEnd();
            if (newLines != "")
            {
                string[] result = newLines.Split('\n');
                foreach (string str in result)
                {
                    if (str != "")
                    {
                        string[] parts = str.Split(',');

                        string lineTimestamp = parts[0];
                        string sourceID = parts[2];
                        string sourceName = parts[3];
                        int hitDamage = Int32.Parse(parts[7]);
                        string attackID = parts[6];
                        string isMultiHit = parts[10];
                        string isMisc = parts[11];
                        string isMisc2 = parts[12];

                        int index = -1;
                        foreach (Combatant x in combatants)
                        {
                            if (x.ID == sourceID) { index = combatants.IndexOf(x); }
                        }

                        if (attackID == "2106601422" && Properties.Settings.Default.SeparateZanverse) {
                            index = -1;
                            foreach (Combatant x in combatants)
                            {
                                if (x.ID == "94857493" && x.Name == "Zanverse") { index = combatants.IndexOf(x); }
                            }
                            sourceID = "94857493";
                            sourceName = "Zanverse";
                        }


                        if (index == -1)
                        {
                            combatants.Add(new Combatant(sourceID, sourceName));
                            index = combatants.Count - 1;
                        }

                        Combatant source = combatants[index];

                        if (hitDamage > 0)
                        {
                            source.Damage += hitDamage;

                            newTimestamp = Int32.Parse(lineTimestamp);
                            if (startTimestamp == 0) { startTimestamp = newTimestamp; }

                            source.Attacks.Add(new Attack(attackID, hitDamage, newTimestamp - startTimestamp));

                            running = true;
                        }
                        else
                        {
                            if (startTimestamp != 0) { source.Healing -= hitDamage; }
                        }

                        if (source.MaxHitNum < hitDamage)
                        {
                            source.MaxHitNum = hitDamage;
                            source.MaxHitID = attackID;
                        }

                        string attack = attackID.ToString();
                        if (MainWindow.skillDict.ContainsKey(attackID))
                        {
                            attack = MainWindow.skillDict[attackID];
                        }
                        Console.WriteLine($" attack: {attack} {hitDamage} | misc: {isMultiHit} {isMisc} {isMisc2} | source: {source.Name} {source.ID}");
                    }
                }


                combatants.Sort((x, y) => y.Damage.CompareTo(x.Damage));

                if (startTimestamp != 0 && newTimestamp != startTimestamp)
                {
                    int elapsed = newTimestamp - startTimestamp;
                    float partyDPS = 0;
                    int filtered = 0;

                    TimeSpan timespan = TimeSpan.FromSeconds(elapsed);
                    string timer = timespan.ToString(@"mm\:ss");

                    encounterData = $"{timer}";

                    foreach (Combatant x in combatants)
                    {
                        if (x.isAlly)
                        {

                            float dps = x.Damage / (newTimestamp - startTimestamp);
                            x.DPS = dps;
                            partyDPS += dps;
                        }
                        else
                        {
                            filtered++;
                        }
                    }

                    foreach (Combatant x in combatants)
                    {
                        if (x.isAlly)
                        {
                            x.PercentDPS = (x.DPS / partyDPS * 100);
                        }
                        else
                        {
                            x.PercentDPS = -1;
                        }
                    }

                    encounterData += $" - {partyDPS.ToString("0.00")} DPS";
                }

            }
        }
    }
}
