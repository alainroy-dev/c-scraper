using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace BoxrecScraper.Utils   
{
    public static class HumanActions
    {
        private static readonly Random _rnd = new Random();

        public static async Task HumanClickAsync(IPage page, string selector)
        {
            var locator = page.Locator(selector);
            await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var box = await locator.BoundingBoxAsync();
            if (box == null)
            {
                Console.WriteLine("Can't find this element:" + selector);
                await locator.HoverAsync();
                await Task.Delay(_rnd.Next(80, 250));
                await locator.ClickAsync();
                return;
            }

            float targetX = (float)(box.X + _rnd.NextDouble() * box.Width);
            float targetY = (float)(box.Y + _rnd.NextDouble() * box.Height);

            int steps = _rnd.Next(9, 11);
            await page.Mouse.MoveAsync(targetX, targetY, new MouseMoveOptions { Steps = steps });
            await Task.Delay(_rnd.Next(60, 250));

            await page.Mouse.ClickAsync(targetX, targetY);
        }

        public static async Task HumanTypeAsync(IPage page, string selector, string text)
        {
            var locator = page.Locator(selector);
            await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            await HumanClickAsync(page, selector);
            await Task.Delay(_rnd.Next(60, 180));

            foreach (char c in text)
            {
                await page.Keyboard.PressAsync(c.ToString());
                await Task.Delay(_rnd.Next(80, 220));
            }
        }
    }
}
