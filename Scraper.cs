using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using System.Text.Json;
using System.Collections.Generic;
using BoxrecScraper.Utils; 
using BoxrecScraper.Models;
class Scraper
{
    private static readonly Random _rnd = new Random();
    private static readonly BrowserTypeLaunchOptions browserTypeLaunchOptions = new BrowserTypeLaunchOptions
    {
        Headless = false, // true = run without UI
        Proxy = ScraperUtils.GetProxy()
    };

    private static readonly BrowserNewContextOptions browserNewContextOptions = new BrowserNewContextOptions
    {
        IgnoreHTTPSErrors = true,   // same as curl -k
        // UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Mobile/15E148 Safari/604.1",
        // ViewportSize = new ViewportSize { Width = 1200, Height = 800 },
        // IsMobile = true,
        // HasTouch = true
    };

    private static readonly PageGotoOptions pageGotoOptions = new PageGotoOptions
    {
        WaitUntil = WaitUntilState.DOMContentLoaded,
        Timeout = 100000 // 60 seconds
    };

    private static readonly PageWaitForSelectorOptions pageWaitForSelectorOptions = new PageWaitForSelectorOptions
    {
        Timeout = 100000 // 60 seconds
    };
    
    private static async Task<List<Match>?> FetchMatchesFromAPI()
    {
        string apiURL = Environment.GetEnvironmentVariable("SOURCE_API");
        string apiEndPoint = Environment.GetEnvironmentVariable("SOURCE_API_ENDPOINT");

        try
        {
            var api = new APIClient(apiURL);
            string jsonData = await api.GetAsync(apiEndPoint);
            var response = JsonSerializer.Deserialize<ApiResponse>(jsonData);

            return response?.matches ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API request failed: {ex.Message}");
            return [];
        }
    }

