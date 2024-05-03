using MES.UI.Repositories;

namespace MES.UI.Services
{
    public class UserUIService
    {
        private readonly IProbeSNRepository _probeSN;

        public UserUIService(IProbeSNRepository ProbeSN)
        {
            _probeSN = ProbeSN;

        }

        public void getData(int resultCnt)
        {
            _probeSN.GetProbeSN2(resultCnt);
        }
    }
}
