using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace OverParse
{
    /// <summary>
    /// Detalis.xaml の相互作用ロジック
    /// </summary>
    public partial class Detalis : Window
    {
        public Detalis(string data1,string data2)
        {
            InitializeComponent();
            Data1.Content = data1;
            Data2.Content = data2;
        }
    }
}
