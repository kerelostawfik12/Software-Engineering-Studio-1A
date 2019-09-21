namespace Studio1BTask.Services
{
    /*public class AuthService
    {
        public readonly string Issuer = "studio1btask";
        public readonly string Audience = "users";
        public readonly string SecurityKey;
        
        
        public AuthService()
        {
            SecurityKey = ReadSecurityKey();
        }

        public string CreateToken()
        {
            // Security key
            var securityKey = SecurityKey;
            
            // Symmetric security key
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            
            // Signing credentials
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            
            // Add claims
            var claims = new List<Claim>
            {
                new Claim("acc_id", 41.ToString())
            };

            // Create token
            var token = new JwtSecurityToken(
                Issuer, 
                Audience,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials,
                claims: claims
            );
            // Return token
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string ReadSecurityKey()
        {
            return Environment.GetEnvironmentVariable("SQLAZURECONNSTR_AUTH_SECURITY_KEY") ??
                File.ReadAllText("auth-security-key.txt");
        }
    }*/
}