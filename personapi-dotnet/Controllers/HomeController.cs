using Microsoft.AspNetCore.Mvc;

namespace personapi_dotnet.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Error() => View();
}
