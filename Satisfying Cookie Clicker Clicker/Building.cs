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
        string _name;
        public By _path;
        public int _price;
        public float _multiplier;
        public float value;

        public Building(string name, By path, int price, float multiplier)
        {
            _name = name;
            _path = path;
            _price = price;
            _multiplier = multiplier;
        }

        public void CalculateValue()
        {
            value = _multiplier / _price;
        }
    }
}
