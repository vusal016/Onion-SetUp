namespace OnionSetUp.Application.Common.Dtos.ServiceDtos
{
    public record UserDto(Guid Id,string FullName,string Email,List<string>Roles);
}
