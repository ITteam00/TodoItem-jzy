using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class ToDoItemsV2Controller : ControllerBase
{
    private readonly ILogger<ToDoItemsV2Controller> _logger;
    private readonly IToDoItemsV2Service _toDoItemsService;

    public ToDoItemsV2Controller(ILogger<ToDoItemsV2Controller> logger, IToDoItemsV2Service toDoItemsService)
    {
        _logger = logger;
        _toDoItemsService = toDoItemsService;
    }

    [HttpGet()]
    public async Task<ActionResult<List<ToDoItemV2Obj>>> Get()
    {
        var result = await _toDoItemsService.GetAllToDoItemsInOneDay(DateTime.Today);
        return Ok(result);
    }




    [HttpGet("{id}")]
    public async Task<ActionResult<ToDoItemV2Obj>> Get(string id)
    {
        var result = await _toDoItemsService.GetToDoItemById(id);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost()]
    public async Task<ActionResult<ToDoItemV2Obj>> Post(ToDoItemCreateRequest createRequest)
    {
        var toDoItemDto = new ToDoItemV2Obj(
            Guid.NewGuid().ToString(),
            createRequest.Description,
            createRequest.Done,
            createRequest.Favorite,
            DateTimeOffset.Now.Date,
            DateTimeOffset.Now.Date,
            1,
            DateTimeOffset.Now.Date,
            DueDateRequirementType.Fewest
        );
        var createdItem = await _toDoItemsService.CreateToDoItem(toDoItemDto);

        return Created("", createdItem);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ToDoItemV2Obj>> Put(string id, [FromBody] ToDoItemV2Obj toDoItemDto)
    {
        toDoItemDto.Id = id;
        if (id != toDoItemDto.Id)
        {
            return BadRequest("ToDo Item ID in URL must be equal to request body!!!");
        }

        var updatedItem = await _toDoItemsService.EditToDoItem(toDoItemDto);

        return Ok(updatedItem);
    }


}
