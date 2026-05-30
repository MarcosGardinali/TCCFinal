using Moq;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Tests.Services;

/// <summary>
/// Classe base para testes de serviços, fornecendo métodos auxiliares para criação de mocks
/// </summary>
public class BaseServiceTest
{
    #region Helper Methods for Unit of Work and Repositories

    /// <summary>
    /// Cria e configura os mocks básicos necessários para os testes com UnitOfWork
    /// </summary>
    /// <typeparam name="TRepository">Tipo do repositório</typeparam>
    /// <typeparam name="TEntity">Tipo da entidade</typeparam>
    /// <returns>Mocks configurados</returns>
    protected static (Mock<IUnitOfWork> mockUnitOfWork, Mock<TRepository> mockRepository) CreateBasicMocks<TRepository, TEntity, TInputFilter>()
        where TRepository : class, IBaseRepository<TEntity, TInputFilter>
        where TEntity : BaseEntity<TEntity>
        where TInputFilter : class
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<TRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<TRepository, TEntity, TInputFilter>())
            .Returns(mockRepository.Object);

        return (mockUnitOfWork, mockRepository);
    }

    /// <summary>
    /// Cria e configura os mocks básicos necessários para os testes com UnitOfWork e repositórios adicionais
    /// </summary>
    /// <typeparam name="TRepository1">Tipo do primeiro repositório</typeparam>
    /// <typeparam name="TEntity1">Tipo da primeira entidade</typeparam>
    /// <typeparam name="TInputFilter1">Tipo da filtragem da primeira entidade</typeparam>
    /// <typeparam name="TRepository2">Tipo do segundo repositório</typeparam>
    /// <typeparam name="TEntity2">Tipo da segunda entidade</typeparam>
    /// <typeparam name="TInputFilter2">Tipo da filtragem da segunda entidade</typeparam>
    /// <returns>Mocks configurados</returns>
    protected static (Mock<IUnitOfWork> mockUnitOfWork, Mock<TRepository1> mockRepository1, Mock<TRepository2> mockRepository2)
        CreateBasicMocks<TRepository1, TEntity1, TInputFilter1, TRepository2, TEntity2, TInputFilter2>()
        where TRepository1 : class, IBaseRepository<TEntity1, TInputFilter1>
        where TEntity1 : BaseEntity<TEntity1>
        where TInputFilter1 : class
        where TRepository2 : class, IBaseRepository<TEntity2, TInputFilter2>
        where TEntity2 : BaseEntity<TEntity2>
        where TInputFilter2 : class
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository1 = new Mock<TRepository1>();
        var mockRepository2 = new Mock<TRepository2>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<TRepository1, TEntity1, TInputFilter1>())
            .Returns(mockRepository1.Object);

        mockUnitOfWork.Setup(uow => uow.GetRepository<TRepository2, TEntity2, TInputFilter2>())
            .Returns(mockRepository2.Object);

        return (mockUnitOfWork, mockRepository1, mockRepository2);
    }

    /// <summary>
    /// Cria e configura os mocks básicos necessários para os testes com UnitOfWork e três repositórios adicionais
    /// </summary>
    /// <typeparam name="TRepository1">Tipo do primeiro repositório</typeparam>
    /// <typeparam name="TEntity1">Tipo da primeira entidade</typeparam>
    /// <typeparam name="TInputFilter1">Tipo da filtragem da primeira entidade</typeparam>
    /// <typeparam name="TRepository2">Tipo do segundo repositório</typeparam>
    /// <typeparam name="TEntity2">Tipo da segunda entidade</typeparam>
    /// <typeparam name="TInputFilter2">Tipo da filtragem da segunda entidade</typeparam>
    /// <typeparam name="TRepository3">Tipo do terceiro repositório</typeparam>
    /// <typeparam name="TEntity3">Tipo da terceira entidade</typeparam>
    /// <typeparam name="TInputFilter3">Tipo da filtragem da terceira entidade</typeparam>
    /// <returns>Mocks configurados</returns>
    protected static (Mock<IUnitOfWork> mockUnitOfWork, Mock<TRepository1> mockRepository1,
        Mock<TRepository2> mockRepository2, Mock<TRepository3> mockRepository3)
        CreateBasicMocks<TRepository1, TEntity1, TInputFilter1, TRepository2, TEntity2, TInputFilter2, TRepository3, TEntity3, TInputFilter3>()
        where TRepository1 : class, IBaseRepository<TEntity1, TInputFilter1>
        where TEntity1 : BaseEntity<TEntity1>
        where TInputFilter1 : class
        where TRepository2 : class, IBaseRepository<TEntity2, TInputFilter2>
        where TEntity2 : BaseEntity<TEntity2>
        where TInputFilter2 : class
        where TRepository3 : class, IBaseRepository<TEntity3, TInputFilter3>
        where TEntity3 : BaseEntity<TEntity3>
        where TInputFilter3 : class
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository1 = new Mock<TRepository1>();
        var mockRepository2 = new Mock<TRepository2>();
        var mockRepository3 = new Mock<TRepository3>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<TRepository1, TEntity1, TInputFilter1>())
            .Returns(mockRepository1.Object);

        mockUnitOfWork.Setup(uow => uow.GetRepository<TRepository2, TEntity2, TInputFilter2>())
            .Returns(mockRepository2.Object);

        mockUnitOfWork.Setup(uow => uow.GetRepository<TRepository3, TEntity3, TInputFilter3>())
            .Returns(mockRepository3.Object);

        return (mockUnitOfWork, mockRepository1, mockRepository2, mockRepository3);
    }

    #endregion
}