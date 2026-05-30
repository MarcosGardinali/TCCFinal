using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Application.Services;

public class ProductService(IUnitOfWork unitOfWork) : BaseService<IProductRepository, InputCreateProduct, InputUpdateProduct, Product, OutputProduct, InputFilterProduct>(unitOfWork), IProductService
{
    public OutputProduct? GetByCode(string code)
    {
        var product = _repository!.Get(x => x.Code == code);
        return product != null ? FromEntityToOutput(product) : null;
    }

    public override OutputProduct Create(InputCreateProduct inputCreate)
    {
        ValidateCodeUniqueness(inputCreate.Code!);
        return base.Create(inputCreate);
    }

    public override OutputProduct Update(long id, InputUpdateProduct inputUpdate)
    {
        return base.Update(id, inputUpdate);
    }

    public override bool Delete(long id)
    {
        var product = _repository!.Get(x => x.Id == id)
            ?? throw new KeyNotFoundException("Id inválido ou inexistente.");
        ValidateProductCanBeDeleted(product);
        ExecuteWithCommit(() => _repository!.Delete(product));
        return true;
    }

    public OutputProduct? GetTopByOrders()
    {
        var topProduct = _repository!.GetTopByOrders();
        return topProduct != null ? FromEntityToOutput(topProduct) : null;
    }

    public OutputMetricsProduct GetMetrics()
    {
        return _repository!.GetMetrics();
    }

    #region Business Validations
    private void ValidateCodeUniqueness(string code)
    {
        var existingProduct = _repository!.Get(x => x.Code == code);
        if (existingProduct != null)
            throw new InvalidOperationException($"Código '{code}' já cadastrado.");
    }

    private static void ValidateProductCanBeDeleted(Product product)
    {
        if (product.ListOrderItem?.Count > 0)
            throw new InvalidOperationException("Esse produto possui vínculo com itens de pedido");
    }
    #endregion

    #region Mappers
    public override OutputProduct FromEntityToOutput(Product entity)
    {
        return new OutputProduct
        {
            Id = entity.Id ?? 0,
            CreationDate = entity.CreationDate ?? DateTime.MinValue,
            ChangeDate = entity.ChangeDate,
            Code = entity.Code,
            Description = entity.Description,
            Price = entity.Price,
            Brand = entity.Brand,
            Category = entity.Category
        };
    }

    public override Product FromInputCreateToEntity(InputCreateProduct inputCreate)
    {
        var entity = new Product();

        entity.SetProperty(nameof(Product.Code), inputCreate.Code);
        entity.SetProperty(nameof(Product.Description), inputCreate.Description);
        entity.SetProperty(nameof(Product.Price), inputCreate.Price);
        entity.SetProperty(nameof(Product.Brand), inputCreate.Brand);
        entity.SetProperty(nameof(Product.Category), inputCreate.Category);

        return entity;
    }
    #endregion
}