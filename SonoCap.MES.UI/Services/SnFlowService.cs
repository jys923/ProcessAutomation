using SonoCap.MES.Models.Enums;
using SonoCap.MES.UI.Models;

namespace SonoCap.MES.UI.Services
{
    public interface ISnFlowService
    {
        Task<SnFlowResult> ProcessAsync(string sn);
    }

    public class SnFlowService : ISnFlowService
    {
        private readonly TestingManagementService _testing;

        public SnFlowService(TestingManagementService testing)
        {
            _testing = testing;
        }

        public async Task<SnFlowResult> ProcessAsync(string sn)
        {
            var result = new SnFlowResult();

            if (string.IsNullOrWhiteSpace(sn) || sn.Length <= 1)
            {
                result.IsValid = false;
                return result;
            }

            if (!await _testing.IsExistsBySnAsync(SnType.Transducer, sn))
            {
                result.IsValid = false;
                return result;
            }

            // 1) Transducer
            var transducer = await _testing.GetLatestTransducerAsync(sn);
            if (transducer is null)
            {
                result.IsValid = false;
                return result;
            }

            result.Transducer = transducer;
            result.TdEnabled = true;

            result.TransducerTests =
                await _testing.GetTestByIdAsync(SnType.Transducer, transducer.Id);


            // 2) TransducerModule
            var tmd = await _testing.GetLatestTransducerModuleAsync(transducer.Id);
            if (tmd is null)
            {
                result.IsValid = true;

                return result;
            }

            result.TransducerModule = tmd;
            result.TdMdEnabled = true;
            result.TDMdWatermark = tmd.Sn;

            result.TransducerModuleTests =
                await _testing.GetTestByIdAsync(SnType.TransducerModule, tmd.Id);


            // 3) Probe
            var probe = await _testing.GetLatestProbeWithMotorAsync(tmd.Id);
            if (probe is null)
            {
                // ⛔ 여기서 멈추고 리턴해야 함
                result.IsValid = true;
                return result;
            }

            result.Probe = probe;
            result.ProbeEnabled = true;
            result.ProbeSnWatermark = probe.Sn;
            result.MotorMdWatermark = probe.MotorModule.Sn;
            result.MotorModule = probe.MotorModule;

            result.ProbeTests =
                await _testing.GetTestByIdAsync(SnType.Probe, probe.Id);

            result.IsValid = true;
            return result;
        }
    }
}