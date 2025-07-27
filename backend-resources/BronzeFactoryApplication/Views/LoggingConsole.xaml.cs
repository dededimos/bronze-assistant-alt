using BronzeFactoryApplication.ViewModels;
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

namespace BronzeFactoryApplication.Views
{
    /// <summary>
    /// Interaction logic for LoggingConsole.xaml
    /// </summary>
    public partial class LoggingConsole
    {
        private const int MAX_RICHTEXTBOX_CHARS = 1500000;
        private readonly RichTextBox richTextBox;
        
        /// <summary>
        /// Represents the Text of the TextBox
        /// </summary>
        public TextRange? RichTextBoxTextRange 
        { 
            get
            {
                if (richTextBox is not null)
                {
                    TextPointer start = richTextBox.Document.ContentStart; //A TextPointer pointing to the Start of the Doc.
                    TextPointer end = richTextBox.Document.ContentEnd;     //A TextPointer pointin to the End of the Doc.
                    return new TextRange(start, end);
                }
                else
                {
                    return null;
                }
                
            } 
        }


        public LoggingConsole(RichTextBox richTextBox, ConsoleViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            this.richTextBox = richTextBox;
            this.richTextBox.Background = new SolidColorBrush(Color.FromArgb(240,18,18,18)); //#121212 Color (Blackish with 5% opacity --240--)
            this.richTextBox.IsReadOnly = true;
            this.richTextBox.TextChanged += RichTextBox_TextChanged;
            
            RichTextBoxScrollViewer.Content = this.richTextBox; //Assign the RichTextBox to the ScrollViewer
            GeometryCollection col = new();
            GeometryGroup colgr = new();
            
        }

        /// <summary>
        /// Scroll the TextBox to the End and Clear when Max Chars is Passed to avoid any overflows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RichTextBoxTextRange is not null && RichTextBoxTextRange.Text.Length > MAX_RICHTEXTBOX_CHARS)
            {
                RichTextBoxTextRange.Text = "";
            }
            // Only this Works . The other two do not 
            RichTextBoxScrollViewer.ScrollToEnd();

            //richTextBox.ScrollToEnd();
            //richTextBox.CaretPosition = richTextBox.Document.ContentEnd;
        }



        /// <summary>
        /// Never Truly Close this Window - It leaves as a Singleton for the Whole Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void ClearTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBoxTextRange is not null)
            {
                RichTextBoxTextRange.Text = "";
            }
        }
    }
}
