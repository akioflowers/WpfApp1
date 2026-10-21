using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Reminder> Reminders {get; set;}
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            Reminders = new ObservableCollection<Reminder>();
            RemindersGrid.ItemsSource = Reminders;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeLabel.Text = "Текущее время: " + DateTime.Now.ToLongTimeString();

            foreach (Reminder reminder in Reminders)
            {
                if (DateTime.Now.Hour == reminder.Hour &&
                    DateTime.Now.Minute == reminder.Minute &&
                    reminder.IsShown == false)
                {
                    reminder.IsShown = true;

                    RemindersGrid.Items.Refresh();

                    MessageBox.Show(reminder.Text, "Напоминание!");
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int hour;
            int minute;

            if (!int.TryParse(HourBox.Text, out hour) ||
                !int.TryParse(MinuteBox.Text, out minute))
            {
                MessageBox.Show("Введите часы и минуты числами!");
                return;
            }

            if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
            {
                MessageBox.Show("Неправильно введено время!");
                return;
            }

            if (TextBoxReminder.Text.Trim() == "")
            {
                MessageBox.Show("Введите текст напоминания!");
                return;
            }

            Reminder reminder = new Reminder();

            reminder.Hour = hour;
            reminder.Minute = minute;
            reminder.Text = TextBoxReminder.Text;
            reminder.IsShown = false;

            Reminders.Add(reminder);

            HourBox.Clear();
            MinuteBox.Clear();
            TextBoxReminder.Clear();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Reminder selectedReminder = RemindersGrid.SelectedItem as Reminder;

            if (selectedReminder != null)
            {
                Reminders.Remove(selectedReminder);
            }
            else
            {
                MessageBox.Show("Выберите напоминание!");
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    Reminders.Clear();

                    string[] lines = File.ReadAllLines(openDialog.FileName);

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(new string[] { "%%" },
                            StringSplitOptions.None);

                        if (parts.Length == 3)
                        {
                            int hour;
                            int minute;

                            if (int.TryParse(parts[0], out hour) &&
                                int.TryParse(parts[1], out minute))
                            {
                                if (hour >= 0 && hour <= 23 &&
                                    minute >= 0 && minute <= 59)
                                {
                                    Reminder reminder = new Reminder();

                                    reminder.Hour = hour;
                                    reminder.Minute = minute;
                                    reminder.Text = parts[2];
                                    reminder.IsShown = false;

                                    Reminders.Add(reminder);
                                }
                            }
                        }
                    }

                    MessageBox.Show("Напоминания загружены!");
                }
                catch
                {
                    MessageBox.Show("Ошибка при открытии файла!");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            saveDialog.FileName = "reminders.txt";

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        foreach (Reminder reminder in Reminders)
                        {
                            string line = reminder.Hour.ToString("00") + "%%" +
                                          reminder.Minute.ToString("00") + "%%" +
                                          reminder.Text;

                            writer.WriteLine(line);
                        }
                    }

                    MessageBox.Show("Напоминания сохранены!");
                }
                catch
                {
                    MessageBox.Show("Ошибка при сохранении файла!");
                }
            }
        }
    }
}