using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutfitTrack.Application.ApiManagement;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;

namespace OutfitTrack.Api.Controllers;

[Route("api/[controller]")]
public class AuthenticationController(IApiDataService apiDataService, IAuthenticationService service) : BaseController_1<IAuthenticationService>(apiDataService, service)
{
    /// <summary>
    /// Autentica um usuário no sistema
    /// </summary>
    /// <param name="inputAuthentication">Dados de login (email e senha)</param>
    /// <returns>Token JWT para acesso às funcionalidades do sistema</returns>
    [AllowAnonymous]
    [HttpPost("Authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponseApi<OutputAuthentication>>> Authenticate(InputAuthentication inputAuthentication)
    {
        try
        {
            return await ResponseAsync(_service!.Authenticate(inputAuthentication));
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    #region IgnoreApi
    [ApiExplorerSettings(IgnoreApi = true)]
    public override Task<ActionResult<BaseResponseApi<PaginatedResult<object>>>> GetAllByFilter([FromBody] object filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return base.GetAllByFilter(filter, pageNumber, pageSize);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override Task<ActionResult<BaseResponseApi<object>>> Get([FromRoute] long id)
    {
        return base.Get(id);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override Task<ActionResult<BaseResponseApi<object>>> Create([FromBody] object inputCreate)
    {
        return base.Create(inputCreate);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override Task<ActionResult<BaseResponseApi<object>>> Update([FromRoute] long id, [FromBody] object inputUpdate)
    {
        return base.Update(id, inputUpdate);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override Task<ActionResult<BaseResponseApi<bool>>> Delete([FromRoute] long id)
    {
        return base.Delete(id);
    }
    #endregion
}