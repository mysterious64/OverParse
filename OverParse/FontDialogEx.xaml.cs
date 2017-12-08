using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup; // XmlLanguage
using System.Windows.Input;
using System.Diagnostics; // Debug

namespace emanual.Wpf.Dialogs
{
    public partial class FontDialogEx : Window
    {
        // メンバ変数と初期値
        private XmlLanguage FXmlLanguage;
        private string FSampleText = "サンプル文字列\r\nSample Text";
        private ToolFont FFont = null;

        private double[] FFontSizeArray = new double[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 20, 22, 24, 26, 28, 30, 32, 36, 48, 64, 72, 96 };

        public ToolFont Font { get { return FFont; } set { FFont = value; } }
        public string SampleText { get { return FSampleText; } set { FSampleText = value; } }
        //-----------------------------------------------------------------------------------------------
        public FontDialogEx()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (FFont == null)
            {
                FFont = new ToolFont();
            }

            FXmlLanguage = XmlLanguage.GetLanguage(this.Font.FontLanguage);

            this.SetFontSizeList();
            this.SetLanguageList();

            this.SampleText = FSampleText;
            txtSample.Text = FSampleText;
        }

        //---------------------------------------------------------------------------------------------
        // 指定のオブジェクトに対してフォント関連のプロパティを設定する
        // obj : プロパティを設定する対象のオブジェクト（TextBox とか TextBlock）
        public void SetPropertyToTargetObject(DependencyObject obj)
        {
            obj.SetValue(FontFamilyProperty, FFont.FontFamily);
            obj.SetValue(FontSizeProperty, FFont.FontSize);
            obj.SetValue(FontStyleProperty, FFont.FontStyle);
            obj.SetValue(FontWeightProperty, FFont.FontWeight);
            obj.SetValue(FontStretchProperty, FFont.FontStretch);
        }

