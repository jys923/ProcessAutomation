using SonoCap.MES.Models.Enums;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories;
using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.Repositories.Base;

namespace SonoCap.MES.UI.Services
{
    public class TestingData
    {
        public Probe? Probe { get; }
        public TransducerModule? TransducerModule { get; }
        public Transducer? Transducer { get; }
        public PTRView? PTRView { get; }

        public TestingData(
            Probe? probe, 
            TransducerModule? transducerModule, 
            Transducer? transducer, 
            PTRView? pTRView)
        {
            Probe = probe;
            TransducerModule = transducerModule;
            Transducer = transducer;
            PTRView = pTRView;
        }
    }

    public class TestingManagementService
    {
        private readonly IPTRViewRepository _pTRViewRepository;
        private readonly IProbeRepository _probeRepository;
        private readonly ITransducerModuleRepository _transducerModuleRepository;
        private readonly ITransducerRepository _transducerRepository;
        private readonly ITestRepository _testRepository;
        private readonly IMotorModuleRepository _motorModuleRepository;
        private readonly ISharedSeqNoRepository _sharedSeqNoRepository;
        private readonly ITesterRepository _testerRepository;

        public TestingManagementService(
            IPTRViewRepository pTRViewRepository,
            IProbeRepository probeRepository,
            ITransducerModuleRepository transducerModuleRepository,
            ITransducerRepository transducerRepository,
            ITestRepository testRepository,
            IMotorModuleRepository motorModuleRepository,
            ISharedSeqNoRepository sharedSeqNoRepository,
            ITesterRepository testerRepository
            )
        {
            _pTRViewRepository = pTRViewRepository;
            _probeRepository = probeRepository;
            _transducerModuleRepository = transducerModuleRepository;
            _transducerRepository = transducerRepository;
            _testRepository = testRepository;
            _motorModuleRepository = motorModuleRepository;
            _sharedSeqNoRepository = sharedSeqNoRepository;
            _testerRepository = testerRepository;
        }

        public async Task<TestingData> GetTestingDataBySnAsync(SnType snType, string sn)
        {
            Probe? probe = null;
            TransducerModule? transducerModule = null;
            Transducer? transducer = null;
            PTRView? pTRView = null;

            var query = _pTRViewRepository.GetQueryable();

            switch (snType)
            {
                case SnType.Probe:
                    probe = await _probeRepository.GetBySn(sn)
                        .Include(p => p.TransducerModule)
                        .Include(p => p.MotorModule)
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefaultAsync();

                    if (probe != null)
                    {
                        transducerModule = await _transducerModuleRepository.GetByIdAsync(probe.TransducerModuleId);
                        if (transducerModule != null)
                        {
                            transducer = await _transducerRepository.GetByIdAsync(transducerModule.TransducerId);
                        }
                    }

                    query = query.Where(ptr => ptr.ProbeSn == sn).OrderByDescending(ptr => ptr.Id);
                    break;

                case SnType.TransducerModule:
                    transducerModule = await _transducerModuleRepository.GetBySn(sn)
                        .Include(tm => tm.Transducer)
                        .OrderByDescending(tm => tm.Id)
                        .FirstOrDefaultAsync();

                    if (transducerModule != null)
                    {
                        transducer = await _transducerRepository.GetByIdAsync(transducerModule.TransducerId);
                    }

                    query = query.Where(ptr => ptr.TransducerModuleSn == sn).OrderByDescending(ptr => ptr.Id);
                    break;

                case SnType.Transducer:
                    transducer = await _transducerRepository.GetBySn(sn).OrderByDescending(t => t.Id).FirstOrDefaultAsync();
                    query = query.Where(ptr => ptr.TransducerSn == sn).OrderByDescending(ptr => ptr.Id);
                    break;
            }

            pTRView = await query.FirstOrDefaultAsync();

            return new TestingData(probe, transducerModule, transducer, pTRView);
        }

        public async Task<List<Test>> GetTestByIdAsync(SnType snType, int id)
        {
            IQueryable<Test> query = _testRepository.GetQueryable();

            return await (snType switch
            {
                SnType.Probe => query.Where(test => test.ProbeId == id).OrderBy(test => test.Id).ToListAsync(),
                SnType.TransducerModule => query.Where(test => test.TransducerModuleId == id).OrderBy(test => test.Id).ToListAsync(),
                SnType.Transducer => query.Where(test => test.TransducerId == id).OrderBy(test => test.Id).ToListAsync(),
                _ => Task.FromResult(new List<Test>())
            });
        }

