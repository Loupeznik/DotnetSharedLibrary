namespace DZarsky.CommonLibraries.MicroserviceEshop.Extensions
{
    public static class ListExtensions
    {
        public static IList<string> ListItemsToString<TSource>(this IList<TSource> items)
        {
            var result = new List<string>();

            foreach (var item in items)
            {
                var stringItem = item?.ToString();

                if (!string.IsNullOrWhiteSpace(stringItem))
                {
                    result.Add(stringItem);
                }
            }

            return result;
        }
    }
}
