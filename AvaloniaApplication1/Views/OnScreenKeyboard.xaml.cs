using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Views
{
    public class OnScreenKeyboard : UserControl
    {
        /// <summary>
        /// Get or set the collection of keys 
        /// </summary>
        private List<Button> keyCollection = new List<Button>();

        public OnScreenKeyboard()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            // add all keys to internal collection
            AddAllKeysToInternalCollection();
            // install clicks
            InstallAllClickEventsForCollection(keyCollection);

        }
        /// <summary>
        /// Add all keys to internal collection
        /// </summary>
        private void AddAllKeysToInternalCollection()
        {
            Grid grid = this.FindControl<Grid>("allKeysGrid");
            // itterate all panels
            foreach (Panel panelElement in grid.Children)
            {
                // itterate all buttons
                foreach (Button buttonElement in panelElement.Children)
                {
                    // add to list
                    keyCollection.Add(buttonElement);
                }
            }
        }

        /// <summary>
        /// Install click events for all keys in a collection
        /// </summary>
        /// <param name="keysToInstall"></param>
        private void InstallAllClickEventsForCollection(List<Button> keysToInstall)
        {
            // itterate all
            foreach (Button buttonElement in keysToInstall)
            {
                buttonElement.Focusable = false;
                // install click event
                buttonElement.Click += ButtonElement_Click;
            }
        }

        /// <summary>
        /// Switch the character case of all buttons in a given list
        /// </summary>
        /// <param name="keysToSwitch"></param>
        private void SwitchCase(List<Button> keysToSwitch)
        {
            // itterate all
            foreach (Button buttonElement in keysToSwitch)
            {
                // if key is single char
                if (buttonElement.Content.ToString().Length == 1)
                {
                    // switch char
                    buttonElement.Content = this.SwitchCase(buttonElement.Content.ToString());
                    // switch command parameter
                    buttonElement.CommandParameter = this.SwitchCase(buttonElement.CommandParameter.ToString());
                }
            }

            // set abc button size
            Button shiftButton = this.FindControl<Button>("shiftButton");
            shiftButton.Content = SwitchCase(shiftButton.Content.ToString());
        }

        /// <summary>
        /// Switch the case of an input string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private String SwitchCase(String inputString)
        {
            // if a string
            if (!String.IsNullOrEmpty(inputString))
            {
                // the string to return
                String returnString = "";

                // switch each char of string
                foreach (Char currentChar in inputString)
                {
                    // if a capital
                    if ((currentChar >= 65) && (currentChar <= 90))
                    {
                        // set to lower
                        returnString += currentChar.ToString().ToLower();
                    }
                    // else if lower
                    else if ((currentChar >= 97) && (currentChar <= 122))
                    {
                        // set to upper
                        returnString += currentChar.ToString().ToUpper();
                    }
                    else
                    {
                        switch (currentChar)
                        {
                            case 'ü':
                            case 'ö':
                            case 'ä':
                                returnString += currentChar.ToString().ToUpper();
                                break;
                            case 'Ü':
                            case 'Ö':
                            case 'Ä':
                                returnString += currentChar.ToString().ToLower();
                                break;
                            default:
                                // add as it is
                                returnString += currentChar.ToString();
                                break;
                        }
                        
                    }
                }

                // return string
                return returnString;
            }
            else
            {
                // return black string
                return "";
            }
        }

        void ButtonElement_Click(object sender, RoutedEventArgs e)
        {
            // create variable for holding string
            String sendString = "";

            try
            {
                // stop all event handling
                e.Handled = true;

                // set sendstring to key
                sendString = ((Button)sender).CommandParameter.ToString();

                // if something to send
                if (!String.IsNullOrEmpty(sendString))
                {
                    if (sendString.Length == 1)
                    {
                        if (Application.Current.FocusManager.Current is TextBox textBox)
                        {
                            if (textBox.IsEnabled && !textBox.IsReadOnly)
                            {
                                string bevor = textBox.Text.Substring(0, textBox.SelectionStart);
                                string finish = textBox.Text.Substring(textBox.SelectionEnd);
                                textBox.Text = bevor + sendString + finish;
                                textBox.SelectionStart = textBox.SelectionStart + 1;
                                textBox.SelectionEnd = textBox.SelectionStart;

                                if (((sendString[0] >= 65) && (sendString[0] <= 90)) || (sendString[0] == 'Ä') || (sendString[0] == 'Ö') || (sendString[0] == 'Ü'))
                                {
                                    SwitchCase(keyCollection);
                                }
                            }
                        }
                    }

                    // if sending a string
                    else if (sendString.Length > 1)
                    {
                        switch (sendString)
                        {
                            case "SHIFT":
                                SwitchCase(keyCollection);
                                break;
                            case "BACKSPACE":
                                if (Application.Current.FocusManager.Current is TextBox textBox)
                                {
                                    if (textBox.IsEnabled && !textBox.IsReadOnly && textBox.Text.Length > 0 && textBox.SelectionEnd > 0)
                                    {
                                        if (textBox.SelectionEnd == textBox.SelectionStart)
                                        {
                                            textBox.SelectionStart--;
                                        }
                                        string bevor = textBox.Text.Substring(0, textBox.SelectionStart);
                                        string finish = textBox.Text.Substring(textBox.SelectionEnd);
                                        textBox.Text = bevor + finish;
                                        textBox.SelectionStart = textBox.SelectionStart;
                                        textBox.SelectionEnd = textBox.SelectionStart;
                                    }
                                }
                                break;
                            case "LEFT":
                                if (Application.Current.FocusManager.Current is TextBox textBox1)
                                {
                                    if (textBox1.IsEnabled && textBox1.Text.Length > 0)
                                    {
                                        if (textBox1.SelectionStart == textBox1.SelectionEnd && textBox1.SelectionEnd > 0)
                                        {
                                            textBox1.SelectionStart--;
                                        }
                                        else if (textBox1.SelectionEnd < textBox1.SelectionStart)
                                        {
                                            textBox1.SelectionStart = textBox1.SelectionEnd;
                                        }
                                        textBox1.SelectionEnd = textBox1.SelectionStart;
                                    }
                                }
                                break;
                            case "RIGHT":
                                if (Application.Current.FocusManager.Current is TextBox textBox2)
                                {
                                    if (textBox2.IsEnabled && textBox2.Text.Length > 0)
                                    {
                                        if (textBox2.SelectionStart == textBox2.SelectionEnd && textBox2.SelectionStart < textBox2.Text.Length)
                                        {
                                            textBox2.SelectionEnd++;
                                        }
                                        else if (textBox2.SelectionStart > textBox2.SelectionEnd)
                                        {
                                            textBox2.SelectionEnd = textBox2.SelectionStart;
                                        }
                                        textBox2.SelectionStart = textBox2.SelectionEnd;
                                    }
                                }
                                break;
                            case "ENTER":
                                if (Application.Current.FocusManager.Current is TextBox textBox3)
                                {
                                    if (textBox3.IsEnabled && !textBox3.IsReadOnly && textBox3.AcceptsReturn)
                                    {
                                        string bevor = textBox3.Text.Substring(0, textBox3.SelectionStart);
                                        string finish = textBox3.Text.Substring(textBox3.SelectionEnd);
                                        textBox3.Text = bevor + '\n' + finish;
                                        textBox3.SelectionStart = textBox3.SelectionStart + 1;
                                        textBox3.SelectionEnd = textBox3.SelectionStart;
                                    }
                                    else if (textBox3.IsEnabled && !textBox3.IsReadOnly)
                                    {
                                        var next = KeyboardNavigationHandler.GetNext(textBox3, NavigationDirection.Next);
                                        next.Focus();
                                    }
                                }
                                break;
                        }
                        sendString = "{" + sendString + "}";
                    }

                    // if a focusable element has been specified
                    System.Console.WriteLine(sendString);
                }
            }
            catch (Exception)
            {
                // do nothing - not important for now
                Console.WriteLine("Could not send key press: {0}", sendString);
            }
        }
    }
}
