using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            CreatDictionary();
            CreateButtons(Button_Click);
        }

        #endregion

        #region Non-Public Properties

        private int NextAvailableRow
        {
            get { return _nextRow; }
            set { _nextRow = value; }
        }
        private int NextAvailableButton { get; set; } = 0;

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Toggles the color of the pressed button in the array of boxes.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clicked_button = (Button)sender;
            if (clicked_button.Background == Brushes.Orange)
                clicked_button.Background = Brushes.GreenYellow;
            else if (clicked_button.Background == Brushes.GreenYellow)
                clicked_button.Background = Brushes.LightGray;
            else
                clicked_button.Background = Brushes.Orange;
        }

        /// <summary>
        /// Called during construction creates a 5x5 array of boxes.
        /// </summary>
        /// <param name="Button_Click"></param>
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

        /// <summary>
        /// Clears the text in the inputbox.
        /// </summary>
        private void InputboxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            inputbox.Text = "";
        }

        /// <summary>
        /// Takes the word in the input box and moves the letter to the next available row.
        /// </summary>
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            // Make sure the word is 5 letters long
            string Inputword = inputbox.Text;
            if (Inputword.Length != 5)
            {
                MessageBox.Show("Submitted word must be 5 letters long", "Word Length Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check if there is an availble row in the button array
            if (NextAvailableButton == 25)
            {
                MessageBox.Show("No more space", "Word space error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Goes through each letter in the input word and adds it
            // to the next available button
            foreach (char letter in inputbox.Text)
            {
                ButtonList[NextAvailableButton].Content = letter.ToString().ToUpper();
                NextAvailableButton++;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
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

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            WordList.Items.Clear();
            bool Missing = false;
            string regExpression = CreateRegExpression();
            Regex re = new Regex(regExpression);
            foreach (string lowerCaseWord in _wordList)
            {
                string word = lowerCaseWord.ToUpper();
                if (re.IsMatch(word))
                {
                    foreach (string letter in MustContainList) 
                    {
                        if (!word.Contains(letter))
                        {
                            Missing = true;
                            break;
                        }                           
                    }
                    if (!Missing) WordList.Items.Add(word);
                    Missing = false;
                }
                    
            }
            MustContainList.Clear();
        }

        private void CreatDictionary()
        {
            string[] lines = File.ReadAllLines(@"..\..\..\Dictionary\WordDictionary.txt");
            foreach (string line in lines)
            {
                _wordList.Add(line);
            }
        }

        private string CreateRegExpression()
        {
            string _regExpression = "";          
            List<RECharacter> RegExpressionList = new List<RECharacter>();
            for (int i = 0; i < 5; i++)
            {
                RECharacter character = new RECharacter();
                RegExpressionList.Add(character);
            }

            int index = 0;
            foreach (Button b in UsedButtonList)
            {
                if (b.Background == Brushes.GreenYellow)
                {
                    RegExpressionList[index].AddCorrectAnswer(b.Content.ToString());
                }
                else if (b.Background == Brushes.Orange)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (i == index) RegExpressionList[index].RemoveCharacter(b.Content.ToString());
                    }
                    MustContainList.Add(b.Content.ToString());
                }
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
            foreach (RECharacter character in RegExpressionList)
            {
                _regExpression += character.FinalResult();
            }
            return _regExpression;
        }

        #endregion

        #region Fields

        private int _nextRow;

        private List<string> _wordList = new List<string>();

        private List<Button> ButtonList = new List<Button>();

        private List<Button> UsedButtonList = new List<Button>();
        private List<string> MustContainList { get; set; } = new List<string>();

        #endregion

    }
}
