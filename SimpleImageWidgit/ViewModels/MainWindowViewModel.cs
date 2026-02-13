namespace SimpleImageWidgit.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public static bool ShowSettings {get;set;} = false;
}