using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinnworksTest.Features.services.linnworks
{
    public class LinnworksContext : DbContext
    {
        public LinnworksContext(DbContextOptions<LinnworksContext> options)
           : base(options)
        { }

        public DbSet<Category> Categories { get; set; }
    }

    public class Category 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
    }
}
