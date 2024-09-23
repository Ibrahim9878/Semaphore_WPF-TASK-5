using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Semaphore_WPF;

public partial class MainWindow : Window
{


    #region Members

    public int ThreadNumbers { get; set; } = 0;

    public int WorkingThreadsSize { get; set; } = 10;

    public Semaphore MainSemaphore { get; set; } = new Semaphore(1, 1);

    private ObservableCollection<CustomThread> createdThreadsList = new();
    private ObservableCollection<CustomThread> waitingThreadsList = new();
    private ObservableCollection<CustomThread> workingThreadsList = new();

    public ObservableCollection<CustomThread> CreatedThreadsList { get => createdThreadsList; set { createdThreadsList = value; OnPropertyChanged(); } }
    public ObservableCollection<CustomThread> WaitingThreadsList { get => waitingThreadsList; set { waitingThreadsList = value; OnPropertyChanged(); } }
    public ObservableCollection<CustomThread> WorkingThreadsList { get => workingThreadsList; set { workingThreadsList = value; OnPropertyChanged(); } }
    #endregion

    #region PropertyChangedEventHandler

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion


    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public  void Some(string threadName)
    {
        bool state = true;
        while (state)
        {
            if (MainSemaphore.WaitOne(500))
            {
                AddToWorkingList(threadName);
                Thread.Sleep(5_000);
                RemoveFromWorkingList(threadName);
                MainSemaphore.Release();
                state = false;
            }
        }
    }

    public async Task AddToWorkingList(string threadName)
    {
        CustomThread hostCustomThread = new(threadName);
        WaitingThreadsList.Remove(hostCustomThread);
        WorkingThreadsList.Add(hostCustomThread);
    }

    public async Task RemoveFromWorkingList(string threadName)
    {
        CustomThread hostCustomThread = new(threadName);
        WorkingThreadsList.Remove(hostCustomThread);
    }

    private void ResizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Wpf.Ui.Controls.Button btn)
        {
            switch (btn.Content.ToString())
            {
                case "🗕":
                    WindowState = WindowState.Minimized;
                    break;
                case "X":
                    this.Close();
                    break;
                default:
                    break;
            }
        }
    }

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void CreateNewThread_Click(object sender, RoutedEventArgs e)
    {
        ThreadNumbers++;
        string newCustomThreadName = $"Thread {ThreadNumbers}";
        CustomThread newCustomThread = new(newCustomThreadName, new Thread(() => { Some(newCustomThreadName); }));
        CreatedThreadsList.Add(newCustomThread);
    }
    private void CreatedSemaphoreListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (CreatedSemaphoreListBox.SelectedItem is not CustomThread customThread || string.IsNullOrEmpty(customThread.ThreadName)) return;
        CreatedThreadsList.Remove(customThread);
        WaitingThreadsList.Add(customThread);
        customThread.Thread.Start();
    }

    private void SemaphoreSizeNumberBox_TextChanged(object sender, TextChangedEventArgs e) => WorkingThreadsSize = Convert.ToInt32(SemaphoreSizeNumberBox.Value);

}
