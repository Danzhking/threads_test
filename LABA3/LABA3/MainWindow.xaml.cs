using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace LABA3
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> ThreadA { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ThreadB { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ThreadC { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ThreadD { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ThreadE { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ThreadG { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ThreadF { get; set; } = new ObservableCollection<string>();

        private readonly object _lock = new object();
        private Queue<int> numberQueue = new Queue<int>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void RunTasks(object sender, RoutedEventArgs e)
        {
            // Очистка перед запуском
            ClearCollections();
            FillNumberQueue();

            // Запуск A и B
            Task taskA = Task.Run(() => RunThread(ThreadA, "A", 4000));
            Task taskB = Task.Run(() => RunThread(ThreadB, "B", 10000));
            await taskA;

            // Запуск D, E и C после завершения A
            Task taskD = Task.Run(() => RunThread(ThreadD, "D", 3000));
            Task taskE = Task.Run(() => RunThread(ThreadE, "E", 3000));
            Task taskC = Task.Run(() => RunThread(ThreadC, "C", 6000));
            await Task.WhenAll(taskD, taskE);

            // Запуск G после завершения D и E
            Task taskG = Task.Run(() => RunThread(ThreadG, "G", 3000));
            await Task.WhenAll(taskB, taskC, taskG);

            // Запуск F после завершения B, C и G
            await Task.Run(() => RunThread(ThreadF, "F", 4000));
        }

        private void RunThread(ObservableCollection<string> collection, string threadName, int duration)
        {
            DateTime endTime = DateTime.Now.AddMilliseconds(duration);
            while (DateTime.Now < endTime)
            {
                int number;
                lock (_lock)
                {
                    if (numberQueue.Count == 0)
                        break;
                    number = numberQueue.Dequeue();
                }

                Application.Current.Dispatcher.Invoke(() => collection.Add($"{threadName}: {number}"));
                Thread.Sleep(500);
            }
        }

        private void ClearCollections()
        {
            ThreadA.Clear();
            ThreadB.Clear();
            ThreadC.Clear();
            ThreadD.Clear();
            ThreadE.Clear();
            ThreadG.Clear();
            ThreadF.Clear();
        }

        private void FillNumberQueue()
        {
            numberQueue.Clear();
            for (int i = 1; i <= 100; i++)
            {
                numberQueue.Enqueue(i);
            }
        }
    }
}
