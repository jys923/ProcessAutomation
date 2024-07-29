using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class InputBoxMotorViewModel : ViewModelBase
    {
        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        private ObservableDictionary<string, ValidationItem> _validationDict = new();

        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _prompt = string.Empty;

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(TestCommand))]
        //[NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string _mTMdSn = default!;

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
            MTMdSnIsPopupOpen = false;
        }

        [ObservableProperty]
        private MotorModule? _response;
        
        private readonly IMotorModuleRepository _motorModuleRepository;

        [RelayCommand]
        private void Ok(Window window)
        {
            window.DialogResult = true;
        }

        public InputBoxMotorViewModel(string title, string prompt, IMotorModuleRepository motorModuleRepository)
        {
            Title = title;
            Prompt = prompt;
            _motorModuleRepository = motorModuleRepository;
            ValidationDict[nameof(MTMdSn)] = new ValidationItem { IsEnabled=true, IsValid=false, Message="not valided", WaterMarkText = $"{nameof(MTMdSn)}을 입력 하세요." };
        }

        partial void OnMTMdSnChanged(string value)
        {
            Response = null;
            MTMdSnFilterItems();
            MTMdSnIsPopupOpen = !string.IsNullOrEmpty(value) && MTMdSnFilteredItems.Any();
            
            //if (value.Length > 5)
            //{
            //    Log.Information($"MTMdSn sn {value}");
            //    if (!IsExistsBySn(SnType.MotorModule, value))
            //    {
            //        ValidateField(nameof(MTMdSn), "MTMdSn Is Not Exist");
            //    }
            //    else
            //    {
            //        SetBySn(SnType.MotorModule, value);
            //        ValidateField(nameof(MTMdSn));
            //    }
            //}
            //else
            //{
            //    ValidateField(nameof(MTMdSn), "MTMdSn Is Not Valid");
            //}
        }
    }
}
