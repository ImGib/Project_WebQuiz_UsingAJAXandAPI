using API.Common.DTOs.CategoryDTO;
using API.Models;
using AutoMapper;

namespace API.Common.Mapper
{
    public class CategoryMapper:Profile
    {
        public CategoryMapper()
        {
            CreateMap<CategoryRequest, Category>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<Category, CategoryResponseBase>();
        }
    }
}
