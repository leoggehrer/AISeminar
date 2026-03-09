//@CodeCopy
#if ACCOUNT_ON
namespace PulseCheck.WebApi.Contracts
{
    partial interface IContextAccessor
    {
        string SessionToken { set; }
    }
}
#endif
