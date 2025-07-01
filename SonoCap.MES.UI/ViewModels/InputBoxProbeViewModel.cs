using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Validation;
using SonoCap.MES.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class InputBoxProbeViewModel : ViewModelBase, IParameterReceiver
    {
        [ObservableProperty]
        private ObservableDictionary<string, ValidationItem> _validationDict = new();

        [ObservableProperty]
        private bool _okEnabled = false;

        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _prompt = string.Empty;

        [ObservableProperty]
        private string _probeSn = string.Empty;

        [ObservableProperty]
        private string? _response;

        [ObservableProperty]
        private bool _probeSnIsPopupOpen;

        [ObservableProperty]
        private int _probeSnSelectedIndex;

        [ObservableProperty]
        private ObservableCollection<string> _probeSnFilteredItems = new();

        private readonly IProbeRepository _probeRepository;

        public InputBoxProbeViewModel(IProbeRepository probeRepository)
        {
            _probeRepository = probeRepository;


            ValidationDict[nameof(ProbeSn)] = new ValidationItem
            {
                IsEnabled = true,
                IsValid = false,
                Message = "존재하지 않는 SN입니다.",
                WaterMarkText = $"{nameof(ProbeSn)}을 입력하세요."
            };

            ProbeSn = "UPAG";
        }

        partial void OnProbeSnChanged(string value)
        {
            OkEnabled = false;
            Response = null;

            if (!Regex.IsMatch(value, @"^UPAG[12]\d{6}\d{3}$"))
            {
                ValidationDict[nameof(ProbeSn)].Message = "ex)UPAG1250701001";
                return;
            }

            _ = CheckDuplicateSnAsync(value);
        }

        private async Task CheckDuplicateSnAsync(string sn)
        {
            var exists = await _probeRepository.GetSingleBySnAsync(sn) != null;
            if (exists)
            {
                ValidationDict[nameof(ProbeSn)].Message = "이미 등록된 SN입니다.";
                OkEnabled = false;
                return;
            }

            ValidationDict[nameof(ProbeSn)].Message = string.Empty;
            OkEnabled = true;
            Response = sn;
        }



        private void ProbeSnFilterItems()
        {
            if (string.IsNullOrWhiteSpace(ProbeSn))
            {
                ProbeSnFilteredItems.Clear();
            }
            else
            {
                var items = _probeRepository.GetFilterItems(ProbeSn).Select(x => x.Sn).ToList();
                ProbeSnFilteredItems = new ObservableCollection<string>(items);
            }
        }

        [RelayCommand]
        private void ProbeSnFocus()
        {
            var recentItems = _probeRepository.GetQueryable()
                .OrderByDescending(p => p.Id)
                .Take(5)
                .Select(p => p.Sn)
                .ToList();

            ProbeSnFilteredItems = new ObservableCollection<string>(recentItems);
            ProbeSnIsPopupOpen = ProbeSnFilteredItems.Any();
        }

        [RelayCommand]
        private void ProbeSnKeyDown(KeyEventArgs e)
        {
            if (e == null) return;

            Log.Information($"ProbeSnKeyDown : {e.Key}");

            if (e.Key == Key.Down && ProbeSnFilteredItems.Count > 0)
            {
                ProbeSnSelectedIndex = (ProbeSnSelectedIndex + 1) % ProbeSnFilteredItems.Count;
            }
            else if (e.Key == Key.Up && ProbeSnFilteredItems.Count > 0)
            {
                ProbeSnSelectedIndex = (ProbeSnSelectedIndex - 1 + ProbeSnFilteredItems.Count) % ProbeSnFilteredItems.Count;
            }
            else if (e.Key == Key.Enter)
            {
                if (ProbeSnSelectedIndex >= 0 && ProbeSnSelectedIndex < ProbeSnFilteredItems.Count)
                {
                    ProbeSn = ProbeSnFilteredItems[ProbeSnSelectedIndex];
                    Response = ProbeSn;
                    ProbeSnIsPopupOpen = false;
                    OkEnabled = true;
                    ValidationService.ValidateField(ValidationDict, nameof(ProbeSn));
                }
            }
            else if (e.Key == Key.Tab)
            {
                ProbeSnIsPopupOpen = false;
            }
        }

        [RelayCommand]
        private void ProbeSnFilteredItemsMouseDoubleClick(string selectedItem)
        {
            ProbeSn = selectedItem;
            Response = null;

            if (selectedItem.Length == 11)
            {
                // 자동으로 연번 제안 흐름 유도
                OnProbeSnChanged(selectedItem);
                return;
            }

            _ = CheckDuplicateSnAsync(selectedItem);
            ProbeSnIsPopupOpen = false;
        }

        [RelayCommand]
        private void Ok(Window window)
        {
            window.DialogResult = true;
        }

        public LabelInput LabelItem { get; set; } = default!;

        public void ReceiveParameter(object parameter)
        {
            if (parameter is LabelInput label)
            {
                Title = label.Title;
                Prompt = label.Prompt;
                Log.Information($"Received parameter {Title}");
            }
        }
    }
}
