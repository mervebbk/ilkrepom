using Microsoft.AspNetCore.Mvc;
using IlkRepom.Domain.Entities;
using IlkRepom.Infrastructure.Repositories;

namespace IlkRepom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository repository;

        public UsersController(UserRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(repository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = repository.GetById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, User user)
        {
            var result = repository.Update(id, user);

            if (!result)
                return NotFound();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = repository.Delete(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
