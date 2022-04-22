using System.Collections.Generic;

namespace _01_MakeUpQuery.Contracts.Product
{
    public interface IProductQuery
    {
        List<ProductQueryModel> GetLatestArrivals();
    }
}
