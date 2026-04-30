using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Go.Core.Services
{
    public interface IUserService
    {
        Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    }
}
