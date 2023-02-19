using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DOTNET_RPG.DTOs.Weapon;

namespace DOTNET_RPG.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        public DataContext _context;
        public IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private int GetUserId() => 
            int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.User!.Id == GetUserId());

                if(character is null){
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }

                var weapon = _mapper.Map<Weapon>(newWeapon);

                _context.Weapons.Add(weapon);
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