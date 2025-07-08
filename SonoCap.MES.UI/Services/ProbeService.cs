using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.WpfCommons;
using System.IO;

namespace SonoCap.MES.UI.Services
{
    public class ProbeService
    {
        private readonly IPTRViewRepository _ptrRepo;
        private readonly IProbeRepository _probeRepo;
        private readonly ITestRepository _testRepo;
        private readonly ITransducerModuleRepository _tdMdRepo;

        public ProbeService(
            IPTRViewRepository ptrRepo,
            IProbeRepository probeRepo,
            ITestRepository testRepo,
            ITransducerModuleRepository tdMdRepo
            )
        {
            _ptrRepo = ptrRepo;
            _probeRepo = probeRepo;
            _testRepo = testRepo;
            _tdMdRepo = tdMdRepo;
        }

        public async Task<bool> DeleteProbe(PTRView ptrView)
        {
            Probe? probe = _probeRepo.GetBySn(ptrView.ProbeSn).FirstOrDefault();
            if (probe == null) return false;

            // 1. 모든 관련 테스트 ID 수집
            var testSet = new HashSet<Test>();

            // PTRView에 연결된 테스트 수집
            if (ptrView.Test01 != null) testSet.Add(ptrView.Test01);
            if (ptrView.Test02 != null) testSet.Add(ptrView.Test02);
            if (ptrView.Test03 != null) testSet.Add(ptrView.Test03);
            if (ptrView.Test04 != null) testSet.Add(ptrView.Test04);
            if (ptrView.Test05 != null) testSet.Add(ptrView.Test05);
            if (ptrView.Test06 != null) testSet.Add(ptrView.Test06);
            if (ptrView.Test07 != null) testSet.Add(ptrView.Test07);
            if (ptrView.Test08 != null) testSet.Add(ptrView.Test08);
            if (ptrView.Test09 != null) testSet.Add(ptrView.Test09);

            // 1. ProbeSn 기반
            var testsByProbe = await _testRepo.GetTestAsync(
                null,        // startDate: 시작일 없음
                null,        // endDate: 종료일 없음
                null,        // categoryId: 테스트 카테고리 필터 없음
                null,        // testTypeId: 테스트 타입 필터 없음
                null,        // tester: 검사자 이름 필터 없음
                null,        // pcId: PC ID 필터 없음
                null,        // result: 결과 필터 없음
                null,           // dataFlagTest: 유효 데이터만 (1)
                probe.Sn,    // probeSn: 이 프로브와 연결된 테스트 조회
                null,        // transducerModuleSn: 필터 없음
                null,        // transducerSn: 필터 없음
                null,        // motorModuleSn: 필터 없음
                null
            );
            if (testsByProbe != null)
                foreach (var t in testsByProbe)
                    testSet.Add(t);

            var moduleSn = probe.TransducerModule?.Sn;
            if (!string.IsNullOrEmpty(moduleSn))
            {
                var testsByModule = await _testRepo.GetTestAsync(
                    null,        // startDate: 시작일 없음
                    null,        // endDate: 종료일 없음
                    null,        // categoryId: 테스트 카테고리 필터 없음
                    null,        // testTypeId: 테스트 타입 필터 없음
                    null,        // tester: 검사자 이름 필터 없음
                    null,        // pcId: PC ID 필터 없음
                    null,        // result: 결과 필터 없음
                    null,        // dataFlagTest: 유효 데이터만 (1)
                    null,        // probeSn: 이 프로브와 연결된 테스트 조회
                    moduleSn,    // transducerModuleSn: 필터 없음
                    null,        // transducerSn: 필터 없음
                    null,        // motorModuleSn: 필터 없음
                    null
                );
                if (testsByModule != null)
                    foreach (var t in testsByModule)
                        testSet.Add(t);
            }

            // 3. Transducer.Sn 기반
            var transducerSn = probe.TransducerModule?.Transducer?.Sn;
            if (!string.IsNullOrEmpty(transducerSn))
            {
                var testsByTransducer = await _testRepo.GetTestAsync(
                    null,        // startDate: 시작일 없음
                    null,        // endDate: 종료일 없음
                    null,        // categoryId: 테스트 카테고리 필터 없음
                    null,        // testTypeId: 테스트 타입 필터 없음
                    null,        // tester: 검사자 이름 필터 없음
                    null,        // pcId: PC ID 필터 없음
                    null,        // result: 결과 필터 없음
                    null,        // dataFlagTest: 유효 데이터만 (1)
                    null,        // probeSn: 이 프로브와 연결된 테스트 조회
                    null,        // transducerModuleSn: 필터 없음
                    transducerSn,// transducerSn: 필터 없음
                    null,        // motorModuleSn: 필터 없음
                    null
                );
                if (testsByTransducer != null)
                    foreach (var t in testsByTransducer)
                        testSet.Add(t);
            }
            // 5. PTRView 삭제
            await _ptrRepo.DeleteByIdAsync(ptrView.Id);

            // 관련 테스트 이미지 파일 삭제
            foreach (var test in testSet)
            {
                try
                {
                    string? sn = test.TransducerModule?.Sn
                        ?? test.Probe?.Sn
                        ?? test.Transducer?.Sn;

                    string dir = Utilities.GetExportImgPath(
                        App.appSettings.Path.ExportImg,
                        App.appSettings.Path.ExportImgPhase,
                        test.TestCategoryId,
                        sn);

                    string origPath = Path.Combine(dir, test.OriginalImg);
                    string changedPath = Path.Combine(dir, test.ChangedImg);

                    if (File.Exists(origPath)) File.Delete(origPath);
                    if (File.Exists(changedPath)) File.Delete(changedPath);

                    // 이미지 파일 삭제 후 폴더가 비었으면 SN 폴더 삭제
                    if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                    {
                        Directory.Delete(dir);
                        Log.Information($"[DeleteProbe] 빈 검사 폴더 삭제됨: {dir}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"[DeleteProbe] 파일 삭제 오류 (TestId={test.Id}): {ex.Message}");
                }

                await _testRepo.DeleteByIdAsync(test.Id);
            }


            // 2. 관련 테스트 삭제
            foreach (var test in testSet)
                await _testRepo.DeleteByIdAsync(test.Id);

            // 4. Probe 삭제
            await _probeRepo.DeleteByIdAsync(probe.Id);
            
            // 3. TransducerModule 삭제 (단독 참조라고 가정)
            if (probe.TransducerModule != null)
                await _tdMdRepo.DeleteByIdAsync(probe.TransducerModule.Id);



            //파일삭제

            return true;
        }

        public async Task<bool> DeleteProbe2(PTRView ptrView)
        {
            Probe? probe = _probeRepo.GetBySn(ptrView.ProbeSn).FirstOrDefault();
            if (probe == null) return false;

            // 1. 모든 관련 테스트 ID 수집
            var testIds = new HashSet<int>();

            if (ptrView.Test01 != null) testIds.Add(ptrView.Test01.Id);
            if (ptrView.Test02 != null) testIds.Add(ptrView.Test02.Id);
            if (ptrView.Test03 != null) testIds.Add(ptrView.Test03.Id);
            if (ptrView.Test04 != null) testIds.Add(ptrView.Test04.Id);
            if (ptrView.Test05 != null) testIds.Add(ptrView.Test05.Id);
            if (ptrView.Test06 != null) testIds.Add(ptrView.Test06.Id);
            if (ptrView.Test07 != null) testIds.Add(ptrView.Test07.Id);
            if (ptrView.Test08 != null) testIds.Add(ptrView.Test08.Id);
            if (ptrView.Test09 != null) testIds.Add(ptrView.Test09.Id);

            // 1. ProbeSn 기반
            var testsByProbe = await _testRepo.GetTestAsync(
                null,        // startDate: 시작일 없음
                null,        // endDate: 종료일 없음
                null,        // categoryId: 테스트 카테고리 필터 없음
                null,        // testTypeId: 테스트 타입 필터 없음
                null,        // tester: 검사자 이름 필터 없음
                null,        // pcId: PC ID 필터 없음
                null,        // result: 결과 필터 없음
                null,           // dataFlagTest: 유효 데이터만 (1)
                probe.Sn,    // probeSn: 이 프로브와 연결된 테스트 조회
                null,        // transducerModuleSn: 필터 없음
                null,        // transducerSn: 필터 없음
                null,        // motorModuleSn: 필터 없음
                null
            );
            if (testsByProbe != null)
                foreach (var t in testsByProbe)
                    testIds.Add(t.Id);

            var moduleSn = probe.TransducerModule?.Sn;
            if (!string.IsNullOrEmpty(moduleSn))
            {
                var testsByModule = await _testRepo.GetTestAsync(
                    null,        // startDate: 시작일 없음
                    null,        // endDate: 종료일 없음
                    null,        // categoryId: 테스트 카테고리 필터 없음
                    null,        // testTypeId: 테스트 타입 필터 없음
                    null,        // tester: 검사자 이름 필터 없음
                    null,        // pcId: PC ID 필터 없음
                    null,        // result: 결과 필터 없음
                    null,        // dataFlagTest: 유효 데이터만 (1)
                    null,        // probeSn: 이 프로브와 연결된 테스트 조회
                    moduleSn,    // transducerModuleSn: 필터 없음
                    null,        // transducerSn: 필터 없음
                    null,        // motorModuleSn: 필터 없음
                    null
                );
                if (testsByModule != null)
                    foreach (var t in testsByModule)
                        testIds.Add(t.Id);
            }

            // 3. Transducer.Sn 기반
            var transducerSn = probe.TransducerModule?.Transducer?.Sn;
            if (!string.IsNullOrEmpty(transducerSn))
            {
                var testsByTransducer = await _testRepo.GetTestAsync(
                    null,        // startDate: 시작일 없음
                    null,        // endDate: 종료일 없음
                    null,        // categoryId: 테스트 카테고리 필터 없음
                    null,        // testTypeId: 테스트 타입 필터 없음
                    null,        // tester: 검사자 이름 필터 없음
                    null,        // pcId: PC ID 필터 없음
                    null,        // result: 결과 필터 없음
                    null,        // dataFlagTest: 유효 데이터만 (1)
                    null,        // probeSn: 이 프로브와 연결된 테스트 조회
                    null,        // transducerModuleSn: 필터 없음
                    transducerSn,// transducerSn: 필터 없음
                    null,        // motorModuleSn: 필터 없음
                    null
                );
                if (testsByTransducer != null)
                    foreach (var t in testsByTransducer)
                        testIds.Add(t.Id);
            }
            // 5. PTRView 삭제
            await _ptrRepo.DeleteByIdAsync(ptrView.Id);

            // 2. 관련 테스트 삭제
            foreach (var id in testIds)
                await _testRepo.DeleteByIdAsync(id);

            // 4. Probe 삭제
            await _probeRepo.DeleteByIdAsync(probe.Id);

            // 3. TransducerModule 삭제 (단독 참조라고 가정)
            if (probe.TransducerModule != null)
                await _tdMdRepo.DeleteByIdAsync(probe.TransducerModule.Id);



            //파일삭제

            return true;
        }
    }
}
