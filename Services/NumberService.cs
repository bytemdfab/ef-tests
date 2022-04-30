using ConsoleApp1.Models.Service;
using System;
using System.Linq;

namespace ConsoleApp1.Service
{
    public class NumberService
    {
        public static int GetNewNumber(string documentName)
        {
            using (StoreDbContext db = new StoreDbContext())
            {
                var lastNumberRow = db.LastDocumentNumbers.Where(x => x.DocumentName == documentName).FirstOrDefault();

                if (lastNumberRow == null)
                {
                    lastNumberRow = new LastDocumentNumber { DocumentName = documentName, LastUsedNumber = 0 };
                    db.LastDocumentNumbers.Add(lastNumberRow);
                }

                int newNumber = lastNumberRow.LastUsedNumber + 1;

                lastNumberRow.LastUsedNumber = newNumber;
                db.SaveChanges();

                return newNumber;

            }

            throw new Exception($"Can't get new number for {documentName}");
        }
    }
}
