using CRM.Application.Interfaces.Generators;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Generators;

public class CustomerCodeGenerator : ICustomerCodeGenerator
{
    private readonly AppDbContext _context;

    public CustomerCodeGenerator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync()
    {
        var nextValue = await GetNextSequenceValueAsync();

        return $"CUS-{nextValue:D6}";
    }

    private async Task<int> GetNextSequenceValueAsync()
    {
        var connection = _context.Database.GetDbConnection();

        await using var command = connection.CreateCommand();

        command.CommandText =
            "SELECT NEXT VALUE FOR CustomerSequence";

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
}