    public static async Task Scrape()
    {
        // Load API
        bool isProductionMode = Environment.GetEnvironmentVariable("IS_PRODUCTION") == "true";

        List<Match>? matches;
        List<string> playerNames = [];
        var (user_email, user_pass) = LoginProvider.GetLoginCredentials();

        var storage = new JsonStorage<List<Match>>("fixtures.json");
        var storageDone = new JsonStorage<List<Match>>("fixturesDone.json");

        if (isProductionMode)
        {
            Console.WriteLine("Running in Production mode");

            matches = await FetchMatchesFromAPI();
            matches = await storageDone.filterSync(matches);
            await storage.SaveAsyncWithOld(matches);

            matches = await storage.LoadAsync();
            matches = ScraperUtils.FilterRecentMatches(matches); // filter matches within today through past
            playerNames = ScraperUtils.DestructData(matches);
        }
        else
        {
            Console.WriteLine("Running in Development mode");

            matches = await storage.LoadAsync();

            playerNames = ScraperUtils.DestructData(matches);

        }

        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(browserTypeLaunchOptions);

        var iPhone = playwright.Devices["iPad Pro 11"];
        var context = await browser.NewContextAsync(browserNewContextOptions);
        var page = await context.NewPageAsync();

        // 1. Navigate to boxrec site and login with the credential
        await page.GotoAsync(Environment.GetEnvironmentVariable("LOGIN_PAGE_URL"), pageGotoOptions);
        await Task.Delay(_rnd.Next(1000, 1500));
        await page.WaitForSelectorAsync(".inputLoginBox", pageWaitForSelectorOptions);
        await Task.Delay(_rnd.Next(4000, 5000));
        await HumanActions.HumanTypeAsync(page, "#username", user_email);
        await Task.Delay(_rnd.Next(2000, 3500));
        await HumanActions.HumanTypeAsync(page, "#password", user_pass);
        await Task.Delay(_rnd.Next(2000, 3000));
        await HumanActions.HumanClickAsync(page, "form[action='/en/login'] button.submitButton");
        await page.WaitForSelectorAsync("table.dataTable", pageWaitForSelectorOptions);
        await Task.Delay(_rnd.Next(1000, 1500));

        // 2. Navigate to the schedule page 
        await page.GotoAsync(Environment.GetEnvironmentVariable("RESULTS_PAGE_URL"), pageGotoOptions);

        // 3. Find a specific games that matches with the games provided by API
        // string[] playerPairs = playerNames.ToArray();
        string[] playerPairs = ["Hatice Akbas - Sakshi Chaudhary"];
        int totalTestCase = 0;
        while (true)
        {
            try
            {
                await page.WaitForSelectorAsync("table#calendarSchedule > tbody", pageWaitForSelectorOptions);
                var scheduleRows = await page.QuerySelectorAllAsync("table#calendarSchedule > tbody");
                foreach (var row in scheduleRows)
                {
                    var tbodyID = await row.GetAttributeAsync("id");
                    if (tbodyID == null) continue;
                    var tbodyIDNumber = tbodyID.Replace("bId", "").Trim();

                    var playerEls = await row.QuerySelectorAllAsync("a.personLink");
                    if (playerEls.Count < 2) continue;

                    var homePlayerEl = playerEls[0];
                    var awayPlayerEl = playerEls[1];
                    var homePlayerName = await homePlayerEl.InnerTextAsync();
                    var awayPlayerName = await awayPlayerEl.InnerTextAsync();
                    string combinedName = homePlayerName.Trim() + " - " + awayPlayerName.Trim();

                    var (index, score) = StringCompareFn.CompareWithArray(combinedName, playerPairs);
                    if (score < 90) continue;

                    // 4. Collect the required data
                    ILocator theadLocator = page.Locator($"tbody#{tbodyID} >> xpath=preceding-sibling::thead[1]");
                    string location = await theadLocator.InnerTextAsync();
                    location = location.Contains('-') ? location.Split("-")[1].Trim().Replace("event", "").Trim() : "";


                    var divisionEl = (await row.QuerySelectorAllAsync("td"))[8];
                    string division = await divisionEl.InnerTextAsync();

                    var boutLinkEl = await row.QuerySelectorAsync("td.actionCell a");
                    var boutLink = await boutLinkEl.GetAttributeAsync("href");
                    var boxrec_id = boutLink.Split("event")[1].Trim();

                    var roundEl = (await row.QuerySelectorAllAsync("td"))[4];
                    string round = (await roundEl.InnerTextAsync()).Trim();
                    string round_scheduled = "";
                    string round_finish = "";
                    if (round.Contains('/') && round.Contains('x'))
                    {
                        round_finish = round.Split("/")[0].Trim();
                        round_scheduled = round.Split("/")[1].Trim().Split("x")[0].Trim();
                    }
                    else if (round.Contains('/') && !round.Contains('x'))
                    {
                        round_finish = round.Split("/")[0].Trim();
                        round_scheduled = round.Split("/")[1].Trim();
                    }
                    else if (!round.Contains('/') && round.Contains('x'))
                    {
                        round_finish = round.Split("x")[0].Trim();
                        round_scheduled = round.Split("x")[0].Trim();
                    }
                    else
                    {
                        round_finish = round;
                        round_scheduled = round;
                    }

                    var boutResultEl = await row.QuerySelectorAsync("div.boutResult");
                    string boutResult = (await boutResultEl.InnerTextAsync()).Trim();
                    string methods = boutResult.Contains('-') ? boutResult.Split('-')[1].Trim() : "";
                    boutResult = boutResult.Equals("?") ? "unknown" : boutResult;

                    if(boutResult.Equals("VS"))
                    {
                        // skip upcoming events
                        continue;
                    }
                    string winner = boutResult.Equals("?") || boutResult.Contains("D-") || boutResult.Contains("NC-") ? "" :
                                    (boutResult.Contains("W-") ? awayPlayerName : homePlayerName);

                    PSMatch pSMatch = new PSMatch();
                    pSMatch.palmerbet_id = matches[index].eventId;
                    pSMatch.boxrec_id = boxrec_id;
                    pSMatch.division = division;
                    pSMatch.location = location;
                    pSMatch.date = matches[index].startTime;
                    pSMatch.status = new Status
                    {
                        fighter_1 = new Team
                        {
                            title = homePlayerName,
                            competitorId = matches[index].homeTeam.competitorId
                        },
                        fighter_2 = new Team
                        {
                            title = awayPlayerName,
                            competitorId = matches[index].awayTeam.competitorId
                        },
                        result = new Result
                        {
                            winner = winner,
                            method = methods,
                            round_finish = round_finish,
                            round_time = "",
                            round_scheduled = round_scheduled,
                        }
                    };

                    string json = JsonSerializer.Serialize(pSMatch, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    Console.WriteLine(json);

                    List<Match> storingData = new List<Match> {matches[index]};
                    await storageDone.SaveAsyncDone(storingData);
                    totalTestCase++;
                    await Task.Delay(_rnd.Next(10000, 15000));
                }

                var pagenationEl = await page.QuerySelectorAsync("div.tableInfoTop .pagerElement:last-of-type a");
                if (totalTestCase >= playerPairs.Length || pagenationEl == null) break;
                await HumanActions.HumanClickAsync(page, "div.tableInfoTop .pagerElement:last-of-type a");

                await Task.Delay(_rnd.Next(10000, 15000));

            }
            catch (TimeoutException)
            {
                Console.WriteLine("Loading schedule rows, please wait...");
                await Task.Delay(2000); // Wait for 2 seconds before retrying
            }
        }

        Console.WriteLine("Press Enter to exit...");

        await browser.CloseAsync();
        
    }
    
}