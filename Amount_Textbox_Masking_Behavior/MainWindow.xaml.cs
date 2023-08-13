using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Amount_Textbox_Masking_Behavior
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private string _amount;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Amount
        {
            get { return _amount; }
            set { _amount = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Amount = "0.00";
        }

        private void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var TxtBox = sender as TextBox;
            if (TxtBox != null)
            {
                string newdigit = e.Text;
                string text = TxtBox.Text.Trim().Replace(" ","");
                int pointIndex = text.IndexOf(".");
                int currentCaretIndex = TxtBox.CaretIndex;
                if (newdigit.Contains("."))
                {
                    if (pointIndex >= 0)
                    {
                        TxtBox.CaretIndex = text.Length;
                        e.Handled = true;
                        return;
                    }
                    else
                    {
                        e.Handled = false;
                        return;
                    }
                }
                if (!newdigit.Where(x => char.IsDigit(x)).Any() )
                {
                    e.Handled = true;
                    return;
                }
                //handle if so how textbox text is empty 
                if(string.IsNullOrEmpty(text))
                {
                    string newAmount = e.Text + ".00";
                    TxtBox.Text = newAmount;
                    e.Handled = true;
                    return;

                }
                if (currentCaretIndex > pointIndex)
                {
                    string[] newText = text.Split(".");
                    string newAmount = newText[0] + "." + newText[1].Remove(0,1) + e.Text;
                    TxtBox.Text = Convert.ToDecimal(newAmount).ToString("N2");
                    //set caretindex at end of amount 
                    TxtBox.CaretIndex = TxtBox.Text.Length;
                    e.Handled = true;
                    return;

                }

                if (currentCaretIndex <= pointIndex)
                {
                    if(TxtBox.Text.Replace(",", "").Length >= 15)
                    {
                        e.Handled = true;
                        return;
                    }
                    string[] newText = text.Split(".");
                    //remove 0 on first place
                    if (newText[0].IndexOf("0") == 0)
                        newText[0] = newText[0].Remove(0, 1);
                    if (newText[0].Length == 0)
                        TxtBox.CaretIndex = 0;
                    string newAmount = newText[0].Insert(TxtBox.CaretIndex,e.Text)  + "." + newText[1] ;
                    TxtBox.Text = Convert.ToDecimal(newAmount).ToString("N2");
                    //set caretindex before point
                    TxtBox.CaretIndex = TxtBox.Text.IndexOf(".") ;
                    e.Handled = true;
                    return;

                }

                TxtBox.Text = Convert.ToDecimal(TxtBox.Text).ToString("N2");
                TxtBox.CaretIndex = TxtBox.Text.IndexOf(".");
                

            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                var TxtBox = sender as TextBox;
                if (TxtBox != null)
                {
                    string text = TxtBox.Text.Trim().Replace(" ", "");
                    int pointIndex = text.IndexOf(".");
                    if (pointIndex <= 1 )
                    {
                        TxtBox.Text = "0" + text.Remove(0,1);
                        e.Handled = true;
                        return;
                    }
                }
            }
        }
    }
}
