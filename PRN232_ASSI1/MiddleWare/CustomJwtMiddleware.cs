using Common.SettingJWT;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PRN232_ASSI1.MiddleWare
{
    public class CustomJwtMiddleware
    {
        private readonly RequestDelegate _next;


        public CustomJwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            // Nếu token không có tiền tố "Bearer", sử dụng token trực tiếp
            if (!token.StartsWith("Bearer "))
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettingModel.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = JwtSettingModel.Issuer,
                    ValidateAudience = true,
                    ValidAudience = JwtSettingModel.Audience,
                    ValidateLifetime = true,
                };

                ClaimsPrincipal principal;
                try
                {
                    principal = jwtHandler.ValidateToken(token, validationParameters, out var validatedToken);
                    context.User = principal; // Thiết lập User trong HttpContext
                }
                catch
                {
                    // Nếu xác thực không thành công, trả về lỗi 401
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }
            await _next(context);

        }

    }
}
