using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOTNET_RPG.DTOs.Fight;

namespace DOTNET_RPG.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        public FightService(DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
            
        }

        public async Task<ServiceResponse<FightResultDTO>> Fight(FightRequestDTO request)
        {
            var response = new ServiceResponse<FightResultDTO>
            {
                Data = new FightResultDTO()
            };

            try
            {
                var characters = await context.Characters
                    .Include(x => x.Weapon)
                    .Include(x => x.Skills)
                    .Where(x => request.CharacterIds.Contains(x.Id))
                    .ToListAsync();

                bool defeated = false;

                while(!defeated)
                {
                    foreach(var attacker in characters)
                    {
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if(useWeapon && attacker.Weapon is not null)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else if(!useWeapon && attacker.Skills is not null)
                        {
                            var random = new Random().Next(attacker.Skills.Count);
                            var skill = attacker.Skills[random];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }
                        else
                        {
                            response.Data.Log
                                .Add($"{attacker.Name} wasn't able to attack!");
                            continue;
                        }

                        response.Data.Log
                            .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");

                        if(opponent.HitPoints <= 0){
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c => 
                    {
                        c.Fights++;
                        c.HitPoints = 10000;
                    }
                );

                await context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDTO>> SkillAttack(SkillAttackDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();
            try
            {
                var attacker = await context.Characters
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Skills is null)
                    throw new Exception("Something is wrong");

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

                if (skill is null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill!";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await context.SaveChangesAsync();

                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<AttackResultDTO>> WeaponAttack(WeaponAttackDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();
            try
            {
                var attacker = await context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Weapon is null)
                    throw new Exception("Something is wrong");

                int damage = DoWeaponAttack(attacker, opponent);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await context.SaveChangesAsync();

                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {   
            if(attacker.Weapon is null)
                throw new Exception("Attacker does not have a weapon");

            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strenght));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDTO>>> GetHighscore()
        {
            var characters = await context.Characters
                .Where(character => character.Fights > 0)
                .OrderByDescending(character => character.Victories)
                .ThenBy(character => character.Defeats)
                .ToListAsync();

            var response = new ServiceResponse<List<HighScoreDTO>>()
            {
                Data = characters.Select(character => mapper.Map<HighScoreDTO>(character)).ToList()
            };

            return response;
        }
    }
}