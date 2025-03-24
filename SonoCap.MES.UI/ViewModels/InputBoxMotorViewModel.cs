using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Validation;
using SonoCap.MES.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class InputBoxMotorViewModel : ViewModelBase, IParameterReceiver
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
        private string _mTMdSn = default!;
        
        [ObservableProperty]
        private MotorModule? _response;

        [ObservableProperty]
        private bool _mTMdSnIsPopupOpen;

        [ObservableProperty]
        private int _mTMdSnSelectedIndex;

        [ObservableProperty]
        private ObservableCollection<string> _mTMdSnFilteredItems = new();

        private void MTMdSnFilterItems()
        {
            if (string.IsNullOrWhiteSpace(MTMdSn))
            {

                MTMdSnFilteredItems.Clear();
            }
            else
            {
                List<string> items = _motorModuleRepository.GetFilterItems(MTMdSn).Select(m => m.Sn).ToList();

                MTMdSnFilteredItems = new ObservableCollection<string>(items);
            }
        }

        [RelayCommand]
        private void MTMdSnKeyDown(KeyEventArgs e)
        {
            if (e == null) return;

            Log.Information($"MTMdSnOnKeyDown : {e.Key}");
            if (e.Key == Key.Down)
            {
                if (MTMdSnFilteredItems.Count > 0)
                {
                    MTMdSnSelectedIndex = (MTMdSnSelectedIndex + 1) % MTMdSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (MTMdSnFilteredItems.Count > 0)
                {
                    MTMdSnSelectedIndex = (MTMdSnSelectedIndex - 1 + MTMdSnFilteredItems.Count) % MTMdSnFilteredItems.Count;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (MTMdSnSelectedIndex >= 0 && MTMdSnSelectedIndex < MTMdSnFilteredItems.Count)
                {
                    MTMdSn = MTMdSnFilteredItems[MTMdSnSelectedIndex];
                    Response = _motorModuleRepository.GetBySn(MTMdSn).OrderByDescending(x => x.Id).First();
                    MTMdSnIsPopupOpen = false;
                    OkEnabled = true;
                    ValidationService.ValidateField(ValidationDict, nameof(MTMdSn));
                }
            }
            else if (e.Key == Key.Tab)
            {
                MTMdSnIsPopupOpen = false;
            }
        }

        [RelayCommand]
        private void MTMdSnFilteredItemsMouseDoubleClick(string selectedItem)
        {
            Log.Information($"MTMdSnFilteredItemsMouseDoubleClick : {selectedItem}");
            MTMdSn = selectedItem;
            Response = _motorModuleRepository.GetBySn(MTMdSn).OrderByDescending(x => x.Id).First();
            MTMdSnIsPopupOpen = false;
            OkEnabled = true;
            ValidationService.ValidateField(ValidationDict, nameof(MTMdSn));
        }

        [RelayCommand]
        private void Ok(Window window)
        {
            window.DialogResult = true;
        }

        private readonly IMotorModuleRepository _motorModuleRepository;
        public InputBoxMotorViewModel(IMotorModuleRepository motorModuleRepository)
        {
            _motorModuleRepository = motorModuleRepository;
            ValidationDict[nameof(MTMdSn)] = new ValidationItem { IsEnabled=true, IsValid=false, Message= "MTMdSn Is Not Exist", WaterMarkText = $"{nameof(MTMdSn)}을 입력 하세요." };
        }

        partial void OnMTMdSnChanged(string value)
        {
            OkEnabled = false;
            Response = null;
            MTMdSnFilterItems();
            MTMdSnIsPopupOpen = !string.IsNullOrEmpty(value) && MTMdSnFilteredItems.Any();
            ValidationService.ValidateField(ValidationDict, nameof(MTMdSn), "MTMdSn Is Not Exist");
        }

        public LabelInput LabelItem { get; set; } = default!;

        public void ReceiveParameter(object parameter)
        {
            if (parameter is LabelInput label)
            {
                //LabelItem = label;
                Title = label.Title;
                Prompt = label.Prompt;

                Log.Information($"Received parameter {Title}");
            }

        }
    }
}
