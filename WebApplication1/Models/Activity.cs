namespace WebApplication1.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Minutes { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }
        public bool IsExerciseActiveAtCreation { get; set; } = true;
    }
}
