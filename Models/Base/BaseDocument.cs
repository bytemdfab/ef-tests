using ConsoleApp1.Service;
using System;

namespace ConsoleApp1.Models.Base
{
    public class BaseDocument : BaseEntity
    {
        public string Number { get; set; }
        public DateTime Date { get; set; }

        public void BeforeSaving()
        {
            if (Date == DateTime.MinValue)
            {
                Date = DateTime.Now;
            }

            if (String.IsNullOrEmpty(Number))
            {
                Number = NumberService.GetNewNumber(this.GetType().Name).ToString("ST-000000");
            }
        }
    }
}
