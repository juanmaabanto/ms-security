using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofisoft.Accounts.Security.API.Application.Services;
using Sofisoft.Accounts.Security.API.Infrastructure.Exceptions;
using Sofisoft.Accounts.Security.API.Models;
using Sofisoft.Accounts.Security.API.ViewModels;

namespace Sofisoft.Accounts.Security.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService
                ?? throw new ArgumentNullException(nameof(favoriteService));
        }

        #region Post

        /// <summary>
        /// Add option to favorites.
        /// </summary>
        /// <param name="optionId">Option id.</param>
        [Authorize]
        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(Favorite), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(string optionId)
        {
            try
            {
                var favorite = await _favoriteService.AddAsync(optionId);

                return Created(string.Empty, favorite);
            }
            catch (SecurityDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        #endregion

        #region Deletes

        /// <summary>
        /// Drop option of favorites.
        /// </summary>
        /// <param name="optionId">Option id.</param>
        [Authorize]
        [Route("")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string optionId)
        {
            try
            {
                await _favoriteService.DropAsync(optionId);

                return NoContent();
            }
            catch (SecurityDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        #endregion
    }
}