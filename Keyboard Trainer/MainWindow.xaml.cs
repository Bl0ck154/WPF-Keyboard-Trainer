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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Keyboard_Trainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int TASK_ROW_LENGHT = 50;
        Dictionary<string, Button> dictionary;
        bool GameStarted = false;
        DispatcherTimer timer;
        const string symbols = "abcdefghijklmnopqrstuvwxyz1234567890,.!?-=[]\\;'/+_)(*&^%$#@><:{}|";
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            KeyDown += MainWindow_KeyDown;
            KeyUp += MainWindow_KeyUp;
            colorMem = new Dictionary<Button, Brush>();
        }
        Dictionary<Button, Brush> colorMem;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled)
                CapsLockFunc();

            dictionary = new Dictionary<string, Button>();
            foreach (UIElement item in gridField.Children)
            {
                Grid grid = item as Grid;
                if (grid != null)
                {
                    foreach (UIElement control in grid.Children)
                    {
                        Button btn = control as Button;
                        if (btn != null)
                        {
                            string keyEnum = keyToKeyEnum(btn.Content.ToString().ToLower());
                            Button tmp;
                            if (!dictionary.TryGetValue(keyEnum, out tmp))
                                 dictionary.Add(keyEnum, btn);
                        }
                    }
                }
            }
        }
        string keyToKeyEnum(string name)
        {
            string tmp = "";

            if (name[0] >= '0' && name[0] <= '9')
                tmp = "d" + name;
          else  if (name == ";")
                tmp = "oem1";
            else if (name == "'")
                tmp = "oemquotes";
            else if (name == "backspace")
                tmp = "back";
            else if (name == "enter")
                tmp = "return";
            else if (name == "caps lock")
                tmp = "capital";
            else if (name == "[")
                tmp = "oemopenbrackets";
            else if (name == "]")
                tmp = "oem6";
            else if (name == "\\")
                tmp = "oem5";
            else if (name == ",")
                tmp = "oemcomma";
            else if (name == ".")
                tmp = "oemperiod";
            else if (name == "/")
                tmp = "oemquestion";
            else if (name == "`")
                tmp = "oem3";
            else if (name == "-")
                tmp = "oemminus";
            else if (name == "=")
                tmp = "oemplus";
            else
                tmp = name;
            
            return tmp;
        }
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
                return;
            if (e.Key == Key.Escape && !GameStarted)
                Close();
            else if (e.Key != Key.CapsLock)
                MainWindow_KeyDown(sender, e);
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat && e.Key != Key.Back)
                return;
            
            Button keyButton;
            string tmp = e.Key.ToString().ToLower();

            if (tmp.Length > 1)
            {
                if (tmp == "system")
                    tmp = e.SystemKey.ToString().ToLower();
            
                if (tmp.Contains("left"))
                    tmp = tmp.Remove(0, 4);
                else if (tmp.Contains("right"))
                    tmp = tmp.Remove(0, 5);
                else if (tmp[0] == 'l')
                    tmp = tmp.Remove(0, 1);
            }
            dictionary.TryGetValue(tmp, out keyButton);
            if(keyButton != null)
            {
                if (e.IsDown && e.Key != Key.CapsLock) // TODO capslock fix
                {
                    colorMem[keyButton] = keyButton.Background;
                }
                keyButton.Background = e.IsDown ? Brushes.PaleGoldenrod : colorMem[keyButton];

                if (e.IsDown && GameStarted)
                {
                    if (tmp == "back" && textboxAnswer.Text.Length > 0)
                        textboxAnswer.Text = textboxAnswer.Text.Remove(textboxAnswer.Text.Length - 1);
                    else
                    {
                        if (keyButton.Content.ToString().Length == 1)
                            textboxAnswer.Text += keyButton.Content.ToString();
                        else if (tmp == "space")
                            textboxAnswer.Text += " ";

                        if (textboxAnswer.Text.Length > 0)
                        {
                            PaintPartBackgrond(textboxAnswer, 0, textboxAnswer.Text.Length);

                            if (textboxAnswer.Text.Length > textboxTask.Text.Length
                             || textboxAnswer.Text[textboxAnswer.Text.Length - 1] != textboxTask.Text[textboxAnswer.Text.Length - 1])
                            {
                                textFails.Text = (Convert.ToInt32(textFails.Text) + 1).ToString();
                                PaintPartBackgrond(textboxAnswer, 0, textboxAnswer.Text.Length,true);
                            }
                           // else
                            //    PaintPartBackgrond(textboxTask, 0, textboxAnswer.Text.Length); // по непонятной причине верхняя строка закрашивается не всегда
                        }
                    }
                }
            }

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.CapsLock)
            {
                CapsLockFunc();
            }
            else if (e.Key == Key.Enter && e.IsDown)
            {
                if (GameStarted)
                    stopClick();
                else
                    startClick();
            }
        }
        void PaintPartBackgrond(TextBlock tb, int startpos, int endpos, bool wrongSymbColor = false)
        {
            new TextRange(tb.ContentStart.GetPositionAtOffset(startpos+1),
          tb.ContentStart.GetPositionAtOffset(endpos+1)).ApplyPropertyValue(TextElement.BackgroundProperty, wrongSymbColor ? Brushes.Red : Brushes.LightGreen);
        }

        void CapsLockFunc()
        {
            foreach (UIElement item in gridField.Children)
            {
                Grid grid = item as Grid;
                if (grid != null)
                {
                    foreach (UIElement control in grid.Children)
                    {
                        Button btn = control as Button;
                        if (btn != null)
                        {
                            if (btn.Content.ToString().Length == 1)
                            {
                                string tmp = btn.Content.ToString();
                                btn.Content = btn.Tag == null ? "" : btn.Tag.ToString();
                                btn.Tag = tmp;
                            }
                        }
                    }
                }
            }
            if ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled)
            {
                colorMem[Caps] = Caps.Background;
                Caps.Background = Brushes.PaleGoldenrod;
            }
            else
                Caps.Background = colorMem[Caps];
        }
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            startClick();
        }
        void startClick()
        {
            buttonStop.IsEnabled = true;
            buttonStart.IsEnabled = false;
            GameStarted = true;

            textboxTask.Text = "";
            textboxAnswer.Text = "";
            Random rand = new Random();
            bool spacing = true;
            Char tmpC;
            for (int i = 0; i < TASK_ROW_LENGHT; i++)
            {
                if (!spacing)
                {
                    if (rand.Next(10) == 0)
                    {
                        textboxTask.Text += " ";
                        spacing = true;
                        continue;
                    }
                }
                else spacing = false;

                tmpC = symbols[rand.Next((int)sliderDifficulty.Value)];
                if (checkboxCaseSens.IsChecked == true)
                {
                    if ((rand.Next(3) == 0))
                        tmpC = Char.ToUpper(tmpC);
                }

                textboxTask.Text += tmpC;
            }

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            startTime = DateTime.Now;
        }
        DateTime startTime;
        private void Timer_Tick(object sender, EventArgs e)
        {
            textTimer.Text = TimeSpan.FromSeconds(getTimeAndShowSpeed()).ToString(@"mm\:ss");
        }
        int getTimeAndShowSpeed()
        {
            int seconds = (int)(DateTime.Now - startTime).TotalSeconds;
            textSpeed.Text = seconds == 0 ? "0" : $"{(int)(textboxAnswer.Text.Length / (seconds / 60.0))}";
            return seconds;
        }
        void stopClick()
        {
            buttonStart.IsEnabled = true;
            buttonStop.IsEnabled = false;
            GameStarted = false;
            timer.Stop();
            getTimeAndShowSpeed();
        }
        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            stopClick();
        }
    }
}