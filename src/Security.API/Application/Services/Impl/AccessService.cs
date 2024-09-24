using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Sofisoft.Accounts.Security.API.Application.Adapters;
using Sofisoft.Accounts.Security.API.Application.WebClients;
using Sofisoft.Accounts.Security.API.Infrastructure.Exceptions;
using Sofisoft.Accounts.Security.API.Models;
using Sofisoft.Enterprise.SeedWork.MongoDB.Domain;

namespace Sofisoft.Accounts.Security.API.Application.Services
{
    public class AccessService : IAccessService
    {
        private readonly IRepository<Access> _accessRepository;
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IIdentityService _identityService;
        private readonly ILoggingWebClient _logger;
        private readonly IWorkingSpaceWebClient _workingSpaceWebClient;

        public AccessService(IRepository<Access> accessRepository,
            IRepository<Group> groupRepository,
            IRepository<Permission> permissionRepository,
            IIdentityService identityService,
            ILoggingWebClient logger,
            IWorkingSpaceWebClient workingSpaceWebClient)
        {
            _accessRepository = accessRepository
                ?? throw new ArgumentNullException(nameof(accessRepository));
            _groupRepository = groupRepository
                ?? throw new ArgumentNullException(nameof(groupRepository));
            _permissionRepository = permissionRepository
                ?? throw new ArgumentNullException(nameof(permissionRepository));
            _identityService = identityService
                ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            _workingSpaceWebClient = workingSpaceWebClient
                ?? throw new ArgumentNullException(nameof(workingSpaceWebClient));
        }

