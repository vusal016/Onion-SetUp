namespace OnionSetUp.Domain.Common
{
    public abstract class AuditEntity:BaseEntity
    {
        protected AuditEntity():base()
        {
            
        }
        protected AuditEntity(Guid id):base(id)
        {
            
        }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get;set; }
    }
}
