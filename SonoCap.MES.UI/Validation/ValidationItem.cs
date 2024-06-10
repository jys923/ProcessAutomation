namespace SonoCap.MES.UI.Validation
{
    public class ValidationItem
    {
        public bool IsValid { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string WaterMarkText { get; set; } = string.Empty;
    }
}
