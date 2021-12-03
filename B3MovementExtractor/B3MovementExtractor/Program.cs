using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace B3MovementExtractor // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var now = DateTime.Now.ToString("yyyyMMddTHHmmss");

            Console.WriteLine("Caminho do Arquivo separado por ; (Padrão: /Users/nubioknupp/Downloads/mov.csv)");

            var pathFileCsv = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(pathFileCsv)) pathFileCsv = "/Users/nubioknupp/Downloads/mov.csv";

            var institutions = new List<FinancialInstitution>
            {
                new FinancialInstitution { Name = "XP INVESTIMENTOS CCTVM S/A", Alias = "RICO" },
                new FinancialInstitution { Name = "CLEAR CORRETORA - GRUPO XP", Alias = "CLEAR" },
                new FinancialInstitution { Name = "INTER DISTRIBUIDORA DE TITULOS E VALORES MOBILIARIOS LTDA", Alias = "INTER" },
            };
            var fileCsvAllLines = File.ReadAllLines(pathFileCsv, Encoding.UTF8);
            var inputSplit = pathFileCsv.Split("/");
            var pathSaveCsv = pathFileCsv.Replace(inputSplit[^1], "");

            foreach (var institution in institutions)
            {
                var dividends = MovementExtractor.ExtractEarnings(fileCsvAllLines, institution);
                var nomeFile = $"{institution.Alias} Proventos {now}.csv";

                File.WriteAllLines(pathSaveCsv + nomeFile, dividends, Encoding.UTF8);
            }

            var others = MovementExtractor.ExtractOtherWithoutTransferAndEarnings(fileCsvAllLines);

            File.WriteAllLines($"{pathSaveCsv}others {now}.csv", others, Encoding.UTF8);
        }
    }
}