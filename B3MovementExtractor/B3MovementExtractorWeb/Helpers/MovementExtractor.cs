using System;
using System.Linq;
using B3MovementExtractorWeb.Models;

namespace B3MovementExtractorWeb.Helpers
{
    public class MovementExtractor
    {
        public static List<string> ExtractEarnings(IEnumerable<string> fileLines, FinancialInstitution institution)
        {
            var earnings = new List<string>();
            var linesInstitution = fileLines.Where(x => x.Contains(institution.Name) || x.Contains(institution.ShortName));
            var linesEarning = linesInstitution.Where(line => !line.Contains(MovementType.IncomeCanceled) && 
                                                              (line.Contains(MovementType.Dividend) ||
                                                              line.Contains(MovementType.InterestOnEquity) ||
                                                              line.Contains(MovementType.Income)));
            foreach (var line in linesEarning)
            {
                var lineSplit = line.Split(";");
                var ticket = lineSplit[3].Split("-")[0].Trim();
                var name = lineSplit[3].Trim();
                var date = lineSplit[1].Trim();
                var type = lineSplit[2].Trim();
                var amountLiquid = decimal.Parse(lineSplit[7].Trim().Replace("R$", ""));
                var count = int.Parse(lineSplit[5].Trim());
                var unitaryValue = decimal.Parse(lineSplit[6].Trim().Replace("R$", ""));
                var amountBrute = unitaryValue * count;
                var irrf = (amountBrute - amountLiquid).ToString().Replace(".", ",");

                type = type.Replace(MovementType.InterestOnEquity, "JSCP");

                if (line.Contains(MovementType.Income) && ticket.Contains("13"))
                {
                    ticket = ticket.Replace("13", "11");
                }
                else if (line.Contains(MovementType.Income) && ticket.Contains("14"))
                {
                    ticket = ticket.Replace("14", "11");
                }

                var str = $"{name}\t{ticket}\t{date}\t{type.ToUpper()}\t{amountBrute.ToString().Replace(".", ",")}" +
                          $"\t{irrf.ToString().Replace(".", ",")}\tBRL\t{institution.Alias}\t{count}" +
                          $"\t{unitaryValue.ToString().Replace(".", ",")}" +
                          $"\t{amountLiquid.ToString().Replace(".", ",")}";

                earnings.Add(str);
            }

            return earnings;
        }

        public static List<string> ExtractOtherWithoutTransferAndEarnings(IEnumerable<string> fileLines)
        {
            var others = fileLines?.Where(line => !line.Contains(MovementType.Dividend) &&
                                                  !line.Contains(MovementType.InterestOnEquity) &&
                                                  !line.Contains(MovementType.Income) &&
                                                  !line.Contains(MovementType.TransferSettlement) &&
                                                  !line.Contains("Aspose.Cells"))
                                                  ?? new List<string>();

            return others.Select(x => x.Replace(";", "\t")).ToList();
        }
    }
}
