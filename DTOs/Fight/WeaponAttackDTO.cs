using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DOTNET_RPG.DTOs.Fight
{
    public class WeaponAttackDTO
    {
        public int AttackerId { get; set; }
        public int OpponentId { get; set; }
    }
}