using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofisoft.Accounts.Security.API.Application.Adapters;
using Sofisoft.Accounts.Security.API.Application.Services;
using Sofisoft.Accounts.Security.API.Infrastructure.Exceptions;
using Sofisoft.Accounts.Security.API.ViewModels;

namespace Sofisoft.Accounts.Security.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IAccessService _accessService;
        private readonly IFavoriteService _favoriteService;

        public AccessController(IAccessService accessService, IFavoriteService favoriteService)
        {
            _accessService = accessService
                ?? throw new ArgumentNullException(nameof(accessService));
            _favoriteService = favoriteService
                ?? throw new ArgumentNullException(nameof(favoriteService));
        }

        #region Gets

        /// <summary>
        /// Returns treelist of options of a module per user.
        /// </summary>
        [Authorize]
        [Route("treelist")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TreeListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAccessToOptions(string moduleId)
        {
            try
            {
                var result = await _accessService.GetAccessToOptionsAsync(moduleId);

                return Ok(result);
            }
            catch (SecurityDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        /// <summary>
        /// Check if the authenticated user has access to the option and actions.
        /// </summary>
        /// <param name="optionId">Option Id.</param>
        [Authorize]
        [Route("options/{optionId}")]
        [HttpGet]
        [ProducesResponseType(typeof(MyAccessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAccessOption(string optionId)
        {
            try
            {
                var tAccess = _accessService.GetAccessOptionAync(optionId);
                var tResult = _favoriteService.CheckFavoriteAsync(optionId);
                var myAccess = await tAccess;
                var isFavorite = await tResult;

                myAccess.Option.Favorite = isFavorite;

                return Ok(myAccess);
            }
            catch (SecurityDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        #endregion
    }
}