using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using urbanMartAPI.BaseEntities;
using urbanMartAPI.Models;
using Serilog;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger; // Use Serilog's ILogger

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;

        // Initialize Serilog logger
        _logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration) // Read settings from appsettings.json
            .CreateLogger();
    }

    public LoginResponse Login(LoginRequest request)
    {
        _logger.Information("Login attempt for user: {Username}", request.Username); // Log login attempt

        // Find user by username
        User user = _userRepository.GetUserByUsername(request.Username);
        if (user == null || user.Password != request.Password) // In production, use password hashing
        {
            _logger.Warning("Authentication failed for user: {Username}", request.Username); // Log failed login attempt
            return null; // Authentication failed
        }

        // Generate token
        string token = GenerateJwtToken(user);

        _logger.Information("User {Username} authenticated successfully. Generating JWT token.", user.Username); // Log successful login

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }

    public RegisterResponse Register(RegisterRequest request)
    {
        _logger.Information("Registration attempt for username: {Username}", request.Username); // Log registration attempt

        // Check if username already exists
        if (_userRepository.GetUserByUsername(request.Username) != null)
        {
            _logger.Warning("Username {Username} is already taken.", request.Username); // Log username already exists
            return null; // Username already taken
        }

        // Check if email already exists
        if (_userRepository.GetUserByEmail(request.Email) != null)
        {
            _logger.Warning("Email {Email} is already registered.", request.Email); // Log email already exists
            return null; // Email already registered
        }

        // Create new user
        User user = new User
        {
            Username = request.Username,
            Password = request.Password, // In production, hash this
            Email = request.Email,
            NicNumber = request.NicNumber,
            Role = Roles.Customer, // Default role
            CreatedBy = "system"
        };

        // Save user
        User createdUser = _userRepository.CreateUser(user);

        _logger.Information("User {Username} registered successfully with role {Role}.", createdUser.Username, createdUser.Role); // Log successful registration

        return new RegisterResponse
        {
            Username = createdUser.Username,
            Email = createdUser.Email,
            Role = createdUser.Role
        };
    }

    private string GenerateJwtToken(User user)
    {
        _logger.Information("Generating JWT token for user: {Username}", user.Username); // Log token generation attempt

        string jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing.");
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        string[] permissionCodes = user.Role switch
        {
            "Admin" => new[] {
                Permissions.ViewAllOrders,
                Permissions.ViewUserOrders,
                Permissions.ViewProductsAdmin,
                Permissions.CreateProduct,
                Permissions.ViewUsers
            },
            "Customer" => new[] {
                Permissions.ViewCart,
                Permissions.AddToCart,
                Permissions.ViewUserOrders,
                Permissions.CreateOrder,
                Permissions.ViewProductsCust
            },
            _ => Array.Empty<string>()
        };

        // Log permissions being assigned to the user
        _logger.Information("User {Username} has the following permissions: {Permissions}", user.Username, string.Join(", ", permissionCodes));

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("NicNumber", user.NicNumber),
            new Claim("PermissionCodes", string.Join(",", permissionCodes)) // Ensure this is set
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        _logger.Information("JWT token successfully generated for user: {Username}", user.Username); // Log success

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



//If your application is simple or if you are in a performance-sensitive environment (where token size matters a lot), storing permissions in a single claim might be the way to go.
//If your application has complex roles, dynamic permissions, or needs granular access control and flexibility, then multiple claims for each permission is the best choice.