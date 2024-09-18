using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerSide.DBinteractions;
using ServerSide.Models;
using System;
using System.Collections.Generic;

namespace ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            UsersDB.SetConnectionString(_configuration.GetConnectionString("DefaultConnection"));
        }

        // GET: api/User
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User[]> Get()
        {
            try
            {
                List<User> users = UsersDB.GetAllUsers();
                return new JsonResult(users);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(string id)
        {
            try
            {
                User user = UsersDB.GetUserById(id);
                if (user == null)
                    return NotFound($"User with id: {id} wasn't found.");

                return new JsonResult(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/User
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] User value)
        {
            try
            {
                if (value == null)
                    return BadRequest("User is null.");

                string newId = UsersDB.InsertUser(value);

                return CreatedAtAction(nameof(Get), new { id = newId }, value);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(string id, [FromBody] User value)
        {
            try
            {
                if (value == null || value.Id != id)
                    return BadRequest();

                int rowsAffected = UsersDB.UpdateUser(value);
                if (rowsAffected == 0)
                    return NotFound($"User with id: {id} wasn't found, can't update.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid ID.");

                bool isDeleted = UsersDB.DeleteUser(id);
                if (!isDeleted)
                    return NotFound($"User with id: {id} wasn't found, can't delete.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/User/authenticate
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Authenticate([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginRequest.EmailAddress) || string.IsNullOrWhiteSpace(loginRequest.Password))
                    return BadRequest("Email and password are required.");

                User user = UsersDB.AuthenticateUser(loginRequest.EmailAddress, loginRequest.Password);

                if (user == null)
                    return NotFound("Invalid email or password.");

                return Ok(new { message = "Authentication successful", user });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/User/{id}/password
        [HttpPut("{id}/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePassword(string id, [FromBody] PasswordUpdateRequest passwordUpdate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(passwordUpdate.NewPassword))
                    return BadRequest("Invalid data.");

                int result = UsersDB.UpdateUserPassword(id, passwordUpdate.NewPassword);

                if (result == 0)
                    return NotFound($"User with id: {id} wasn't found.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/User/forgot-password
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(forgotPasswordRequest.EmailAddress))
                    return BadRequest("Email address is required.");

                UsersDB.HandleForgotPassword(forgotPasswordRequest.EmailAddress);

                return Ok("Temporary password has been sent to your email.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }

    // Login request model
    public class LoginRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    // Password update request model
    public class PasswordUpdateRequest
    {
        public string NewPassword { get; set; }
    }

    // Forgot password request model
    public class ForgotPasswordRequest
    {
        public string EmailAddress { get; set; }
    }
}
