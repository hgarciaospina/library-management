using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // Enables automatic model validation and 400 responses
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _service;

        public MembersController(IMemberService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all members
        /// </summary>
        /// <returns>List of members</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var members = await _service.GetAllAsync();
            return Ok(members);
        }

        /// <summary>
        /// Get member by ID
        /// </summary>
        /// <param name="id">Member ID</param>
        /// <returns>Member details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var member = await _service.GetByIdAsync(id);
            if (member == null)
                return NotFound(); // 404 if not found

            return Ok(member);
        }

        /// <summary>
        /// Create a new member
        /// </summary>
        /// <param name="dto">Member creation DTO</param>
        /// <returns>Created member with location header</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MemberCreateDto dto)
        {
            // ModelState is automatically populated by FluentValidation
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Returns all validation errors

            var createdMember = await _service.CreateAsync(dto);

            // Return 201 Created with Location header pointing to the new resource
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdMember.Id },
                createdMember
            );
        }

        /// <summary>
        /// Update an existing member
        /// </summary>
        /// <param name="id">Member ID</param>
        /// <param name="dto">Member update DTO</param>
        /// <returns>NoContent if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MemberUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch."); // Ensure route ID matches DTO

            var exists = await _service.GetByIdAsync(id);
            if (exists == null)
                return NotFound(); // 404 if not found

            await _service.UpdateAsync(dto);
            return NoContent(); // 204 No Content for successful update
        }

        /// <summary>
        /// Delete a member by ID
        /// </summary>
        /// <param name="id">Member ID</param>
        /// <returns>NoContent if deleted</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _service.GetByIdAsync(id);
            if (exists == null)
                return NotFound(); // 404 if member does not exist

            await _service.DeleteAsync(id);
            return NoContent(); // 204 No Content for successful deletion
        }
    }
}
