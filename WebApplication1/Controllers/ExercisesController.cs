using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExercisesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
        {
            var exercises = await _context.Exercises
                .Include(e => e.TrainingProgram)
                .ToListAsync();

            var result = exercises.Select(e => new
            {
                e.Id,
                e.Name,
                e.TrainingProgramId,
                TrainingProgramName = e.TrainingProgram != null ? e.TrainingProgram.Name : "No Program",
                e.IsActive,
                CreatedDate = DateTime.UtcNow,
                TrainingProgram = e.TrainingProgram != null ? new
                {
                    e.TrainingProgram.Id,
                    e.TrainingProgram.Name,
                    e.TrainingProgram.Type,
                    e.TrainingProgram.IsActive
                } : null
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(int id)
        {
            var exercise = await _context.Exercises
                .Include(e => e.TrainingProgram)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exercise == null) return NotFound();
            return exercise;
        }

        [HttpPost]
        public async Task<ActionResult<Exercise>> CreateExercise(Exercise exercise)
        {
            var program = await _context.TrainingPrograms.FindAsync(exercise.TrainingProgramId);
            if (program == null) return BadRequest("Программа не найдена");

            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExercise), new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExercise(int id, Exercise exercise)
        {
            if (id != exercise.Id) return BadRequest();

            var program = await _context.TrainingPrograms.FindAsync(exercise.TrainingProgramId);
            if (program == null) return BadRequest("Программа не найдена");

            _context.Entry(exercise).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Exercises.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null) return NotFound();

            var hasActivities = await _context.Activities.AnyAsync(a => a.ExerciseId == id);
            if (hasActivities) return BadRequest("Нельзя удалить упражнение с связанными активностями");

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
