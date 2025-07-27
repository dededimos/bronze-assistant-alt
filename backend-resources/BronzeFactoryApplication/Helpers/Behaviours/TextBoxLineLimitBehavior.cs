using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xaml.Behaviors;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BronzeFactoryApplication.Helpers.Behaviours
{
    /// <summary>
    /// A behavior for <see cref="TextBox"/> that limits the number of characters per line,
    /// transferring excess input to a new line when the limit is reached.
    /// </summary>
    public class TextBoxLineLimitBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Dependency property for the maximum number of characters allowed per line.
        /// </summary>
        public static readonly DependencyProperty MaxCharsPerLineProperty =
            DependencyProperty.Register(
                nameof(MaxCharsPerLine),           // Property name
                typeof(int),                       // Property type
                typeof(TextBoxLineLimitBehavior),  // Owner type
                new PropertyMetadata(21));         // Default value of 21

        /// <summary>
        /// Gets or sets the maximum number of characters allowed per line in the TextBox.
        /// </summary>
        public int MaxCharsPerLine
        {
            get => (int)GetValue(MaxCharsPerLineProperty);
            set => SetValue(MaxCharsPerLineProperty, value);
        }

        /// <summary>
        /// Attaches the behavior to the associated <see cref="TextBox"/> and subscribes to input events.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            // Subscribe to input events to monitor and control text entry
            AssociatedObject.PreviewTextInput += TextBox_PreviewTextInput; // Handles typed text
            AssociatedObject.KeyDown += TextBox_KeyDown;                   // Handles key presses like Space
            DataObject.AddPastingHandler(AssociatedObject, TextBox_Pasting); // Handles paste operations
        }

        /// <summary>
        /// Detaches the behavior from the associated <see cref="TextBox"/> and unsubscribes from events.
        /// </summary>
        protected override void OnDetaching()
        {
            // Unsubscribe from events to prevent memory leaks
            AssociatedObject.PreviewTextInput -= TextBox_PreviewTextInput;
            AssociatedObject.KeyDown -= TextBox_KeyDown;
            DataObject.RemovePastingHandler(AssociatedObject, TextBox_Pasting);
            base.OnDetaching();
        }

        /// <summary>
        /// Handles text composition events, transferring excess input to a new line when the limit is reached.
        /// </summary>
        /// <param name="sender">The object raising the event, expected to be a <see cref="TextBox"/>.</param>
        /// <param name="e">Event data containing the composed text input.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox) // Ensure the sender is a TextBox
            {
                string currentLine = GetCurrentLine(textBox); // Get the line at the caret
                int caretIndex = textBox.CaretIndex;          // Current cursor position

                // If the current line is at or exceeds the limit
                if (currentLine.Length >= MaxCharsPerLine)
                {
                    // Insert a newline and the typed text at the caret position
                    string newText = InsertTextAtCaret(textBox.Text, caretIndex, Environment.NewLine + e.Text);
                    textBox.Text = newText; // Update the TextBox content
                                            // Move the caret to the end of the newly added text
                    textBox.CaretIndex = caretIndex + Environment.NewLine.Length + e.Text.Length;
                    e.Handled = true; // Mark the event as handled to prevent default text insertion
                }
            }
        }

        /// <summary>
        /// Handles key press events, transferring specific key inputs (e.g., Space) to a new line when the limit is reached.
        /// </summary>
        /// <param name="sender">The object raising the event, expected to be a <see cref="TextBox"/>.</param>
        /// <param name="e">Event data containing the key pressed.</param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox) // Ensure the sender is a TextBox
            {
                string currentLine = GetCurrentLine(textBox); // Get the line at the caret
                int caretIndex = textBox.CaretIndex;          // Current cursor position

                // Allow Enter key for manual line breaks
                if (e.Key == Key.Enter)
                {
                    e.Handled = false; // Let the Enter key proceed normally
                    return;
                }

                // If the line is at or exceeds the limit and the key isn’t a control key
                if (currentLine.Length >= MaxCharsPerLine && !IsControlKey(e.Key))
                {
                    // Convert the key to its text representation (e.g., Space -> " ")
                    string keyText = KeyToText(e.Key);
                    if (!string.IsNullOrEmpty(keyText)) // If the key produces printable text
                    {
                        // Insert a newline and the key’s text at the caret
                        string newText = InsertTextAtCaret(textBox.Text, caretIndex, Environment.NewLine + keyText);
                        textBox.Text = newText; // Update the TextBox content
                                                // Move the caret to the end of the newly added text
                        textBox.CaretIndex = caretIndex + Environment.NewLine.Length + keyText.Length;
                        e.Handled = true; // Prevent the key from being processed further
                    }
                }
            }
        }

        /// <summary>
        /// Handles paste operations, applying the line limit to pasted text and splitting it across lines as needed.
        /// </summary>
        /// <param name="sender">The object raising the event, expected to be a <see cref="TextBox"/>.</param>
        /// <param name="e">Event data containing the pasted data.</param>
        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is TextBox textBox && e.DataObject.GetDataPresent(typeof(string))) // Check for text data
            {
                string pastedText = (string)e.DataObject.GetData(typeof(string)); // Get the pasted text
                                                                                  // Apply the line limit to the combined text
                string newText = ApplyLineLimit(textBox.Text, textBox.CaretIndex, pastedText);
                int newCaretIndex = newText.Length; // Place caret at the end of the pasted content

                textBox.Text = newText;         // Update the TextBox content
                textBox.CaretIndex = newCaretIndex; // Set the new caret position
                e.CancelCommand();              // Cancel the default paste to use our logic
            }
        }

        /// <summary>
        /// Retrieves the current line of text where the caret is positioned in the <see cref="TextBox"/>.
        /// </summary>
        /// <param name="textBox">The <see cref="TextBox"/> to analyze.</param>
        /// <returns>The text of the current line, or an empty string if no lines exist.</returns>
        private string GetCurrentLine(TextBox textBox)
        {
            int caretIndex = textBox.CaretIndex;    // Current cursor position
            string text = textBox.Text;             // Full text content
            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None); // Split into lines
            int charCount = 0;                      // Track character position

            // Iterate through lines to find the one containing the caret
            for (int i = 0; i < lines.Length; i++)
            {
                charCount += lines[i].Length;       // Add current line’s length
                if (caretIndex <= charCount)        // If caret is within this line
                {
                    return lines[i];                // Return the current line
                }
                charCount += Environment.NewLine.Length; // Add newline length
            }
            // Return the last line if caret is at the end
            return lines.Length > 0 ? lines[^1] : string.Empty;
        }

        /// <summary>
        /// Inserts text at the specified caret position within the original text.
        /// </summary>
        /// <param name="originalText">The original text content.</param>
        /// <param name="caretIndex">The position where text should be inserted.</param>
        /// <param name="textToInsert">The text to insert.</param>
        /// <returns>The updated text with the insertion applied.</returns>
        private string InsertTextAtCaret(string originalText, int caretIndex, string textToInsert)
        {
            // Split the original text at the caret and insert the new text
            return originalText.Substring(0, caretIndex) + textToInsert + originalText.Substring(caretIndex);
        }

        /// <summary>
        /// Applies the line limit to text being inserted, splitting it into lines of <see cref="MaxCharsPerLine"/> or less.
        /// </summary>
        /// <param name="originalText">The original text content of the <see cref="TextBox"/>.</param>
        /// <param name="caretIndex">The position where text is being inserted.</param>
        /// <param name="textToInsert">The text to insert and split.</param>
        /// <returns>The updated text with line limits enforced.</returns>
        private string ApplyLineLimit(string originalText, int caretIndex, string textToInsert)
        {
            string[] lines = originalText.Split(new[] { Environment.NewLine }, StringSplitOptions.None); // Split into lines
            int charCount = 0;  // Track character position
            int lineIndex = 0;  // Track current line index

            // Find the line where the caret is located
            for (; lineIndex < lines.Length; lineIndex++)
            {
                charCount += lines[lineIndex].Length;
                if (caretIndex <= charCount)
                {
                    break;
                }
                charCount += Environment.NewLine.Length;
            }

            // Combine the text to insert with the current line at the caret position
            string combinedText = lines[lineIndex].Substring(0, caretIndex - (charCount - lines[lineIndex].Length)) +
                                  textToInsert +
                                  lines[lineIndex].Substring(caretIndex - (charCount - lines[lineIndex].Length));
            string[] newLines = combinedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None); // Split combined text

            // Rebuild the text, splitting lines that exceed MaxCharsPerLine
            string result = string.Join(Environment.NewLine, lines.Take(lineIndex)); // Lines before insertion
            if (result.Length > 0) result += Environment.NewLine;

            string currentLine = ""; // Buffer for building lines
            foreach (string line in newLines)
            {
                currentLine += line;
                // While the current line exceeds the limit, split it
                while (currentLine.Length > MaxCharsPerLine)
                {
                    result += currentLine.Substring(0, MaxCharsPerLine) + Environment.NewLine;
                    currentLine = currentLine.Substring(MaxCharsPerLine);
                }
            }
            if (currentLine.Length > 0) // Add any remaining text
            {
                result += currentLine;
            }

            // Append any lines after the insertion point
            if (lineIndex + 1 < lines.Length)
            {
                result += Environment.NewLine + string.Join(Environment.NewLine, lines.Skip(lineIndex + 1));
            }

            return result; // Return the final text with line limits applied
        }

        /// <summary>
        /// Determines if a key is a control key that should not trigger text transfer.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is a control key (e.g., Backspace, arrows); otherwise, false.</returns>
        private static bool IsControlKey(Key key)
        {
            // Return true for keys that don’t produce text and should be allowed
            return key == Key.Back || key == Key.Delete || key == Key.Left || key == Key.Right ||
                   key == Key.Up || key == Key.Down || key == Key.Home || key == Key.End;
        }

        /// <summary>
        /// Converts a <see cref="Key"/> to its text representation for printable characters.
        /// </summary>
        /// <param name="key">The key to convert.</param>
        /// <returns>The text representation of the key (e.g., "A", "1", " "), or an empty string if non-printable.</returns>
        private static string KeyToText(Key key)
        {
            // Map common printable keys to their text;
            if (key >= Key.A && key <= Key.Z) return key.ToString();       // Letters A-Z
            if (key >= Key.D0 && key <= Key.D9) return (key - Key.D0).ToString(); // Numbers 0-9
            if (key == Key.Space) return " ";                             // Space key
            return string.Empty;                                          // Non-printable keys
        }
    }

}
