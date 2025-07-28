using SonoCap.MES.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.Models
{
    public class SettingItemResult
    {
        public required string Key { get; set; } // <-- Key 속성 추가
        public required string Value { get; set; }
        public required SettingValueType Type { get; set; }
    }

    public enum SettingValueType
    {
        // DB의 DEFAULT 'STRING'과 맞추기 위해 첫 번째 값을 STRING으로 합니다.
        // 또는 0을 Unspecified 등으로 하고, 모든 enum에 [EnumMember(Value = "DB_STRING_VALUE")] 어노테이션을 붙일 수 있습니다.
        STRING,
        INT,
        BOOLEAN,
        DATETIME,
        JSON // 필요하다면 추가
        // 기타 필요한 타입 추가
    }
    public class AppSettings : ModelBase
    {
        // ModelBase에서 Id, DataFlag, Detail, CreatedDate를 상속받습니다.

        [Required]
        [StringLength(100)] // SettingKey의 VARCHAR(100)과 일치
        public string SettingKey { get; set; } = string.Empty; // NULL을 허용하지 않으므로 초기값 지정

        [Required]
        // TEXT 타입에 매핑됩니다.
        public string SettingValue { get; set; } = string.Empty; // NULL을 허용하지 않으므로 초기값 지정

        [Required]
        [StringLength(50)] // ValueType의 VARCHAR(50)과 일치
        public SettingValueType ValueType { get; set; } = SettingValueType.STRING; // enum 타입으로 변경
    }
}
