using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using B3MovementExtractorWeb.Models;
using B3MovementExtractorWeb.Helpers;

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



        //monta a ViewData que será exibida na view como resultado do envio 
        ViewData["Resultado"] = $"arquivos foram enviados ao servidor, ";


        //retorna a viewdata
        return View(ViewData);
    }
}

