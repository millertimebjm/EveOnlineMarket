using System.Collections;
using Dapper;
using Eve.Models.Users;
using Microsoft.Data.Sqlite;
using Eve.Repositories.Interfaces.Users;

namespace Eve.Repositories.Sqlite;

public class SqliteUserRepsitory : IUserRepository
{
    public async Task<User?> Get(long userId)
    {
        using var connection = new SqliteConnection("Data Source=EveOnlineMarket.db");
        var sql = @"
SELECT 
    UserId,
    AuthorizationCode,
    AccessToken,
    TokenGrantedDateTime,
    BearerToken,
    ClientId,
    ClientSecret,
    TokenExpirationDate
FROM User
WHERE UserId = $UserId
        ";
        return await connection.QuerySingleOrDefaultAsync<User>(sql);
    }

    // public long UserId { get; set;}
    // public string AuthorizationCode {get; set;} = string.Empty;
    // public string AccessToken {get; set;} = string.Empty;
    // public DateTime? TokenGrantedDateTime {get; set;}
    // public string BearerToken { get; set; } = string.Empty;
    // public string ClientId {get; set;} = string.Empty;
    // public string ClientSecret {get; set;} = string.Empty;
    // public DateTime TokenExpirationDate { get; set; }

    public async Task<IEnumerable<User>> GetAll()
    {
        using var connection = new SqliteConnection("Data Source=EveOnlineMarket.db");
        var sql = @"
SELECT 
    UserId,
    AuthorizationCode,
    AccessToken,
    TokenGrantedDateTime,
    BearerToken,
    ClientId,
    ClientSecret,
    TokenExpirationDate
FROM User
        ";
        return await connection.QueryAsync<User>(sql);
    }

    public async Task<User> Upsert(User user)
    {
        await Task.Delay(1);
        throw new NotImplementedException();
//         using var connection = new SqliteConnection("Data Source=EveOnlineMarket.db");
//         var sql = @"
// SELECT 
//     UserId,
//     AuthorizationCode,
//     AccessToken,
//     TokenGrantedDateTime,
//     BearerToken,
//     ClientId,
//     ClientSecret,
//     TokenExpirationDate
// FROM User
//         ";
//         return await connection.QueryAsync<User>(sql);
    }
}