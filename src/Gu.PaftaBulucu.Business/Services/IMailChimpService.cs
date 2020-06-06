using System.Threading.Tasks;
using Gu.PaftaBulucu.Business.Dtos;

namespace Gu.PaftaBulucu.Business.Services
{
    public interface IMailChimpService
    {
        Task<bool> AddMemberAsync(AddSubscriberDto addSubscriberDto);
    }
}