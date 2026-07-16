namespace OnionSetUp.Application.Common.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, UserDto>()
                .ConstructUsing(x => new UserDto(x.Id, x.FullName,x.Email!, new List<string>()))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
            //CreateMap<UserDto, AppUser>()
            //    .ConstructUsing(x => new(x.FullName));
            CreateMap<AppUser, ProfileDto>()
                .ConstructUsing(x => new ProfileDto(x.Id, x.FullName, x.Email!, x.ImageUrl, new List<string>()))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }
}