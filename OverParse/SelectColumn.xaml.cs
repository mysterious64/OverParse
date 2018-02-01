using System.Windows;

namespace OverParse
{
    /// <summary>
    /// SelectColumn.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectColumn : Window
    {
        public bool ResultName, Pct, Dmg, Dmgd, DPS, JA, Cri, Hit, Atk, Vrb;

        public SelectColumn(bool Name, bool Pct, bool Dmg, bool Dmgd, bool DPS, bool JA, bool Cri, bool Hit, bool Atk, bool Vrb)
        {
            InitializeComponent();
            PlayerName.IsChecked = Name;
            Percent.IsChecked = Pct;
            Damage.IsChecked = Dmg;
            Damaged.IsChecked = Dmgd;
            PlayerDPS.IsChecked = DPS;
            PlayerJA.IsChecked = JA;
            Critical.IsChecked = Cri;
            MaxHit.IsChecked = Hit;
            AtkName.IsChecked = Atk;
            Variable.IsChecked = Vrb;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            ResultName = (bool)PlayerName.IsChecked;
            Pct = (bool)Percent.IsChecked;
            Dmg = (bool)Damage.IsChecked;
            Dmgd = (bool)Damaged.IsChecked;
            DPS = (bool)PlayerDPS.IsChecked;
            JA = (bool)PlayerJA.IsChecked;
            Cri = (bool)Critical.IsChecked;
            Hit = (bool)MaxHit.IsChecked;
            Atk = (bool)AtkName.IsChecked;
            Vrb = (bool)Variable.IsChecked;
            Close();
        }
    }
}
