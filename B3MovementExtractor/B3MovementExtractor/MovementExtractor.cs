using System;
using System.Linq;

namespace B3MovementExtractor
{
    public class MovementExtractor
    {
        public static List<string> ExtractEarnings(string[] fileLines, FinancialInstitution institution, bool isHeader)
        {
            var earnings = new List<string>();
            var lines = fileLines?.Where(line => line.Contains(MovementType.Dividend) ||
                                                 line.Contains(MovementType.InterestOnEquity) ||
                                                 line.Contains(MovementType.Income))
                                 ?.Where( x => x.Contains(institution.Name)) ?? new List<string>();
            if (isHeader)
            {
                earnings.Add("Nome do Ativo\tTicket do Ativo\tData de Pagamento\tTipo de Evento\tValor líquido\tCorretora\tQuantidade\tValor unitário");
            }

            foreach (var line in lines)
            {
                var lineSplit = line.Split(";");
                var ticket = lineSplit[3].Split("-")[0].Trim();
                var name = lineSplit[3].Trim();
                var date = lineSplit[1].Trim();
                var type = lineSplit[2].Trim();
                var amount = lineSplit[7].Trim().Replace("R$", "");
                var count = lineSplit[5].Trim();
                var unitaryValue = lineSplit[6].Trim().Replace("R$", "");

                type = type.Replace(MovementType.InterestOnEquity, "JRS CAP PROPRIO");

                if (line.Contains(MovementType.Income) && ticket.Contains("13"))
                {
                    ticket = ticket.Replace("13", "11");
                }
                else if (line.Contains(MovementType.Income) && ticket.Contains("14"))
                {
                    ticket = ticket.Replace("14", "11");
                }

                var str = $"{name}\t{ticket}\t{date}\t{type.ToUpper()}\t{amount}\t{institution.Alias}\t{count}\t{unitaryValue}";

                earnings.Add(str);
            }

            return earnings;
        }

        public static List<string> ExtractOtherWithoutTransferAndEarnings(string[] fileLines)
        {
            var others = fileLines?.Where(line => !line.Contains(MovementType.Dividend) &&
                                                  !line.Contains(MovementType.InterestOnEquity) &&
                                                  !line.Contains(MovementType.Income) &&
                                                  !line.Contains(MovementType.TransferSettlement))
                                                  ?? new List<string>();

            return others.Select(x => x.Replace(";", "\t")).ToList();
        }
    }
}
