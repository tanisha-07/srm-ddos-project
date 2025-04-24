using System.Data;

namespace SRM.DDOS.API.Infrastructure.Abstraction
{
    public interface IDapperContext
    {
        public IDbConnection Connection { get; }
    }
}
