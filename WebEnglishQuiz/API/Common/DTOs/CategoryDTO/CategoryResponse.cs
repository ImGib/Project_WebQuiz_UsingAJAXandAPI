using API.Common.DTOs.SubjectDTO;

namespace API.Common.DTOs.CategoryDTO
{
    public class CategoryResponse : CategoryResponseBase
    {
        public virtual IEnumerable<SubjectResponseBase> Subjects { get; set; }
    }
}
