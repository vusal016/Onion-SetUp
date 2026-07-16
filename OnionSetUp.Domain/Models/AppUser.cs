namespace OnionSetUp.Domain.Models
{
    public class AppUser : IdentityUser<Guid>, ISoftDeletable
    {
        public AppUser()
        {

        }
        public AppUser(string fullName,string ImageUrl)
        {
            SetImageUrl(ImageUrl);
            SetFullName(fullName);
        }
        public string FullName { get; private set; }
        public string ImageUrl { get; private set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        private void SetImageUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image can't be empty");
            ImageUrl = imageUrl;
        }
        private void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("FullName can't be empty");
            FullName = fullName;
        }
        public void UpdateImageUrl(string updateImage)
        {
            SetImageUrl(updateImage);
        }
        public void UpdateFullName(string updateName)
        {
            SetFullName(updateName);
        }
    }
}