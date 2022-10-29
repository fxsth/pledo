using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServerController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<AccountController> _logger;

    public ServerController(UnitOfWork unitOfWork, ILogger<AccountController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Server>> Get()
    {
        return _unitOfWork.ServerRepository.GetAll();
    }
}