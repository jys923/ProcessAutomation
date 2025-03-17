using SonoCap.MES.Models.Enums;
using SonoCap.MES.UI.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SonoCap.MES.UI.Validation
{
    public static class ValidationService
    {
        public static void ClearValidating(ObservableDictionary<string, ValidationItem> validationDict, string key)
        {
            if (validationDict.ContainsKey(key))
            {
                validationDict[key].IsValid = true;
                validationDict[key].Message = string.Empty;
            }
            else
            {
                validationDict[key] = new ValidationItem { IsValid = true, Message = string.Empty };
            }
        }

        public static void SetValidating(ObservableDictionary<string, ValidationItem> validationDict, string key, string message)
        {
            if (validationDict.ContainsKey(key))
            {
                validationDict[key].IsValid = false;
                validationDict[key].Message = message;
            }
            else
            {
                validationDict[key] = new ValidationItem { IsValid = false, Message = message };
            }
        }

        public static void ClearValidatingWaterMark(ObservableDictionary<string, ValidationItem> validationDict)
        {
            foreach (var item in validationDict)
            {
                item.Value.WaterMarkText = $"{item.Key}를 입력하세요.";
            }
        }

        public static bool GetValidating(ObservableDictionary<string, ValidationItem> validationDict, string key)
        {
            return validationDict.ContainsKey(key) && validationDict[key].IsValid;
        }

        public static void ValidateField(ObservableDictionary<string, ValidationItem> validationDict, string key, string value = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ClearValidating(validationDict, key);
            }
            else
            {
                SetValidating(validationDict, key, value);
            }
        }
    }
}
