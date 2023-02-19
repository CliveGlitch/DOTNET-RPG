using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOTNET_RPG.DTOs.Skill;
using DOTNET_RPG.DTOs.Weapon;

namespace DOTNET_RPG.DTOs.Character
{
    public class GetCharacterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int HitPoints { get; set; } = 100;
        public int Strenght { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; }
        public RPGClass Class { get; set; } = RPGClass.Thief;
        public GetWeaponDTO? Weapon { get; set; }
        public List<GetSkillDTO>? Skills { get; set; }
        public int Fights { get; set; }
        public int Victories { get; set; } 
        public int Defeats { get; set; }
    }
}