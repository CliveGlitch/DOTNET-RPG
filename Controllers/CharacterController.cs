using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DOTNET_RPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> Get(){
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("GetById_{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> GetById(int id){
            var response = await _characterService.GetCharacterById(id);

            if(!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> AddCharacter(AddCharacterDTO newCharacter){
            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> UpdateCharacter(UpdateCharacterDTO updatedCharacter){
            var response = await _characterService.UpdateCharacter(updatedCharacter);

            if(!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> DeleteCharacter(int id){
            var response = await _characterService.DeleteCharacterById(id);

            if(!response.Success) return NotFound(response);
            return Ok(response);
        }
    }
}