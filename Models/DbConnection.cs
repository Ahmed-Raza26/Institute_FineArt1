using Microsoft.EntityFrameworkCore;

namespace Institute_FineArt1.Models
{
    
        public class DbConnection : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);
			//optionsBuilder.UseSqlServer("Server=.;Database=InstituteFineArt;User Id=sa;Password=aptech; TrustServerCertificate=True;Connection Timeout=60;");


			optionsBuilder.UseSqlServer("Data Source=DESKTOP-4E5GC9O\\SQLEXPRESS; Initial Catalog=InstituteFineArt; Integrated Security = True; TrustServerCertificate=True;Connection Timeout=60;");
		}
		public  DbSet<Users> Users {  get; set; }

        public DbSet<BestArts> BestArt { get; set; }
        public DbSet<Painting> StudentArt { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CheckOutOrders> Orders { get; set; }

        
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Explicitly map foreign key relationships (if needed)
			modelBuilder.Entity<Painting>()
				.HasOne(p => p.User1)
				.WithMany()
				.HasForeignKey(p => p.UserId);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany()
				.HasForeignKey(r => r.UserId);
		}

	}

        

    
}
