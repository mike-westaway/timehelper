using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeHelper.Data.Models
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public virtual ICollection<Association> Associations { get; set; }
    }
}
