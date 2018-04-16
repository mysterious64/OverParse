using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace OverParse
{
    // Handles the logging section of the parser.
    // TODO: Optimise the rest of the codes
    public class Log
    {
        // File Setup Variables
        public bool valid, notEmpty, running, nagMe;
        public string filename;
        public DirectoryInfo logDirectory;

        private const int pluginVersion = 5;

        // Logging Variables
        public int startTimestamp = 0;
        public int newTimestamp = 0;
        public List<Combatant> combatants = new List<Combatant>();
        public List<Combatant> backupCombatants = new List<Combatant>();

        private string encounterData;
        private List<int> instances = new List<int>();
        private StreamReader logReader;

        public string ManualattemptDirectory;

        // Constructor
        public Log(string attemptDirectory)
        {
            valid    = false;
            notEmpty = false;
            running  = false;
            nagMe    = false;
            ManualattemptDirectory = attemptDirectory;

            SetupWarning(); // Setup first time warning

            PromptBinDirectory(attemptDirectory); // Invalid pso2_bin directory, prompting for new one...

            if (!File.Exists($"{attemptDirectory}\\pso2.exe")) { return; } // If pso2_bin isn't selected, exiting ...

            valid = true; // pso2_bin selected correctly!

            logDirectory = new DirectoryInfo($"{attemptDirectory}\\damagelogs"); // Making sure pso2_bin\damagelogs exists

            CheckLaunchMethod(logDirectory); // Check launch method

            Properties.Settings.Default.FirstRun = false; // Passed first time setup, skipping above on future launch

            /* ---------------------------------------------------------------------------------------------------- */ 

            if (!logDirectory.Exists) { return; }                 // Abort if damage log directory doesn't exist
            if (logDirectory.GetFiles().Count() == 0) { return; } // Abort if damage log directory content is empty

            notEmpty = true; // Log directory is not empty!

            FileInfo log = logDirectory.GetFiles().Where(f => Regex.IsMatch(f.Name, @"\d+\.csv")).OrderByDescending(f => f.Name).First();
            filename = log.Name; // Reading from {log.DirectoryName}\{log.Name}")

            FileStream fileStream = File.Open(log.DirectoryName + "\\" + log.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(0, SeekOrigin.Begin);
            logReader = new StreamReader(fileStream);

            string existingLines = logReader.ReadToEnd(); // Gotta get the dummy line for current player name
            string[] result = existingLines.Split('\n');
            foreach (string s in result)
            {
                if (s == "")
                    continue;
                string[] parts = s.Split(',');
                if (parts[0] == "0" && parts[3] == "YOU")
                {
                    Hacks.currentPlayerID = parts[2]; // Found existing active player ID
                }
            }
        }

        /* CLASS FUNCTIONS */ 

        // Updates all Parser related plugins
        public bool UpdatePlugin(string attemptDirectory)
        {
            try
            {
                // Copy file into 'pso2_bin' folder
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\pso2h.dll", attemptDirectory + "\\pso2h.dll", true);
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\ddraw.dll", attemptDirectory + "\\ddraw.dll", true);

                Directory.CreateDirectory(attemptDirectory + "\\plugins"); // Create the directory

                // Copy file into 'plugins' folder
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\PSO2DamageDump.dll", attemptDirectory + "\\plugins" + "\\PSO2DamageDump.dll", true);
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\PSO2DamageDump.cfg", attemptDirectory + "\\plugins" + "\\PSO2DamageDump.cfg", true);

                Properties.Settings.Default.InstalledPluginVersion = pluginVersion; // Plugin version noted

                MessageBox.Show("Setup complete! A few files have been copied to your pso2_bin folder.\n\n" 
                              + "If PSO2 is running right now, you'll need to close it before the changes can take effect."
                              , "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;  // Plugin install successful
            }
            catch
            {
                MessageBox.Show("Something went wrong with manual installation. " 
                              + "This usually means that the files are already in use: try again with PSO2 closed.\n\n" 
                              + "If you've recieved this message even after closing PSO2, you may need to run OverParse as administrator."
                              , "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                return false; // PLUGIN INSTALL FAILED - REEEEEE
            }
        }

        // Copy the parse data to clipboard -- To be removed ? --
        public void WriteClipboard()
        {
            string log = "";
            foreach (Combatant c in combatants)
            {
                if (c.IsAlly)
                {
                    string shortname = c.Name;
                    if (c.Name.Length > 6)
                    {
                        shortname = c.Name.Substring(0, 6);
                    }

                    log += $"{shortname} {(c.Damage).ToString("N0")} | ";
                }
            }

            if (log == "") { return; }
            log = log.Substring(0, log.Length - 2);

            try
            {
                Clipboard.SetText(log);
            }
            catch
            {
                //LMAO
            }
        }

        // Writes combat data to log file
        public string WriteLog()
        {
            if (combatants.Count != 0) // Players found
            {
                int elapsed = newTimestamp - startTimestamp;
                TimeSpan timespan = TimeSpan.FromSeconds(elapsed);

                // Logging the total time occured through out the encounter
                string totalDamage = combatants.Sum(x => x.Damage).ToString("N0");
                string timer       = timespan.ToString(@"mm\:ss");
                string log         = DateTime.Now.ToString("F") + " | " + timer + " | Total MPA Damage : "  + totalDamage + Environment.NewLine + Environment.NewLine;

                log += "[ Encounter Overview ]" + Environment.NewLine; // Title

                foreach (Combatant c in combatants)
                {
                    if (c.IsAlly || c.IsZanverse || c.IsFinish)
                    {
                        log += Environment.NewLine + $"# {c.Name}"+ Environment.NewLine + $"# Contrib: {c.PercentReadDPSReadout}% | Dealt: {c.ReadDamage.ToString("N0")} dmg | Taken: {c.Damaged} dmgd | {c.DPS} DPS | JA: {c.WJAPercent}% | Critical: {c.WCRIPercent}% | Max: {c.MaxHitdmg} ({c.MaxHit})" + Environment.NewLine;
                    }
                }

                log += Environment.NewLine + Environment.NewLine;

                foreach (Combatant c in combatants)
                {
                    if (c.IsAlly || c.IsZanverse || c.IsFinish)
                    {
                        string header = $"[ {c.Name} - {c.PercentReadDPSReadout}% - {c.ReadDamage.ToString("N0")} dmg ]";
                        log += header + Environment.NewLine + Environment.NewLine;

                        List<string> attackNames = new List<string>();
                        List<string> finishNames = new List<string>();
                        List<Tuple<string, List<int>, List<int>, List<int>>> attackData = new List<Tuple<string, List<int>, List<int>, List<int>>>();

                        if (c.IsZanverse && Properties.Settings.Default.SeparateZanverse)
                        {
                            foreach (Combatant c2 in backupCombatants)
                            {
                                if (c2.ZvsDamage > 0)
                                    attackNames.Add(c2.ID);
                            }

                            foreach (string s in attackNames)
                            {
                                Combatant targetCombatant = backupCombatants.First(x => x.ID == s);
                                List<int> matchingAttacks = targetCombatant.Attacks.Where(a => a.ID == "2106601422").Select(a => a.Damage).ToList();
                                List<int> jaPercents      = c.Attacks.Where(a => a.ID == "2106601422").Select(a => a.JA).ToList();
                                List<int> criPercents     = c.Attacks.Where(a => a.ID == "2106601422").Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<int>, List<int>, List<int>>(targetCombatant.Name, matchingAttacks, jaPercents, criPercents));
                            }
                        }

                        else if (c.IsFinish && Properties.Settings.Default.SeparateFinish)
                        {
                            foreach (Combatant c3 in backupCombatants)
                            {
                                if (c3.HTFDamage > 0)
                                    finishNames.Add(c3.ID);
                            }

                            foreach (string htf in finishNames)
                            {
                                Combatant tCombatant       = backupCombatants.First(x => x.ID == htf);
                                List<int> fmatchingAttacks = tCombatant.Attacks.Where(a => Combatant.FinishAttackIDs.Contains(a.ID)).Select(a => a.Damage).ToList();
                                List<int> jaPercents       = c.Attacks.Where(a => Combatant.FinishAttackIDs.Contains(a.ID)).Select(a => a.JA).ToList();
                                List<int> criPercents      = c.Attacks.Where(a => Combatant.FinishAttackIDs.Contains(a.ID)).Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<int>, List<int>, List<int>>(tCombatant.Name, fmatchingAttacks, jaPercents, criPercents));
                            }

                        }
                        else
                        {
                            foreach (Attack a in c.Attacks)
                            {
                                if ((a.ID == "2106601422" && Properties.Settings.Default.SeparateZanverse) || (Combatant.FinishAttackIDs.Contains(a.ID) && Properties.Settings.Default.SeparateFinish))
                                    continue; // Don't do anything
                                if (MainWindow.skillDict.ContainsKey(a.ID))
                                    a.ID = MainWindow.skillDict[a.ID]; // these are getting disposed anyway, no 1 cur
                                if (!attackNames.Contains(a.ID))
                                    attackNames.Add(a.ID);
                            }

                            foreach (string s in attackNames)
                            {
                                // Choose damage from matching attacks
                                List<int> matchingAttacks = c.Attacks.Where(a => a.ID == s).Select(a => a.Damage).ToList();
                                List<int> jaPercents      = c.Attacks.Where(a => a.ID == s).Select(a => a.JA).ToList();
                                List<int> criPercents     = c.Attacks.Where(a => a.ID == s).Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<int>, List<int>, List<int>>(s, matchingAttacks, jaPercents, criPercents));
                            }
                        }

                        attackData = attackData.OrderByDescending(x => x.Item2.Sum()).ToList();

                        foreach (var i in attackData)
                        {
                            double percent = i.Item2.Sum() * 100d / c.ReadDamage;

                            string spacer        = (percent >= 9) ? "" : " ";
                            string paddedPercent = percent.ToString("00.00");

                            string hits = i.Item2.Count().ToString("N0");
                            string sum  = i.Item2.Sum().ToString("N0");
                            string min  = i.Item2.Min().ToString("N0");
                            string max  = i.Item2.Max().ToString("N0");
                            string avg  = i.Item2.Average().ToString("N0");
                            string ja   = (i.Item3.Average() * 100).ToString("N2") ?? "null";
                            string cri  = (i.Item4.Average() * 100).ToString("N2") ?? "null" ;

                            log += $"{paddedPercent}% | {i.Item1} ({sum} dmg)" + Environment.NewLine;
                            log += $"       |   JA : {ja}% - Critical : {cri}%" + Environment.NewLine;
                            log += $"       |   {hits} hits - {min} min, {avg} avg, {max} max" + Environment.NewLine;
                        }

                        log += Environment.NewLine;
                    }
                }

                log += "Instance IDs: " + String.Join(", ", instances.ToArray());

                DateTime thisDate = DateTime.Now;
                string directory = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

                Directory.CreateDirectory($"Logs/{directory}");
                string datetime = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
                string filename = $"Logs/{directory}/OverParse - {datetime}.txt";

                File.WriteAllText(filename, log);

                return filename;
            }

            return null;
        }

        // Returns log status messages
        public string LogStatus()
        {
            if (!valid) // If not valid ...
            {
                return "USER SHOULD PROBABLY NEVER SEE THIS";
            }

            if (!notEmpty) // If damage log is empty ...
            {
                return "Directory No logs: Enable plugin and check pso2_bin!";
            }

            if (!running) // If parser is running ...
            {
                return $"Waiting for combat data...";
            }

            return encounterData;
        }

        // Updates the logging display
        public void UpdateLog(object sender, EventArgs e)
        {
            if (!valid || !notEmpty) { return; }

            string newLines = logReader.ReadToEnd();

            if (newLines != "")
            {
                string[] result = newLines.Split('\n');
                foreach (string str in result)
                {
                    if (str != "")
                    {
                        string[] parts = str.Split(',');

                        string sourceID   = parts[2];
                        string sourceName = parts[3];
                        string targetID   = parts[4];
                        string targetName = parts[5];
                        string attackID   = parts[6];

                        // string isMultiHit = parts[10];
                        // string isMisc = parts[11];
                        // string isMisc2 = parts[12];

                        int lineTimestamp = int.Parse(parts[0]);
                        int instanceID    = int.Parse(parts[1]);
                        int hitDamage     = int.Parse(parts[7]);
                        int justAttack    = int.Parse(parts[8]);
                        int critical      = int.Parse(parts[9]);

                        int index = -1;
                        
                        if (lineTimestamp == 0 && parts[3] == "YOU")
                        {
                            Hacks.currentPlayerID = parts[2];
                            continue;
                        }

                        if (sourceID != Hacks.currentPlayerID && Properties.Settings.Default.Onlyme){ continue; }

                        if (!instances.Contains(instanceID)) { instances.Add(instanceID); }

                        if (hitDamage < 1) { continue; }

                        if (sourceID == "0" || attackID == "0") { continue; }

                        // Process start

                        if (10000000 < int.Parse(sourceID))
                        {
                            foreach (Combatant x in combatants)
                            {
                                if (x.ID == sourceID && x.isTemporary == "no") 
                                { 
                                    index = combatants.IndexOf(x); 
                                }
                            }

                            if (index == -1)
                            {
                                combatants.Add(new Combatant(sourceID, sourceName));
                                index = combatants.Count - 1;
                            }

                            Combatant source = combatants[index];
                            
                            newTimestamp = lineTimestamp;
                            if (startTimestamp == 0) 
                            { 
                                startTimestamp = newTimestamp; 
                            }

                            source.Attacks.Add(new Attack(attackID, hitDamage, justAttack, critical, newTimestamp - startTimestamp)); 

                            running = true;
                        } 
                        else 
                        {
                            // Damage Taken Process
                            foreach (Combatant x in combatants)
                            {
                                if (x.ID == targetID && x.isTemporary == "no") 
                                {
                                    index = combatants.IndexOf(x); 
                                }
                            }

                            if (index == -1)
                            {
                                combatants.Add(new Combatant(targetID, targetName));
                                index = combatants.Count - 1;
                            }

                            Combatant source = combatants[index];

                            newTimestamp = lineTimestamp;

                            if (startTimestamp == 0)
                            {
                                startTimestamp = newTimestamp;
                            }

                            source.Damaged += hitDamage;
                            running = true;
                            
                        }

                    }
                }

                combatants.Sort((x, y) => y.ReadDamage.CompareTo(x.ReadDamage));

                if (startTimestamp != 0)
                {
                    encounterData = "0:00:00 - ∞ DPS";
                }

                if (startTimestamp != 0 && newTimestamp != startTimestamp)
                {
                    foreach (Combatant x in combatants)
                    {
                        if (x.IsAlly || x.IsZanverse)
                            x.ActiveTime = (newTimestamp - startTimestamp);
                    }
                }
            }
        }

        /* HELPER FUNCTIONS */ 

        // First time warning setup
        private void SetupWarning() 
        {
            if (Properties.Settings.Default.BanWarning)
            {
                MessageBoxResult panicResult = MessageBox.Show("OverParse is a 3rd-party tool that breaks PSO2's Terms and Conditions."
                                                             + "SEGA has confirmed in an official announcement that accounts found using parsing tools may be banned.\n\n"
                                                             + "If account safety is your first priority, do NOT use OverParse. You use this tool entirely at your own risk.\n\n"
                                                             + "Would you like to continue with setup?", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (panicResult == MessageBoxResult.No)
                {
                    Environment.Exit(-1);
                }

                Properties.Settings.Default.BanWarning = false;
            }
        }

        // Prompts user for pso2_bin directory
        private void PromptBinDirectory(string attemptDirectory) 
        {
            while (!File.Exists($"{attemptDirectory}\\pso2.exe"))
            {
                if (nagMe)
                {
                    MessageBox.Show("That doesn't appear to be a valid pso2_bin directory.\n\n" 
                                  + "If you installed the game using default settings, it will probably be in C:\\PHANTASYSTARONLINE2\\pso2_bin\\. " 
                                  + "Otherwise, find the location you installed to.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Please select your pso2_bin directory. OverParse uses this to read your damage logs.\n\n" 
                                  + "If you picked a folder while setting up the Tweaker, choose that. " 
                                  + "Otherwise, it will be in your PSO2 installation folder.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    nagMe = true;
                }

                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "Select your pso2_bin folder. This will be inside the folder you installed PSO2 to."
                };

                System.Windows.Forms.DialogResult picked = dialog.ShowDialog();
                if (picked == System.Windows.Forms.DialogResult.OK)
                {
                    // Testing {attemptDirectory} as pso2_bin directory...
                    attemptDirectory = dialog.SelectedPath;
                    Properties.Settings.Default.Path = attemptDirectory;
                }
                else
                {
                    // Canceled out of directory picker
                    MessageBox.Show("OverParse needs a valid PSO2 installation to function.\n" 
                                  + "The application will now close.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    Environment.Exit(-1); // ABORT ABORT ABORT
                    break;
                }
            }
        }

        // Checks user preferred launch method
        private void CheckLaunchMethod(DirectoryInfo logDirectory)
        {
            if (Properties.Settings.Default.LaunchMethod == "Unknown")
            {
                MessageBoxResult tweakerResult = MessageBox.Show("Do you use the PSO2 Tweaker?", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                Properties.Settings.Default.LaunchMethod = (tweakerResult == MessageBoxResult.Yes) ? "Tweaker" : "Manual";
            }

            // Tweaker method of launching... else manually
            if (Properties.Settings.Default.LaunchMethod == "Tweaker")
            {
                bool warn = true;
                if (logDirectory.Exists)
                {
                    if (logDirectory.GetFiles().Count() > 0)
                    {
                        warn = false;
                    }
                }

                if (warn && Hacks.DontAsk)
                {
                    MessageBox.Show("Your PSO2 folder doesn't contain any damagelogs. This is not an error, just a reminder!\n\n" 
                                  + "Please turn on the Damage Parser plugin in PSO2 Tweaker (orb menu > Plugins). OverParse needs this to function. " 
                                  + "You may also want to update the plugins while you're there.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    Hacks.DontAsk = true;
                    Properties.Settings.Default.FirstRun = false;
                    Properties.Settings.Default.Save();
                    return;
                }
            }
            else if (Properties.Settings.Default.LaunchMethod == "Manual")
            {
                bool pluginsExist = File.Exists(ManualattemptDirectory + "\\pso2h.dll") && File.Exists(ManualattemptDirectory + "\\ddraw.dll") && File.Exists(ManualattemptDirectory + "\\plugins" + "\\PSO2DamageDump.dll");
                if (!pluginsExist)
                    Properties.Settings.Default.InstalledPluginVersion = -1;

                if (Properties.Settings.Default.InstalledPluginVersion < pluginVersion)
                {
                    MessageBoxResult selfdestructResult;

                    if (pluginsExist)
                    {
                        // Prompting for plugin update
                        selfdestructResult = MessageBox.Show("This release of OverParse includes a new version of the parsing plugin. Would you like to update now?\n\n" 
                                                           + "OverParse may behave unpredictably if you use a different version than it expects.", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }
                    else
                    {
                        // Prompting for initial plugin install
                        selfdestructResult = MessageBox.Show("OverParse needs a Tweaker plugin to recieve its damage information.\n\n" 
                                                           + "The plugin can be installed without the Tweaker, but it won't be automatically updated, and I can't provide support for this method.\n\n" 
                                                           + "Do you want to try to manually install the Damage Parser plugin?", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }

                    if (selfdestructResult == MessageBoxResult.No && !pluginsExist)
                    {
                        // Denied plugin install
                        MessageBox.Show("OverParse needs the Damage Parser plugin to function.\n\n" 
                                      + "The application will now close.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                        Environment.Exit(-1);
                        return;
                    }
                    else if (selfdestructResult == MessageBoxResult.Yes)
                    {
                        // Accepted plugin install
                        bool success = UpdatePlugin(ManualattemptDirectory);
                        if (!pluginsExist && !success)
                            Environment.Exit(-1);
                    }
                }
            }
        }

        /* DEBUG MODE ONLY - Not used on production

        // Debug for ID mapping
        private void DebugMapping() 
        {
            foreach (Combatant c in combatants)
            {
                if (c.IsAlly)
                {
                    foreach (Attack a in c.Attacks)
                    {
                        if (!MainWindow.skillDict.ContainsKey(a.ID))
                        {
                            TimeSpan t = TimeSpan.FromSeconds(a.Timestamp);
                            Console.WriteLine($"{t.ToString(@"dd\.hh\:mm\:ss")} unmapped: {a.ID} ({a.Damage} dmg from {c.Name})");
                        }
                    }
                }
            }
        }

        */
    }
}
