using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutfitTrack.Application.ApiManagement;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;

namespace OutfitTrack.Api.Controllers;

[Route("api/[controller]")]
public class UserController(IApiDataService apiDataService, IUserService service) : BaseController_1<IUserService, InputCreateUser, InputUpdateUser, OutputUser>(apiDataService, service)
{
    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="inputCreate">Dados do novo usuário (email, senha, confirmação)</param>
    /// <returns>Dados do usuário criado</returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<BaseResponseApi<OutputUser>>> Create([FromBody] InputCreateUser inputCreate)
    {
        try
        {
            return await ResponseAsync(_service!.Create(inputCreate), 201);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    /// <summary>
    /// Busca um usuário pelo email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Dados do usuário encontrado</returns>
    [HttpGet("email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputUser>>> GetByEmail([FromRoute] string email)
    {
        try
        {
            var result = _service!.GetByEmail(email);
            if (result == null)
                return NotFound(new BaseResponseApi<string> { ErrorMessage = "Item não encontrado." });

            return await ResponseAsync(result);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    /// <summary>
    /// Redefine a senha de um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="inputRedefinePassword">Nova senha e confirmação</param>
    /// <returns>Confirmação da alteração da senha</returns>
    [HttpPut("{id:long}/redefinePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponseApi<bool>>> RedefinePassword([FromRoute] long id, [FromBody] InputRedefinePasswordUser inputRedefinePassword)
    {
        try
        {
            var result = _service!.RedefinePassword(id, inputRedefinePassword);
            return await ResponseAsync(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new BaseResponseApi<string> { ErrorMessage = "Id inválido ou inexistente." });
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    #region IgnoreApi
    [ApiExplorerSettings(IgnoreApi = true)]
    public override Task<ActionResult<BaseResponseApi<PaginatedResult<OutputUser>>>> GetAllByFilter([FromBody] object filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return base.GetAllByFilter(filter, pageNumber, pageSize);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override Task<ActionResult<BaseResponseApi<OutputUser>>> Get([FromRoute] long id)
    {
        return base.Get(id);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override Task<ActionResult<BaseResponseApi<bool>>> Delete([FromRoute] long id)
    {
        return base.Delete(id);
    }
    #endregion
}