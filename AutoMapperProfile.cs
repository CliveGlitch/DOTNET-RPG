using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOTNET_RPG.DTOs.Fight;
using DOTNET_RPG.DTOs.Skill;
using DOTNET_RPG.DTOs.Weapon;

namespace DOTNET_RPG
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDTO>();
            CreateMap<AddCharacterDTO, Character>();
            CreateMap<UpdateCharacterDTO, Character>();
            CreateMap<AddWeaponDTO, Weapon>();
            CreateMap<Weapon,GetWeaponDTO>();
            CreateMap<Skill, GetSkillDTO>();
            CreateMap<Character, HighScoreDTO>();
        }
    }
}