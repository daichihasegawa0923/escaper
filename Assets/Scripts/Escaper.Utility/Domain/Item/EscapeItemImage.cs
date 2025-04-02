#nullable enable

namespace Escaper.Utility.Domain.Item
{
    public class EscapeItemImage
    {
        public EscapeItemImage(string imagePath, EscapeItemReturner returner)
        {
            ImagePath = imagePath;
            Returner = returner;
        }

        public string ImagePath { get; set; } = "";

        public EscapeItemReturner? Returner { get; set; }
    }
}