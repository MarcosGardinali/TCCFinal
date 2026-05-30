namespace OutfitTrack.Arguments;

/// <summary>
/// Métricas consolidadas de produtos
/// </summary>
public class OutputMetricsProduct
{
    /// <summary>
    /// Total de produtos cadastrados no sistema
    /// </summary>
    public int TotalProducts { get; set; }
    /// <summary>
    /// Preço médio dos produtos do catálogo
    /// </summary>
    public decimal AveragePrice { get; set; }
}
