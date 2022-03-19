using System.Collections.Generic;

namespace _01_MakeUpQuery.Contracts.Slide
{
    public interface ISlideQuery
    {
        List<SlideQueryModel> GetSlides();
    }
}
