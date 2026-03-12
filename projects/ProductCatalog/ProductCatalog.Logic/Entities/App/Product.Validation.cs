//@AiCode
namespace ProductCatalog.Logic.Entities.App
{
    using Microsoft.EntityFrameworkCore;
    using ProductCatalog.Logic.Modules.Exceptions;

    partial class Product : ProductCatalog.Logic.Contracts.IValidatableEntity
    {
        /// <summary>
        /// Validates the product entity before persisting.
        /// </summary>
        /// <param name="context">The current database context.</param>
        /// <param name="entityState">The state of the entity (Added, Modified, etc.).</param>
        public void Validate(ProductCatalog.Logic.Contracts.IContext context, EntityState entityState)
        {
            bool handled = false;
            BeforeExecuteValidation(ref handled, context, entityState);

            if (!handled)
            {
                if (!IsNameValid(Name))
                {
                    throw new BusinessRuleException(
                        $"The value of {nameof(Name)} '{Name}' is not valid. Name must not be empty or whitespace.");
                }

                if (!IsPriceValid(Price))
                {
                    throw new BusinessRuleException(
                        $"The value of {nameof(Price)} '{Price}' is not valid. Price must be greater than or equal to 0.");
                }

                if (!IsStockValid(Stock))
                {
                    throw new BusinessRuleException(
                        $"The value of {nameof(Stock)} '{Stock}' is not valid. Stock must be greater than or equal to 0.");
                }
            }
        }

        #region methods
        /// <summary>
        /// Validates that the product name is not empty or whitespace.
        /// </summary>
        /// <param name="value">The name value to validate.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool IsNameValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Validates that the price is non-negative.
        /// </summary>
        /// <param name="value">The price value to validate.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool IsPriceValid(decimal value)
        {
            return value >= 0;
        }

        /// <summary>
        /// Validates that the stock quantity is non-negative.
        /// </summary>
        /// <param name="value">The stock value to validate.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool IsStockValid(int value)
        {
            return value >= 0;
        }
        #endregion methods

        #region partial methods
        partial void BeforeExecuteValidation(ref bool handled, ProductCatalog.Logic.Contracts.IContext context, EntityState entityState);
        #endregion partial methods
    }
}
