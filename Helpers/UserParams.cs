namespace WebApi.Helpers
{
    public class UserParams
    {
        public int MaxPageSize { get; set; } = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize 
        { 
            get{ return pageSize; } 
            set{ pageSize = ( value > MaxPageSize ) ? MaxPageSize : value; }
        }
    }
}