        //---------------------------------------------------------------------------------------------
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        //---------------------------------------------------------------------------------------------
        private void FirstFamilyName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirstFamilyName.Items.Count < 1)
                return;

            FontFamily item = FirstFamilyName.SelectedItem as FontFamily;

            if (item != null)
            {
                txtFamilyName.Text = item.ToString();
                FFont.FontFamily = item;

                this.UpdateTypeFace();
                this.UpdateSampleText();
            }
        }

        //---------------------------------------------------------------------------------------------
        private void FirstTypeface_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock item = FirstTypeface.SelectedItem as TextBlock;

            if (item != null)
            {
                txtTypeface.Text = item.Text as string;
                var style = (TypefaceStyle)item.Tag;
                FFont.FontStyle = style.FontStyle;
                FFont.FontStretch = style.FontStretch;
                FFont.FontWeight = style.FontWeight;

                this.UpdateSampleText();
            }
        }

        //---------------------------------------------------------------------------------------------
        private void FirstFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirstFontSize.Items.Count < 1)
                return;

            FFont.FontSize = (double)FirstFontSize.SelectedItem;
            txtFontSize.Text = Convert.ToString(FFont.FontSize);
            this.UpdateSampleText();
        }

        //---------------------------------------------------------------------------------------------
        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbLanguage.Items.Count < 1)
                return;

            ComboBoxItem item = cmbLanguage.SelectedItem as ComboBoxItem;
            FXmlLanguage = item.Tag as XmlLanguage;

            if (FXmlLanguage == null)
                FFont.FontLanguage = null;
            else
                FFont.FontLanguage = FXmlLanguage.IetfLanguageTag;

            this.UpdateFamilyName();
        }

        //---------------------------------------------------------------------------------------------
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        //---------------------------------------------------------------------------------------------
        // cmbLanguage の選択に伴って、FirstFamilyName を更新する
        private void UpdateFamilyName()
        {
            var list = new List<FontFamily>();

            // すべての言語のとき
            if (FFont.FontLanguage == null)
            {
                foreach (FontFamily family in Fonts.SystemFontFamilies)
                {
                    LanguageSpecificStringDictionary dic1 = family.FamilyNames;

                    foreach (XmlLanguage lang in dic1.Keys)
                    {
                        var item1 = new FontFamily();

                        string s = dic1[lang] as string;

                        if ((s != null) && (s.Length > 0))
                        {
                            item1 = family;
                            list.Add(item1);
                        }
                    }
                }
            }
            else // 特定の言語のとき
            {
                foreach (FontFamily family in Fonts.SystemFontFamilies)
                {
                    LanguageSpecificStringDictionary dic2 = family.FamilyNames;
                    var item2 = new FontFamily();

                    string s = "";

                    if (dic2.ContainsKey(FXmlLanguage))
                    {
                        s = dic2[FXmlLanguage] as string;

                        if ((s != null) && (s.Length > 0))
                        {
                            item2 = family;

                            list.Add(item2);
                        }
                    }
                }
            }

            list.Sort(SortComparison);
            FirstFamilyName.ItemsSource = list;

            // 指定のフォントを選択状態にする
            int index = 0;

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].Equals(FFont.FontFamily))
                {
                    index = i;
                    break;
                }
            }

            FirstFamilyName.SelectedIndex = index;
            txtFamilyName.Text = list[index].ToString();
            FirstFamilyName.ScrollIntoView(FirstFamilyName.SelectedItem);
        }

        //---------------------------------------------------------------------------------------------
        // FirstFamilyName の選択の変更に伴って、FirstTypeface を更新する
        private void UpdateTypeFace()
        {
            var list = new List<TextBlock>();
            var family = new FontFamily(txtFamilyName.Text);

            foreach (Typeface face in family.GetTypefaces())
            {
                TextBlock item = new TextBlock();

                foreach (KeyValuePair<XmlLanguage, string> dic in face.FaceNames)
                {
                    if (dic.Key.IetfLanguageTag == this.Font.FontLanguage)
                    {
                        // シミュレートするフォントのとき
                        if ((face.IsBoldSimulated) || (face.IsObliqueSimulated))
                            item.Text = String.Format("{0} (simulated)", dic.Value);
                        else
                            item.Text = String.Format("{0}", dic.Value);
                    }
                }

                item.Tag = new TypefaceStyle(face.Style, face.Stretch, face.Weight);

                list.Add(item);
            }

            FirstTypeface.ItemsSource = list;

            //-------------------------------------------------------
            // 初期値を設定する
            if (FirstTypeface.Items.Count > 0)
            {
                var style = new TypefaceStyle(FFont.FontStyle, FFont.FontStretch, FFont.FontWeight);

                int index = 0;
                for (index = 0; index < FirstTypeface.Items.Count; ++index)
                {
                    TextBlock item = FirstTypeface.Items[index] as TextBlock;

                    if (style.Equals((TypefaceStyle)item.Tag))
                    {
                        break;
                    }
                }

                if (index == FirstTypeface.Items.Count)
                    index = 0;

                FirstTypeface.SelectedIndex = index;
                txtTypeface.Text = ((TextBlock)FirstTypeface.SelectedItem).Text;
                FirstTypeface.ScrollIntoView(FirstTypeface.Items[index]);
            }
        }

        //---------------------------------------------------------------------------------------------
        // 昇順ソートのためのコールバック関数
        private int SortComparison(FontFamily item1, FontFamily item2)
        {
            string s1 = item1.ToString();
            string s2 = item2.ToString();

            return s1.CompareTo(s2);
        }

        //---------------------------------------------------------------------------------------------
        // txtSampleText のフォントを変更する
        private void UpdateSampleText()
        {
            this.SetPropertyToTargetObject(txtSample);
        }

        //---------------------------------------------------------------------------------------------
        // cmbLanguage の項目データを設定する
        // このメソッドは、Window_Loadede で一度だけ呼び出される
        private void SetLanguageList()
        {
            ComboBoxItem item = new ComboBoxItem();
            item.Content = "日本語（ja-jp）";
            XmlLanguage language = XmlLanguage.GetLanguage("ja-jp");
            item.Tag = language;
            cmbLanguage.Items.Add(item);

            item = new ComboBoxItem();
            item.Content = "すべての言語";
            item.Tag = null;
            cmbLanguage.Items.Add(item);

            string s = FFont.FontLanguage.ToLower();

            // 現在の FLanguage に一致する項目を選択状態にする
            if (s == "ja-jp")
                cmbLanguage.SelectedIndex = 0;
            else
                cmbLanguage.SelectedIndex = 1;
        }

        //---------------------------------------------------------------------------------------------
        // フォントのサイズに基づいて FirstFontSize の項目を選択する
        // このメソッドは、Window_Loadede で一度だけ呼び出される
        private void SetFontSizeList()
        {
            int index = 0;
            FirstFontSize.Items.Clear();

            for (int i = 0; i < FFontSizeArray.Length; i++)
            {
                FirstFontSize.Items.Add(FFontSizeArray[i]);
            }

            // 現在のサイズを選択状態にする
            for (int i = 0; i < FFontSizeArray.Length; ++i)
            {
                if (FFontSizeArray[i] == FFont.FontSize)
                {
                    index = i;
                    break;
                }
            }

            // FFontSize に一致する項目を選択状態にする
            FirstFontSize.SelectedIndex = index;
            FirstFontSize.ScrollIntoView(FirstFontSize.SelectedItem);
        }

        //---------------------------------------------------------------------------------------------
        // DlgLanguage プロパティの設定をチェックする
        private bool CheckLanguage(string s)
        {
            s = s.ToLower();

            // null または String.Empty のとき、すべての言語とみなす
            if ((s == "ja-jp"))
                return true;
            else
                return false;
        }

        //---------------------------------------------------------------------------------------------
        // Symbol かどうかを取得する
        private bool IsSymbolFont(FontFamily fontFamily)
        {
            foreach (Typeface typeface in fontFamily.GetTypefaces())
            {
                GlyphTypeface face;

                if (typeface.TryGetGlyphTypeface(out face))
                {
                    return face.Symbol;
                }
            }

            return false;
        }

    } // end of FontDialogEx class

    //*********************************************************************************************
    // TypefaceStyle class （FirstTypeface の Tag プロパティに設定するためのクラス）
    //*********************************************************************************************
    public class TypefaceStyle
    {
        private FontStyle FFontStyle;
        private FontStretch FFontStretch;
        private FontWeight FFontWeight;

        public FontStyle FontStyle { get { return FFontStyle; } set { FFontStyle = value; } }
        public FontStretch FontStretch { get { return FFontStretch; } set { FFontStretch = value; } }
        public FontWeight FontWeight { get { return FFontWeight; } set { FFontWeight = value; } }

        //---------------------------------------------------------------------------------------------
        public TypefaceStyle(FontStyle style, FontStretch stretch, FontWeight weight)
        {
            FFontStyle = style;
            FFontStretch = stretch;
            FFontWeight = weight;
        }

        //---------------------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            TypefaceStyle style = obj as TypefaceStyle;
            bool check = false;

            if (FFontStyle.Equals(style.FontStyle) && (FFontStretch.Equals(style.FontStretch)) && (FFontWeight.Equals(style.FontWeight)))
                check = true;

            return check;
        }

        //-------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", FFontStyle, FFontWeight, FFontStretch);
        }

        //---------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    //*********************************************************************************************
    // ToolFont class （テキストツール用フォントクラス）
    //*********************************************************************************************
    public class ToolFont
    {
        private FontFamily FFontFamily;
        private FontStyle FFontStyle;
        private FontWeight FFontWeight;
        private FontStretch FFontStretch;
        private double FFontSize;
        private string FFontLanguage;

        public FontFamily FontFamily { get { return FFontFamily; } set { FFontFamily = value; } }
        public FontStyle FontStyle { get { return FFontStyle; } set { FFontStyle = value; } }
        public FontWeight FontWeight { get { return FFontWeight; } set { FFontWeight = value; } }
        public FontStretch FontStretch { get { return FFontStretch; } set { FFontStretch = value; } }
        public double FontSize { get { return FFontSize; } set { FFontSize = value; } }
        public string FontLanguage { get { return FFontLanguage; } set { FFontLanguage = value; } }

        //-------------------------------------------------------------------------------------------
        public ToolFont()
        {
            FFontFamily = new FontFamily("メイリオ");
            FFontStyle = FontStyles.Normal;
            FFontWeight = FontWeights.Normal;
            FFontStretch = FontStretches.Normal;
            FFontSize = 13.0;
            FFontLanguage = "ja-jp";
        }

        //-------------------------------------------------------------------------------------------
        public ToolFont Clone(ToolFont font)
        {
            var clonedFont = new ToolFont();
            clonedFont.FontFamily = font.FontFamily;
            clonedFont.FontStyle = font.FontStyle;
            clonedFont.FontWeight = font.FontWeight;
            clonedFont.FontStretch = font.FontStretch;
            clonedFont.FontSize = font.FontSize;
            clonedFont.FontLanguage = font.FontLanguage;

            return clonedFont;
        }

        //-------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}", FFontFamily, FFontSize, FFontWeight, FFontStyle, FFontStretch, FFontLanguage);
        }
    }
}
