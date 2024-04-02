namespace Diary.Entities
{
    public class Entry : EntityBase
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime EditedAt { get; set; }

        public bool IsFavourite { get; set; }

        public int Mood { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Altitude { get; set; }

        public virtual ICollection<Label> Labels { get; set; } = [];
    }
}
