//@CodeCopy
#if ACCOUNT_ON
namespace ProductCatalog.WebApi.Contracts
{
    partial interface IContextAccessor
    {
        string SessionToken { set; }
    }
}
#endif
