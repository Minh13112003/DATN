using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Services
{
    public interface ITokenServices
    {
        string CreateToken(AppUser appUser, List<string> roles);
        string GenerateRefreshToken();
    }
}
