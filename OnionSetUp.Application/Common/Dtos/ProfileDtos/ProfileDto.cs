namespace OnionSetUp.Application.Common.Dtos.ProfileDtos
{
    public record ProfileDto(Guid Id,string FullName,string Email,string ImageUrl,List<string>Roles);
}
