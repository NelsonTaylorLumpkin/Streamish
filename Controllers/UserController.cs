using Microsoft.AspNetCore.Mvc;
using Streamish.Models;
using Streamish.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Streamish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _userProfileRepository;
        public UserProfileController(IUserProfileRepository _userProfileRepository) 
        {
            IUserProfileRepository UserProfileRepository = _userProfileRepository;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userProfileRepository.GetAll());
        }
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _userProfileRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Post(UserProfile user)
        {
            _userProfileRepository.Add(user);
            return CreatedAtAction("Get", new { id = user.Id }, user);
        }
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, UserProfile user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _userProfileRepository.Update(user);
            return NoContent();
        }
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userProfileRepository.Delete(id);
            return NoContent();
        }
        [Authorize]
        [HttpGet("{id}/GetUserWithVideosAndComments")]
        public IActionResult GetUserByIdWithVideosAndComments(int id)
        {
            var user = _userProfileRepository.GetUserByIdWithVideosAndComments(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [Authorize]
        [HttpGet("GetWithVideos")]
        public IActionResult GetWithVideos()
        {
            var users = _userProfileRepository.GetAllUsersWithVideos();
            return Ok(users);
        }
        [Authorize]
        [HttpGet("firebase/{firebaseUserId}")]
        public IActionResult GetByFirebaseUserId(string firebaseUserId)
        {
            var user = _userProfileRepository.GetByFirebaseUserId(firebaseUserId);
            if (user == null)
            {
                return NotFound();  
            }
            return Ok(user);
        }

    }
}

