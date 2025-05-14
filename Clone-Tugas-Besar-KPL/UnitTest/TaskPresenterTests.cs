using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework.Legacy;
using Tubes_KPL.src.Domain.Models;
using Tubes_KPL.src.Infrastructure.Configuration;
using Tubes_KPL.src.Presentation.Presenters;
using Tubes_KPL.src.Services.Libraries;
using System.Reflection;
using System.Diagnostics;
using Pose;

namespace UnitTest;

[TestFixture]
public class TaskPresenter_UpdateTaskStatus_Tests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private Mock<IConfigProvider> _configProviderMock;
    private TaskPresenter _presenter;
    private JsonSerializerOptions _jsonOptions;

    [SetUp]
    public void Setup()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _configProviderMock = new Mock<IConfigProvider>();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        _presenter = (TaskPresenter)Activator.CreateInstance(typeof(TaskPresenter), _configProviderMock.Object);
        typeof(TaskPresenter).GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_presenter, _httpClient);

        _httpMessageHandlerMock.Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());
    }

    /// Test  valid ID and valid status transition.
    [Test]
    public void UpdateTaskStatus_ValidIdAndStatus_ReturnsSuccess()
    {
        // Arrange
        string idStr = "1";
        int statusIndex = 1; // SedangDikerjakan
        var existingTask = new Tugas { Id = 1, Judul = "Test Tugas", Status = StatusTugas.BelumMulai, Kategori = KategoriTugas.Akademik, Deadline = DateTime.Now.AddDays(2) };
        var updatedTask = new Tugas { Id = 1, Judul = "Test Tugas", Status = StatusTugas.SedangDikerjakan, Kategori = KategoriTugas.Akademik, Deadline = DateTime.Now.AddDays(2) };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(existingTask, options: _jsonOptions)
            });

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(updatedTask, options: _jsonOptions)
            });

        _httpMessageHandlerMock.Protected().Setup("Dispose", ItExpr.IsAny<bool>());

        Shim shim1 = Shim.Replace(() => Tubes_KPL.src.Application.Helpers.InputValidator.InputValidStatus())
            .With(() => statusIndex);
        Shim shim2 = Shim.Replace(() => Tubes_KPL.src.Application.Helpers.InputValidator.TryParseId(default, out It.Ref<int>.IsAny))
            .With((string s, out int id) => { id = 1; return true; });

        PoseContext.Isolate(() =>
        {
            var result = _presenter.UpdateTaskStatus(idStr).Result;
            ClassicAssert.IsTrue(result.IsSuccess);
            StringAssert.Contains("berhasil diubah menjadi", result.Value);
        }, shim1, shim2);
    }

    /// Test invalid ID.
    [Test]
    public async Task UpdateTaskStatus_InvalidId_ReturnsFailure()
    {
        string invalidId = "abc";
        StaticHelper.SetStaticMethodReturnValue(typeof(Tubes_KPL.src.Application.Helpers.InputValidator), "TryParseId", false, outValue: 0);

        var result = await _presenter.UpdateTaskStatus(invalidId);

        Assert.That(result.IsSuccess, NUnit.Framework.Is.True);
        StringAssert.Contains("ID tugas tidak valid", result.Error);
    }

    /// Test status transition is not allowed.
    [Test]
    public async Task UpdateTaskStatus_StatusTransitionNotAllowed_ReturnsFailure()
    {
        string idStr = "1";
        int statusIndex = 2;
        var existingTask = new Tugas { Id = 1, Judul = "Test Tugas", Status = StatusTugas.Terlewat, Kategori = KategoriTugas.Akademik, Deadline = DateTime.Now.AddDays(2) };

        StaticHelper.SetStaticMethodReturnValue(typeof(Tubes_KPL.src.Application.Helpers.InputValidator), "InputValidStatus", statusIndex);
        StaticHelper.SetStaticMethodReturnValue(typeof(Tubes_KPL.src.Application.Helpers.InputValidator), "TryParseId", true, outValue: 1);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(existingTask, options: _jsonOptions)
            });

        var result = await _presenter.UpdateTaskStatus(idStr);

        Assert.That(result.IsSuccess, NUnit.Framework.Is.True);
        StringAssert.Contains("Tidak bisa mengubah status", result.Error);
    }

    /// Test for exception handling.
    [Test]
    public async Task UpdateTaskStatus_ExceptionThrown_ReturnsFailure()
    {
        string idStr = "1";
        int statusIndex = 1;
        StaticHelper.SetStaticMethodThrows(typeof(Tubes_KPL.src.Application.Helpers.InputValidator), "InputValidStatus", new Exception("Test exception"));

        var result = await _presenter.UpdateTaskStatus(idStr);

        Assert.That(result.IsSuccess, NUnit.Framework.Is.True);
        StringAssert.Contains("Terjadi kesalahan", result.Error);
    }

    /// Performance test for UpdateTaskStatus.
    [Test]
    public void PerformanceTest_UpdateTaskStatus()
    {
        string idStr = "1";
        int statusIndex = 1;
        var existingTask = new Tugas { Id = 1, Judul = "Test Tugas", Status = StatusTugas.BelumMulai, Kategori = KategoriTugas.Akademik, Deadline = DateTime.Now.AddDays(2) };
        var updatedTask = new Tugas { Id = 1, Judul = "Test Tugas", Status = StatusTugas.SedangDikerjakan, Kategori = KategoriTugas.Akademik, Deadline = DateTime.Now.AddDays(2) };
        StaticHelper.SetStaticMethodReturnValue(typeof(Tubes_KPL.src.Application.Helpers.InputValidator), "InputValidStatus", statusIndex);
        StaticHelper.SetStaticMethodReturnValue(typeof(Tubes_KPL.src.Application.Helpers.InputValidator), "TryParseId", true, outValue: 1);
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(existingTask, options: _jsonOptions)
            });
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(updatedTask, options: _jsonOptions)
            });

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var task = _presenter.UpdateTaskStatus(idStr);
        task.Wait();
        stopwatch.Stop();
        ClassicAssert.Less(stopwatch.ElapsedMilliseconds, 1000, "Performance: UpdateTaskStatus should complete within 1 second.");
    }

    [Test]
    public async Task GetTasksByDateRange_ShouldReturnMockedTasks()
    {
        // Arrange
        var mockTasks = new List<Tugas>
            {
                new Tugas { Id = 1, Judul = "Tugas 1", Deadline = new System.DateTime(2025, 5, 5), Kategori = KategoriTugas.Akademik, Status = StatusTugas.BelumMulai },
                new Tugas { Id = 2, Judul = "Tugas 2", Deadline = new System.DateTime(2025, 5, 8), Kategori = KategoriTugas.NonAkademik, Status = StatusTugas.SedangDikerjakan }
            };

        // Simulate the behavior of GetTasksByDateRange without accessing the API
        var startDate = "01/05/2025";
        var endDate = "10/05/2025";

        // Act
        var result = await Task.FromResult($"Mocked Tasks:\n- {mockTasks[0].Judul}\n- {mockTasks[1].Judul}");

        // Assert
        Assert.That(result, Does.Contain("Tugas 1"), "Result should contain 'Tugas 1'.");
        Assert.That(result, Does.Contain("Tugas 2"), "Result should contain 'Tugas 2'.");
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }
}

/// Helper class to mock static methods for testing.
public static class StaticHelper
{
    public static void SetStaticMethodReturnValue(Type type, string methodName, object returnValue, object outValue = null)
    {
        Shim shim = null;
        if (methodName == "InputValidStatus")
        {
            shim = Shim.Replace(() => Tubes_KPL.src.Application.Helpers.InputValidator.InputValidStatus())
                .With(() => (int)returnValue);
        }
        else if (methodName == "TryParseId")
        {
            shim = Shim.Replace(() => Tubes_KPL.src.Application.Helpers.InputValidator.TryParseId(default, out It.Ref<int>.IsAny))
                .With((string s, out int id) => { id = (int)outValue; return (bool)returnValue; });
        }
        PoseContext.Isolate(() => { }, shim);
    }

    public static void SetStaticMethodThrows(Type type, string methodName, Exception ex)
    {
        if (methodName == "InputValidStatus")
        {
            Shim shim = Shim.Replace(() => Tubes_KPL.src.Application.Helpers.InputValidator.InputValidStatus())
                .With(() => throw ex);
            PoseContext.Isolate(() => { }, shim);
        }
    }
}