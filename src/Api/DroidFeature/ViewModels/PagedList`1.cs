using System.Collections.Generic;

namespace Cds.DroidManagement.Api.DroidFeature.ViewModels
{
    /// <summary>
    /// Paged List T Items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// The page Size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The Page Index.
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// True if there is a next page of Items
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Collection of Items
        /// </summary>
        public IReadOnlyCollection<T> Items { get; set; }
    }
}
