using System.Data.Common;

namespace MiniFlexCrmApi.Db;

public interface IConnectionProvider
{
    DbConnection GetConnection();
}