using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOTNET_RPG.DTOs.Weapon;
using DOTNET_RPG.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOTNET_RPG.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeaponController : ControllerBase
    {
        public IWeaponService _weaponService { get; }
        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;

        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> AddWeapon(AddWeaponDTO newWeapon)
        {
            return Ok(await _weaponService.AddWeapon(newWeapon));
        }
    }
}