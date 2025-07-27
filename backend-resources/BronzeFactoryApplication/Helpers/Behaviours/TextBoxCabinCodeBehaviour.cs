using FluentValidation;
using HandyControl.Controls;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BronzeFactoryApplication.Helpers.Behaviours;

public class TextBoxCabinCodeBehaviour : Behavior<TextBox>
{
    private readonly TypingCabinCodeValidator validator2 = new();
    private readonly ValidatorCabinCode validator = new(true);


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
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            this.AssociatedObject.Text = "";
            this.AssociatedObject.CaretIndex = 0;
        }
    }

    private void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        //Add a Dash before the 5th typed charachter
        if ((this.AssociatedObject.Text.Length is 4 || this.AssociatedObject.Text.Length is 7)
            && this.AssociatedObject.SelectionLength <= 1 //If more text is selected do not replace with dash
            && e.Text != "-")  // Ensure typed is not already a dash
            
        {
            int index = this.AssociatedObject.Text.Length; // index of where to insert the dash '-'
            this.AssociatedObject.Text = this.AssociatedObject.Text.Insert(index, "-");
            this.AssociatedObject.CaretIndex = index + 1;
        }
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
        return validator.Validate(input).IsValid;
    }
}

public class TypingCabinCodeValidator : AbstractValidator<string>
{
    public TypingCabinCodeValidator()
    {
        
    }
}
