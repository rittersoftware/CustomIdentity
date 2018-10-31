using identity.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace identity.Data
{
    using Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class ApplicationDbContext : IdentityDbContext
    <
        ApplicationUser, ApplicationRole, string,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken
    >
    {

        // Add TableNameSettings reference
        private TableNameSettings TableNameSettings { get; set; }

        public ApplicationDbContext()
        {
            
        }

        // Inject TableNameSettings into IOptions within constructor
        public ApplicationDbContext(IOptionsSnapshot<TableNameSettings> settings)
        {
            TableNameSettings = settings.Value;
        }

        // Inject TableNameSettings into IOptions within constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptionsSnapshot<TableNameSettings> settings)
            : base(options)
        {
            TableNameSettings = settings.Value;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var migrationAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;

            base.OnConfiguring(builder);
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer("Data Source=(local);Initial Catalog=identity;User ID=testuser;Password=password;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False", optionsBuilder =>
                {
                    optionsBuilder.UseRowNumberForPaging();
                    optionsBuilder.EnableRetryOnFailure(5);
                    optionsBuilder.CommandTimeout(40);
                    optionsBuilder.MigrationsAssembly(migrationAssembly);
                    optionsBuilder.UseRelationalNulls();
                }).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: TableNameSettings.UsersTableName, schema: TableNameSettings.Schema);

                // Each User can have many UserClaims
                entity.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                entity.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                entity.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                entity.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                entity.Property(b => b.EmailConfirmed).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.PhoneNumberConfirmed).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.TwoFactorEnabled).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.AccessFailedCount).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.Age).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.IsEmployee).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.IsExternalLogonProvider).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.IsPermittedToLogon).HasDefaultValueSql(sql: "1").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.IsSystemUser).HasDefaultValueSql(sql: "1").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.IsSalesPerson).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: TableNameSettings.RolesTableName, schema: TableNameSettings.Schema);

                // Each Role can have many entries in the UserRole join table
                entity.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                entity.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();

                entity.Property(b => b.IsActive).HasDefaultValueSql(sql: "1").ValueGeneratedOnAddOrUpdate();
                entity.Property(b => b.IsAdministrator).HasDefaultValueSql(sql: "0").ValueGeneratedOnAddOrUpdate();
            });

            builder.Entity<ApplicationRoleClaim>(entity =>
            {
                entity.ToTable(name: TableNameSettings.RoleClaimsTableName, schema: TableNameSettings.Schema);
            });

            builder.Entity<ApplicationUserClaim>(entity =>
            {
                entity.ToTable(name: TableNameSettings.UserClaimsTableName, schema: TableNameSettings.Schema);
            });

            builder.Entity<ApplicationUserLogin>(entity =>
            {
                entity.ToTable(name: TableNameSettings.UserLoginsTableName, schema: TableNameSettings.Schema);
            });

            builder.Entity<ApplicationUserRole>(entity =>
            {
                entity.ToTable(name: TableNameSettings.UserRolesTableName, schema: TableNameSettings.Schema);
            });

            builder.Entity<ApplicationUserToken>(entity =>
            {
                entity.ToTable(name: TableNameSettings.UserTokensTableName, schema: TableNameSettings.Schema);
            });
        }
    }
}
