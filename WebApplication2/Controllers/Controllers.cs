using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UsersController(IUserRepository repo)
        {
            _repo = repo;
        }

        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            return Ok(_repo.GetAll());
        }

        // GET: api/users/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<User> GetById(Guid id)
        {
            var user = _repo.GetById(id);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<User> Create([FromBody] User user)
        {
            // Basic server-side validation
            if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { message = "FirstName and Email are required." });

            var created = _repo.Create(user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/users/{id}
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { message = "FirstName and Email are required." });

            var success = _repo.Update(id, user);
            if (!success) return NotFound(new { message = "User not found" });
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var success = _repo.Delete(id);
            if (!success) return NotFound(new { message = "User not found" });
            return NoContent();
        }
    }
}
