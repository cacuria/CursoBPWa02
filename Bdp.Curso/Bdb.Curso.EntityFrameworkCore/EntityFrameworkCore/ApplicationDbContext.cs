using Bdb.Curso.Core.Entities;
using Bdb.Curso.Core.Shared;
using Microsoft.EntityFrameworkCore;
                                       
namespace Bdb.Curso.EntityFrameworkCore
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #region Listado de colecciones
        public DbSet<User> Users { get; set; }          
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductKardex> ProductKardexs { get; set; } 
        public DbSet<ProductBalance> ProductBalances { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }            
        public DbSet<Category> Categories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        #endregion

        // programacion fluent para darle caracticas a las relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {    
            #region  user entity

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("tbUser"); // Nombre de la tabla en la base de datos

                entity.HasKey(u => u.Id); // Define la clave primaria
                                                                       

                entity.Property(u => u.UserName)
                      .IsRequired()
                      .HasMaxLength(GeneralConstants.NameMaxlength); // Longitud de UserName

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(GeneralConstants.NameMaxlength); // Longitud de Name

                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(GeneralConstants.NameMaxlength); // Longitud de Password

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(GeneralConstants.NameMaxlength); // Establece restricciones en Email

                entity.Property(u => u.EmailConfirmed)
                      .IsRequired(); // Asegúrate de que EmailConfirmed es requerido

                entity.Property(u => u.Created)
                      .IsRequired(); // Asegúrate de que Created es requerido

                entity.Property(u => u.Roles)
                      .IsRequired()
                      .HasMaxLength(GeneralConstants.TxtMaxlength); // Longitud de Roles

                entity.Property(u => u.TwoFactorCode)
               .IsRequired()
               .HasMaxLength(GeneralConstants.NameMaxlength); // clave para 2do factor

                //relaciones

            });

            #endregion
                                     
            #region Entity Category
                      
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("tbCategories"); // Nombre de la tabla en la base de datos

                entity.HasKey(u => u.Id); // Define la clave primaria
                                              
                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(GeneralConstants.NameMaxlength); // Longitud de Name
                          
            });


            #endregion
                                         
            #region Entity Supplier

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("tbSuppliers"); // Nombre de la tabla en la base de datos

                entity.HasKey(u => u.Id); // Define la clave primaria

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(GeneralConstants.NameMaxlength); // Longitud de Name

            });


            #endregion           

            #region Entity Product

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("tbProducts"); // Nombre de la tabla en la base de datos

                entity.HasKey(u => u.Id); // Define la clave primaria

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(GeneralConstants.NameMaxlength); // Longitud de Name
                                   
                // Configurar la precisión y escala del campo Price
                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);  // Hasta 18 dígitos con 2 decimales

                entity.HasOne(p=>p.Category)
                    .WithMany(c=>c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction);


                entity.HasOne(p => p.Supplier)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(p=>p.ProductBalances)
                    .WithOne(pb=>pb.Product)
                    .HasForeignKey(pb => pb.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(p => p.ProductKardexs)
                    .WithOne(pb => pb.Product)
                    .HasForeignKey(pb => pb.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);


            });


            #endregion

            #region ProductBalance Entity

            modelBuilder.Entity<ProductBalance>(entity =>
            {
                entity.ToTable("tbBalances");

                entity.HasKey(pb => pb.Id);

                entity.Property(pb => pb.Created)
                      .IsRequired();

                entity.Property(pb => pb.Amount)
                      .HasPrecision(18, 2);

                // Configuración de la relación con Product
                entity.HasOne(pb => pb.Product)
                      .WithMany(p => p.ProductBalances)
                      .HasForeignKey(pb => pb.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Configuración de la relación con User
                entity.HasOne(pb => pb.User)
                      .WithMany()
                      .HasForeignKey(pb => pb.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

            });


            #endregion

            #region ProductKarex Entity

            modelBuilder.Entity<ProductKardex>(entity =>
            {
                entity.ToTable("tbKardexs");

                entity.HasKey(pk => pk.Id);

                entity.Property(pk => pk.Created)
                      .IsRequired();

                entity.Property(pk => pk.Amount)
                      .HasPrecision(18, 2);

                // Configuración de la relación con Product
                entity.HasOne(pk => pk.Product)
                      .WithMany(p => p.ProductKardexs)
                      .HasForeignKey(pk => pk.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Configuración de la relación con User
                entity.HasOne(pk => pk.User)
                      .WithMany()
                      .HasForeignKey(pk => pk.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });


            #endregion



            #region Entity RefreshToken

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("tbRefreshTokens"); // Nombre de la tabla en la base de datos

                entity.HasKey(rt => rt.Token); // Define la clave primaria
                entity.Property(rt => rt.Expires).IsRequired(); // Asegúrate de que Expires es requerido
                entity.Property(rt => rt.UserId).IsRequired(); // Asegúrate de que UserId es requerido
                entity.Property(rt => rt.Created).IsRequired(); // Asegúrate de que Created es requerido

                // Si necesitas un índice único para el token, puedes agregarlo
                entity.HasIndex(rt => rt.Token).IsUnique();

                //entity.HasOne(pb => pb.User)
                //  .WithMany()
                //  .HasForeignKey(pb => pb.UserId)
                //  .OnDelete(DeleteBehavior.NoAction);
                                            
            });
            #endregion



        }



    }


}
