using Core.Exceptions;
using Core.Services.PredictionServices;
using Microsoft.AspNetCore.Mvc;

namespace WebPresentation.Controllers;

[ApiController]
[Route("api/prediction")]
public class PredictionController : ControllerBase
{
    private readonly IPredictionService _predictionService;

    public PredictionController(IPredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpGet]
    public IActionResult GetDemand([FromQuery]long productId, [FromQuery]int daysAmount)
    {
        try {
            return Ok(_predictionService);
        }
        catch (ValueNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}