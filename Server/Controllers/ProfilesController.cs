using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Models;
using Server.ModelDTO;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfilesController> _logger;

        public ProfilesController(ApplicationDbContext context, ILogger<ProfilesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Profiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> ViewProfile(int id)
        {
            try
            {
                _logger.LogInformation("Get profile by Id");

                var profile = await _context.Profiles.FindAsync(id);

                if (profile == null)
                {
                    return NotFound();
                }

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        // POST: api/Profiles
        [HttpPost]
        public async Task<ActionResult<Profile>> AddProfile([FromBody] ProfileDto profileDto)
        {
            try
            {
                _logger.LogInformation("Adding a profile");

                var user = await _context.Users.FindAsync(profileDto.Id);

                if (user == null)
                {
                    return NotFound();
                }

                var profile = new Profile
                {
                    FirstName = profileDto.FirstName,
                    LastName = profileDto.LastName,
                    DateOfBirth = profileDto.DateOfBirth,
                };

                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An internal error occurred.");
            }
        }
    }
}
