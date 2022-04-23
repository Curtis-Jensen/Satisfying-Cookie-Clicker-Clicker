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

            buildingList = new Building[]
            {
                new Building("Cursor",  By.XPath("//*[@id='product0']"), 15, .1f),
                new Building("Grandma", By.XPath("//*[@id='product1']"), 100, 1)
            };

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

            foreach(Building building in buildingList)
            {
                if (webDriver.FindElement(building._path).GetAttribute("Class") == "product unlocked enabled")
                {
                    webDriver.FindElement(building._path).Click();
                    building.CalculateValue();
                }
            }
        }
        #endregion

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("Minutes past: " + minutesPast);
            webDriver.Close();
        }
    }
}