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

            Console.WriteLine("Adicionar o cabeçalho nos arquivos de proventos? [S]im | [N]ão (Padrão: Não)");

            var isHeader = Console.ReadLine()?.ToUpper() == "S" ? true : false;
            var institutions = new List<FinancialInstitution>
            {
                new FinancialInstitution { Name = "XP INVESTIMENTOS CCTVM S/A", ShortName = "XP INVESTIMENTOS CCTVM S/A", Alias = "RICO" },
                new FinancialInstitution { Name = "CLEAR CORRETORA - GRUPO XP", ShortName = "CLEAR CORRETORA - GRUPO XP", Alias = "CLEAR" },
                new FinancialInstitution { Name = "INTER DISTRIBUIDORA DE TITULOS E VALORES MOBILIARIOS LTDA", ShortName = "INTER DTVM LTDA", Alias = "INTER" },
            };
            var fileCsvAllLines = File.ReadAllLines(pathFileCsv, Encoding.UTF8);
            var inputSplit = pathFileCsv.Contains('/') ? pathFileCsv.Split("/") : pathFileCsv.Split(@"\");
            var pathSaveCsv = pathFileCsv.Replace(inputSplit[^1], "");
            var logs = new List<string>();
            var countLine = 0;
            var countTransfer = fileCsvAllLines.Count(x => x.Contains(MovementType.TransferSettlement));

            Console.WriteLine("\nArquivos gerados:");

            foreach (var institution in institutions)
            {
                var dividends = MovementExtractor.ExtractEarnings(fileCsvAllLines, institution, isHeader);
                var nomeFile = $"{institution.Alias} Proventos {now}.csv";
                var fileEarning = pathSaveCsv + nomeFile;

                File.WriteAllLines(fileEarning, dividends, Encoding.UTF8);

                Console.WriteLine(fileEarning);
                countLine += dividends.Count;
                logs.Add($"\t- Proventos {institution.Alias}: {dividends.Count}");
            }

            var others = MovementExtractor.ExtractOtherWithoutTransferAndEarnings(fileCsvAllLines);
            var fileOthers = $"{pathSaveCsv}others {now}.csv";

            Console.WriteLine(fileOthers);
            File.WriteAllLines(fileOthers, others, Encoding.UTF8);
            countLine += others.Count;
            logs.Add($"\t- Others: {others.Count}");
            logs.Add($"\t- {MovementType.TransferSettlement}: {countTransfer}");
            countLine += countTransfer;
            logs.Add($"Total: {countLine}");
            Console.WriteLine("\nLogs:");
            Console.WriteLine(string.Join("\n", logs));
        }
    }
}