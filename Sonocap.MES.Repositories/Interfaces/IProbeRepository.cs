﻿using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;

namespace SonoCap.MES.Repositories.Interfaces
{
    public interface IProbeRepository : IRepositoryBase<Probe>
    {
        Task<List<ProbeTestResult>> GetProbeTestResultAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);

        List<ProbeTestResult> GetProbeTestResultSql();
        Task<List<ProbeTestResult>> GetProbeTestResultSqlAsync();

        List<ProbeTestResult> GetProbeTestResultSql(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);

        Task<List<ProbeTestResultDao>> GetProbeTestResultSqlAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);
    }
}