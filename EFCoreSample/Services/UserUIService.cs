using EFCoreSample.Repositories;

namespace EFCoreSample.Services
{
    public class UserUIService
    {
        private readonly IProbeSNRepository _probeSN;

        public UserUIService(IProbeSNRepository ProbeSN)
        {
            _probeSN = ProbeSN;

        }

        public void getData(int numberOfResults)
        {
            _probeSN.GetProbeSN(numberOfResults);
        }
    }
}
