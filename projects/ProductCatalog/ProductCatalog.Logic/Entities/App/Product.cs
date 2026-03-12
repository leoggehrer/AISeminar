//@AiCode
namespace ProductCatalog.Logic.Entities.App
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

#if SQLITE_ON
    [Table("Products")]
#else
    [Table("Products", Schema = "app")]
#endif
    [Index(nameof(Name), IsUnique = true)]
    /// <summary>
    /// Represents a luxury product in the catalog.
    /// </summary>
    public partial class Product : EntityObject
    {
        #region fields
        private string _name = string.Empty;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name
        {
            get => _name;
            set
            {
                bool handled = false;
                OnNameChanging(ref handled, ref value);
                if (!handled)
                {
                    _name = value;
                }
                OnNameChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the category of the product.
        /// </summary>
        public CommonEnums.ProductCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the available stock quantity.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Gets or sets the AI-generated description of the product.
        /// </summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of AI-generated tags for the product.
        /// </summary>
        [MaxLength(500)]
        public string? Tags { get; set; }

        /// <summary>
        /// Gets or sets the tone for the AI-generated description.
        /// </summary>
        public CommonEnums.ProductTone? Tone { get; set; }

        /// <summary>
        /// Gets or sets the status of the AI-generated description.
        /// </summary>
        public CommonEnums.DescriptionStatus DescriptionStatus { get; set; } = CommonEnums.DescriptionStatus.None;

        /// <summary>
        /// Gets or sets the date and time when the product was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        #endregion properties

        #region partial methods
        partial void OnNameChanging(ref bool handled, ref string value);
        partial void OnNameChanged(string value);
        #endregion partial methods
    }
}
