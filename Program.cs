using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using DotNetEnv;

class Program
{
    public static async Task Main()
    {
        Env.Load();
        
        using var cts = new CancellationTokenSource();

        // string browsersPath = Path.Combine(AppContext.BaseDirectory, "playwright-browsers");
        // Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", browsersPath);
        // if (!Directory.Exists(Path.Combine(browsersPath, "chromium")))
        // {
        //     Console.WriteLine("Installing Playwright browsers...");
        //     int exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
        //     if (exitCode != 0)
        //     {
        //         Console.WriteLine($"Browser installation failed with exit code {exitCode}");
        //         return;
        //     }
        //     Console.WriteLine("Browsers installed successfully.");
        // }
        
        // Run a task to listen for key press
        _ = Task.Run(() =>
        {
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
            cts.Cancel();
        });

        while (!cts.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine($"[START] Run at {DateTime.Now}");

                await Scraper.Scrape();

                Console.WriteLine($"[END] Run completed at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.WriteLine("Restarting scraper in 10 seconds...");

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            // Wait 20 minutes or until user cancels
            try
            {
                string? intervalEnv = Environment.GetEnvironmentVariable("SCRAPER_INTERVAL_MINS");
                Console.WriteLine($"Scraper will re-run {intervalEnv}mins later...");

                await Task.Delay(TimeSpan.FromMinutes(Convert.ToInt32(intervalEnv)), cts.Token);
            }
            catch (TaskCanceledException)
            {
                // Break loop if canceled during delay
                break;
            }
        }

        Console.WriteLine("Scraper stopped by requests.");
        
    }
    
}