using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace User.Models
{
    public class AuctionLot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
        {
        }

        public DbSet<AuctionLot> Lots { get; set; }
    }
}
