using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class PaginationMetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPageCount { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }

        public PaginationMetaData(int totalItemCount, int pageSize, int currentPage)
        {
            TotalItemCount = totalItemCount;
            TotalPageCount = (int)Math.Ceiling(TotalPageCount / (double)pageSize);
            PageSize = pageSize;
            CurrentPage = currentPage;
        }
    }
}