        public async Task<MyAccessDto> GetAccessOptionAync(string optionId)
        {
            var companyId = _identityService.CompanyId;
            var userId = _identityService.UserId;
            var userName = _identityService.UserName;

            try
            {
                var option = await _workingSpaceWebClient.GetOptionByIdAsync(optionId);
                var actionsArray = new BsonArray(option.Actions.Select(s => new ObjectId(s.ActionId)));

                PipelineDefinition<Group, OptionAccessDto> pipeline = new BsonDocument[]
                {
                    new BsonDocument {{"$match", new BsonDocument {{"users", new ObjectId(userId)}}}},
                    new BsonDocument {{"$graphLookup", new BsonDocument
                        {
                            {"from", "group"},
                            {"startWith", "$parents"},
                            {"connectFromField", "parents"},
                            {"connectToField", "_id"},
                            {"as", "parents"}
                        }
                    }},
                    new BsonDocument {{"$project", new BsonDocument
                        {
                            {"_id", 0},
                            {"items", new BsonDocument
                                {
                                    {"$concatArrays", new BsonArray {
                                        new BsonArray {"$_id"}, "$parents._id"
                                    }}
                                }
                            }
                        }
                    }},
                    new BsonDocument {{"$unwind", "$items"}},
                    new BsonDocument {{"$group", new BsonDocument
                        {
                            {"_id", "$items"}
                        }
                    }},
                    new BsonDocument {{"$lookup", new BsonDocument
                        {
                            {"from", "access"},
                            {"localField", "_id"},
                            {"foreignField", "groupId"},
                            {"as", "access"}
                        }
                    }},
                    new BsonDocument {{"$unwind", "$access"}},
                    new BsonDocument {{"$unionWith", new BsonDocument
                        {
                            {"coll", "access" },
                            {"pipeline", new BsonArray
                                {
                                    new BsonDocument {{"$match", new BsonDocument
                                        {
                                            {"userId", new ObjectId(userId)},
                                            {"optionId", new ObjectId(optionId)}
                                        }
                                    }},
                                    new BsonDocument {{"$project", new BsonDocument
                                        {
                                            {"access.optionId", "$optionId"},
                                            {"access.companyId", "$companyId"},
                                            {"access.allow", "$allow"},
                                            {"access.deny", "$deny"}
                                        }
                                    }}
                                }
                            }
                        }
                    }},
                    new BsonDocument {{"$match", new BsonDocument
                        {
                            {"access.companyId", new ObjectId(companyId)},
                            {"access.optionId", new ObjectId(optionId)}
                        }
                    }},
                    new BsonDocument {{"$replaceRoot",
                        new BsonDocument {{"newRoot",
                            new BsonDocument
                            {
                                {"OptionId", new BsonDocument {{ "$toString", "$access.optionId" }}},
                                {"Allow", "$access.allow"},
                                {"Deny", "$access.deny"}
                            }
                        }}
                    }}
                };

                PipelineDefinition<Group, ActionPermissionDto> pipeline2 = new BsonDocument[]
                {
                    new BsonDocument {{"$match", new BsonDocument {{"users", new ObjectId(userId)}}}},
                    new BsonDocument {{"$graphLookup", new BsonDocument
                        {
                            {"from", "group"},
                            {"startWith", "$parents"},
                            {"connectFromField", "parents"},
                            {"connectToField", "_id"},
                            {"as", "parents"}
                        }
                    }},
                    new BsonDocument {{"$project", new BsonDocument
                        {
                            {"_id", 0},
                            {"items", new BsonDocument
                                {
                                    {"$concatArrays", new BsonArray {
                                        new BsonArray {"$_id"}, "$parents._id"
                                    }}
                                }
                            }
                        }
                    }},
                    new BsonDocument {{"$unwind", "$items"}},
                    new BsonDocument {{"$group", new BsonDocument
                        {
                            {"_id", "$items"}
                        }
                    }},
                    new BsonDocument {{"$lookup", new BsonDocument
                        {
                            {"from", "permission"},
                            {"localField", "_id"},
                            {"foreignField", "groupId"},
                            {"as", "permissions"}
                        }
                    }},
                    new BsonDocument {{"$unwind", "$permissions"}},
                    new BsonDocument {{"$unionWith", new BsonDocument
                        {
                            {"coll", "permission" },
                            {"pipeline", new BsonArray
                                {
                                    new BsonDocument {{"$match", new BsonDocument
                                        {
                                            {"userId", new ObjectId(userId)},
                                            {"$expr", new BsonDocument{{"$in", new BsonArray {"$actionId", actionsArray}}}}
                                        }
                                    }},
                                    new BsonDocument {{"$project", new BsonDocument
                                        {
                                            {"permissions.actionId", "$actionId"},
                                            {"permissions.companyId", "$companyId"},
                                            {"permissions.allow", "$allow"},
                                            {"permissions.deny", "$deny"}
                                        }
                                    }}
                                }
                            }
                        }
                    }},
                    new BsonDocument {{"$match", new BsonDocument
                        {
                            {"permissions.companyId", new ObjectId(companyId)},
                            {"$expr", new BsonDocument{{"$in", new BsonArray {"$permissions.actionId", actionsArray}}}}
                        }
                    }},
                    new BsonDocument {{"$replaceRoot",
                        new BsonDocument {{"newRoot",
                            new BsonDocument
                            {
                                {"ActionId", new BsonDocument {{ "$toString", "$permissions.actionId" }}},
                                {"Allow", "$permissions.allow"},
                                {"Deny", "$permissions.deny"}
                            }
                        }}
                    }}
                };

                var tAccess = _groupRepository.AggregateAsync(pipeline);
                var tPermissions = _groupRepository.AggregateAsync(pipeline2);
                var access = (await tAccess).ToList();
                var permissions = (await tPermissions).ToList();

                if (access.Count(f => f.Deny) > 0 || access.Count(f => f.Allow) == 0)
                {
                    return new MyAccessDto { Status = 401 };
                }

                var myActions = (from a in option.Actions
                    where permissions.Count(p => p.ActionId == a.ActionId && p.Allow == true) > 0
                        && permissions.Count(p => p.ActionId == a.ActionId && p.Deny == true) == 0
                    select a
                );

                return new MyAccessDto {
                    Status = 200,
                    Option = new OptionDto {
                        Actions = myActions,
                        Icon = option.Icon,
                        Name = option.Name,
                        OptionId = option.OptionId
                    }
                };
            }
            catch (SecurityDomainException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.StackTrace, userName);

                throw new SecurityDomainException("Ocurrio un error al obtener.", result);
            }
        }

