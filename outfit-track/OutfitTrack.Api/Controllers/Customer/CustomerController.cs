using Microsoft.AspNetCore.Mvc;
using OutfitTrack.Application.ApiManagement;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;

namespace OutfitTrack.Api.Controllers;

[Route("api/[controller]")]
public class CustomerController(IApiDataService apiDataService, ICustomerService service) : BaseController<ICustomerService, InputCreateCustomer, InputUpdateCustomer, OutputCustomer, InputFilterCustomer>(apiDataService, service)
{
    /// <summary>
    /// Busca um cliente pelo CPF
    /// </summary>
    /// <param name="cpf">CPF do cliente (apenas números)</param>
    /// <returns>Dados completos do cliente encontrado</returns>
    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputCustomer>>> GetByCpf([FromRoute] string cpf)
    {
        try
        {
            var result = _service!.GetByCpf(cpf);
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
    /// Obtém o cliente com maior número de condicionais
    /// </summary>
    /// <returns>Cliente que mais fez condicionais na loja</returns>
    [HttpGet("top-orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputCustomer>>> GetTopByOrders()
    {
        try
        {
            var result = _service!.GetTopByOrders();
            if (result == null)
                return NotFound(new BaseResponseApi<string> { ErrorMessage = "Nenhum cliente encontrado." });

            return await ResponseAsync(result);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
    /// <summary>
    /// Obtém métricas completas dos clientes (Total, Ativos, Idade Média, Cadastros/Mês)
    /// </summary>
    /// <returns>Objeto contendo métricas de performance dos clientes</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputMetricsCustomer>>> GetMetrics()
    {
        try
        {
            var result = _service!.GetMetrics();
            return await ResponseAsync(result);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
}