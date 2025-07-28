using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Repositories.Interfaces;
using System.Data;

namespace SonoCap.MES.Repositories
{
    public class AppSettingsRepository : RepositoryBase<AppSettings>, IAppSettingsRepository
    {
        public AppSettingsRepository(MESDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 지정된 설정 키에 대한 다음 시퀀스 값을 가져오고 업데이트합니다.
        /// 시퀀스가 존재하지 않으면 '1'로 초기화됩니다.
        /// 이 메서드는 EF Core의 ORM 기능을 사용하여 동시성 및 데이터 일관성을 보장합니다.
        /// </summary>
        /// <param name="settingKey">시퀀스를 가져올 설정 키입니다.</param>
        /// <returns>다음 시퀀스 값입니다.</returns>
        /// <exception cref="InvalidOperationException">설정 키가 INT 타입이 아니거나 업데이트된 시퀀스를 가져올 수 없을 때 발생합니다.</exception>
        /// <exception cref="Exception">시퀀스 발급 및 업데이트 중 기타 오류가 발생할 때 발생합니다.</exception>
        public async Task<int> GetNextSequenceAsync(string settingKey)
        {
            // _context는 RepositoryBase에서 MESDbContext 타입으로 이미 주입받았을 것입니다.
            // 필요에 따라 MESDbContext 타입으로 캐스팅하여 AppSettings DbSet에 직접 접근합니다.
            var mesDbContext = (MESDbContext)_context;

            // 시퀀스 발급은 민감한 작업이므로 반드시 트랜잭션 내에서 원자적으로 처리합니다.
            // IsolationLevel.RepeatableRead는 트랜잭션 시작 시의 스냅샷을 유지하지만,
            // EF Core의 변경 추적기와 SaveChangesAsync가 내부적으로 동시성 문제를 관리합니다.
            // MySQL의 기본값이 RepeatableRead이므로, 명시적으로 ReadCommitted를 지정하는 것이
            // 다른 트랜잭션의 커밋된 변경사항을 더 빨리 볼 수 있어서 유리할 수 있습니다.
            using (IDbContextTransaction transaction = await mesDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted))
            {
                try
                {
                    // 1. 설정 키에 해당하는 AppSettings 엔티티를 조회합니다.
                    // EF Core는 이 쿼리 결과로 엔티티를 Change Tracker에 로드하고 추적합니다.
                    var currentSetting = await mesDbContext.AppSettings
                                                           .FirstOrDefaultAsync(s => s.SettingKey == settingKey);

                    int nextSequenceValue;

                    if (currentSetting == null)
                    {
                        // 설정 키가 존재하지 않으면, 새로운 AppSettings 엔티티를 추가하고 시퀀스를 '1'로 초기화합니다.
                        nextSequenceValue = 1;
                        await mesDbContext.AppSettings.AddAsync(new AppSettings
                        {
                            SettingKey = settingKey,
                            SettingValue = nextSequenceValue.ToString(), // 정수 값을 문자열로 저장
                            ValueType = SettingValueType.INT,
                            DataFlag = 1, // 필요에 따라 기본값 설정
                            Detail = $"{settingKey} sequence counter",
                            CreatedDate = DateTime.Now
                        });
                    }
                    else
                    {
                        // 설정 키가 존재하는 경우, INT 타입인지 유효성 검사를 수행합니다.
                        if (currentSetting.ValueType != SettingValueType.INT)
                        {
                            throw new InvalidOperationException($"SettingKey '{settingKey}' is not of type INT. Current type: {currentSetting.ValueType}. Cannot get sequence.");
                        }

                        // 현재 SettingValue를 정수로 파싱하고 1 증가시킵니다.
                        if (int.TryParse(currentSetting.SettingValue, out int currentValue))
                        {
                            nextSequenceValue = currentValue + 1;
                            currentSetting.SettingValue = nextSequenceValue.ToString(); // 증가된 값을 문자열로 업데이트
                            // 필요에 따라 Detail이나 LastUpdatedDate 같은 다른 속성도 업데이트할 수 있습니다.
                            // currentSetting.Detail = currentSetting.Detail ?? $"{settingKey} sequence counter";
                            // currentSetting.LastUpdatedDate = DateTime.Now; // LastUpdatedDate 컬럼이 있다면 사용
                        }
                        else
                        {
                            // SettingValue가 유효한 정수가 아닐 경우, 예외 처리 또는 '1'로 재설정
                            throw new InvalidOperationException($"Existing SettingValue for key '{settingKey}' is not a valid integer: {currentSetting.SettingValue}. Cannot get next sequence.");
                            // 또는 nextSequenceValue = 1; currentSetting.SettingValue = "1"; 으로 초기화
                        }
                    }

                    // EF Core의 Change Tracker가 엔티티의 상태 변경(추가 또는 수정)을 감지하고 있습니다.
                    // SaveChangesAsync()를 호출하여 변경사항을 데이터베이스에 반영합니다.
                    // 이 시점에서 EF Core는 적절한 INSERT 또는 UPDATE SQL을 생성하고 실행합니다.
                    await mesDbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // 모든 DB 작업이 성공적으로 완료되면 트랜잭션 커밋

                    // 커밋된 최신 시퀀스 값을 반환합니다.
                    return nextSequenceValue;
                }
                catch (Exception ex)
                {
                    // 오류 발생 시 트랜잭션을 롤백하여 데이터베이스 변경사항을 취소합니다.
                    await transaction.RollbackAsync();
                    // 원본 예외를 포함하여 새로운 예외를 throw하여 호출자에게 알립니다.
                    throw new Exception($"Failed to get next sequence for key '{settingKey}': {ex.Message}", ex);
                }
            } // using 블록을 벗어나면 트랜잭션 객체가 Dispose 됩니다.
        }
        public async Task<int> GetNextSequenceSqlAsync(string settingKey)
        {
            // _context를 MESDbContext 타입으로 캐스팅하여 특정 DbSet에 접근
            // (MESDbContext)_context는 AppSettings 속성에 접근할 수 있게 해줍니다.
            var mesDbContext = (MESDbContext)_context; // 캐스팅

            // 1. 사전 검증
            var existingSetting = await mesDbContext.AppSettings.AsNoTracking().FirstOrDefaultAsync(s => s.SettingKey == settingKey);
            if (existingSetting != null && existingSetting.ValueType != SettingValueType.INT)
            {
                throw new InvalidOperationException($"SettingKey '{settingKey}' is not of type INT. Current type: {existingSetting.ValueType}. Cannot get sequence.");
            }

            // 트랜잭션 시작
            using (IDbContextTransaction transaction = await mesDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted))
            {
                try
                {
                    // **STEP 1: UPSERT (데이터 변경) - ExecuteSqlRawAsync 사용 (FromSqlRaw 사용 불가!)**
                    string upsertSql = @"
                        INSERT INTO AppSettings (SettingKey, SettingValue, ValueType, DataFlag, Detail, CreatedDate)
                        VALUES (@p0, @p1, @p2, @p3, @p4, @p5)
                        ON DUPLICATE KEY UPDATE
                            SettingValue = CAST(SettingValue AS SIGNED) + 1,
                            Detail = Detail;
                        ";

                    var parameters = new object[]
                    {
                        settingKey,
                        "1",
                        SettingValueType.INT.ToString(),
                        1,
                        $"{settingKey} sequence counter",
                        DateTime.Now
                    };

                    await mesDbContext.Database.ExecuteSqlRawAsync(upsertSql, parameters);

                    // **STEP 2: 변경된 값 조회 (SELECT) - FromSqlRaw 사용**
                    // 이 부분은 Select 쿼리이므로 FromSqlRaw를 사용할 수 있습니다.
                    // mesDbContext.AppSettings.FromSqlRaw()는 _context.Set<AppSettings>().FromSqlRaw()와 동일한 효과를 줍니다.
                    var updatedSetting = await mesDbContext.AppSettings // <-- 여기가 _context.Set<AppSettings>()에 해당!
                                                                .FromSqlRaw("SELECT * FROM AppSettings WHERE SettingKey = @key",
                                                                            new MySqlParameter("key", settingKey))
                                                                .AsNoTracking() // 안전을 위해 명시적 AsNoTracking 유지
                                                                .SingleOrDefaultAsync();

                    if (updatedSetting == null)
                    {
                        throw new InvalidOperationException($"Could not retrieve the updated sequence for key: {settingKey}");
                    }

                    if (int.TryParse(updatedSetting.SettingValue, out int result))
                    {
                        await transaction.CommitAsync();
                        return result;
                    }
                    else
                    {
                        throw new InvalidOperationException($"The retrieved SettingValue for key '{settingKey}' is not a valid integer: {updatedSetting.SettingValue}");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to get next sequence for key '{settingKey}': {ex.Message}", ex);
                }
            }
        }

