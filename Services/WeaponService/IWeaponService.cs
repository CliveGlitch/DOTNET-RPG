using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOTNET_RPG.DTOs.Weapon;

namespace DOTNET_RPG.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon);
    }
}