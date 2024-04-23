using Core.Exceptions;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebPresentation.Controllers;

[ApiController]
[Route("api/ads")]
public class AdsController : ControllerBase
{
    private readonly IAdsService _adsService;

    public AdsController(IAdsService adsService)
    {
        _adsService = adsService;
    }

    [HttpGet]
    public IActionResult GetAds([FromQuery]long productId)
    {
        try
        {
            var ads = _adsService.CalculateAds(productId);
            return Ok(ads);
        }
        catch (ValueNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
