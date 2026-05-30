using Microsoft.AspNetCore.Mvc;
using OutfitTrack.Application.ApiManagement;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;
using System.ComponentModel.DataAnnotations;

namespace OutfitTrack.Api.Controllers;

[Route("api/[controller]")]
public class OrderController(IApiDataService apiDataService, IOrderService service) : BaseController<IOrderService, InputCreateOrder, InputUpdateOrder, OutputOrder, InputFilterOrder>(apiDataService, service)
{
    /// <summary>
    /// Busca uma condicional específica pelo número
    /// </summary>
    /// <param name="number">Número da condicional</param>
    /// <returns>Dados completos da condicional encontrada</returns>
    [HttpGet("number/{number:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputOrder>>> GetByNumber([FromRoute] long number)
    {
        try
        {
            var result = _service!.GetByNumber(number);
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
    /// Obtém métricas detalhadas dos pedidos em um período específico
    /// </summary>
    /// <param name="startDate">Data inicial (obrigatória)</param>
    /// <param name="endDate">Data final (obrigatória)</param>
    /// <returns>Objeto contendo total de pedidos, receita, pendentes, status e evolução diária</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputMetricsOrder>>> GetMetrics([FromQuery][Required] DateTime startDate, [FromQuery][Required] DateTime endDate)
    {
        try
        {
            var result = _service!.GetMetrics(startDate, endDate);
            return await ResponseAsync(result);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    /// <summary>
    /// Obtém métricas gerais de vendas (receita, volume, ticket médio) em um período específico
    /// </summary>
    /// <param name="startDate">Data inicial (obrigatória)</param>
    /// <param name="endDate">Data final (obrigatória)</param>
    /// <returns>Resumo consolidado de vendas e itens vendidos</returns>
    [HttpGet("sales-metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputSalesMetricsOrder>>> GetSalesMetrics([FromQuery][Required] DateTime startDate, [FromQuery][Required] DateTime endDate)
    {
        try
        {
            var result = _service!.GetSalesMetrics(startDate, endDate);
            return await ResponseAsync(result);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    /// <summary>
    /// Fecha uma condicional, finalizando o processo de venda
    /// </summary>
    /// <param name="id">ID da condicional a ser fechada</param>
    /// <returns>Confirmação do fechamento da condicional</returns>
    [HttpPut("{id:long}/close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<bool>>> Close([FromRoute] long id)
    {
        try
        {
            var result = _service!.Close(id);
            return await ResponseAsync(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new BaseResponseApi<string> { ErrorMessage = "Item não encontrado." });
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
}