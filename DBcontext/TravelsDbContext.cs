using Microsoft.EntityFrameworkCore;
using travels_server_side.Entities;

namespace travels_server_side.DBcontext
{
    public class TravelsDbContext : DbContext
    {
        public TravelsDbContext(DbContextOptions<TravelsDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<VisitsEO>()
            //    .HasKey(v => new { v.userEmailFK, v.siteIdFK, v.travelIdFK });
            //  modelBuilder.Entity<TravelsEO>().HasMany(s=>s.travelId).

            /*
            modelBuilder.Entity<TravelsEO>()
                .HasKey(t => new { t.travelId, t.userEmailFK });
            */

            //modelBuilder.Entity<TravelsEO>().HasKey(t => t.userEmailFK).HasForeignKey(t => t.userEmailFK);

            modelBuilder.Entity<UserPreferencesEO>().
                HasKey(u => new { u.userEmail, u.categoryId });

            modelBuilder.Entity<SiteRatingsEO>().HasKey(s => new { s.userEmail, s.siteId });

        }
        public virtual DbSet<SitesEO> sites { get; set; }
        public virtual DbSet<UsersEO> users { get; set; }
        public virtual DbSet<VisitsEO> visits { get; set; }
        public virtual DbSet<TravelsEO> travels { get; set; }
        public virtual DbSet<CategoriesEO> categories { get; set; }
        public virtual DbSet<AdminEO> admins { get; set; }
        public virtual DbSet<ManagersEO> managers { get; set; }
        public virtual DbSet<UserPreferencesEO> preferences { get; set; }
        public virtual DbSet<SiteRatingsEO> ratings { get; set; }
    }
}
