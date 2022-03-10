using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace FifteenWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FifteenGUI.Game game;
        FifteenGUI.GameHistory gameHistory;
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            game = new FifteenGUI.Game(4);
            gameHistory = new FifteenGUI.GameHistory();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
        }

        private Button GetButton(int index)
        {
            return (Button)FindName("button" + index);
        }

        private void RefreshButtonField()
        {
            for (int i = 0; i < 16; i++)
            {
                Button tempButton = GetButton(i);
                tempButton.Content = game.GetNumber(i).ToString();
                String tempString = tempButton.Content.ToString();
                if (tempString == "0") tempButton.Visibility = Visibility.Hidden;
                else tempButton.Visibility = Visibility.Visible;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void MenuStartGame_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            MenuCancelMyTurn.Visibility = Visibility.Hidden;
            timer.Stop();
            game.Start();
            for (int i = 0; i < 100; i++) game.ShiftRandom();
            RefreshButtonField();
            labelTime.Content = "0";
            labelScore.Content = "0";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            int position = Convert.ToInt32(((Button)sender).Tag);

            int x, y;
            game.TurnPositionToCoordinates(position, out x, out y);
            if (game.CanShift(x, y))
            {
                gameHistory.History.Push(game.SaveState());
                game.Shift(x, y);
                labelScore.Content = $"{Convert.ToInt32(labelScore.Content) + 1}";
                RefreshButtonField();
                MenuCancelMyTurn.Visibility = Visibility.Visible;
            }

            if (game.CheckWin())
            {
                timer.Stop();
                MessageBox.Show("You win the game!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelMyTurn_Click(object sender, RoutedEventArgs e)
        {
            CancelTurn();
        }

        private void CancelTurn()
        {
            if (gameHistory.History.Count > 0)
            {
                game.RestoreState(gameHistory.History.Pop());
                if (gameHistory.History.Count == 0) MenuCancelMyTurn.Visibility = Visibility.Hidden;
                RefreshButtonField();
            }
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            labelTime.Content = $"{Convert.ToInt32(labelTime.Content) + 1}";
        }

        private void DockPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Z) && (e.KeyboardDevice.Modifiers == ModifierKeys.Control)) CancelTurn();
        }
    }
}
