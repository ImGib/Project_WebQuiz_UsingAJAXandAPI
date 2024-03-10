using API.Common.DTOs.CategoryDTO;
using API.Models;

namespace API.Common.DTOs.SubjectDTO
{
    public class SubjectResponse : SubjectResponseBase
    {
        public CategoryResponseBase CategorynoNavigation { get; set; }
    }
}
