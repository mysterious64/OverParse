using System.Windows;

namespace OverParse
{
    /// <summary>
    /// Detalis.xaml の相互作用ロジック
    /// </summary>
    public partial class Detalis : Window
    {
        public Detalis(Combatant data)
        {
            InitializeComponent();
            //Testing...
            ID.Content = data.ID;
            PlayerName.Content = data.Name;
            Percent.Content = data.RatioPercent;
            Damage.Content = data.ReadDamage;
            Damaged.Content = data.ReadDamaged;
            DPS.Content = data.ReadDPS;
            JA.Content = data.WJAPercent;
            Critical.Content = data.WCRIPercent;
            Max.Content = data.MaxHitdmg;
            Atk.Content = data.MaxHit;
        }
    }
}
