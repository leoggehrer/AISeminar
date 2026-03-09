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
global using CommonModules = PulseCheck.Common.Modules;
global using PulseCheck.Common.Extensions;
global using CommonStaticLiterals = PulseCheck.Common.StaticLiterals;
global using TemplatePath = PulseCheck.Common.Modules.Template.TemplatePath;
