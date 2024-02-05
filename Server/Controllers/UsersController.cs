using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Models;
using Server.ModelDTO;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users");

                var users = await _context
                .Users
                .Include(u => u.Profile)
                .Include(u => u.Roles)
                .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        [HttpGet("FindByRole")]
        public async Task<IActionResult> FindByRole([FromQuery] string role)
        {
            try
            {
                _logger.LogInformation("Find users by Role");

                if (string.IsNullOrWhiteSpace(role))
                {
                    return BadRequest("Role query parameter is required.");
                }

                var users = await _context.Users
                                          .Where(u => u.Roles.Any(x => x.Name == role))
                                          .Include(u => u.Roles)
                                          .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        [HttpGet("FindByUsername")]
        public async Task<IActionResult> SearchByUsername([FromQuery] string username)
        {
            try
            {
                _logger.LogInformation("Searching users by Username");

                if (string.IsNullOrWhiteSpace(username))
                {
                    return BadRequest("Username query parameter is required.");
                }

                var users = await _context.Users
                                          .Where(u => u.Username == username)
                                          .Include(u => u.Profile)
                                          .Include(u => u.Roles)
                                          .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        [HttpGet("FilterByEmail")]
        public async Task<IActionResult> FilterByEmailDomain([FromQuery] string emailDomain)
        {
            try
            {
                _logger.LogInformation("Filtering users by email");

                if (string.IsNullOrWhiteSpace(emailDomain))
                {
                    return BadRequest("Email domain query parameter is required.");
                }

                var users = await _context.Users
                              .Where(u => u.Email.EndsWith(emailDomain))
                              .Include(u => u.Profile)
                              .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                _logger.LogInformation("Getting user by id");

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser([FromBody] UserDto userDto)
        {
            try
            {
                _logger.LogInformation("Editing a user");

                var user = await _context.Users.FindAsync(userDto.Id);

                if (user is null)
                {
                    return BadRequest();
                }

                user.Username = userDto.Username;
                user.Email = userDto.Email;

                _context.Entry(user).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok("User has been successfully edited!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        [HttpPost]
        [Route("AddRole")]
        public async Task<ActionResult<Role>> AddRoleToUser([FromBody] RoleDto roleDto)
        {
            try
            {
                _logger.LogInformation("Add role to a user");

                var user = await _context.Users.FindAsync(roleDto.UserId);

                if (user == null)
                {
                    return NotFound();
                }

                var role = new Role()
                {
                    Name = roleDto.Name,
                    UserId = user.Id,
                };

                user.Roles.Add(role);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> Register([FromBody] UserDto userDto)
        {
            try
            {
                _logger.LogInformation("Register a user");

                if (UsernameExists(userDto.Id, userDto.Username))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Username already exists!");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Profile = new Profile()
                    {
                        FirstName = userDto.Profile.FirstName,
                        LastName = userDto.Profile.LastName,
                        DateOfBirth = userDto.Profile.DateOfBirth
                    }
                };


                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.Id }, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Deleting a user");

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok("User has been successfully deleted!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private bool UsernameExists(int id, string username)
        {
            return _context.Users.Any(e => e.Username == username);
        }
    }
}
