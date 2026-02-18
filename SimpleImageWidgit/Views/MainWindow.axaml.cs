using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using Newtonsoft.Json.Linq;
using SimpleImageWidgit.ViewModels;
using Point = System.Drawing.Point;
using Vector = System.Numerics.Vector;

namespace SimpleImageWidgit.Views;

public partial class MainWindow : Window
{
    private bool newImage = false;
    private int endOfTimeEpoch = 0;
    private int nextImageAmount = 10;
    private Vector2 maxWindowSize = new Vector2(300, 300);
    private bool sillyMode = false;

    private List<Dictionary<string, string>> apiList = new List<Dictionary<string, string>>();
    
    static readonly Random rnd = new Random();

    private string currentImg = string.Empty;

    private Bitmap loadingImage;
    private Bitmap errorImage;

    private Vector2 velocity = new Vector2(1f, 1f);
    private bool helder = false;
    private bool stopImageThing = false;
    
    string outputDir = Path.Combine(Path.GetTempPath(), "cat");
    
    public MainWindow()
    {
        InitializeComponent();
            
        // Keybinds
        AddHandler(InputElement.KeyUpEvent, HandleKey, RoutingStrategies.Tunnel);
        
        // Styling
        MainWindower.MaxHeight = maxWindowSize.Y;
        MainWindower.MaxWidth = maxWindowSize.X;
        
        MainWindower.Width = maxWindowSize.X;
        MainWindower.Height = maxWindowSize.Y;

        AddAPI("https://cataas.com/cat");
        AddAPI("https://some-random-api.com/animal/cat", "image");
        //AddAPI("https://files.catbox.moe/aiymm6.jpg", "");

        loadingImage = new Bitmap(AssetLoader.Open(new Uri("avares://SimpleImageWidgit/Assets/lildude.png")));
        errorImage = new Bitmap(AssetLoader.Open(new Uri("avares://SimpleImageWidgit/Assets/thinggoboom.png")));

        MainWindower.Show();
        MainWindower.Topmost = true;

        Task.Run(BackgroundUpdates);
    }
    
    void AddAPI(string url, string path="")
    {
        Dictionary<string, string> coolish = new Dictionary<string, string>();
        coolish.Add("url", url);
        coolish.Add("path", path);
        
        apiList.Add(coolish);
    }
    
    string GetImage(Dictionary<string, string> api)
    {
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string imgThingggggg = Path.Combine(outputDir, Path.GetRandomFileName());
        
        Console.WriteLine("Output Path: "+imgThingggggg);
        Console.WriteLine("API URL: "+api["url"]);
        Console.WriteLine("API Path: "+api["path"]);

        using (var client = new WebClient())
        {
            string urler = api["url"];

            try
            {
                JObject json = JObject.Parse(client.DownloadString(urler));
                JToken pathLocation = json;
                foreach (string part in api["path"].Split("/"))
                {
                    if (int.TryParse(part, out int cool))
                    {
                        pathLocation = pathLocation[cool];
                    }
                    else
                    {
                        pathLocation = pathLocation[part];
                    }
                }
                urler = pathLocation.ToString();
            } catch (Exception) {  }
            
            client.DownloadFile(new Uri(urler), imgThingggggg);
        }
        return imgThingggggg;
    }

    int CurrentEpoch()
    {
        return (int)DateTime.Now.TimeOfDay.TotalSeconds;
    }
    
    async void HandleKey(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            MainWindower.Close();
        }

        if (e.Key == Key.F1)
        {
            sillyMode = !sillyMode;
            Console.WriteLine($"Silly Mode: {sillyMode}");
        }

        if (e.Key == Key.F2)
        {
            stopImageThing = !stopImageThing;
            Console.WriteLine($"Continuing Images: {!stopImageThing}");
        }

