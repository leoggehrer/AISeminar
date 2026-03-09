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
global using Common = ProductCatalog.Common;
global using CommonModules = ProductCatalog.Common.Modules;
global using ProductCatalog.Common.Extensions;
global using CommonStaticLiterals = ProductCatalog.Common.StaticLiterals;
global using TemplatePath = ProductCatalog.Common.Modules.Template.TemplatePath;
