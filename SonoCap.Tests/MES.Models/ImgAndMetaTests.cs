namespace SonoCap.MES.Models.Tests
{
    public class ImgAndMetaTests
    {
        [Fact]
        public void ImgAndMeta_Constructor_SetsProperties()
        {
            // 테스트할 값 설정
            var img = new byte[] { 1, 2, 3, 4, 5 };
            var meta = "Test Meta Data";

            // ImgAndMeta 객체 생성
            var imgAndMeta = new ImgAndMeta(img, meta);

            // 속성 값 확인
            Assert.Equal(img, imgAndMeta.Img);
            Assert.Equal(meta, imgAndMeta.Meta);
        }
    }
}
