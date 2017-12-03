using System.Windows;

namespace OverParse
{
    public partial class Inputbox : Window
    {
        public string ResultText = "";

        public Inputbox(string title ="", string text="", string defalutvalue = "")
        {
            InitializeComponent();

            Title = title;
            Description.Content = text;
            InputBox.Text = defalutvalue;
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            ResultText = InputBox.Text;
            Close();
        }

        private void InputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                DialogResult = true;
                ResultText = InputBox.Text;
                Close();
            }
            if(e.Key == System.Windows.Input.Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
