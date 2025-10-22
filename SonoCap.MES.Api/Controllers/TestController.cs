using Microsoft.AspNetCore.Mvc;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Api.Dto;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestRepository _testRepository;

        public TestController(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        // ------------------------------------------------------------------
        // [1] GetTestAsync에 대응하는 엔드포인트 구현 (검색 기능)
        // ------------------------------------------------------------------
        // 경로: GET /api/test/search
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Test>), 200)]
        public async Task<IActionResult> GetTestListAsync([FromQuery] TestSearchRequest request)
        {
            // Repository 메서드 호출: GetTestAsync
            var tests = await _testRepository.GetTestAsync(
                request.StartDate,
                request.EndDate,
                request.CategoryId,
                request.TestTypeId,
                request.Tester,
                request.PcId,
                request.Result,
                request.DataFlagTest,
                request.ProbeSn,
                request.TransducerModuleSn,
                request.TransducerSn,
                request.MotorModuleSn,
                request.DataFlagProbe
            );

            return Ok(tests);
        }

        // ------------------------------------------------------------------
        // [2] GetLatestTests에 대응하는 엔드포인트 구현 (최신 테스트 조회)
        // ------------------------------------------------------------------
        // Reposiotry가 Entity 객체(Transducer, Probe)를 인수로 받으므로,
        // ID를 입력받아 임시 Entity 객체를 만들어 전달하는 방식을 사용합니다.
        // 경로: GET /api/test/latest?transducerId=1&probeId=2
        [HttpGet("latest")]
        [ProducesResponseType(typeof(IEnumerable<Test>), 200)]
        // 메서드 자체를 비동기로 만들고 Task<IActionResult>를 반환합니다.
        public async Task<IActionResult> GetLatestTests(int? transducerId, int? transducerModuleId, int? probeId)
        {
            if (transducerId == null && transducerModuleId == null && probeId == null)
            {
                return BadRequest("최소한 TransducerId, TransducerModuleId 또는 ProbeId 중 하나를 제공해야 합니다.");
            }

            try
            {
                // Repository의 비동기 메서드 GetLatestTestsAsync를 호출하고 await 합니다.
                IEnumerable<Test> latestTests = await _testRepository.GetLatestTestsAsync(
                    transducerId,
                    transducerModuleId,
                    probeId
                );

                return Ok(latestTests);
            }
            catch (ArgumentException ex)
            {
                // 오류 처리
                return BadRequest(ex.Message);
            }
        }
    }
}