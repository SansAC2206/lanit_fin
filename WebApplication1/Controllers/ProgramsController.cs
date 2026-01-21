using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgramsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingProgram>>> GetPrograms()
        {
            return await _context.TrainingPrograms.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingProgram>> GetProgram(int id)
        {
            var program = await _context.TrainingPrograms.FindAsync(id);
            if (program == null) return NotFound();
            return program;
        }

        [HttpPost]
        public async Task<ActionResult<TrainingProgram>> CreateProgram(TrainingProgram program)
        {
            _context.TrainingPrograms.Add(program);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, program);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgram(int id, TrainingProgram program)
        {
            if (id != program.Id) return BadRequest();
            _context.Entry(program).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TrainingPrograms.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var program = await _context.TrainingPrograms.FindAsync(id);
            if (program == null) return NotFound();

            var hasExercises = await _context.Exercises.AnyAsync(e => e.TrainingProgramId == id);
            if (hasExercises) return BadRequest("Нельзя удалить программу с связанными упражнениями");

            _context.TrainingPrograms.Remove(program);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
