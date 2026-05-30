using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Application.Services;

public class CustomerService(IUnitOfWork unitOfWork) : BaseService<ICustomerRepository, InputCreateCustomer, InputUpdateCustomer, Customer, OutputCustomer, InputFilterCustomer>(unitOfWork), ICustomerService
{
    public OutputCustomer? GetByCpf(string cpf)
    {
        var customer = _repository!.Get(x => x.Cpf == cpf);
        return customer != null ? FromEntityToOutput(customer) : null;
    }

    public OutputCustomer? GetTopByOrders()
    {
        var topCustomer = _repository!.GetTopByOrders();
        return topCustomer != null ? FromEntityToOutput(topCustomer) : null;
    }

    public OutputMetricsCustomer GetMetrics()
    {
        return _repository!.GetMetrics();
    }

    public override OutputCustomer Create(InputCreateCustomer inputCreate)
    {
        ValidateCpfUniqueness(inputCreate.Cpf!);
        return base.Create(inputCreate);
    }

    public override OutputCustomer Update(long id, InputUpdateCustomer inputUpdate)
    {
        return base.Update(id, inputUpdate);
    }

    public override bool Delete(long id)
    {
        var customer = _repository!.Get(x => x.Id == id)
            ?? throw new KeyNotFoundException("Id inválido ou inexistente.");
        ValidateCustomerCanBeDeleted(customer);
        ExecuteWithCommit(() => _repository!.Delete(customer));
        return true;
    }

    #region Business Validations
    private void ValidateCpfUniqueness(string cpf)
    {
        var existingCustomer = _repository!.Get(x => x.Cpf == cpf);
        if (existingCustomer != null)
            throw new InvalidOperationException($"Cpf '{cpf}' já cadastrado.");
    }

    private static void ValidateCustomerCanBeDeleted(Customer customer)
    {
        if (customer.ListOrder?.Count > 0)
            throw new InvalidOperationException("Esse cliente possui vínculo com pedidos");
    }
    #endregion

    #region Mappers
    public override OutputCustomer FromEntityToOutput(Customer entity)
    {
        return new OutputCustomer
        {
            Id = entity.Id ?? 0,
            CreationDate = entity.CreationDate ?? DateTime.MinValue,
            ChangeDate = entity.ChangeDate,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            Cpf = entity.Cpf,
            Street = entity.Street,
            Complement = entity.Complement,
            Neighborhood = entity.Neighborhood,
            Number = entity.Number,
            CityName = entity.CityName,
            StateAbbreviation = entity.StateAbbreviation,
            PostalCode = entity.PostalCode,
            Rg = entity.Rg,
            MobilePhoneNumber = entity.MobilePhoneNumber,
            Email = entity.Email
        };
    }

    public override Customer FromInputCreateToEntity(InputCreateCustomer inputCreate)
    {
        return new Customer(
            inputCreate.FirstName,
            inputCreate.LastName,
            inputCreate.BirthDate,
            inputCreate.Cpf,
            inputCreate.Street,
            inputCreate.Complement,
            inputCreate.Neighborhood,
            inputCreate.Number,
            inputCreate.CityName,
            inputCreate.StateAbbreviation,
            inputCreate.PostalCode,
            inputCreate.Rg,
            inputCreate.MobilePhoneNumber,
            inputCreate.Email,
            null
        );
    }
    #endregion
}