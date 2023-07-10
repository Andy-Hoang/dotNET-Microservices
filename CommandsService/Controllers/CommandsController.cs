using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _commandRepo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"---> GetCommandsForPlatform() id: {platformId}");
            // Business logic
            if(!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }
            // Data access logic
            IEnumerable<Command> commands = _commandRepo.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }


        // {commandId}: way to pass Id into the url route
        // Name: to refer later in create method CreatedAtRoute()
        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
             Console.WriteLine($"---> GetCommandForPlatform() platformId: {platformId} / commandId: {commandId}");
            // Business logic
            if(!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }
            // Data access logic
            Command command = _commandRepo.GetCommand(platformId, commandId);

            return Ok(_mapper.Map<CommandReadDto>(command));
        } 

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
             Console.WriteLine($"---> CreateCommandForPlatform() platformId: {platformId}");
            if(!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandDto);
            _commandRepo.CreateCommand(platformId, command);
            _commandRepo.SaveChanges();

            var commandReadDto =  _mapper.Map<CommandReadDto>(command);     //command here after SaveChanges() will have Id

            return CreatedAtRoute(nameof(GetCommandForPlatform), 
                new {platformId = platformId, commandId = commandReadDto.Id}, commandReadDto );
        } 
    }
}