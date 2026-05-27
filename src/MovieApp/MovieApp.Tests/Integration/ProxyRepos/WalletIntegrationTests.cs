using System.Net;
using System.Net.Http.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class WalletIntegrationTests
{
    [Fact]
    public async Task GetBalance_SeededUser_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/users/{ProxyRepoSeedIds.SeededUserId}/balance");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBalance_SeededUser_ReturnsSeededBalance()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        UserProxyService userService = new(testContext.ApiClient);

        decimal balance = await userService.GetBalanceAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.True(balance >= 0);
    }

    [Fact]
    public async Task UpdateBalance_SeededUser_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PutAsJsonAsync(
            $"api/users/{ProxyRepoSeedIds.SeededUserId}/balance",
            new { NewBalance = 999.99m });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBalance_BalanceIsConsistentAcrossUserAndBalanceEndpoints()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        UserProxyService userService = new(testContext.ApiClient);

        const decimal expectedBalance = 4242.42m;
        await userService.UpdateBalanceAsync(ProxyRepoSeedIds.SeededUserId, expectedBalance);

        decimal balanceFromEndpoint = await userService.GetBalanceAsync(ProxyRepoSeedIds.SeededUserId);
        User? userDto = await userService.GetUserByIdAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(expectedBalance, balanceFromEndpoint);
        Assert.Equal(expectedBalance, userDto?.Balance);
    }

    [Fact]
    public async Task LogTransaction_ValidBuyerAndMovie_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync("api/transactions", new
        {
            Amount = 9.99m,
            Type = "BuyMovie",
            Status = "Completed",
            Timestamp = DateTime.UtcNow,
            BuyerId = ProxyRepoSeedIds.SeededUserId,
            MovieId = ProxyRepoSeedIds.SeededMovieId,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTransactionsByUserId_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/transactions/users/{ProxyRepoSeedIds.SeededUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTransactionsByUserId_AfterLogging_ContainsLoggedTransaction()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        TransactionProxyService transactionService = new(testContext.ApiClient);

        await transactionService.LogTransactionAsync(new Transaction
        {
            Amount = 14.99m,
            Type = "BuyMovie",
            Status = "Completed",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });

        List<Transaction> transactions = await transactionService.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Contains(transactions, t => t.Amount == 14.99m && t.Type == "BuyMovie");
    }


    [Fact]
    public async Task GetTransactionsByUserId_MultipleTransactions_OrderedNewestFirst()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        TransactionProxyService transactionService = new(testContext.ApiClient);

        DateTime older = DateTime.UtcNow.AddMinutes(-5);
        DateTime newer = DateTime.UtcNow;

        await transactionService.LogTransactionAsync(new Transaction
        {
            Amount = 5.00m,
            Type = "BuyMovie",
            Status = "Completed",
            Timestamp = older,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });
        await transactionService.LogTransactionAsync(new Transaction
        {
            Amount = 10.00m,
            Type = "BuyMovie",
            Status = "Completed",
            Timestamp = newer,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });

        List<Transaction> transactions = await transactionService.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.True(transactions.Count >= 2);
        Assert.True(transactions[0].Timestamp >= transactions[1].Timestamp);
    }

    [Fact]
    public async Task UpdateTransactionStatus_PendingToCompleted_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        TransactionProxyService transactionService = new(testContext.ApiClient);

        await transactionService.LogTransactionAsync(new Transaction
        {
            Amount = 8.50m,
            Type = "BuyMovie",
            Status = "Pending",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });

        List<Transaction> transactions = await transactionService.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);
        int transactionId = transactions[0].Id;

        HttpResponseMessage response = await testContext.HttpClient.PutAsJsonAsync(
            $"api/transactions/{transactionId}/status",
            new { NewStatus = "Completed" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTransactionStatus_PendingToCompleted_StatusReflectsChange()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        TransactionProxyService transactionService = new(testContext.ApiClient);

        await transactionService.LogTransactionAsync(new Transaction
        {
            Amount = 8.50m,
            Type = "BuyMovie",
            Status = "Pending",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });

        List<Transaction> before = await transactionService.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);
        int transactionId = before[0].Id;
        await transactionService.UpdateTransactionStatusAsync(transactionId, "Completed");
        List<Transaction> after = await transactionService.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal("Completed", after.First(t => t.Id == transactionId).Status);
    }

    [Fact]
    public async Task LogTransaction_BuyerTransaction_OnlyAppearsInBuyerHistory()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        TransactionProxyService transactionService = new(testContext.ApiClient);

        const int otherUserId = 2;

        await transactionService.LogTransactionAsync(new Transaction
        {
            Amount = 7.77m,
            Type = "BuyMovie",
            Status = "Completed",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });

        List<Transaction> buyerHistory = await transactionService.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);
        List<Transaction> otherHistory = await transactionService.GetTransactionsByUserIdAsync(otherUserId);

        Assert.Contains(buyerHistory, t => t.Amount == 7.77m);
        Assert.DoesNotContain(otherHistory, t => t.Amount == 7.77m);
    }


    [Fact]
    public async Task WalletData_BalanceAndTransactionHistory_BothReturnValidData()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        UserProxyService userService = new(testContext.ApiClient);
        TransactionProxyService transactionService = new(testContext.ApiClient);

        await userService.UpdateBalanceAsync(ProxyRepoSeedIds.SeededUserId, 500m);
        await transactionService.LogTransactionAsync(new Transaction
        {
            Amount = 9.99m,
            Type = "BuyMovie",
            Status = "Completed",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });

        decimal balance = await userService.GetBalanceAsync(ProxyRepoSeedIds.SeededUserId);
        List<Transaction> history = await transactionService.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(500m, balance);
        Assert.NotEmpty(history);
    }
}
