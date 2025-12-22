using Homy.Domin.models;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
