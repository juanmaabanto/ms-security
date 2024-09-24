using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sofisoft.Accounts.Security.API.Application.Adapters;
using Sofisoft.Accounts.Security.API.Application.Services;
using Sofisoft.Accounts.Security.API.Infrastructure.Exceptions;
using Sofisoft.Accounts.Security.API.ViewModels;

namespace Sofisoft.Accounts.Security.API.Application.WebClients
{
    public class WorkingSpaceWebClient : IWorkingSpaceWebClient
    {
        private readonly IIdentityService _identityService;
        private readonly ILoggingWebClient _logger;
        private IOptions<SecuritySetting> _settings;

        public WorkingSpaceWebClient(IIdentityService identityService,
            ILoggingWebClient logger,
            IOptions<SecuritySetting> settings)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<OptionDto> GetOptionByIdAsync(string optionId)
        {
            var token = _identityService.Token;
            var userName = _identityService.UserName;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync($"{_settings.Value.Services.WorkingSpaceUrl}/api/v1/options/{optionId}");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var serializeOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = true
                        };

                        return JsonSerializer.Deserialize<OptionDto>(content, serializeOptions);
                    }
                    else if(response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var error = JsonSerializer.Deserialize<ErrorViewModel>(content);

                        throw new SecurityDomainException(error.Message, error.EventLogId);
                    }
                    else if(response.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new KeyNotFoundException(nameof(optionId));
                    }
                    else
                    {
                        throw new Exception($"No se pudo conectar al servicio: {response.StatusCode}");
                    }
                }
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

                throw new SecurityDomainException("Ocurrio un error conectandose al servicio.", result);
            }
        }
        
        public async Task<List<OptionListDto>> GetOptionsByModuleAsync(string moduleId)
        {
            var token = _identityService.Token;
            var userName = _identityService.UserName;

            try
            {
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(_settings.Value.Services.WorkingSpaceUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync($"{_settings.Value.Services.WorkingSpaceUrl}/api/v1/Modules/{moduleId}/options");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var serializeOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = true
                        };
                        return JsonSerializer.Deserialize<List<OptionListDto>>(content, serializeOptions);
                    }
                    else if(response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var error = JsonSerializer.Deserialize<ErrorViewModel>(content);

                        throw new SecurityDomainException(error.Message, error.EventLogId);
                    }
                    else
                    {
                        throw new Exception($"No se pudo conectar al servicio: {response.StatusCode}");
                    }
                }
            }
            catch (SecurityDomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.StackTrace, userName);

                throw new SecurityDomainException("Ocurrio un error conectandose al servicio.", result);
            }
        }
    }
}