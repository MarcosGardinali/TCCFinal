using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OutfitTrack.Api.Controllers;
using OutfitTrack.Application.ApiManagement;
using System.Security.Claims;

namespace OutfitTrack.Tests;

/// <summary>
/// Classe base para testes de controladores, contendo métodos auxiliares comuns
/// </summary>
public abstract class BaseControllerTests
{
    // Mocks usados em todos os testes de controladores
    protected readonly Mock<IApiDataService> MockApiDataService;
    protected readonly Mock<HttpContext> MockHttpContext;
    protected readonly Mock<ClaimsPrincipal> MockClaimsPrincipal;

    /// <summary>
    /// Construtor que inicializa os mocks comuns
    /// </summary>
    protected BaseControllerTests()
    {
        MockApiDataService = new Mock<IApiDataService>();
        MockHttpContext = new Mock<HttpContext>();
        MockClaimsPrincipal = new Mock<ClaimsPrincipal>();
    }

    /// <summary>
    /// Configura o contexto HTTP para os testes (simula informações da requisição)
    /// </summary>
    protected void SetupHttpContext()
    {
        MockHttpContext.Setup(x => x.User).Returns(MockClaimsPrincipal.Object);
        var mockHttpRequest = new Mock<HttpRequest>();
        var mockHost = new HostString("localhost");
        mockHttpRequest.Setup(x => x.Host).Returns(mockHost);
        MockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
    }

    /// <summary>
    /// Verifica se o resultado da action é um OK (200) com o valor esperado
    /// </summary>
    /// <typeparam name="T">Tipo do resultado esperado</typeparam>
    /// <param name="result">ActionResult retornado pelo método do controlador</param>
    /// <param name="expectedValue">Valor esperado no resultado</param>
    protected static void AssertOkResult<T>(ActionResult<BaseResponseApi<T>> result, T expectedValue)
    {
        // Verificar se o resultado é um ObjectResult (que é o que o ResponseAsync retorna)
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<T>;
        Assert.NotNull(response);
        Assert.Equal(expectedValue, response!.Result);
    }

    /// <summary>
    /// Verifica se o resultado da action é um Created (201) com o valor esperado
    /// </summary>
    /// <typeparam name="T">Tipo do resultado esperado</typeparam>
    /// <param name="result">ActionResult retornado pelo método do controlador</param>
    /// <param name="expectedValue">Valor esperado no resultado</param>
    protected static void AssertCreatedResult<T>(ActionResult<BaseResponseApi<T>> result, T expectedValue)
    {
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(201, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<T>;
        Assert.NotNull(response);
        Assert.Equal(expectedValue, response!.Result);
    }

    /// <summary>
    /// Verifica se o resultado da action é um NotFound (404) com a mensagem esperada
    /// </summary>
    /// <typeparam name="T">Tipo do resultado esperado</typeparam>
    /// <param name="result">ActionResult retornado pelo método do controlador</param>
    /// <param name="expectedMessage">Mensagem de erro esperada</param>
    protected static void AssertNotFoundResult<T>(ActionResult<BaseResponseApi<T>> result, string expectedMessage)
    {
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(404, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<string>;
        Assert.NotNull(response);
        Assert.Equal(expectedMessage, response!.ErrorMessage);
    }

    /// <summary>
    /// Verifica se o resultado da action é um BadRequest (400) com a mensagem esperada
    /// </summary>
    /// <typeparam name="T">Tipo do resultado esperado</typeparam>
    /// <param name="result">ActionResult retornado pelo método do controlador</param>
    /// <param name="expectedMessage">Mensagem de erro esperada</param>
    protected static void AssertBadRequestResult<T>(ActionResult<BaseResponseApi<T>> result, string expectedMessage)
    {
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(400, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<string>;
        Assert.NotNull(response);
        Assert.Equal(expectedMessage, response!.ErrorMessage);
    }
}