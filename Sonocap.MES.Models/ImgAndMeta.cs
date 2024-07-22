namespace SonoCap.MES.Models
{
    public class ImgAndMeta
    {
        public byte[] Img { get; set; }
        public string Meta { get; set; }

        public ImgAndMeta(byte[] Img, string Meta)
        {
            this.Img = Img;
            this.Meta = Meta;
        }
    }
}
