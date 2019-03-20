using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebApi.Helpers
{
    public static class Extensions
    {
        public static void AddPagination(
            this HttpResponse response, 
            int currentPage,
            int itemsPerPage, 
            int totalItems, 
            int totalPages
        )
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader));
            response.Headers.Add("Acces-Control-Expose-Headers","Pagination");
        }
    }
}