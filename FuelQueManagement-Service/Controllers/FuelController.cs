using FuelQueManagement_Service.Models;
using FuelQueManagement_Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuelQueManagement_Service.Controllers;

[ApiController]
[Route("[controller]")]
public class FuelController : ControllerBase
{
    //Decleare the fuel service 
    private readonly FuelService _fuelService;
    private readonly FuelStationService _fuelStationService;
    public FuelController(FuelService fuelService, FuelStationService fuelStationService)
    {
        _fuelService = fuelService;
        _fuelStationService = fuelStationService;
    }

    // create a fuel object 
    [HttpPost]
    public async Task<FuelStationModel> Create(FuelModel request) 
    {
        try
        {
            var currentAmount = await _fuelStationService.getCurrentFuelAmount(request.StationsId, request.Type);
            _fuelStationService.UpdateTotalFuelAmount(request.StationsId, request.Amount, request.Type, currentAmount);
            var res = await _fuelService.Create(request);
            return res;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
