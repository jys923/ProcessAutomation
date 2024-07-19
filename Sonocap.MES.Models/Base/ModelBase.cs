using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace SonoCap.MES.Models.Base
{
    public class ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///  create = 1, delete = 0
        /// </summary>
        [Required]
        public int DataFlag { get; set; } = 1;

        [StringLength(100)]
        public string? Detail { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public override string ToString()
        {
            var properties = GetType().GetProperties()
                .Where(p => p.CanRead && !p.GetIndexParameters().Any())
                .Select(p => new { Name = p.Name, Value = p.GetValue(this, null) });

            return string.Join(", ", properties.Select(p => $"{p.Name}: {p.Value}"));
        }

        public string ToJson()
        {
            try
            {
                return JsonSerializer.Serialize(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting to JSON: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
