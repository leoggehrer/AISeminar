//@CodeCopy
namespace AIToDoGenerator.Logic.Contracts
{
    public partial interface IValidatableEntity
    {
        void Validate(IContext context, EntityState entityState);
    }
}
