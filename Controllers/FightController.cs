using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOTNET_RPG.DTOs.Fight;
using Microsoft.AspNetCore.Mvc;

namespace DOTNET_RPG.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService fightService;
        public FightController(IFightService fightService)
        {
            this.fightService = fightService;         
        }

        [HttpPost("Weapon")]
        public async Task<ActionResult<ServiceResponse<WeaponAttackDTO>>> WeaponAttack(WeaponAttackDTO request)
        {
            return Ok(await fightService.WeaponAttack(request));
        }

        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<SkillAttackDTO>>> SkillAttack(SkillAttackDTO request)
        {
            return Ok(await fightService.SkillAttack(request));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<FightResultDTO>>> Fight(FightRequestDTO request)
        {
            return Ok(await fightService.Fight(request));
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<HighScoreDTO>>>> GetHighscore()
        {
            return Ok(await fightService.GetHighscore());
        }
    }
}