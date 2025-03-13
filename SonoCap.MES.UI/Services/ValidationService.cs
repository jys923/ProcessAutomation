using SonoCap.MES.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SonoCap.MES.UI.Validation
{
    public class ValidationService : INotifyPropertyChanged
    {
        private readonly Dictionary<string, ValidationItem> _validationDict = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public IReadOnlyDictionary<string, ValidationItem> ValidationDict => _validationDict;

        public ValidationService()
        {
            _validationDict["TDSn"] = new ValidationItem { WaterMarkText = $"TDSn을 입력 하세요.", IsEnabled = true };
            _validationDict["TDMdSn"] = new ValidationItem { WaterMarkText = $"TDMdSn을 입력 하세요." };
            _validationDict["ProbeSn"] = new ValidationItem { WaterMarkText = $"ProbeSn을 입력 하세요." };
            _validationDict["TestResult"] = new ValidationItem { };
        }

        public event EventHandler? ValidationChanged;

        private void OnValidationChanged()
        {
            ValidationChanged?.Invoke(this, EventArgs.Empty);
        }


        public bool GetValidating(string key)
        {
            return _validationDict.TryGetValue(key, out var validationItem) && validationItem.IsValid;
        }

        public void ValidateField(string key, string value = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ClearValidating(key);
            }
            else
            {
                SetValidating(key, value);
            }
        }

        public void ClearValidating(string key)
        {
            if (_validationDict.ContainsKey(key))
            {
                _validationDict[key].IsValid = true;
                _validationDict[key].Message = string.Empty;
                OnPropertyChanged(nameof(ValidationDict));
            }
        }

        public void SetValidating(string key, string message)
        {
            if (_validationDict.ContainsKey(key))
            {
                _validationDict[key].IsValid = false;
                _validationDict[key].Message = message;
                OnPropertyChanged(nameof(ValidationDict));
            }
        }

        public void ClearValidatingWaterMark()
        {
            foreach (var item in _validationDict.Values)
            {
                item.WaterMarkText = $"{item.WaterMarkText}";
            }
            OnPropertyChanged(nameof(ValidationDict));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
