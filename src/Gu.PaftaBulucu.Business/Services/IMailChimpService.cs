using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Business.Services
{
    public interface IMailChimpService
    {
        Task<bool> AddMemberAsync(string email);
    }
}