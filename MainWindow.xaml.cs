using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TaskScheduler.View.ViewModel;
using static TaskScheduler.MainWindow;


namespace TaskScheduler 
{
public partial class MainWindow : Window
    {
        private string _taskDescription;

        public string TaskDescription
        {
            get => _taskDescription;
            set => _taskDescription = value;
        }
        private DateTime currentDate; // Добавлено объявление переменной для текущей даты
        private List<int> daysInCurrentMonth;
        public ObservableCollection<CalendarDay> CalendarDays { get; set; }
        
        public MainWindow()
        {
            currentDate = DateTime.Today; // Инициализация текущей даты
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            CalendarDays = new ObservableCollection<CalendarDay>();
            CalendarGrid.ItemsSource = CalendarDays;
            pdateCalendar();
        }
        private void UpdateCalendar()
        {
            CalendarDays.Clear();
            MonthTextBlock.Text = currentDate.ToString("MMMM yyyy");
            pdateCalendar();
            CalendarGrid.ItemsSource = CalendarDays;
        }
        private void pdateCalendar()
        {
            CalendarDays.Clear(); // Очищаем коллекцию перед обновлением
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            // Добавляем дни текущего месяца
            for (int day = 1; day <= daysInMonth; day++)
            {
                CalendarDays.Add(new CalendarDay(day));
            }

            MonthTextBlock.Text = currentDate.ToString("MMMM yyyy"); // Обновляем текст месяца
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            UpdateCalendar();
        }
        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(1); 
            UpdateCalendar();
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDaysInCurrentMonth();
            FillDaysListBox();
        }

        private void FillDaysListBox()
        {
            DaysListBox.Items.Clear();
            foreach (var day in daysInCurrentMonth)
            {
                DaysListBox.Items.Add(day);
            }
        }

        private void AddTasksButton_Click(object sender, RoutedEventArgs e)
        {
            string[] taskDescriptions = TaskDescriptionTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (DaysListBox.SelectedItems.Count > 0)
            {
                foreach (int selectedDay in DaysListBox.SelectedItems)
                {
                    foreach (string taskDescription in taskDescriptions)
                    {
                        string trimmedTask = taskDescription.Trim();

                        if (!string.IsNullOrWhiteSpace(trimmedTask))
                        {
                            AddTaskToDay(selectedDay, trimmedTask);
                            string currentDateTime = DateTime.Now.ToString("yyyy"); // Форматирование текущей даты
                            string monthName = DateTime.Now.ToString("MMMM"); // Получаем название текущего месяца
                            TasksListBox.Items.Add($" {selectedDay} {monthName} {currentDateTime}: {trimmedTask}");
                        }
                    }
                }

                TaskDescriptionTextBox.Clear(); // Очистка поля ввода
                DaysListBox.SelectedItems.Clear(); // Сброс выбора дней
            }
        }


        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow();
            if (addTaskWindow.ShowDialog() == true) // Ожидаем результат закрытия окна
            {
                string taskDescription = addTaskWindow.TaskDescription;
                string currentDateTime = DateTime.Now.ToString("dd MMMM yyyy"); // Форматирование текущей даты

                // Добавляем задачу в ListBox с указанием даты добавления
                TasksListBox.Items.Add($"{currentDateTime}: {taskDescription}"); // Предполагая, что TasksListBox - это ваш ListBox
            }
        }

        private void RemoveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem != null)
            {
                TasksListBox.Items.Remove(TasksListBox.SelectedItem); // Удаление выделенной задачи
            }
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem != null)
            {
                string selectedTask = TasksListBox.SelectedItem.ToString();
                string currentDescription = selectedTask.Substring(selectedTask.IndexOf(":") + 2); // Извлечение описания задачи

                AddTaskWindow addTaskWindow = new AddTaskWindow();
                TaskDescription = currentDescription; // Установка текущего описания в окно

                if (addTaskWindow.ShowDialog() == true) // Ожидаем результат закрытия окна
                {
                    // Извлечение даты и времени из элемента ListBox
                    string dateTime = selectedTask.Substring(0, selectedTask.IndexOf(":"));

                    // Обновление элемента ListBox с новым описанием и датой
                    TasksListBox.Items[TasksListBox.SelectedIndex] = $"{dateTime}: {addTaskWindow.TaskDescription}";
                }
            }
        }



        private void AddTaskToDay(int day, string task)
        {
            var calendarDay = CalendarDays.FirstOrDefault(d => d.Day == day);
            if (calendarDay != null)
            {
                calendarDay.Tasks.Add(new TaskItem(task)); // Добавляем новую задачу в соответствующий день
                UpdateTasksList(calendarDay); // Обновляем список задач, если завязаны на ListBox
            }
        }


        private void LoadDaysInCurrentMonth()
        {
            daysInCurrentMonth = new List<int>();

            // Получаем текущий месяц и год
            DateTime now = DateTime.Now;
            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

            // Заполняем список днями текущего месяца
            for (int day = 1; day <= daysInMonth; day++)
            {
                daysInCurrentMonth.Add(day);
            }
        }

        public class CalendarDay
        {
            public int Day { get; set; }
            public ObservableCollection<TaskItem> Tasks { get; set; }
            public bool IsSelected { get; set; }

            public CalendarDay(int day)
            {
                Day = day;
                Tasks = new ObservableCollection<TaskItem>();
            }
        }


        public class TaskItem
        {
            private string task;

            public string Description { get; set; }
            public string Month { get; set; } // Хранит месяц, когда была добавлена задача

            public TaskItem(string description, string month)
            {
                Description = description;
                Month = month;
            }

            public TaskItem(string task)
            {
                this.task = task;
            }
        }


        private void UpdateTasksList(CalendarDay calendarDay)
        {
            
        }


      
    }
}
