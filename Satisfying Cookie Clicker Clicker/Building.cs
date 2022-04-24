using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satisfying_Cookie_Clicker_Clicker
{
    class Building
    {
        public string _name;
        public By path;
        public By priceField;
        public int _price;
        public float _multiplier;
        public float value;

        private bool debug = false;

        public Building(string name, int buildingNumber, int price, float multiplier)
        {
            _name = name;
            path =       By.XPath($"//*[@id='product{buildingNumber}']");
            priceField = By.XPath($"//*[@id='productPrice{buildingNumber}']");
            _price = price;
            _multiplier = multiplier;
            CalculateValue();
        }

        public void CalculateValue()
        {
            value = _multiplier / _price;
            if (debug)
            {
                Console.WriteLine(_name + "'s value: " + value);
                Console.WriteLine("Multiplier: " + _multiplier);
                Console.WriteLine("Price: " + _price);
            }
        }
    }
}
