using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sofisoft.Accounts.Security.API.Application.Adapters;
using Sofisoft.Accounts.Security.API.Application.WebClients;
using Sofisoft.Accounts.Security.API.Infrastructure.Exceptions;
using Sofisoft.Accounts.Security.API.Models;
using Sofisoft.Enterprise.SeedWork.MongoDB.Domain;

namespace Sofisoft.Accounts.Security.API.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IRepository<Favorite> _favoriteRepository;
        private readonly IIdentityService _identityService;
        private readonly ILoggingWebClient _logger;

        public FavoriteService(IRepository<Favorite> favoriteRepository,
            IIdentityService identityService,
            ILoggingWebClient logger)
        {
            _favoriteRepository = favoriteRepository
                ?? throw new ArgumentNullException(nameof(favoriteRepository));
            _identityService = identityService
                ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Favorite> AddAsync(string optionId)
        {
            var companyId = _identityService.CompanyId;
            var userId = _identityService.UserId;
            var userName = _identityService.UserName;

            try
            {
                var favorite = await _favoriteRepository.FindOneAsync(
                    f => f.CompanyId == companyId &&
                    f.OptionId == optionId &&
                    f.UserId == userId
                );

                if(favorite is null)
                {
                    favorite = new Favorite {
                        CompanyId = companyId,
                        CreatedBy = userName,
                        OptionId = optionId,
                        UserId = userId
                    };

                    await _favoriteRepository.InsertOneAsync(favorite);
                }

                return favorite;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.StackTrace, userName);

                throw new SecurityDomainException("Ocurrio un error al agregar.", result);
            }
        }

        public async Task<bool> CheckFavoriteAsync(string optionId)
        {
            var companyId = _identityService.CompanyId;
            var userId = _identityService.UserId;
            var userName = _identityService.UserName;

            try
            {
                var favorite = await _favoriteRepository.FindOneAsync(f =>
                    f.CompanyId == companyId &&
                    f.OptionId == optionId &&
                    f.UserId == userId
                );

                return favorite is not null;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.StackTrace, userName);

                throw new SecurityDomainException("Ocurrio un error al obtener.", result);
            }
        }

        public async Task DropAsync(string optionId)
        {
            var companyId = _identityService.CompanyId;
            var userId = _identityService.UserId;
            var userName = _identityService.UserName;

            try
            {
                var favorite = await _favoriteRepository.FindOneAsync(
                    f => f.CompanyId == companyId &&
                    f.OptionId == optionId &&
                    f.UserId == userId
                );

                if(favorite is not null)
                {
                    await _favoriteRepository.DeleteByIdAsync(favorite.Id);
                }
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.StackTrace, userName);

                throw new SecurityDomainException("Ocurrio un error al agregar.", result);
            }
        }

        public Task<(long total, IEnumerable<FavoriteListDto> data)> ListAsync(string name, string sort, int pageSize, int start)
        {
            throw new System.NotImplementedException();
        }
    }
}