using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DOTNET_RPG.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        private int GetUserId() =>
            int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var dbCharacters = await _context.Characters
            .Include(x => x.Weapon)
            .Include(x => x.Skills)
            .Where(c => c.User!.Id == GetUserId()).ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            var dbCharacter = await _context.Characters
            .Include(x => x.Weapon)
            .Include(x => x.Skills)
            .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());

            serviceResponse.Data = _mapper.Map<GetCharacterDTO>(dbCharacter);
            serviceResponse.Success = dbCharacter is not null;

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            _context.Characters.Add(character);

            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Characters.Where(c => c.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
                if (character is null) throw new Exception($"Invalid character id of {id}");

                _context.Characters.Remove(character);

                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Characters.Where(c => c.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }


            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            try
            {
                var character = await _context.Characters.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

                if (character is null || character.User!.Id != GetUserId())
                    throw new Exception($"Invalid character id of {updatedCharacter.Id}");

                _mapper.Map(updatedCharacter, character);

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }


            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> AddCharacterSkill(AddCharacterSkillDTO newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User!.Id == GetUserId());

                if(character is null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                var skill = await _context.Skills.FirstOrDefaultAsync(c => c.Id == newCharacterSkill.SkillId);
                if(skill is null)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }

                character.Skills!.Add(skill);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDTO>(character);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}