namespace DZarsky.CommonLibraries.MicroserviceEshop.Api.Results
{
    public class PagedResult<TClass> where TClass : class
    {
        public IList<TClass> Records { get; set; } = new List<TClass>();

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public PagedResult(IList<TClass> records, int pageSize, int pageCount)
        {
            Records = records;
            PageSize = pageSize;
            PageCount = pageCount;
        }

        public PagedResult()
        {

        }
    }
}
