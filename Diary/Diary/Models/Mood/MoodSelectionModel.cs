namespace Diary.Models.Mood
{
    public class MoodSelectionModel
    {
        public int Mood { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is MoodSelectionModel other)
            {
                return Mood == other.Mood;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Mood.GetHashCode();
        }
    }
}
