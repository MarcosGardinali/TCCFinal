using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OutfitTrack.Application.ApiManagement;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;

namespace OutfitTrack.Api.Controllers;

[ApiController]
[Authorize]
[Produces("application/json")]
[Consumes("application/json")]
public class BaseController<TIService, TInputCreate, TInputUpdate, TOutput, TInputFilter> : Controller
    where TIService : IBaseService<TInputCreate, TInputUpdate, TOutput, TInputFilter>
    where TInputCreate : class
    where TInputUpdate : class
    where TOutput : class
    where TInputFilter : class
{
    protected readonly IApiDataService? _apiDataService;
    public Guid _guidApiDataRequest;
    public TIService? _service;

    public BaseController(IApiDataService apiDataService, TIService service)
    {
        _apiDataService = apiDataService;
        _service = service;
    }

    public BaseController(IApiDataService apiDataService)
    {
        _apiDataService = apiDataService;
    }

    #region Read
    /// <summary>
    /// Busca registros com filtros e paginação
    /// </summary>
    /// <param name="filter">Filtros de busca</param>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <returns>Lista paginada de registros</returns>
    [HttpPost("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<PaginatedResult<TOutput>>>> GetAllByFilter([FromBody] TInputFilter filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            return await ResponseAsync(_service!.GetAllByFilter(filter, pageNumber, pageSize));
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    /// <summary>
    /// Busca um registro específico pelo ID
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <returns>Dados completos do registro encontrado</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<TOutput>>> Get([FromRoute] long id)
    {
        try
        {
            var result = _service!.Get(id);
            if (result == null)
                return NotFound(new BaseResponseApi<string> { ErrorMessage = "Item não encontrado." });

            return await ResponseAsync(result);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
    #endregion

    #region Create
    /// <summary>
    /// Cria um novo registro
    /// </summary>
    /// <param name="inputCreate">Dados para criação do registro</param>
    /// <returns>Dados do registro criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<TOutput>>> Create([FromBody] TInputCreate inputCreate)
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
    #endregion

    #region Update
    /// <summary>
    /// Atualiza um registro existente
    /// </summary>
    /// <param name="id">ID do registro a ser atualizado</param>
    /// <param name="inputUpdate">Dados para atualização</param>
    /// <returns>Dados do registro atualizado</returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<TOutput>>> Update([FromRoute] long id, [FromBody] TInputUpdate inputUpdate)
    {
        try
        {
            var result = _service!.Update(id, inputUpdate);
            if (result == null)
                return NotFound(new BaseResponseApi<string> { ErrorMessage = "Item não encontrado para atualização." });

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
    #endregion

    #region Delete
    /// <summary>
    /// Exclui um registro
    /// </summary>
    /// <param name="id">ID do registro a ser excluído</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<bool>>> Delete([FromRoute] long id)
    {
        try
        {
            var result = _service!.Delete(id);
            return await ResponseAsync(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new BaseResponseApi<string> { ErrorMessage = "Item não encontrado para exclusão." });
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    #endregion

    [NonAction]
    public async Task<ActionResult> ResponseAsync<ResponseType>(ResponseType result, int? statusCode = null)
    {
        try
        {
            return await Task.FromResult(StatusCode(statusCode ?? 200, new BaseResponseApi<ResponseType> { Result = result }));
        }
        catch (Exception ex)
        {
            return await Task.FromResult(BadRequest(new BaseResponseApi<string> { ErrorMessage = $"Houve um problema interno com o servidor. Entre em contato com o Administrador do sistema caso o problema persista. Erro interno: {ex.InnerException?.Message ?? ex.Message}" }));
        }
    }

    [NonAction]
    public async Task<ActionResult> ResponseExceptionAsync(Exception ex)
    {
        return await Task.FromResult(BadRequest(new BaseResponseApi<string> { ErrorMessage = ex.InnerException?.Message ?? ex.Message }));
    }

    [NonAction]
    public void SetData()
    {
        Guid guidApiDataRequest = ApiData.CreateApiDataRequest();
        SetGuid(guidApiDataRequest);
    }

    [NonAction]
    public void SetGuid(Guid guidApiDataRequest)
    {
        _guidApiDataRequest = guidApiDataRequest;
        GenericModule.SetGuidApiDataRequest(this, guidApiDataRequest);
    }

    [NonAction]
    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            SetData();
        }
        catch (Exception ex)
        {
            context.Result = await ResponseExceptionAsync(ex);
        }
    }
}

#region TInputCreate, TInputUpdate, TOutput
public class BaseController_1<TIService>(IApiDataService apiDataService, TIService service) : BaseController<TIService, object, object, object, object>(apiDataService, service)
    where TIService : IBaseService_0
{ }
#endregion

#region TInputFilter
public class BaseController_1<TIService, TInputCreate, TInputUpdate, TOutput>(IApiDataService apiDataService, TIService service) : BaseController<TIService, TInputCreate, TInputUpdate, TOutput, object>(apiDataService, service)
    where TIService : IBaseService_1<TInputCreate, TInputUpdate, TOutput>
    where TInputCreate : class
    where TInputUpdate : class
    where TOutput : class
{ }
#endregion