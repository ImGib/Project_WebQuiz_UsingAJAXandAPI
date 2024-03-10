namespace API.Common.DTOs.CategoryDTO
{
    public class CategoryResponseBase
    {
        public int Categoryno { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
    }
}
