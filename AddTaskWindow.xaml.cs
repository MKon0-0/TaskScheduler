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
using System.Windows.Shapes;

namespace TaskScheduler
{
    public partial class AddTaskWindow : Window
    {
        public string TaskDescription { get; private set; }
        public DateTime TaskDate { get; internal set; }

        public AddTaskWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string taskDescription = TaskDescriptionTextBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(taskDescription))
            {
                TaskDescription = taskDescription;
                DialogResult = true; // Закрываем окно с результатом успеха
                Close();
            }
            else
            {
                MessageBox.Show("Введите описание задачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрываем окно без сохранения
            Close();
        }
    }
}