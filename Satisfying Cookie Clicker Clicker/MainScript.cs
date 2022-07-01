using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using System.Linq;

namespace Satisfying_Cookie_Clicker_Clicker
{
    public class MainScript
    {
        public ChromeDriver webDriver;
        private WebDriverWait wait;
        private readonly By bigCookie = By.XPath("//*[@id='bigCookie']");
        private int i;
        private Building[] buildingList;
        private Building nextBuilding;
        private int buildingLimit;
        private int buildingsBought;

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
                @"C:\Users\me\source\repos\Satisfying Cookie Clicker Clicker\WebDrivers",
                options);
            wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(30));

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

        private void SetPreferences()
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

            Thread.Sleep(1000);
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

        #region🍪Main Code
        /* product0 is cursor
         * product1 is grandma, etc
         */
        [Test]
        public void MainLoop()
        {
            #region🏛Building List
            buildingList = new Building[]
            {
                new Building("Cursor", buildingNumber: 0, price: 15, multiplier: .1f),
                new Building("Grandma", buildingNumber: 1, price: 100, multiplier: 1),
                new Building("", buildingNumber: 2, price: 1100, multiplier: 8),
                new Building("Mine", buildingNumber: 3, price: 12000, multiplier: 47),
                new Building("Factory", buildingNumber: 4, price: 130000, multiplier: 260),
                new Building("Bank", buildingNumber: 5, price: 1400000, multiplier: 1400),
                new Building("Temple", buildingNumber: 6, price: 20000000, multiplier: 7800),
                new Building("Wizard Tower", buildingNumber: 7, price: 330000000, multiplier: 44000),
                new Building("Shipment", buildingNumber: 8, price: 5100000000, multiplier: 8),
                new Building("", buildingNumber: 9, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 10, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 11, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 12, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 13, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 14, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 15, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 16, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 17, price: 1100, multiplier: 8),
                new Building("", buildingNumber: 18, price: 1100, multiplier: 8),
            };
            nextBuilding = buildingList[1];
            #endregion

            buildingLimit = 20;

            while (i < 99)
            {
                WaitThenClick(bigCookie);
                i++;
            }

            while (true)
            {
                while (i%1000 != 0)
                {
                    WaitThenClick(bigCookie);
                    BuyCrates();
                    BuyBuildings();
                    i++;
                }
                WaitThenClick(bigCookie);
                if (TrackTotalCookies() > 1000000) return;
                i++;
            }
        }

        private void BuyCrates()
        {
            //Try catch here so that, before the first upgrade is available, it doesn't cause an error.
            //Checking to see that the current cookies is more than 15 could work too
            try
            {
                By crateUpgrade = By.XPath($"//*[@id='upgrade0']");
                if (webDriver.FindElement(crateUpgrade).GetAttribute("Class") == "crate upgrade enabled")
                    WaitThenClick(crateUpgrade);
            }
            catch (Exception) { }
        }

        /* Clicks on whatever crates it can, then clicks on whatever products it can
         */
        private void BuyBuildings()
        {
            if (webDriver.FindElement(nextBuilding.path).GetAttribute("Class") != "product unlocked enabled") return;

            WaitThenClick(nextBuilding.path);

            var updatedPrice = WaitThenClick(nextBuilding.priceField).Text;
            nextBuilding._price = int.Parse(updatedPrice.Replace(",", ""));
            nextBuilding.CalculateValue();

            for (int i = 0; i < buildingLimit / 10; i++)
            {
                if (buildingList[i].value > nextBuilding.value)
                        nextBuilding = buildingList[i];
            }

            buildingsBought++;
            if(buildingsBought == buildingLimit)
            {
                buildingLimit += 10;
                buildingsBought = 0;
                if (buildingLimit > 180) buildingLimit = 180;
            }
        }
        #endregion

        #region Click Focussed
        /* This strategy focusses only on buying upgrades that improve how effective clicking the big cookie is
         * 
         * Always clicking:
         * First, it buys a cursor, then it looks to see if any upgrades have come up
         * If there is a new upgrade, stop buying cursors, save up for the upgrade
         * Once it has bought an upgrade, check if there is another upgrade, if not, repeat process
         */
        [Test]
        public void ClickFocussed()
        {
            StartWebDriver();

            By cursor = By.XPath($"//*[@id='product0']");
            int cursorsBought = 0;
            By crate;

            while (true)
            {
                WaitThenClick(bigCookie);
                if (GetClass(cursor) != "product unlocked enabled") continue;
                WaitThenClick(cursor);
                cursorsBought++;
                if (cursorsBought % 50 != 0) continue;

                try
                {
                    crate = By.XPath($"//*[@id='upgrade0']");

                    if (GetClass(crate) == "crate upgrade" || GetClass(crate) == "crate upgrade enabled")
                        while (true)
                        {
                            WaitThenClick(bigCookie);
                            if (GetClass(crate) == "crate upgrade enabled")
                            {
                                WaitThenClick(crate);
                                break;
                            }
                        }
                }
                catch(Exception){ Console.WriteLine("It looked for a crate when there was none... Probably..."); }
            }
        }

        string GetClass(By path) => webDriver.FindElement(path).GetAttribute("Class");
        #endregion

        IWebElement WaitThenClick(By element, string input = "Click")
        {
            wait.Until(w => w.FindElement(element));

            var webElement = webDriver.FindElement(element);
            if (input == "Click") webElement.Click();
            else                  webElement.SendKeys(input);
            return webElement;
        }

        private int TrackTotalCookies()
        {
            #region🚦Paths
            By stats = By.XPath("//*[@id='statsButton']");
            By totalCookiesField = By.XPath("//*[@id='statsGeneral']/div[3]/div");
            #endregion

            WaitThenClick(stats);
            var totalCookiesText = WaitThenClick(totalCookiesField).Text;
            Console.WriteLine(totalCookiesText);
            var totalCookiesNum = int.Parse(totalCookiesText.Replace(",", ""));
            WaitThenClick(stats);

            Console.WriteLine("Total cookies: " + totalCookiesNum);
            return totalCookiesNum;
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("Iterations ran: " + i);
            //Console.WriteLine("Next building is: " + nextBuilding._name);
            TrackTotalCookies();
            //webDriver.Close();
        }
    }
}