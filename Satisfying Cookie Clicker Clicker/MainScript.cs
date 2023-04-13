using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using System.Linq;
using System.Timers;

namespace Satisfying_Cookie_Clicker_Clicker
{
    public class MainScript
    {
        public ChromeDriver webDriver;
        WebDriverWait wait;
        By goldenCookie = By.XPath("//*[@id='shimmers']/div");
        readonly By bigCookie = By.XPath("//*[@id='bigCookie']");
        int upgradesBought;

        #region🖥StartUp Browser
        [SetUp]
        public void SetUp()
        {
            StartWebDriver();
            CloseAdBlockerTab();
            SetPreferences();
        }

        public void StartWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument(@"load-extension=C:\Users\me\Desktop\AdBlocker");

            webDriver = new ChromeDriver(
                @"C:\Users\me\Desktop\Web Drivers",
                options);
            wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));

            webDriver.Url = "https://orteil.dashnet.org/cookieclicker/";
        }

        void CloseAdBlockerTab()
        {
            var cookieTab = webDriver.CurrentWindowHandle;

            webDriver.WindowHandles.Where(w => w != cookieTab).ToList()
                .ForEach(w =>
                {
                    webDriver.SwitchTo().Window(w);
                    webDriver.Close();
                });
            webDriver.SwitchTo().Window(cookieTab);
            webDriver.Url = "https://orteil.dashnet.org/cookieclicker/";
        }

        void SetPreferences()
        {
            #region🚦Paths
            By english =         By.XPath("//*[@id='langSelect-EN']");
            By computerCookieConfirm = By.XPath("/html/body/div[1]/div/a[1]");
            By options =         By.XPath("//*[@id='prefsButton']");
            By numbers =         By.XPath("//*[@id='numbersButton']");
            By altFont =         By.XPath("//*[@id='monospaceButton']");
            By shortNumbers =    By.XPath("//*[@id='formatButton']");
            By fastNotes =       By.XPath("//*[@id='notifsButton']");
            By extraButtons =    By.XPath("//*[@id='extraButtonsButton']");

            By bakeryName =      By.XPath("//*[@id='bakeryName']");
            By bakeryNameInput = By.XPath("//*[@id='bakeryNameInput']");
            By bakeryNameConfirm=By.XPath("//*[@id='promptOption0']");
            By dontRemindBackups=By.XPath("//*[@id='note-1']/div[3]/h5/a");
            #endregion

            WaitThenClick(english);

            Thread.Sleep(1500);
            webDriver.FindElement(computerCookieConfirm).Click();
            Thread.Sleep(100);

            //This could all be a for loop if I make a list in the constructors.  Would definitely need more comments though
            WaitThenClick(options);
            WaitThenClick(numbers);
            WaitThenClick(altFont);
            WaitThenClick(shortNumbers);
            WaitThenClick(fastNotes);
            WaitThenClick(extraButtons);
            WaitThenClick(options);

            WaitThenClick(bakeryName);
            WaitThenClick(bakeryNameInput, "Curtis");
            WaitThenClick(bakeryNameConfirm);
            WaitThenClick(dontRemindBackups);
        }
        #endregion

        #region🍪Click Focussed
        /* This strategy focusses only on buying upgrades that improve how effective clicking the big cookie is
         * 
         * Phase 1 & 2: 
         * Buy only cursors and its upgrades until 2 upgrades and 10 cursors are purchased
         * (This unlocks the third cursor upgrade)
         * 
         * Phase 3:
         * Shoot for the first clicking upgrade
         */
        [Test]
        public void ClickFocussed()
        {
            var cursorsBought = 1;
            By cursor =  By.XPath($"//*[@id='product0']");
            By upgrade = By.XPath($"//*[@id='upgrade0']");

            for (int i=0; i < 15; i++)
                ClickCookies();

            WaitThenClick(cursor);

            while (upgradesBought < 2)
            {
                ClickCookies();
                if (Buy(upgrade)) upgradesBought++;
            }
            
            while (cursorsBought < 10)
            {
                ClickCookies();
                if (Buy(cursor)) cursorsBought++;
            }

            while (upgradesBought < 3)
            {
                ClickCookies();
                if (Buy(upgrade)) upgradesBought++;
            }

            while (cursorsBought < 25)
            {
                ClickCookies();
                if (Buy(cursor)) cursorsBought++;
            }

            while (true)
            {
                ClickCookies();
                if (Buy(upgrade))    upgradesBought++;
                else if (Buy(cursor)) cursorsBought++;
            }
        }

        string GetClass(By path) => webDriver.FindElement(path).GetAttribute("Class");
        #endregion

        #region Other Tests
        static System.Timers.Timer clickTimer = new(1000);
        int secondsCount;

        /* Simply clicks the cookie without buying any upgrades to see how many times selenium can click the cookie
         * 
         * Starts the timer when entering this test
         * 
         * Clicks the cookie
         * 
         * Checks to see if a minute has passed
         * 
         * When the timer is done it divides the amount of cookies by the amount of time
         */
        [Test]
        public void ClickPerSecondTest()
        {
            clickTimer.Elapsed += ClickTimer_Elapsed;
            clickTimer.Enabled = true;
            clickTimer.AutoReset = true;
            clickTimer.Start();

            while (secondsCount < 60)
            {
                WaitThenClick(bigCookie);
            }
        }

        [Test]
        public void BlankSlate(){}

        private void ClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            secondsCount++;
        }
        #endregion

        #region Helper Methods
        IWebElement WaitThenClick(By element, string input = "Click")
        {
            wait.Until(w => w.FindElement(element));

            var webElement = webDriver.FindElement(element);
            if (input == "Click") webElement.Click();
            else if (input == "") { }
            else webElement.SendKeys(input);
            return webElement;
        }

        void ClickCookies()
        {
            try
            {
                webDriver.FindElement(goldenCookie).Click();
                Assert.Pass("🎵I've got a golden coookiiee!!🎵");
            }
            catch (Exception) { }

            webDriver.FindElement(bigCookie).Click();

            Thread.Sleep(4);
        }

        /* Buys buildings and upgrades.
         * 
         * Checks to see if the upgrade is ready to be bought by inspecting it's class
         * 
         * Hovers over the upgrade for a moment both to hilight what is being purchased
         * and to wait until it is fully ready to be purchased TODO: Actually hover.
         * 
         * Buys the upgrade
         * 
         * Hovers some more
         * 
         * Waits for the apearance of buyability to go away
         * 
         * Increments the number of buildings / upgrades purchased?
         */
        bool Buy(By element)
        {
            WaitThenClick(element, "");
            var style = GetClass(element);
            if (style != "product unlocked enabled" && style != "crate upgrade enabled") return false;

            Console.Write(style);
            Thread.Sleep(600);
            WaitThenClick(element);
            Thread.Sleep(100);

            return true;
        }

        private int TrackTotalCookies()
        {
            #region🚦Paths
            By stats = By.XPath("//*[@id='statsButton']");
            By totalCookiesField = By.XPath("//*[@id='statsGeneral']/div[3]/div");
            #endregion

            WaitThenClick(stats);
            var totalCookiesText = WaitThenClick(totalCookiesField).Text;
            var totalCookiesNum = int.Parse(totalCookiesText.Replace(",", ""));
            WaitThenClick(stats);

            Console.WriteLine("Total cookies: " + totalCookiesNum);
            return totalCookiesNum;
        }
        #endregion

        [TearDown]
        public void TearDown()
        {
            TrackTotalCookies();
            //webDriver.Close();
        }
    }
}