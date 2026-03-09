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
global using Common = AIToDoGenerator.Common;
global using CommonEnums = AIToDoGenerator.Common.Enums;
global using CommonContracts = AIToDoGenerator.Common.Contracts;
global using CommonModels = AIToDoGenerator.Common.Models;
global using CommonModules = AIToDoGenerator.Common.Modules;
global using AIToDoGenerator.Common.Extensions;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using Microsoft.EntityFrameworkCore;
global using Validator = AIToDoGenerator.Common.Modules.Validations.Validator;

