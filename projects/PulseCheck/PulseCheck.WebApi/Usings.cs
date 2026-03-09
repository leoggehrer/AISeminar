//@CodeCopy

#if IDINT_ON
global using IdType = System.Int32;
#elif IDLONG_ON
global using IdType = System.Int64;
#elif IDGUID_ON
global using IdType = System.Guid;
#else
global using IdType = System.Int32;
#endif
global using Common = PulseCheck.Common;
global using CommonContracts = PulseCheck.Common.Contracts;
global using CommonModels = PulseCheck.Common.Models;
global using CommonModules = PulseCheck.Common.Modules;
global using PulseCheck.Common.Extensions;
