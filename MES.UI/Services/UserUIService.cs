using MES.UI.Repositories;
using MES.UI.Repositories.interfaces;

namespace MES.UI.Services
{
    public class UserUIService
    {
        private readonly IProbeRepository _probeRepository;

        public UserUIService(IProbeRepository probeRepository)
        {
            _probeRepository = probeRepository;

        }

        public void getData(int resultCnt)
        {
            //_probeRepository.GetProbeSN2(resultCnt);
        }
    }
}
