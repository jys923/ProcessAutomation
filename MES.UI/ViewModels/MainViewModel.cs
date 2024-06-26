﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.UI.Commons;
using MES.UI.Models;
using MES.UI.Models.Base;
using MES.UI.Repositories;
using MES.UI.Repositories.interfaces;
using MES.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace MES.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IMotorModuleRepository _motorModuleRepository;
        private readonly IPcRepository _pcRepository;
        private readonly IProbeRepository _probeRepository;
        private readonly ITestCategoryRepository _testCategoryRepository;
        private readonly ITesterRepository _testerRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestTypeRepository _testTypeRepository;
        private readonly ITransducerModuleRepository _transducerModuleRepository;
        private readonly ITransducerTypeRepository _transducerTypeRepository;

        [ObservableProperty]
        private string _title = default!;

        public MainViewModel(
            IMotorModuleRepository motorModuleRepository,
            IPcRepository pcRepository,
            IProbeRepository probeRepository,
            ITestCategoryRepository testCategoryRepository,
            ITesterRepository testerRepository,
            ITestRepository testRepository,
            ITestTypeRepository testTypeRepository,
            ITransducerModuleRepository transducerModuleRepository,
            ITransducerTypeRepository transducerTypeRepository)
        {
            _motorModuleRepository = motorModuleRepository;
            _pcRepository = pcRepository;
            _probeRepository = probeRepository; 
            _testCategoryRepository = testCategoryRepository;
            _testerRepository = testerRepository;
            _testRepository = testRepository;
            _testTypeRepository = testTypeRepository;
            _transducerModuleRepository = transducerModuleRepository;
            _transducerTypeRepository = transducerTypeRepository;
            Title = this.GetType().Name;
        }

        [RelayCommand]
        private void ToTest()
        {
            Debug.WriteLine($"{MethodBase.GetCurrentMethod()}");
            TestView? testView = App.Current.Services.GetService<TestView>()!;
            //testView.Show();
            testView.ShowDialog();
        }

        [RelayCommand]
        private void ToList()
        {
            Debug.WriteLine($"{MethodBase.GetCurrentMethod()}");
            ProbeListView? probeListView = App.Current.Services.GetService<ProbeListView>()!;
            probeListView.Show();
        }

        [RelayCommand]
        private void ToTestList()
        {
            Debug.WriteLine($"{MethodBase.GetCurrentMethod()}");
            TestListView? testListView = App.Current.Services.GetService<TestListView>()!;
            testListView.Show();
        }

        [RelayCommand]
        private async Task Master()
        {
            Debug.WriteLine($"{MethodBase.GetCurrentMethod()}");
            //var aaa =_probeRepository.GetProbeSN();
            //var aaa = _testRepository.GetTestProbe();
            //Debug.WriteLine($"{MethodBase.GetCurrentMethod()}" + aaa.ToList().Count);

            int maxCnt = 100000;
            int resultCnt = 5;

            string currentDate = DateTime.Now.ToString("yyMMdd");

            Random random = new Random();


            List<MotorModule> moterModules = new List<MotorModule>();
            List<TransducerModule> transducerModules = new List<TransducerModule>();
            List<Test> tests = new List<Test>();
            List<Probe> probes = new List<Probe>();

            #region MotorModules
            for (int i = 1; i <= maxCnt; ++i)
            {
                string MotorModuleSn = "mtm-sn " + currentDate + " " + i.ToString("D6");
                moterModules.Add(new MotorModule { MotorModuleSn = MotorModuleSn });
                //await _motorModuleRepository.InsertAsync(new MotorModule { MotorModuleSn = MotorModuleSn });
            }
            await _motorModuleRepository.BulkInsertAsync(moterModules);
            #endregion

            await _pcRepository.InsertAsync(new Pc { Name = "left" });
            await _pcRepository.InsertAsync(new Pc { Name = "middle" });
            await _pcRepository.InsertAsync(new Pc { Name = "right" });

            //_probeRepository--

            await _testCategoryRepository.InsertAsync(new TestCategory { Name = Enums.TestCategoryKor.공정용.ToString() });
            await _testCategoryRepository.InsertAsync(new TestCategory { Name = Enums.TestCategoryKor.최종용.ToString() });

            await _testerRepository.InsertAsync(new Tester { Name = "yoon" });
            await _testerRepository.InsertAsync(new Tester { Name = "sang" });
            await _testerRepository.InsertAsync(new Tester { Name = "kwon" });

            await _testTypeRepository.InsertAsync(new TestType { Name = "Align" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Axial" });
            await _testTypeRepository.InsertAsync(new TestType { Name = "Lateral" });

            //_testRepository --

            //_transducerModuleRepository --

            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = Enums.TransducerType.SCP01.ToString(), Type = "5Mhz" });
            await _transducerTypeRepository.InsertAsync(new TransducerType { Code = Enums.TransducerType.SCP02.ToString(), Type = "7.5Mhz" });

            #region TransducerModules
            for (int i = 1; i <= maxCnt; ++i)
            {
                int transducerTypeId = random.Next(1, 3);
                string TransducerSn = "td-sn " + currentDate + " " + i.ToString("D6");
                string TransducerModuleSn = "tdm-sn " + currentDate + " " + i.ToString("D6");
                transducerModules.Add(new TransducerModule { TransducerModuleSn = TransducerModuleSn, TransducerSn = TransducerSn, TransducerTypeId = transducerTypeId });
                //await _transducerModuleRepository.InsertAsync(new TransducerModule { TransducerModuleSn = TransducerModuleSn, TransducerSn = TransducerSn,TransducerTypeId = transducerTypeId });
            }
            await _transducerModuleRepository.BulkInsertAsync(transducerModules);
            #endregion

            #region Tests
            for (int i = 1; i <= maxCnt; ++i)
            {
                for (int j = 1; j <= Enum.GetNames(typeof(Enums.TestCategory)).Length; ++j) //2
                {
                    for (int k = 1; k <= Enum.GetNames(typeof(Enums.TestType)).Length; ++k) //3
                    {
                        int randomValue = 0;
                        int result = 0;
                        for (; ; )
                        {
                            randomValue = random.Next(65, 100);
                            result = randomValue < 70 ? 0 : randomValue;
                            Test test = new Test
                            {
                                CategoryId = j,
                                TestTypeId = k,
                                TesterId = random.Next(1, 4),
                                PcId = random.Next(1, 4),
                                OriginalImg = $"/img/{currentDate}/{Commons.Commons.MKRandom(10)}",
                                ChangedImg = $"/img/{currentDate}/{Commons.Commons.MKRandom(10)}",
                                ChangedImgMetadata = Commons.Commons.MKSHA256(),
                                Result = result,
                                Method = random.Next(1, 3),
                            };
                            test.TransducerModuleId = i;
                            tests.Add(test);
                            //await _testRepository.InsertAsync(test);

                            if (result >= 70)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            await _testRepository.BulkInsertAsync(tests);
            #endregion

            #region Probes
            for (int i = 1; i <= maxCnt; ++i)
            {
                string ProbeSn = "SCGP01" + currentDate + " " + i.ToString("D6");
                probes.Add(new Probe { ProbeSn = ProbeSn, TransducerModuleId = i, MotorModuleId = i });
                //await _probeRepository.InsertAsync(new Probe { ProbeSn = ProbeSn, TransducerModuleId = i, MotorModuleId = i});
            }
            await _probeRepository.BulkInsertAsync(probes);
            #endregion
        }

        [RelayCommand]
        [PerformanceInterceptor]
        private async Task Select1Async()
        {
            Debug.WriteLine($"{MethodBase.GetCurrentMethod()}");
            //IEnumerable<Models.Test> enumerable = await _testRepository.GetAllAsync();
            //List<ProbeTestResult> enumerable = _probeRepository.GetProbeSNSql();
            List<ProbeTestResult> enumerable = _probeRepository.GetProbeSN();
            Debug.WriteLine($"{MethodBase.GetCurrentMethod()}" + enumerable.ToList().Count);
        }
    }
}