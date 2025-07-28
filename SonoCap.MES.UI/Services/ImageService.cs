using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.UI.Services
{
    // 2. ImageService 구현체
    public class ImageService
    {
        private readonly IAppSettingsRepository _appSettingsRepository;

        // 생성자를 통해 IAppSettingsRepository를 주입받습니다.
        public ImageService(IAppSettingsRepository appSettingsRepository)
        {
            _appSettingsRepository = appSettingsRepository;
        }

        public async Task<string> GenNextImageFullPathAsync(string prefix, string exportDirectory, string extension)
        {
            // 1. 저장 디렉토리가 없으면 생성합니다.
            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }

            // 2. AppSettingsRepository를 통해 다음 시퀀스 번호를 가져옵니다.
            // "ImageSequence_{prefix}" 키를 사용하여 각 접두사별로 독립적인 시퀀스를 관리합니다.
            // 예를 들어 "ProductA_001", "ProductB_001"과 같이 시작할 수 있습니다.
            // 만약 모든 이미지에 대해 단일 시퀀스를 사용하고 싶다면, "ImageSequence"와 같이 고정된 키를 사용할 수 있습니다.
            string sequenceKey = $"ImageSequence_{prefix}";
            int newIndex = await _appSettingsRepository.GetNextSequenceAsync(sequenceKey);

            // 3. 파일 확장자의 시작에 '.'이 붙어있지 않도록 처리합니다.
            string cleanedExtension = extension.TrimStart('.');

            // 4. 파일명을 조합합니다 (예: "ProductA_001.bmp").
            // D3는 숫자를 최소 3자리로 만들고 앞에 0을 채우라는 포맷입니다. (예: 1 -> 001, 10 -> 010)
            string fileName = $"{prefix}_{newIndex:D3}.{cleanedExtension}";

            // 5. 전체 파일 경로를 반환합니다.
            string fullPath = Path.Combine(exportDirectory, fileName);

            return fullPath;
        }

        public async Task<string> GenNextImgNameAsync(string prefix)
        {
            // AppSettingsRepository를 통해 'prefix' 자체를 시퀀스 키로 사용하여 다음 번호를 가져옵니다.
            // 이렇게 하면 각 prefix (예: "ProductA", "ProductB")가 자신만의 독립적인 시퀀스를 갖게 됩니다.
            int newIndex = await _appSettingsRepository.GetNextSequenceAsync(prefix);

            // 파일명을 조합합니다 (예: "ProductA_001").
            // D3는 숫자를 최소 3자리로 만들고 앞에 0을 채우라는 포맷입니다. (예: 1 -> 001, 10 -> 010)
            string fileName = $"{prefix}_{newIndex:D3}";

            // 생성된 파일 이름 (확장자 없음)만 반환합니다.
            return fileName;
        }
    }
}