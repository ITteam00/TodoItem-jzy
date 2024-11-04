//using Microsoft.AspNetCore.Mvc;
//using ToDoList.Api.Models;
//using ToDoList.Api.Services;

//namespace ToDoList.Api.Controllers;

//[ApiController]
//[Route("api/v2/[controller]")]
//public class ToDoItemsV2Controller : ControllerBase
//{
//    private readonly ILogger<ToDoItemsV2Controller> _logger;
//    private readonly IToDoItemsService _toDoItemsService;

//    public ToDoItemsV2Controller(ILogger<ToDoItemsV2Controller> logger, IToDoItemsV2Service toDoItemsService)
//    {
//        _logger = logger;
//        _toDoItemsService = toDoItemsService;
//    }

//    [HttpGet()]
//    public async Task<ActionResult<List<ToDoItemDto>>> Get()
//    {
//        var result = await _toDoItemsService.GetAllAsync();
//        return Ok(result);
//    }

//    [HttpGet("{id}")]
//    public async Task<ActionResult<ToDoItemDto>> Get(string id)
//    {
//        var result = await _toDoItemsService.GetAsync(id);
//        if (result is null)
//        {
//            return NotFound();
//        }

//        return Ok(result);

//    }

//    [HttpPost()]
//    public async Task<ActionResult<ToDoItemDto>> Post(ToDoItemCreateRequest createRequest)
//    {
//        var toDoItemDto = new ToDoItemDto
//        {
//            Description = createRequest.Description,
//            Id = Guid.NewGuid().ToString(),
//            Favorite = createRequest.Favorite,
//            Done = createRequest.Done,
//            CreatedTime = DateTimeOffset.Now

//        };
//        await _toDoItemsService.CreateAsync(toDoItemDto);

//        return Created("", toDoItemDto);

//    }

//    [HttpPut("{id}")]
//    public async Task<ActionResult<ToDoItemDto>> Put(string id, [FromBody] ToDoItemDto toDoItemDto)
//    {
//        if (id != toDoItemDto.Id)
//        {
//            return BadRequest("ToDo Item ID in url must be equal to request body");
//        }
//        bool isCreate = false;
//        var existingItem = await _toDoItemsService.GetAsync(id);
//        if (existingItem is null)
//        {
//            isCreate = true;
//            await _toDoItemsService.CreateAsync(toDoItemDto);
//        }
//        else
//        {
//            toDoItemDto.CreatedTime = existingItem.CreatedTime;
//            await _toDoItemsService.ReplaceAsync(id, toDoItemDto);
//        }

//        return isCreate ? Created("", toDoItemDto) : Ok(toDoItemDto);

//    }

//    [HttpDelete("{id}")]
//    public async Task<ActionResult> Delete(string id)
//    {
//        var found = await _toDoItemsService.RemoveAsync(id);
//        return found ? NoContent() : NotFound();

//    }
//}