        public async Task<IEnumerable<TreeListDto>> GetAccessToOptionsAsync(string moduleId)
        {
            var companyId = _identityService.CompanyId;
            var userId = _identityService.UserId;
            var userName = _identityService.UserName;

            try
            {
                PipelineDefinition<Group, OptionAccessDto> pipeline = new BsonDocument[]
                {
                    new BsonDocument {{"$match", new BsonDocument {{"users", new ObjectId(userId)}}}},
                    new BsonDocument {{"$graphLookup", new BsonDocument
                        {
                            {"from", "group"},
                            {"startWith", "$parents"},
                            {"connectFromField", "parents"},
                            {"connectToField", "_id"},
                            {"as", "parents"}
                        }
                    }},
                    new BsonDocument {{"$project", new BsonDocument
                        {
                            {"_id", 0},
                            {"items", new BsonDocument
                                {
                                    {"$concatArrays", new BsonArray {
                                        new BsonArray {"$_id"}, "$parents._id"
                                    }}
                                }
                            }
                        }
                    }},
                    new BsonDocument {{"$unwind", "$items"}},
                    new BsonDocument {{"$group", new BsonDocument
                        {
                            {"_id", "$items"}
                        }
                    }},
                    new BsonDocument {{"$lookup", new BsonDocument
                        {
                            {"from", "access"},
                            {"localField", "_id"},
                            {"foreignField", "groupId"},
                            {"as", "access"}
                        }
                    }},
                    new BsonDocument {{"$unwind", "$access"}},
                    new BsonDocument {{"$unionWith", new BsonDocument
                        {
                            {"coll", "access" },
                            {"pipeline", new BsonArray
                                {
                                    new BsonDocument {{"$match", new BsonDocument
                                        {
                                            {"userId", new ObjectId(userId)}
                                        }
                                    }},
                                    new BsonDocument {{"$project", new BsonDocument
                                        {
                                            {"access.optionId", "$optionId"},
                                            {"access.companyId", "$companyId"},
                                            {"access.allow", "$allow"},
                                            {"access.deny", "$deny"}
                                        }
                                    }}
                                }
                            }
                        }
                    }},
                    new BsonDocument {{"$match", new BsonDocument
                        {
                            {"access.companyId", new ObjectId(companyId)}
                        }
                    }},
                    new BsonDocument {{"$replaceRoot",
                        new BsonDocument {{"newRoot",
                            new BsonDocument
                            {
                                {"OptionId", new BsonDocument {{ "$toString", "$access.optionId" }}},
                                {"Allow", "$access.allow"},
                                {"Deny", "$access.deny"}
                            }
                        }}
                    }}
                };

                var tAccess = _groupRepository.AggregateAsync(pipeline);
                var tOptions = _workingSpaceWebClient.GetOptionsByModuleAsync(moduleId);
                var access = (await tAccess).ToList();
                var options = await tOptions;

                var myOptions = (from o in options
                    where o.Leaf == false
                        || (access.Count(p => p.OptionId == o.OptionId && p.Allow == true) > 0
                            && access.Count(p => p.OptionId == o.OptionId && p.Deny == true) == 0)
                    select new OptionListDto {
                        Collapsible = o.Collapsible,
                        Icon = o.Icon,
                        Leaf = o.Leaf,
                        Name = o.Name,
                        OptionId = o.OptionId,
                        Order = o.Order,
                        ParentId = o.ParentId,
                        Tooltip = o.Tooltip,
                        Paths = o.Paths
                    });

                return GetTreeList(myOptions, null);
            }
            catch (SecurityDomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.StackTrace, userName);

                throw new SecurityDomainException("Ocurrio un error al obtener.", result);
            }
        }

        private List<TreeListDto> GetTreeList(IEnumerable<OptionListDto> options, string parentId)
        {
            var children = new List<TreeListDto>();

            foreach(var item in options.Where(p => p.ParentId == parentId).OrderBy(o => o.Order))
            {
                var option = new TreeListDto
                {
                    OptionId = item.OptionId,
                    Name = item.Name,
                    Tooltip = item.Tooltip,
                    Leaf = item.Leaf,
                    Icon = item.Icon,
                    Collapsible = item.Collapsible,
                    Paths = item.Leaf ? item.Paths : null,
                    Children = item.Leaf ? null : GetTreeList(options, item.OptionId)
                };

                if (item.Leaf || option.Children.Count > 0)
                {
                    children.Add(option);
                }
            }

            return children;
        }

    }
}