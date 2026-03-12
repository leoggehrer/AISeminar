//@AiCode
namespace ProductCatalog.Common.Enums
{
    /// <summary>
    /// Represents the status of an AI-generated product description.
    /// </summary>
    public enum DescriptionStatus : int
    {
        /// <summary>No description has been generated yet.</summary>
        None = 0,
        /// <summary>Description generation is currently in progress.</summary>
        Generating = 1,
        /// <summary>Description has been successfully generated.</summary>
        Done = 2,
    }
}
