using AutoMapper;
using ECommerce.Api.Dtos.Cart;
using ECommerce.Api.Dtos.Category;
using ECommerce.Api.Dtos.Order;
using ECommerce.Api.Dtos.Product;
using ECommerce.Api.Dtos.Review;
using ECommerce.Api.Dtos.Wishlist;
using ECommerce.Api.Entities;
using ECommerce.Api.Entities.OrderAggregate;

namespace ECommerce.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category Mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.ParentCategoryName, o => o.MapFrom(s => s.ParentCategory != null ? s.ParentCategory.Name : null));

            CreateMap<CreateCategoryDto, Category>()
                .ForMember(d => d.Slug, o => o.MapFrom(s => s.Name.ToLower().Replace(" ", "-")))
                .ForMember(d => d.ParentCategoryId, o => o.MapFrom(s => s.ParentCategoryId));

            // Product Mappings
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.MainImageUrl, o => o.MapFrom(s => s.Images.FirstOrDefault(x => x.IsMain).Url));

            CreateMap<CreateProductDto, Product>()
                .ForMember(d => d.Images, o => o.Ignore());

            CreateMap<ProductImage, ProductImageDto>();

            // Cart Mappings
            CreateMap<Cart, CartDto>();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Product.Price))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Product.Images.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Product.Category.Name));

            // Order Mappings
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.OrderDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.GetTotal()))
                .ForMember(d => d.PaymentMethod, o => o.MapFrom(s => s.PaymentMethod.ToString()));

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<Address, AddressDto>().ReverseMap();

            // Review Mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName));
            CreateMap<CreateReviewDto, Review>();

            // Wishlist Mappings
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.Id))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Product.Price))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Product.Category.Name))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Product.Images.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.AddedAt, o => o.MapFrom(s => s.CreatedAt));
        }
    }
}
