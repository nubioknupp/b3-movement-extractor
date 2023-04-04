using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using B3MovementExtractorWeb.Models;
using B3MovementExtractorWeb.Helpers;
using Aspose.Cells;
using System.Text;

namespace B3MovementExtractorWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    public async Task<IActionResult> EnviarArquivo(List<IFormFile> arquivos)
    {
        var contents = await ExcelHelper.ToStringFormatCSV(arquivos.First());
        var others = MovementExtractor.ExtractOtherWithoutTransferAndEarnings(contents);
        var institutions = new List<FinancialInstitution>
                               {
                                   new FinancialInstitution
                                   {
                                       Name = "CLEAR CORRETORA - GRUPO XP",
                                       ShortName = "CLEAR CORRETORA - GRUPO XP",
                                       Alias = "CLEAR"
                                   },
                                   new FinancialInstitution
                                   {
                                       Name = "XP INVESTIMENTOS CCTVM S/A",
                                       ShortName = "XP INVESTIMENTOS CCTVM S/A",
                                       Alias = "RICO"
                                   },
                                   new FinancialInstitution
                                   {
                                       Name = "INTER DISTRIBUIDORA DE TITULOS E VALORES MOBILIARIOS LTDA",
                                       ShortName = "INTER DTVM LTDA", Alias = "INTER"
                                   }
                               };

        foreach (var institution in institutions)
        {
            var dividends = MovementExtractor.ExtractEarnings(contents, institution);

            ViewData[$"Earnings{institution.Alias}"] = string.Join("\n", dividends);
        }

        ViewData["FinancialInstitutions"] = institutions;
        ViewData["ExtractOtherWithoutTransferAndEarnings"] = string.Join("\n", others);

        return View(ViewData);
    }
}

