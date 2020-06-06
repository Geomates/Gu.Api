using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Business.Services
{
    public interface IParameterService
    {
        Task<string> GetParameterValueAsync(string parameterName);
    }
}