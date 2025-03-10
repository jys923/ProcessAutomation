using SonoCap.MES.Models.Enums;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.UI.Services
{
    public class TestService
    {
        private readonly IProbeRepository _probeRepository;
        private readonly ITransducerRepository _transducerRepository;
        private readonly ITransducerModuleRepository _transducerModuleRepository;
        private readonly IPTRViewRepository _pTRViewRepository;

        public TestService(
            IProbeRepository probeRepository,
            ITransducerRepository transducerRepository,
            ITransducerModuleRepository transducerModuleRepository,
            IPTRViewRepository pTRViewRepository)
        {
            _probeRepository = probeRepository;
            _transducerRepository = transducerRepository;
            _transducerModuleRepository = transducerModuleRepository;
            _pTRViewRepository = pTRViewRepository;
        }

        public async Task<(Transducer?, TransducerModule?, Probe?, PTRView?)> GetTestComponentsBySnAsync(SnType snType, string sn)
        {
            Transducer? transducer = null;
            TransducerModule? transducerModule = null;
            Probe? probe = null;
            PTRView? pTRView = null;

            switch (snType)
            {
                case SnType.Probe:
                    probe = await _probeRepository.GetBySn(sn)
                        .Include(p => p.TransducerModule)
                        .Include(p => p.MotorModule)
                        .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    if (probe != null)
                    {
                        transducerModule = await _transducerModuleRepository.GetByIdAsync(probe.TransducerModuleId);
                        transducer = await _transducerRepository.GetByIdAsync(transducerModule.TransducerId);
                        pTRView = await _pTRViewRepository.GetQueryable()
                            .Where(p => p.ProbeSn == sn)
                            .OrderByDescending(p => p.Id)
                            .FirstOrDefaultAsync();
                    }
                    break;

                case SnType.TransducerModule:
                    transducerModule = await _transducerModuleRepository.GetBySn(sn)
                        .Include(tm => tm.Transducer)
                        .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    if (transducerModule != null)
                    {
                        transducer = await _transducerRepository.GetByIdAsync(transducerModule.TransducerId);
                        pTRView = await _pTRViewRepository.GetQueryable()
                            .Where(p => p.TransducerModuleSn == sn)
                            .OrderByDescending(p => p.Id)
                            .FirstOrDefaultAsync();
                    }
                    break;

                case SnType.Transducer:
                    transducer = await _transducerRepository.GetBySn(sn)
                        .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    pTRView = await _pTRViewRepository.GetQueryable()
                        .Where(p => p.TransducerSn == sn)
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefaultAsync();
                    break;
            }

            return (transducer, transducerModule, probe, pTRView);
        }

        public (Transducer?, TransducerModule?, Probe?, PTRView?) GetTestComponentsBySn(SnType snType, string sn)
        {
            Transducer? transducer = null;
            TransducerModule? transducerModule = null;
            Probe? probe = null;
            PTRView? pTRView = null;

            switch (snType)
            {
                case SnType.Probe:
                    probe = _probeRepository.GetBySn(sn)
                        .Include(p => p.TransducerModule)
                        .Include(p => p.MotorModule)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();
                    if (probe != null)
                    {
                        transducerModule = _transducerModuleRepository.GetById(probe.TransducerModuleId);
                        transducer = _transducerRepository.GetById(transducerModule.TransducerId);
                        pTRView = _pTRViewRepository.GetQueryable()
                            .Where(p => p.ProbeSn == sn)
                            .OrderByDescending(p => p.Id)
                            .FirstOrDefault();
                    }
                    break;

                case SnType.TransducerModule:
                    transducerModule = _transducerModuleRepository.GetBySn(sn)
                        .Include(tm => tm.Transducer)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();
                    if (transducerModule != null)
                    {
                        transducer = _transducerRepository.GetById(transducerModule.TransducerId);
                        pTRView = _pTRViewRepository.GetQueryable()
                            .Where(p => p.TransducerModuleSn == sn)
                            .OrderByDescending(p => p.Id)
                            .FirstOrDefault();
                    }
                    break;

                case SnType.Transducer:
                    transducer = _transducerRepository.GetBySn(sn)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();
                    pTRView = _pTRViewRepository.GetQueryable()
                        .Where(p => p.TransducerSn == sn)
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefault();
                    break;
            }

            return (transducer, transducerModule, probe, pTRView);
        }

    }

}