        public async Task<bool> IsExistsBySnAsync(SnType snType, string sn)
        {
            return snType switch
            {
                SnType.Probe => await _probeRepository.GetBySn(sn).AnyAsync(),
                SnType.TransducerModule => await _transducerModuleRepository.GetBySn(sn).AnyAsync(),
                SnType.Transducer => await _transducerRepository.GetBySn(sn).AnyAsync(),
                SnType.MotorModule => await _motorModuleRepository.GetBySn(sn).AnyAsync(),
                _ => false
            };
        }

        public async Task<TransducerModule?> GetLatestTransducerModuleAsync(int transducerId)
        {
            return await _transducerModuleRepository.GetQueryable()
                .Where(tm => tm.TransducerId == transducerId)
                .OrderByDescending(tm => tm.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<Probe?> GetLatestProbeWithMotorAsync(int transducerModuleId)
        {
            return await _probeRepository.GetQueryable()
                .Where(p => p.TransducerModuleId == transducerModuleId)
                .OrderByDescending(p => p.Id)
                .Include(p => p.MotorModule)  // ✅ MotorModule 포함
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PassTestCategoryAsync(
            TestCategories testCategory,
            Transducer? transducer = null,
            TransducerModule? transducerModule = null,
            Probe? probe = null)
        {
            if (transducer == null && transducerModule == null && probe == null)
            {
                throw new ArgumentException("At least one of transducer, transducerModule, or probe must be provided.");
            }

            IQueryable<Test> query;
            Test latestTest;

            switch (testCategory)
            {
                case TestCategories.Processing:
                    if (transducer == null) return false; // Ensure transducer is provided

                    for (int i = 1; i < 4; i++)
                    {
                        query = _testRepository.GetQueryable()
                            .Where(tests => tests.TransducerId == transducer.Id &&
                                            tests.TestCategoryId == 1 &&
                                            tests.TestTypeId == i)
                            .OrderByDescending(tests => tests.Id);

                        latestTest = await query.FirstOrDefaultAsync() ?? new Test();

                        if (latestTest.Id == 0 || latestTest.Result < App.TestThresholdDict[10 + i])
                        {
                            return false;
                        }
                    }
                    return true;

                case TestCategories.Process:
                    if (transducerModule == null) return false; // Ensure transducerModule is provided

                    for (int i = 1; i < 4; i++)
                    {
                        query = _testRepository.GetQueryable()
                            .Where(tests => tests.TransducerModuleId == transducerModule.Id &&
                                            tests.TestCategoryId == 2 &&
                                            tests.TestTypeId == i)
                            .OrderByDescending(tests => tests.Id);

                        latestTest = await query.FirstOrDefaultAsync() ?? new Test();

                        if (latestTest.Id == 0 || latestTest.Result < App.TestThresholdDict[20 + i])
                        {
                            return false;
                        }
                    }
                    return true;

                case TestCategories.Dispatch:
                    if (probe == null) return false; // Ensure probe is provided

                    for (int i = 1; i < 4; i++)
                    {
                        query = _testRepository.GetQueryable()
                            .Where(tests => tests.ProbeId == probe.Id &&
                                            tests.TestCategoryId == 3 &&
                                            tests.TestTypeId == i)
                            .OrderByDescending(tests => tests.Id);

                        latestTest = await query.FirstOrDefaultAsync() ?? new Test();

                        if (latestTest.Id == 0 || latestTest.Result < App.TestThresholdDict[30 + i])
                        {
                            return false;
                        }
                    }
                    return true;

                default:
                    return false;
            }
        }

        public async Task<bool> SaveAsync(Test insertTest)
        {
            return await _testRepository.InsertAsync(insertTest);
        }

        //public async Task PTRViewUpsertAsync(Probe probe, PTRView existingPTRView)
        //{
        //    if (probe is null)
        //        throw new ArgumentNullException(nameof(probe));

        //    if (existingPTRView is null)
        //        throw new ArgumentNullException(nameof(existingPTRView));

        //    var tmpPTR = await _probeRepository.GetPTRViewAsync(probe.Sn);
        //    if (tmpPTR is null)
        //        throw new ArgumentNullException(nameof(tmpPTR));

        //    if (existingPTRView is not null)
        //        tmpPTR.Id = existingPTRView.Id;

        //    await _pTRViewRepository.UpsertAsync(tmpPTR);
        //}

        public async Task PTRViewUpsertAsync(Probe probe, PTRView? existingPTRView)
        {
            if (probe is null) { return; }
            //throw new ArgumentNullException(nameof(probe));

            PTRView? tmpPTR = await _probeRepository.GetPTRViewAsync(probe.Sn);
            //PTRView? tmpPTR = await _pTRViewRepository.GetPTRView(probeSn: probe.Sn).FirstOrDefaultAsync();
            if (tmpPTR is not null)
            {
                if (existingPTRView is not null)
                    tmpPTR.Id = existingPTRView.Id;

                await _pTRViewRepository.UpsertAsync(tmpPTR);
            }
        }

        public HashSet<int> GetExistingTestTypeIds(TestCategories category, Transducer? transducer, TransducerModule? transducerModule, Probe? probe)
        {
            // 하나만 존재하므로 category 기준으로 정확히 분기
            IEnumerable<Test> existing = category switch
            {
                TestCategories.Processing when transducer is not null =>
                    _testRepository.GetLatestTests(transducer: transducer),

                TestCategories.Process when transducerModule is not null =>
                    _testRepository.GetLatestTests(transducerModule: transducerModule),

                TestCategories.Dispatch when probe is not null =>
                    _testRepository.GetLatestTests(probe: probe),

                _ => Enumerable.Empty<Test>()
            };

            return existing
                .Where(t => t.TestCategoryId == (int)category)
                .Select(t => t.TestTypeId)
                .Distinct()
                .ToHashSet();
        }


        public IEnumerable<Test> GetLatestTests(Transducer? transducer = null, TransducerModule? transducerModule = null, Probe? probe = null)
        {
            if (transducer is not null)
            {
                return _testRepository.GetLatestTests(transducer: transducer);
            }
            if (transducerModule is not null)
            {
                return _testRepository.GetLatestTests(transducerModule: transducerModule);
            }
            if (probe is not null)
            {
                return _testRepository.GetLatestTests(probe: probe);
            }

            return Enumerable.Empty<Test>();
        }

        public Task<SharedSeqNo?> GetSeqNoAsync(DateTime dateTime = default)
        {
            return _sharedSeqNoRepository.GetSeqNoAsync(dateTime);
        }

        public Task<bool> SetSeqNoAsync(SnType type, DateTime dateTime = default)
        {
            return _sharedSeqNoRepository.SetSeqNoAsync(type, dateTime);
        }

        public async Task<bool> InsertTdMdAsync(TransducerModule transducerModule)
        {
            return await _transducerModuleRepository.InsertAsync(transducerModule);
        }

        public async Task<bool> InsertProbeAsync(Probe probe)
        {
            return await _probeRepository.InsertAsync(probe);
        }

        public async Task<bool> InsertTesterAsync(Tester tester)
        {
            return await _testerRepository.InsertAsync(tester);
        }

        private List<string> GetFilteredSn<T>(IRepositoryBase<T> repository, Func<T, string> snSelector, string sn) where T : class
        {
            return repository.GetFilterItems(sn).Select(snSelector).ToList();
        }

        public List<string> GetFilteredSn(SnType snType, string sn)
        {
            return snType switch
            {
                SnType.Probe => GetFilteredSn(_probeRepository, m => m.Sn, sn),
                SnType.TransducerModule => GetFilteredSn(_transducerModuleRepository, m => m.Sn, sn),
                SnType.Transducer => GetFilteredSn(_transducerRepository, m => m.Sn, sn),
                _ => throw new ArgumentException($"Unsupported SnType: {snType}")
            };
        }

        public async Task<Tester?> InsertAndRetrieveTesterAsync2(Tester tester)
        {
            if (await _testerRepository.InsertAsync(tester))
            {
                return _testerRepository.GetQueryable()
                    .Where(t => t.PcId == tester.PcId && t.Name == tester.Name)
                    .OrderByDescending(t => t.Id)
                    .FirstOrDefault();
            }
            return null;
        }

        public async Task<Tester?> InsertAndRetrieveTesterAsync(Tester tester)
        {
            tester.Id = await _testerRepository.InsertAsyncInt(tester);

            return tester.Id > 0 ? tester : null;
        }
    }
}
