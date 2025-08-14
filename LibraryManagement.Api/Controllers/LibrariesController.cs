using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrariesController : ControllerBase
    {
        private readonly ILibraryService _service;

        public LibrariesController(ILibraryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var libraries = await _service.GetAllAsync();
            return Ok(libraries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var library = await _service.GetByIdAsync(id);
            if (library == null) return NotFound();
            return Ok(library);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LibraryCreateDto dto)
        {
            var library = await _service.CreateAsync(dto); 
            return CreatedAtAction(nameof(GetById), new { id = library.Id }, library);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LibraryUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
