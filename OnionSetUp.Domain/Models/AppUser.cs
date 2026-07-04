using OnionSetUp.Domain.Common;

namespace OnionSetUp.Domain.Models
{
  public class AppUser:IdentityUser<Guid>,ISoftDeletable  
    {
        public AppUser()
        {
            
        }
        public AppUser(string fullName)
        {
            SetFullName(fullName);
        }   
        public string FullName { get;private set; }
        public bool IsDeleted { get; set;}
        public DateTime? DeletedAt { get; set ;}
        public string? DeletedBy { get; set; }

        private void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("FullName can't be empty");
            FullName = fullName;
        }
        public void UPdateFullName(string updateName)
        {
            SetFullName(updateName);
        }
    }
}