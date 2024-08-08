namespace SonoCap.MES.Models.Base
{
    public class AddOn
    {
        public override string ToString()
        {
            var properties = GetType().GetProperties()
                .Where(p => p.CanRead && !p.GetIndexParameters().Any())
                .Select(p => new { Name = p.Name, Value = p.GetValue(this, null) });

            return string.Join(", ", properties.Select(p => $"{p.Name}: {p.Value}"));
        }
    }
}
