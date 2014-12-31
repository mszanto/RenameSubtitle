using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace RenameSubTitle
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DialogBox : Window
    {          
        public DialogBox()
        {
            InitializeComponent();
        }

        public DialogBox(string text)
        {
            InitializeComponent();
            tb1.Text = text;
        }        

        private void bt1_Click(object sender, RoutedEventArgs e)
        {               
            this.Close();
        }        
    }
}
