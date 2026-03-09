//@CodeCopy
#if ACCOUNT_ON
namespace AIToDoGenerator.WebApi.Contracts
{
    partial interface IContextAccessor
    {
        string SessionToken { set; }
    }
}
#endif
