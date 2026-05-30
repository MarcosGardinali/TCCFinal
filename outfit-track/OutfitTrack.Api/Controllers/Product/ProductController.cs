using Microsoft.AspNetCore.Mvc;
using OutfitTrack.Application.ApiManagement;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;

namespace OutfitTrack.Api.Controllers;

[Route("api/[controller]")]
public class ProductController(IApiDataService apiDataService, IProductService service) : BaseController<IProductService, InputCreateProduct, InputUpdateProduct, OutputProduct, InputFilterProduct>(apiDataService, service)
{
    /// <summary>
    /// Busca um produto pelo código
    /// </summary>
    /// <param name="code">Código único do produto</param>
    /// <returns>Dados completos do produto encontrado</returns>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputProduct>>> GetByCode([FromRoute] string code)
    {
        try
        {
            var result = _service!.GetByCode(code);
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
    /// Obtém o produto mais popular em condicionais
    /// </summary>
    /// <returns>Produto que mais aparece nas malinhas condicionais</returns>
    [HttpGet("top-orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputProduct>>> GetTopByOrders()
    {
        try
        {
            var result = _service!.GetTopByOrders();
            if (result == null)
                return NotFound(new BaseResponseApi<string> { ErrorMessage = "Nenhum produto encontrado." });

            return await ResponseAsync(result);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
    /// <summary>
    /// Obtém métricas completas dos produtos (Total, Preço Médio)
    /// </summary>
    /// <returns>Objeto contendo métricas de performance dos produtos</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponseApi<string>>(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<BaseResponseApi<OutputMetricsProduct>>> GetMetrics()
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