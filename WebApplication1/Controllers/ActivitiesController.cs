using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetActivities(
            [FromQuery] DateTime? date,
            [FromQuery] int? month,
            [FromQuery] int? year)
        {
            IQueryable<Activity> query = _context.Activities
                .Include(a => a.Exercise)
                .ThenInclude(e => e!.TrainingProgram);

            if (date.HasValue)
            {
                query = query.Where(a => a.Date.Date == date.Value.Date);
            }
            else if (month.HasValue && year.HasValue)
            {
                query = query.Where(a => a.Date.Month == month && a.Date.Year == year);
            }

            var activities = await query.OrderByDescending(a => a.Date).ToListAsync();
            
            var result = activities.Select(a => new
            {
                a.Id,
                a.Date,
                a.Minutes,
                a.Notes,
                a.ExerciseId,
                Exercise = a.Exercise != null ? new
                {
                    a.Exercise.Id,
                    a.Exercise.Name,
                    a.Exercise.IsActive,
                    TrainingProgram = a.Exercise.TrainingProgram != null ? new
                    {
                        a.Exercise.TrainingProgram.Id,
                        a.Exercise.TrainingProgram.Name,
                        a.Exercise.TrainingProgram.Type,
                        a.Exercise.TrainingProgram.IsActive
                    } : null
                } : null,
                a.IsExerciseActiveAtCreation
            }).ToList();

            return Ok(result);
        }

        [HttpGet("daily-summary/{date}")]
        public async Task<ActionResult<object>> GetDailySummary(DateTime date)
        {
            var activities = await _context.Activities
                .Where(a => a.Date.Date == date.Date)
                .Include(a => a.Exercise)
                .ThenInclude(e => e!.TrainingProgram)
                .ToListAsync();

            var totalMinutes = activities.Sum(a => a.Minutes);
            var stickerColor = totalMinutes < 30 ? "yellow" :
                            totalMinutes <= 90 ? "green" : "red";

            var result = new
            {
                Date = date.Date,
                TotalMinutes = totalMinutes,
                StickerColor = stickerColor,
                ActivityCount = activities.Count,
                Activities = activities.Select(a => new
                {
                    a.Id,
                    a.Date,
                    a.Minutes,
                    a.Notes,
                    a.ExerciseId,
                    Exercise = a.Exercise != null ? new
                    {
                        a.Exercise.Id,
                        a.Exercise.Name,
                        TrainingProgram = a.Exercise.TrainingProgram != null ? new
                        {
                            a.Exercise.TrainingProgram.Name
                        } : null
                    } : null
                })
            };

            return Ok(result);
        }

        [HttpGet("today-summary")]
        public async Task<ActionResult<object>> GetTodaySummary()
        {
            var today = DateTime.Today;

            var activities = await _context.Activities
                .Where(a => a.Date.Date == today)
                .ToListAsync();

            var totalMinutes = activities.Sum(a => a.Minutes);
            var activityCount = activities.Count;

            var result = new
            {
                Date = today,
                TotalMinutes = totalMinutes,
                ActivityCount = activityCount,
                Activities = activities.Select(a => new
                {
                    a.Id,
                    a.Date,
                    a.Minutes,
                    a.Notes,
                    a.ExerciseId
                })
            };

            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetActiveExercises()
        {
            return await _context.Exercises
                .Include(e => e.TrainingProgram)
                .Where(e => e.IsActive)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Activity>> CreateActivity(Activity activity)
        {
            var exercise = await _context.Exercises.FindAsync(activity.ExerciseId);
            if (exercise == null || !exercise.IsActive)
                return BadRequest("Упражнение не найдено или не активно");

            var dailyMinutes = await _context.Activities
                .Where(a => a.Date.Date == activity.Date.Date)
                .SumAsync(a => a.Minutes);

            if (dailyMinutes + activity.Minutes > 1440)
                return BadRequest("Превышен дневной лимит в 1440 минут");

            if (activity.Minutes <= 0 || activity.Minutes > 1440)
                return BadRequest("Некорректное количество минут");

            activity.IsExerciseActiveAtCreation = exercise.IsActive;
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivity), new { id = activity.Id }, activity);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.Exercise)
                .ThenInclude(e => e!.TrainingProgram)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null) return NotFound();
            return activity;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, Activity activity)
        {
            if (id != activity.Id) return BadRequest();

            var existingActivity = await _context.Activities.FindAsync(id);
            if (existingActivity == null) return NotFound();

            if (existingActivity.ExerciseId != activity.ExerciseId)
            {
                if (!existingActivity.IsExerciseActiveAtCreation)
                    return BadRequest("Нельзя изменить упражнение, так как оно стало неактивным");

                var exercise = await _context.Exercises.FindAsync(activity.ExerciseId);
                if (exercise == null || !exercise.IsActive)
                    return BadRequest("Новое упражнение не найдено или не активно");
            }

            var dailyMinutes = await _context.Activities
                .Where(a => a.Date.Date == activity.Date.Date && a.Id != id)
                .SumAsync(a => a.Minutes);

            if (dailyMinutes + activity.Minutes > 1440)
                return BadRequest("Превышен дневной лимит в 1440 минут");

            if (activity.Minutes <= 0 || activity.Minutes > 1440)
                return BadRequest("Некорректное количество минут");

            _context.Entry(existingActivity).CurrentValues.SetValues(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return NotFound();

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
