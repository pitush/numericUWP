using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace numericUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag != null)
            {
                ((SharedClasses.ClsNumTextTagIn)tb.Tag).Refresh(tb);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag != null)
            {
                ((SharedClasses.ClsNumTextTagIn)tb.Tag).Refresh(tb);
            }
        }

        private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag != null)
            {
                ((SharedClasses.ClsNumTextTagIn)tb.Tag).NumericText(tb);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //System.Globalization.CultureInfo current = System.Globalization.CultureInfo.CurrentUICulture;

            System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo((string)((Windows.UI.Xaml.Controls.Button)sender).Content);
        }
    }
}
