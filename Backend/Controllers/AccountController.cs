using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.DateTimeVietNam;
using DoAnTotNghiep.Helper.StringHelper;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenServices _tokenServices;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailAuthenticationServices _emailService;
        private readonly DatabaseContext _database;
        

        public AccountController(UserManager<AppUser> userManager, ITokenServices tokenServices, SignInManager<AppUser> signInManager, IEmailAuthenticationServices emailService, DatabaseContext database)
        {
            _userManager = userManager;
            _tokenServices = tokenServices;
            _signInManager = signInManager;
            _emailService = emailService;
            _database = database;
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.UserName);
                if (user == null) return Unauthorized("UserName or PassWord is incorrect");

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

                if (!result.Succeeded) return Unauthorized(new { message = "UserName or PassWord is incorrect" });
                /*if (user.OTP == null || user.OTP != loginDTO.OTP)
                {
                    return Unauthorized(new { message = "Sai hoặc không tìm thấy OTP" });
                }
                if (user.OtpCreatedAt.HasValue && user.OtpCreatedAt.Value.AddMinutes(5) < DateTime.UtcNow) return Unauthorized(new { message = "OTP đã hết hạn" });
                user.OTP = null;
                user.OtpCreatedAt = null;
                await _userManager.UpdateAsync(user);*/
                var Role = await _userManager.GetRolesAsync(user);
                var roleString = string.Join(", ", Role);
                var refreshToken = _tokenServices.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
                await _userManager.UpdateAsync(user);
                return Ok(new UserToken
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenServices.CreateToken(user, Role.ToList()),
                    Roles = roleString,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                    Image = user.Image,
                    NickName = user.Nickname
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountLoginDTO accountLoginDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var age = DateTimeHelper.CalculateAge(accountLoginDTO.Birthday);
                var verify = StringHelper.GenerateRandomUppercaseString();
                var appUser = new AppUser
                {
                    UserName = accountLoginDTO.UserName,
                    Email = accountLoginDTO.EmailAddress,
                    Age = age,
                    PhoneNumber = accountLoginDTO.PhoneNumber,
                    VerificationCode = verify
                };
                var createUser = await _userManager.CreateAsync(appUser, accountLoginDTO.Password!);
                if (createUser.Succeeded)
                {
                    
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        
                        return Ok(new UserToken
                        {
                            Email = appUser.Email,
                            UserName = appUser.UserName,
                            VerifyCode = appUser.VerificationCode
                        });
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Mật khẩu phải chứa ít nhất 1 kí tự Hoa, 1 kí tự đặc biệt, 1 kí tự số" });
                    }
                }
                else
                {
                    return StatusCode(501, new { message = "Tài khoản hoặc mật khẩu đã được đăng kí" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(502, ex.Message);
            }
        }

        [HttpPost("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AccountLoginDTO accountLoginDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var age = DateTimeHelper.CalculateAge(accountLoginDTO.Birthday);
                var appUser = new AppUser
                {
                    UserName = accountLoginDTO.UserName,
                    Email = accountLoginDTO.EmailAddress,
                    Age = age,
                    PhoneNumber = accountLoginDTO.PhoneNumber,
                };
                var createUser = await _userManager.CreateAsync(appUser, accountLoginDTO.Password!);
                if (createUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "Admin");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new UserToken
                        {
                            Email = appUser.Email,
                            UserName = appUser.UserName,
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete/{userName}")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            try
            {

                var user = await _userManager.FindByNameAsync(userName);


                if (user == null)
                {
                    return NotFound(new { Message = $"User with username '{userName}' not found." });
                }

                var deleteResult = await _userManager.DeleteAsync(user);

                if (deleteResult.Succeeded)
                {
                    return Ok(new { Message = $"User '{userName}' has been successfully deleted." });
                }
                else
                {
                    return StatusCode(500, deleteResult.Errors);
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu xảy ra ngoại lệ
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("check-role/{userName}")]
        public async Task<IActionResult> CheckUserRole(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound(new { Message = $"User '{userName}' not found." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.IsNullOrEmpty()) await _userManager.AddToRoleAsync(user, "Admin");
            return Ok(new { User = user.UserName, Roles = roles });
        }
        [HttpGet("check-role-user/{userName}")]
        public async Task<IActionResult> CheckRoleUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound(new { Message = $"User '{userName}' not found." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.IsNullOrEmpty()) await _userManager.AddToRoleAsync(user, "User");
            return Ok(new { User = user.UserName, Roles = roles });
        }
        [HttpPost("userinfor")]
        public async Task<IActionResult> GetUserInfor([FromBody] UserRequestDTOs userRequestDTOs)
        {
            if (string.IsNullOrEmpty(userRequestDTOs.UserName))
            {
                return BadRequest(new { message = "Username is required" });
            }

            var user = await _userManager.FindByNameAsync(userRequestDTOs.UserName);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleString = string.Join(", ", roles);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Age,
                Roles = roleString,
                Nickname = user.Nickname ?? null,
                Image = user.Image ?? null
            });
        }

        [HttpPost("SendOTP/{username}/{type}")]
        public async Task<IActionResult> SendOtpToEmail(string username, int type)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || string.IsNullOrEmpty(user.Email)) return NotFound(new {message = "Không tìm thấy email người dùng hoặc trống thông tin"});

            var otp = new Random().Next(100000, 999999).ToString();
            user.OTP = otp;
            user.OtpCreatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var subject = "Mã OTP";
            var body = $"Mã OTP của bạn là: <b>{otp}</b> (có hiệu lực trong 5 phút).";
            switch (type)
            {
                case 1:
                    subject = "Mã OTP đăng nhập";
                    body = $"Mã OTP đăng nhập của bạn là: <b>{otp}</b> (có hiệu lực trong 5 phút).";
                    break;
                case 2:
                    subject = "Mã OTP đổi mật khẩu";
                    body = $"Bạn đang yêu cầu đổi mật khẩu. Mã OTP: <b>{otp}</b> (có hiệu lực trong 5 phút).";
                    break;
                case 3:
                    subject = "Mã OTP đổi thông tin ";
                    body = $"Đây là mã xác minh đổi thông tin: <b>{otp}</b> (có hiệu lực trong 5 phút).";
                    break;
                default:
                    break;
            }

            await _emailService.SendEmailAsync(user.Email, subject, body);
            return Ok(new {message = "Mã OTP của bạn đã được gửi đi"});
        }

        [HttpPost("VerifyOTP/{username}/{otp}")]
        public async Task<IActionResult> VerifyOTP(string username, string otp)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user.OTP == null || user.OTP != otp)
                {
                    return Unauthorized(new { message = "Sai hoặc không tìm thấy OTP" });
                }
                if (user.OtpCreatedAt.HasValue && user.OtpCreatedAt.Value.AddMinutes(5) < DateTime.UtcNow) return Unauthorized(new { message = "OTP đã hết hạn" });
                user.OTP = null;
                user.OtpCreatedAt = null;
                await _userManager.UpdateAsync(user);
                return Ok(new { message = "Xác thực thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO request)
        {
            if (request is null || string.IsNullOrEmpty(request.RefreshToken) || string.IsNullOrEmpty(request.UserName))
                return BadRequest("Invalid client request");

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenServices.CreateToken(user, roles.ToList());
            var newRefreshToken = _tokenServices.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new UserToken
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = newAccessToken,
                Roles = string.Join(", ", roles),
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            });
        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTOs changePasswordDTOs)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(changePasswordDTOs.UserName);
                if (user != null)
                {
                    var removeresult = await _userManager.RemovePasswordAsync(user);
                    if (removeresult.Succeeded) 
                    {
                        var addresult = await _userManager.AddPasswordAsync(user, changePasswordDTOs.Password);
                        if (addresult.Succeeded)
                        {
                            return Ok(new { message = "Đổi mật khẩu thành công, vui lòng đăng nhập lại" });
                        }
                    }
                }
                return BadRequest(new { message = "Đã có lỗi xảy ra" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("GetVip")]
        public async Task<IActionResult> GetVip()
        {
            var UserName = User.Identity?.Name;
            if (UserName == null) // Người dùng chưa đăng nhập
            {
                return Ok(false);
            }
            var user = await _userManager.FindByNameAsync(UserName!);
            if (user != null) 
            {
                return Ok(user.IsVip);
            }
            return BadRequest(new { message = "Không tìm thấy người dùng" });
        }
        [Authorize]
        [HttpPut("ChangeAccountInfor")]
        public async Task<IActionResult> ChangeAccountInfor(UserToken userDTO)
        {

            var UserName = string.IsNullOrEmpty(userDTO.UserName)
                ? User.Identity?.Name
                : userDTO.UserName;
            var user = await _userManager.FindByNameAsync(UserName!);

            if (user != null)
            {
                bool isUpdated = false;

                if (!string.IsNullOrEmpty(userDTO.NickName))
                {
                    user.Nickname = userDTO.NickName;
                    isUpdated = true;
                }
                if (!string.IsNullOrEmpty(userDTO.Phonenumber))
                {
                    user.PhoneNumber = userDTO.Phonenumber;
                    isUpdated = true;
                }
                if (userDTO.isVip == true)
                {
                    var now = DateTimeHelper.GetDateTimeVnNowWithDateTimeUTC();
                    //Nạp lần đầu hoặc lâu mới nạp
                    if (user.TimeTopUp == null || user.ExpirationTime < now)
                    {
                        user.TimeTopUp = DateTimeHelper.GetDateTimeVnNowWithDateTimeUTC();
                        user.ExpirationTime = user.TimeTopUp.Value.AddDays(3);
                    }
                    else //Nạp duy trì
                    {
                        user.TimeTopUp = DateTimeHelper.GetDateTimeVnNowWithDateTimeUTC();
                        user.ExpirationTime = user.ExpirationTime.Value.AddDays(3);
                    }
                    user.IsVip = true;
                    isUpdated = true;
                }

                if (!string.IsNullOrEmpty(userDTO.Password))
                {
                    if (userDTO.OTP == user.OTP)
                    {
                        var removeresult = await _userManager.RemovePasswordAsync(user);
                        if (removeresult.Succeeded)
                        {
                            var addresult = await _userManager.AddPasswordAsync(user, userDTO.Password);
                            if (!addresult.Succeeded)
                            {
                                return BadRequest(new { message = "Đổi mật khẩu thất bại." });
                            }
                            isUpdated = true;
                        }
                    }else return BadRequest(new { message = "Mã OTP không chính xác" });
                
                }
                if (!string.IsNullOrEmpty(userDTO.Email))
                {
                    if (!string.IsNullOrEmpty(userDTO.VerifyCode))
                    {
                        if (userDTO.VerifyCode == user.VerificationCode)
                        {
                            user.Email = userDTO.Email;
                            isUpdated = true;
                        }
                        else
                        {
                            return BadRequest(new { message = "Sai mã xác thực Email." });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Thiếu mã xác thực Email." });
                    }
                }
                if (isUpdated)
                {
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        return BadRequest(new { message = "Cập nhật thông tin thất bại." });
                    }
                }

                return Ok(new { message = "Cập nhật thông tin thành công." });
            }

            return NotFound(new { message = "Không tìm thấy người dùng." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var usersInRoleUser = from user in _database.Users
                                  join userRole in _database.UserRoles on user.Id equals userRole.UserId
                                  join role in _database.Roles on userRole.RoleId equals role.Id
                                  where role.Name == "User"
                                  select new
                                  {
                                      user.UserName,
                                      user.Email,
                                      user.PhoneNumber,
                                      user.IsVip,
                                      user.Image,
                                      user.Nickname
                                  };

            var result = await usersInRoleUser.ToListAsync();
            return Ok(result);
        }
        /*[HttpPost("UpNotification")]
        public async Task<IActionResult> UpNotification(UserToken userDTO)
        {
            if(string.IsNullOrEmpty(userDTO.UserName) || string.IsNullOrEmpty(userDTO.Email) || string.IsNullOrEmpty(userDTO.NewEmail) || string.IsNullOrEmpty(userDTO.VerfiCode))
            {
                return BadRequest(new { message = "Thiếu thông tin" });
            }
            var user = await _userManager.FindByNameAsync(userDTO.UserName);
            if (user == null)
            {
                return BadRequest(new { message = "Không tìm thấy người dùng" });
            }
            if (user.VerificationCode != userDTO.VerfiCode) return BadRequest(new { message = "Sai mã xác thực" });

        }*/
        // Muốn tk User tự đổi hay để admin đổi giùm hắn ?

    }
}