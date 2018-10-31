# Custom ASP.NET Core Identity Table Generation Example Project

## ASP.NET CORE 2.1

## Nuget Dependencies
*Microsoft.EntityFrameworkCore.Sqlite* (If your using SQLite)

*Microsoft.EntityFrameworkCore.Tools*

*Microsoft.Extensions.Options.ConfigurationExtensions*

You can rename these to whatever works for you for cusomizing the table names that are generated
 
### appsettings.json file

Change the SQLServerConnection string setting to match your datasource. Please follow best practice guidelines and don't include any sensitive information in plain text such as this json file.
Best option is to use Azure Key Vault for secret storage

```json
 "SQLServerConnection": "{insert your sql server connection string}"
 ```

 ###Table Naming Section
 ```json
 "CustomIdentityTables": {
    "UsersTableName": "Users",
    "RolesTableName": "Roles",
    "RoleClaimsTableName": "RoleClaims",
    "UserClaimsTableName": "UserClaims",
    "UserLoginsTableName": "UserLogins",
    "UserRolesTableName": "UserRoles",
    "UserTokensTableName": "UserTokens",
    "SchemaName":  "Security"
  }
```

### Startup.cs

```c#
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<TableNameSettings>(Configuration.GetSection("CustomIdentityTables"));

            // *If* you need access to generic IConfiguration this is **required**
            services.AddSingleton<IConfiguration>(Configuration);
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SQLServerConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
```

### ApplicationDbContext.cs

```C#
    public class ApplicationDbContext : IdentityDbContext
    <
        ApplicationUser, ApplicationRole, string,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken
    >
    {
        private TableNameSettings TableNameSettings { get; set; }

        public ApplicationDbContext()
        {
            
        }

        public ApplicationDbContext(IOptions<TableNameSettings> settings)
        {
            TableNameSettings = settings.Value;
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<TableNameSettings> settings)
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

```