namespace DZarsky.CommonLibraries.MicroserviceEshop.Entities
{
    public class BaseEntity
    {
        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public bool IsDeleted { get; set; }
    }
}
