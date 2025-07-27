using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BronzeFactoryApplication.Helpers.Behaviours
{
    public class TextBoxNumericInputBehavior : Behavior<TextBox>
    {
        const NumberStyles validNumberStyles = NumberStyles.AllowDecimalPoint;
        public TextBoxNumericInputBehavior()
        {
            this.JustPositivDecimalInput = false;
        }

        public TextBoxInputMode InputMode
        {
            get { return (TextBoxInputMode)GetValue(InputModeProperty); }
            set { SetValue(InputModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputModeProperty =
            DependencyProperty.Register("InputMode", typeof(TextBoxInputMode), typeof(TextBoxNumericInputBehavior), new PropertyMetadata(TextBoxInputMode.None));



        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register("Precision", typeof(int),
            typeof(TextBoxNumericInputBehavior), new FrameworkPropertyMetadata(0));

        /// <summary>
        /// How Many Digits to Allow after decimal seperator
        /// </summary>
        public int Precision
        {
            get { return (int)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        public static readonly DependencyProperty JustPositivDecimalInputProperty =
         DependencyProperty.Register("JustPositivDecimalInput", typeof(bool),
         typeof(TextBoxNumericInputBehavior), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Wheather this TextBox should Allow only Positive Input
        /// </summary>
        public bool JustPositivDecimalInput
        {
            get { return (bool)GetValue(JustPositivDecimalInputProperty); }
            set { SetValue(JustPositivDecimalInputProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;

            DataObject.AddPastingHandler(AssociatedObject, Pasting);

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;

            DataObject.RemovePastingHandler(AssociatedObject, Pasting);
        }

        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = (string)e.DataObject.GetData(typeof(string));

                if (!this.IsValidInput(this.GetText(pastedText)))
                {
                    System.Media.SystemSounds.Beep.Play();
                    e.CancelCommand();
                }
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                e.CancelCommand();
            }
        }

        private void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!this.IsValidInput(this.GetText(" ")))
                {
                    System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
                }
            }
        }

        private void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!this.IsValidInput(this.GetText(e.Text)))
            {
                System.Media.SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }

        private string GetText(string input)
        {
            var txt = this.AssociatedObject;

            int selectionStart = txt.SelectionStart;
            if (txt.Text.Length < selectionStart)
                selectionStart = txt.Text.Length;

            int selectionLength = txt.SelectionLength;
            if (txt.Text.Length < selectionStart + selectionLength)
                selectionLength = txt.Text.Length - selectionStart;

            var realtext = txt.Text.Remove(selectionStart, selectionLength);

            int caretIndex = txt.CaretIndex;
            if (realtext.Length < caretIndex)
                caretIndex = realtext.Length;

            var newtext = realtext.Insert(caretIndex, input);

            return newtext;
        }

        private bool IsValidInput(string input)
        {
            switch (InputMode)
            {
                case TextBoxInputMode.None:
                    return true;
                case TextBoxInputMode.DigitInput:
                    if (input.Contains('-'))
                    {
                        if (this.JustPositivDecimalInput)
                            return false;

                        if (input.IndexOf("-", StringComparison.Ordinal) > 0)
                            return false;

                        if (input.ToCharArray().Count(x => x == '-') > 1)
                            return false;

                        if (input.Length == 1)
                            return true;
                        //Remove The Dash if there to check if rest is digit
                        input = input.Remove(0);
                    }
                    return CheckIsDigit(input);
                case TextBoxInputMode.DecimalInput:
                    decimal d;

                    if (input.Contains('-'))
                    {
                        if (this.JustPositivDecimalInput)
                            return false;


                        if (input.IndexOf("-", StringComparison.Ordinal) > 0)
                            return false;

                        if (input.ToCharArray().Count(x => x == '-') > 1)
                            return false;

                        if (input.Length == 1)
                            return true;
                    }

                    //If there is a decimal seperator and the it has more digits than the precision (+1 for the dot) do not let it 
                    if (input.Contains('.') && input.Substring(input.LastIndexOf('.')).Length > (Precision + 1))
                    {
                        return false;
                    }

                    //Invariant Culture (Only with dot as decimal seperator)
                    var result = decimal.TryParse(input, validNumberStyles, CultureInfo.InvariantCulture, out d);
                    return result;

                default: throw new ArgumentException("Unknown TextBoxInputMode");
            }
        }

        private bool CheckIsDigit(string text)
        {
            return text.ToCharArray().All(char.IsDigit);
        }
    }

    public enum TextBoxInputMode
    {
        None,
        DecimalInput,
        DigitInput
    }
}
