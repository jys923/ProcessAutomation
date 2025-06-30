using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Validation;
using SonoCap.MES.UI.ViewModels.Base;
using System.Collections.ObjectModel;
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
        }

        partial void OnProbeSnChanged(string value)
        {
            OkEnabled = false;
            Response = null;
            ProbeSnFilterItems();
            ProbeSnIsPopupOpen = !string.IsNullOrWhiteSpace(value) && ProbeSnFilteredItems.Any();
            ValidationService.ValidateField(ValidationDict, nameof(ProbeSn));
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
            Log.Information($"ProbeSnFilteredItemsMouseDoubleClick : {selectedItem}");
            ProbeSn = selectedItem;
            Response = selectedItem;
            ProbeSnIsPopupOpen = false;
            OkEnabled = true;
            ValidationService.ValidateField(ValidationDict, nameof(ProbeSn));
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
