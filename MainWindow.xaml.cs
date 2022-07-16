﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wordle_Aid.RECaracter;

namespace Wordle_Aid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            CreateWordList();
            CreateButtons(Button_Click);
            this.KeyDown += MainWindow_KeyDown;
        }

        #endregion

        #region Non-Public Properties

        private int NextAvailableButton { get; set; } = 0;

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Takes the word in the input box and moves the letter to the next available row.
        /// </summary>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Equals("Return"))
            {
                // Make sure the word is 5 letters long
                string Inputword = inputbox.Text;
                if (Inputword.Length != 5)
                {
                    ErrorMessage.Text = "WORD MUST BE 5 LETTERS LONG";
                    return;
                }
                // Check if there is an availble row in the button array
                if (NextAvailableButton == 25)
                {
                    ErrorMessage.Text = "NO MORE SPACE";
                    return;
                }
                // Clear error message box
                ErrorMessage.Text = "";
                // Goes through each letter in the input word and adds it
                // to the next available button
                foreach (char letter in inputbox.Text)
                {

                    ButtonList[NextAvailableButton].Content = letter.ToString().ToUpper();
                    UsedButtonList.Add(ButtonList[NextAvailableButton]);
                    NextAvailableButton++;
                }
                inputbox.Text = "";
            }
        }

        /// <summary>
        /// Toggles the color of the pressed button in the array of boxes.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clicked_button = (Button)sender;
            if (clicked_button.Background == Brushes.Yellow)
                clicked_button.Background = Brushes.LightGreen;
            else if (clicked_button.Background == Brushes.LightGreen)
                clicked_button.Background = Brushes.LightGray;
            else
                clicked_button.Background = Brushes.Yellow;
        }

        /// <summary>
        /// Clears the text in the inputbox.
        /// </summary>
        private void InputboxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            inputbox.Text = "";          
        }

        /// <summary>
        /// Resets everything back to original state.
        /// </summary>
        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            WordList.Items.Clear();
            NextAvailableButton = 0;
            foreach (Button b in ButtonList)
            {
                b.Content = "";
                b.Background = Brushes.LightGray;
            }
            UsedButtonList.Clear();
        }

        /// <summary>
        /// Gets the regular expression and searches for matches.
        /// </summary>
        private void GenerateButtonClick(object sender, RoutedEventArgs e)
        {
            // Clears current list of words
            WordList.Items.Clear();
            // Calls for creation of the Regular Expression
            string regExpression = CreateRegExpression();
            Regex re = new(regExpression);

            // Begin the search for compatible words
            foreach (string lowerCaseWord in _wordList)
            {
                bool missing = false;
                string word = lowerCaseWord.ToUpper();
                if (re.IsMatch(word))
                {
                    // Check if the word contains all of the
                    // required letters
                    foreach (string letter in MustContainList) 
                    {
                        if (!word.Contains(letter))
                        {
                            missing = true;
                            break;
                        }                           
                    }
                    if (!missing) WordList.Items.Add(word);
                    
                }
                    
            }
            MustContainList.Clear();
        }

        /// <summary>
        /// Reads text file of 5 letter words into a list. 
        /// </summary>
        private void CreateWordList()
        {
            string[] lines = File.ReadAllLines(@"..\..\..\Dictionary\WordDictionary.txt");
            foreach (string line in lines)
            {
                _wordList.Add(line);
            }
        }

        private string CreateRegExpression()
        {
            // Creates classes cooresponding to each letter in our RE
            string _regExpression = "";          
            List<RECharacter> RegExpressionList = new List<RECharacter>();
            for (int i = 0; i < 5; i++)
            {
                RECharacter character = new RECharacter();
                RegExpressionList.Add(character);
            }
            // Goes through each inputed charcter and edits the RE
            // classes based on the color of the button
            int index = 0;
            foreach (Button b in UsedButtonList)
            {
                // Green means correct letter and correct spot
                if (b.Background == Brushes.LightGreen)
                {
                    RegExpressionList[index].AddCorrectAnswer(b.Content.ToString());
                }
                // Yellow means correct letter and incorrect spot
                else if (b.Background == Brushes.Yellow)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (i == index) RegExpressionList[index].RemoveCharacter(b.Content.ToString());
                    }
                    MustContainList.Add(b.Content.ToString());
                }
                // Gray means the letter is nowhere in our target word
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        RegExpressionList[i].RemoveCharacter(b.Content.ToString());
                    }
                }

                if (index == 4) index = 0;
                else index++;
            }
            // Puts the RE together from the RECharacter classes results
            foreach (RECharacter character in RegExpressionList)
            {
                _regExpression += character.FinalResult;
            }
            return _regExpression;
        }

        /// <summary>
        /// Called during construction creates a 5x5 array of boxes.
        /// </summary>
        private void CreateButtons(RoutedEventHandler Button_Click)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Button b = new Button
                    {
                        Name = $"UxButton{j}{i}",
                        Background = Brushes.LightGray,
                        Margin = new Thickness(2),
                        FontSize = 25,
                        FontFamily = new FontFamily("Clear Sans"),
                        FontWeight = FontWeights.Bold,
                    };
                    b.Click += Button_Click;
                    MyGrid.Children.Add(b);
                    Grid.SetColumn(b, j + 1);
                    Grid.SetRow(b, i);
                    ButtonList.Add(b);
                }
            }
        }

        #endregion

        #region Fields

        private List<string> _wordList = new List<string>();
        private List<Button> ButtonList = new List<Button>();
        private List<Button> UsedButtonList = new List<Button>();
        private List<string> MustContainList  = new List<string>();

        #endregion

    }
}
