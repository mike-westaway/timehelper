using System.ComponentModel.DataAnnotations.Schema;

namespace TimeHelper.Data.Models
{

    public enum AssociationType { Subject, Category}

    public class Association
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssociationId { get; set; }
        public string UserId { get; set; }
        public AssociationType Type { get; set; }
        public string Criteria { get; set; }
        public int ProjectId { get; set; }
        public Project Project  { get; set; }
    }
}