        if (e.Key == Key.F3)
        {
            newImage = true;
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                ImgThing.Source = loadingImage;
            } );
            Console.WriteLine("Getting new image...");
        }
    }
    
    private async void BackgroundUpdates()
    {
        while (true)
        {
            try
            {
                if (newImage && !stopImageThing)
                {
                    currentImg = GetImage(apiList[rnd.Next(0, apiList.Count)]);
                    Bitmap cool = new Bitmap(
                        currentImg
                    );

                    Console.WriteLine($"Img Size: {cool.Size.Width}x{cool.Size.Height}");
                    Console.WriteLine($"Img DPI: {cool.Dpi.X}x{cool.Dpi.Y}");

                    await Dispatcher.UIThread.InvokeAsync(async () =>
                        {
                            ImgThing.Source = cool;
                        }
                    );

                    newImage = false;
                }

                if (endOfTimeEpoch == 0 || endOfTimeEpoch == CurrentEpoch())
                {
                    endOfTimeEpoch = CurrentEpoch() + nextImageAmount;
                    newImage = true;
                }

                if (sillyMode && !helder)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        PixelPoint curPos = MainWindower.Position;
                        
                        double nextX = curPos.X + velocity.X;
                        double nextY = curPos.Y + velocity.Y;
                        //Console.WriteLine($"Pre Process: {nextX}, {nextY}");

                        int newX = (int)nextX;
                        int newY = (int)nextY;

                        if (nextX <= Screens.Primary.WorkingArea.X)
                        {
                            newX = (int)Screens.Primary.WorkingArea.X;
                            velocity = new Vector2(Math.Abs(velocity.X), velocity.Y);  

                        }

                        else if (nextX + MainWindower.Width >= Screens.Primary.WorkingArea.X + Screens.Primary.WorkingArea.Width)
                        {
                            newX = (int)(Screens.Primary.WorkingArea.X + Screens.Primary.WorkingArea.Width - MainWindower.Width);
                            velocity = new Vector2(-Math.Abs(velocity.X), velocity.Y); 

                        }

                        if (nextY <= Screens.Primary.WorkingArea.Y)
                        {
                            newY = (int)Screens.Primary.WorkingArea.Y;
                            velocity = new Vector2(velocity.X, Math.Abs(velocity.Y));  

                        }

                        else if (nextY + MainWindower.Height >= Screens.Primary.WorkingArea.Y + Screens.Primary.WorkingArea.Height)
                        {
                            newY = (int)(Screens.Primary.WorkingArea.Y + Screens.Primary.WorkingArea.Height - MainWindower.Height);
                            velocity = new Vector2(velocity.X, -Math.Abs(velocity.Y)); 

                        }

                        newX = (int)Math.Clamp(newX, Screens.Primary.WorkingArea.X, Screens.Primary.WorkingArea.X + Screens.Primary.WorkingArea.Width - MainWindower.Width);
                        newY = (int)Math.Clamp(newY, Screens.Primary.WorkingArea.Y, Screens.Primary.WorkingArea.Y + Screens.Primary.WorkingArea.Height - MainWindower.Height);
                        //Console.WriteLine($"Image Position: {newX}, {newY}");
                        
                        MainWindower.Position = new PixelPoint(newX, newY);
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n===========================\n{e.ToString()}\n===========================\n");
                //Console.WriteLine($"\n=========================\nError Message:\n{e.Message}\n\nStack Trace:\n{e.StackTrace}\n\nSource: {e.Source}\nHelp Link: {e.HelpLink}\n=========================\n");
                await Dispatcher.UIThread.InvokeAsync(async () => { ImgThing.Source = errorImage; });
                Task.Delay(500).Wait();
            }
        }
    }

    private void ImgThing_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        MainWindower.Opacity = 0.5;
        BeginMoveDrag(e);
        helder = true;
    }


    private void ImgThing_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        MainWindower.Opacity = 1;
        helder = false;
    }

    private void TopLevel_OnClosed(object? sender, EventArgs e)
    {
        Directory.Delete(outputDir);
    }
}
