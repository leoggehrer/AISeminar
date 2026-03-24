//@Ignore
namespace SETemplate.Logic.Entities.App
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

#if SQLITE_ON
    [Table("BlogEntries")]
#else
    [Table("BlogEntries", Schema = "app")]
#endif
    [Index(nameof(Title), IsUnique = false)]
    /// <summary>
    /// Represents a developer blog entry.
    /// </summary>
    public partial class BlogEntry : EntityObject
    {
        #region fields
        private string _title = string.Empty;
        private string _content = string.Empty;
        private string _author = string.Empty;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the title of the blog entry.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title
        {
            get => _title;
            set
            {
                bool handled = false;
                OnTitleChanging(ref handled, ref value);
                if (!handled)
                {
                    _title = value;
                }
                OnTitleChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets a short summary of the blog entry.
        /// </summary>
        [MaxLength(500)]
        public string? Summary { get; set; }

        /// <summary>
        /// Gets or sets the full content of the blog entry (Markdown supported).
        /// </summary>
        [Required]
        public string Content
        {
            get => _content;
            set
            {
                bool handled = false;
                OnContentChanging(ref handled, ref value);
                if (!handled)
                {
                    _content = value;
                }
                OnContentChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the author's name or handle.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Author
        {
            get => _author;
            set
            {
                bool handled = false;
                OnAuthorChanging(ref handled, ref value);
                if (!handled)
                {
                    _author = value;
                }
                OnAuthorChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets a comma-separated list of tags for the blog entry.
        /// </summary>
        [MaxLength(300)]
        public string? Tags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the blog entry is published.
        /// </summary>
        public bool IsPublished { get; set; } = false;

        /// <summary>
        /// Gets or sets the date and time when the blog entry was published (UTC).
        /// Null if not yet published.
        /// </summary>
        public DateTime? PublishedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the blog entry was created (UTC).
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets a value indicating whether the blog entry has been published.
        /// </summary>
        [NotMapped]
        public bool HasBeenPublished
        {
            get => PublishedOn.HasValue;
            set { /* Required for serialization */ }
        }
        #endregion properties

        #region partial methods
        partial void OnTitleChanging(ref bool handled, ref string value);
        partial void OnTitleChanged(string value);
        partial void OnContentChanging(ref bool handled, ref string value);
        partial void OnContentChanged(string value);
        partial void OnAuthorChanging(ref bool handled, ref string value);
        partial void OnAuthorChanged(string value);
        #endregion partial methods
    }
}
