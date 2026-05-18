using System.ComponentModel.DataAnnotations;

namespace MyNotes.Server.Domain.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        public int AuthorId { get; set; }

        public bool IsPinned { get; set; }

        public string ColorTag { get; set; } = "#FFFFFF";

        public bool IsArchived { get; set; }

        // Position and Size
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double Width { get; set; } = 250;
        public double Height { get; set; } = 200;

        public int SortOrder { get; set; }
        public string Color { get; set; } = "#ffffff";
        public int ZIndex { get; set; } = 0;
        public bool IsDeleted { get; set; }

        public string Tags { get; set; } = string.Empty; // Simplified for demo/initial implementation
    }
}
