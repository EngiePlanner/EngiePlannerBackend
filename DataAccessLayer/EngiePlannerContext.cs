using BusinessObjectLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class EngiePlannerContext : DbContext
    {
        private readonly DbContextOptions<EngiePlannerContext> _options;

        public DbContextOptions<EngiePlannerContext> Options
        {
            get
            {
                return _options;
            }
        }

        public EngiePlannerContext(DbContextOptions<EngiePlannerContext> options) : base(options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=DESKTOP-OHBS1P5;Database=EngiePlanner;Trusted_Connection=True;")
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDepartmentMapping>()
                .HasKey(nameof(UserDepartmentMapping.UserUsername), nameof(UserDepartmentMapping.DepartmentId));
            modelBuilder.Entity<UserGroupMapping>()
                .HasKey(nameof(UserGroupMapping.UserUsername), nameof(UserGroupMapping.GroupId));
            modelBuilder.Entity<UserTaskMapping>()
                .HasKey(nameof(UserTaskMapping.UserUsername), nameof(UserTaskMapping.TaskId));
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<DepartmentEntity> Departments { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<UserDepartmentMapping> UserDepartmentMappings { get; set; }
        public DbSet<UserGroupMapping> UserGroupMappings { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<DeliveryEntity> Delivery { get; set; }
        public DbSet<UserTaskMapping> UserTaskMappings { get; set; }
    }
}
