using Core.Exceptions;
using Core.Services.DemandServices;
using Microsoft.AspNetCore.Mvc;

namespace WebPresentation.Controllers;

[ApiController]
[Route("api/demand")]
public class DemandController : ControllerBase
{
    private readonly IDemandService _demandService;
    private DemandController(IDemandService demandService)
    {
        _demandService = demandService;
    }
    
    [HttpGet]
    public IActionResult GetDemand([FromQuery]long productId, [FromQuery]int daysAmount)
    {
        try {
            var demand = _demandService.Demand(productId, daysAmount);
            return Ok(demand);
        }
        catch (ValueNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}