        public async Task<SettingItemResult?> GetSettingValueAndTypeAsync(string settingKey)
        {
            var mesDbContext = (MESDbContext)_context;
            // SettingValue와 ValueType을 함께 조회합니다.
            var setting = await mesDbContext.AppSettings
                                             .AsNoTracking() // 변경 추적 불필요
                                             .Where(s => s.SettingKey == settingKey)
                                             .Select(s => new { s.SettingKey, s.SettingValue, s.ValueType }) // 익명 타입으로 두 속성만 선택
                                             .FirstOrDefaultAsync();

            if (setting == null)
            {
                return null; // 해당 설정 키가 없으면 null 반환
            }

            return new SettingItemResult
            {
                Key = setting.SettingKey,
                Value = setting.SettingValue, // <-- 이제 이곳에 할당되는 Value는 required 속성을 만족합니다.
                Type = setting.ValueType
            };
        }

        public async Task SetSettingValueAsync(string settingKey, string value, SettingValueType valueType = SettingValueType.STRING)
        {
            var mesDbContext = (MESDbContext)_context;
            var setting = await mesDbContext.AppSettings.FirstOrDefaultAsync(s => s.SettingKey == settingKey);
            if (setting == null)
            {
                await mesDbContext.AppSettings.AddAsync(new AppSettings
                {
                    SettingKey = settingKey,
                    SettingValue = value,
                    ValueType = valueType,
                    DataFlag = 1,
                    Detail = $"Setting for {settingKey}",
                    CreatedDate = DateTime.Now
                });
            }
            else
            {
                setting.SettingValue = value;
                setting.ValueType = valueType;
                setting.DataFlag = 1;
                setting.Detail = setting.Detail ?? $"Setting for {settingKey}";
            }
            await mesDbContext.SaveChangesAsync();
        }
    }
}