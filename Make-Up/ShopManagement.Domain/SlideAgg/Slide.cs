using _0_Framework.Domain;

namespace ShopManagement.Domain.SlideAgg
{
    public class Slide : EntityBase
    {
        public string Picture { get; private set; }
        public string PictureAlt { get; private set; }
        public string PictureTitle { get; private set; }
        public string Heading { get; private set; }
        public string Title { get; private set; }
        public string BtnText { get; private set; }
        public string Text { get; private set; }
        public string Link { get; private set; }
        public bool IsRemoved { get; private set; }

        public Slide(string picture, string pictureAlt, string pictureTitle,
            string heading, string title, string btnText, string text, string link)
        {
            Picture = picture;
            PictureAlt = pictureAlt;
            PictureTitle = pictureTitle;
            Heading = heading;
            Title = title;
            BtnText = btnText;
            Text = text;
            Link = link;
            IsRemoved = false;
        }

        public void Edit(string picture, string pictureAlt, string pictureTitle,
            string heading, string title, string btnText, string text, string link)
        {
            if (!string.IsNullOrWhiteSpace(picture))
                Picture = picture;

            PictureAlt = pictureAlt;
            PictureTitle = pictureTitle;
            Heading = heading;
            Title = title;
            BtnText = btnText;
            Text = text;
            Link = link;
        }

        public void Remove()
        {
            IsRemoved = true;
        }

        public void Restore()
        {
            IsRemoved = false;
        }
    }
}