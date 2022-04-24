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
        private ChromeDriver webDriver;
        private WebDriverWait wait;
        private int minutesPast;
        private By bigCookie = By.XPath("//*[@id='bigCookie']");
        private Building[] buildingList;
        private Building nextBuilding;

        #region🖥StartUp Browser
        public void StartWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument(@"load-extension=C:\Users\me\Desktop\AdBlocker");

            webDriver = new ChromeDriver(
                @"C:\Users\me\source\repos\Satisfying Cookie Clicker Clicker\WebDrivers",
                options);
            wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));

            webDriver.Url = "https://orteil.dashnet.org/cookieclicker/";

            CloseAdBlockerTab();
            SetPreferences();
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
            var options =         By.XPath("//*[@id='prefsButton']");
            var numbers =         By.XPath("//*[@id='numbersButton']");
            var altFont =         By.XPath("//*[@id='monospaceButton']");
            var shortNumbers =    By.XPath("//*[@id='formatButton']");
            var fastNotes =       By.XPath("//*[@id='notifsButton']");
            var extraButtons =    By.XPath("//*[@id='extraButtonsButton']");
            var bakeryName =      By.XPath("//*[@id='bakeryName']");
            var bakeryNameInput=  By.XPath("//*[@id='bakeryNameInput']");
            var bakeryNameConfirm=By.XPath("//*[@id='promptOption0']");

            var computerCookieConfirm = By.XPath("/html/body/div[1]/div/a[1]");
            #endregion

            Thread.Sleep(1000);
            webDriver.FindElement(computerCookieConfirm).Click();
            Thread.Sleep(100);

            //This could all be a for loop if I make a list in the constructors.  Would definitely need more comments though
            webDriver.FindElement(options).Click();
            webDriver.FindElement(numbers).Click();
            webDriver.FindElement(altFont).Click();
            webDriver.FindElement(shortNumbers).Click();
            webDriver.FindElement(fastNotes).Click();
            webDriver.FindElement(extraButtons).Click();
            webDriver.FindElement(options).Click();

            webDriver.FindElement(bakeryName).Click();
            webDriver.FindElement(bakeryNameInput).SendKeys("Curtis");
            webDriver.FindElement(bakeryNameConfirm).Click();
        }
        #endregion

        #region🍪Main Code
        /* product0 is cursor
         * product1 is grandma, etc
         */
        [Test]
        public void MainLoop()
        {
            StartWebDriver();
            int i = 0;

            #region🏛Building List
            buildingList = new Building[]
            {
                new Building("Cursor", buildingNumber: 0, price: 15, multiplier: .1f),
                new Building("Grandma", buildingNumber: 1, price: 100, multiplier: 1),
                new Building("Farm", buildingNumber: 2, price: 1100, multiplier: 8),
                new Building("Mine", buildingNumber: 3, price: 12000, multiplier: 47),
                new Building("Factory", buildingNumber: 4, price: 130000, multiplier: 260),
                new Building("Bank", buildingNumber: 5, price: 1400000, multiplier: 1400),
                new Building("Temple", buildingNumber: 6, price: 20000000, multiplier: 7800),
                new Building("Wizard Tower", buildingNumber: 7, price: 330000000, multiplier: 44000),
                //new Building("Farm", buildingNumber: 8, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 9, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 10, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 11, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 12, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 13, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 14, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 15, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 16, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 17, price: 1100, multiplier: 8),
                //new Building("Farm", buildingNumber: 18, price: 1100, multiplier: 8),
            };
            nextBuilding = buildingList[1];
            #endregion

            while (i < 99)
            {
                webDriver.FindElement(bigCookie).Click();
                i++;
            }

            while (true)
            {
                webDriver.FindElement(bigCookie).Click();
                ClickOnUpgrades();
                i++;
            }
        }

        /* Clicks on whatever crates it can, then clicks on whatever products it can
         */
        private void ClickOnUpgrades()
        {
            //Try catch here so that, before the first upgrade is available, it doesn't cause an error.
            //Checking to see that the current cookies is more than 15 could work too
            try
            {
                var crateUpgrade = By.XPath($"//*[@id='upgrade0']");
                if (webDriver.FindElement(crateUpgrade).GetAttribute("Class") == "crate upgrade enabled")
                    webDriver.FindElement(crateUpgrade).Click();
            }
            catch(Exception){}

            if (webDriver.FindElement(nextBuilding.path).GetAttribute("Class") == "product unlocked enabled")
            {
                webDriver.FindElement(nextBuilding.path).Click();

                var updatedPrice = webDriver.FindElement(nextBuilding.priceField).Text;
                nextBuilding._price = int.Parse(updatedPrice.Replace(",", ""));
                nextBuilding.CalculateValue();

                for (int i = 0; i < buildingList.Length - 1; i++)
                {
                    if (buildingList[i].value > buildingList[i + 1].value)
                        nextBuilding = buildingList[i];
                    else nextBuilding = buildingList[i + 1];
                }
            }
        }
        #endregion

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("Minutes past: " + minutesPast);
            Console.Write("Second target: " + nextBuilding._name);
            //webDriver.Close();
        }
    }
}