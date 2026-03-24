//@Ignore
namespace SETemplate.Logic.Entities.App
{
    using Microsoft.EntityFrameworkCore;
    using SETemplate.Logic.Modules.Exceptions;

    partial class BlogEntry : SETemplate.Logic.Contracts.IValidatableEntity
    {
        /// <summary>
        /// Validates the blog entry according to business rules.
        /// </summary>
        /// <param name="context">The current data context.</param>
        /// <param name="entityState">The state of the entity (Added, Modified, etc.).</param>
        public void Validate(SETemplate.Logic.Contracts.IContext context, EntityState entityState)
        {
            bool handled = false;
            BeforeExecuteValidation(ref handled, context, entityState);

            if (!handled)
            {
                if (!IsTitleValid(Title))
                {
                    throw new BusinessRuleException(
                        $"The '{nameof(Title)}' must not be empty and must not exceed 200 characters.");
                }

                if (!IsContentValid(Content))
                {
                    throw new BusinessRuleException(
                        $"The '{nameof(Content)}' must not be empty.");
                }

                if (!IsAuthorValid(Author))
                {
                    throw new BusinessRuleException(
                        $"The '{nameof(Author)}' must not be empty and must not exceed 100 characters.");
                }

                if (Summary != null && Summary.Length > 500)
                {
                    throw new BusinessRuleException(
                        $"The '{nameof(Summary)}' must not exceed 500 characters.");
                }

                if (Tags != null && Tags.Length > 300)
                {
                    throw new BusinessRuleException(
                        $"The '{nameof(Tags)}' must not exceed 300 characters.");
                }

                if (IsPublished && !PublishedOn.HasValue)
                {
                    throw new BusinessRuleException(
                        $"A published blog entry must have a '{nameof(PublishedOn)}' date.");
                }

                if (PublishedOn.HasValue && PublishedOn.Value.Kind != DateTimeKind.Utc)
                {
                    throw new BusinessRuleException(
                        $"The '{nameof(PublishedOn)}' date must be in UTC.");
                }
            }
        }

        #region methods
        /// <summary>
        /// Checks whether the given title is valid.
        /// </summary>
        public static bool IsTitleValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length <= 200;
        }

        /// <summary>
        /// Checks whether the given content is valid.
        /// </summary>
        public static bool IsContentValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Checks whether the given author name is valid.
        /// </summary>
        public static bool IsAuthorValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length <= 100;
        }
        #endregion methods

        #region partial methods
        partial void BeforeExecuteValidation(ref bool handled,
            SETemplate.Logic.Contracts.IContext context, EntityState entityState);
        #endregion partial methods
    }
}
