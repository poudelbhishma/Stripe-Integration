using Microsoft.EntityFrameworkCore;

namespace Stripe_Integration.Models
{
    public partial class tattsContext : DbContext
    {
        public tattsContext()
        {
        }

        public tattsContext(DbContextOptions<tattsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TechengineeMisAnswer> TechengineeMisAnswers { get; set; } = null!;
        public virtual DbSet<TechengineeMisCredential> TechengineeMisCredentials { get; set; } = null!;
        public virtual DbSet<TechengineeMisQuestion> TechengineeMisQuestions { get; set; } = null!;
        public virtual DbSet<TechengineeMisStudent> TechengineeMisStudents { get; set; } = null!;
        public virtual DbSet<TechengineeMisSubject> TechengineeMisSubjects { get; set; } = null!;
        public virtual DbSet<TechengineeMisTeacher> TechengineeMisTeachers { get; set; } = null!;
        public virtual DbSet<ManageProduct> ManageProducts { get; set; } = null!;
        public virtual DbSet<TechengineeMisUserType> TechengineeMisUserTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TechengineeMisAnswer>(entity =>
            {
                entity.HasKey(e => e.AnswerId)
                    .HasName("PK__techengi__D482502426080D21");

                entity.ToTable("techenginee_MIS_Answer");

                entity.Property(e => e.AnswerId).HasColumnName("AnswerID");

                entity.Property(e => e.AnswerTxt).IsUnicode(false);

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                // Configure the relationship with TechengineeMisQuestion
                entity.HasOne(a => a.Question)
                    .WithMany()
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure the relationship with TechengineeMisStudent
                entity.HasOne(a => a.Student)
                    .WithMany()
                    .HasForeignKey(a => a.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<TechengineeMisCredential>(entity =>
            {
                entity.HasKey(e => e.CredentialId)
                    .HasName("PK__techengi__2C58F9EC50DB4FE4");

                entity.ToTable("techenginee_MIS_Credential");

                entity.HasIndex(e => e.Email, "UQ__techengi__A9D105349751EF61")
                    .IsUnique();

                entity.Property(e => e.CredentialId).HasColumnName("CredentialID");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TechengineeMisQuestion>(entity =>
            {
                entity.HasKey(e => e.QuestionId)
                    .HasName("PK__techengi__0DC06F8C0281243E");

                entity.ToTable("techenginee_MIS_Question");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.QuestionsTxt).IsUnicode(false);

                // Configure the relationship with Subject
                entity.HasOne(d => d.Subject)                  // Question has one Subject
                    .WithMany()                                // Subject can have many Questions
                    .HasForeignKey(d => d.SubjectCode)         // Foreign key in Question table
                    .IsRequired();                             // Subject is required for a Question
            });

            modelBuilder.Entity<TechengineeMisStudent>(entity =>
            {
                entity.HasKey(e => e.StudentId)
                    .HasName("PK__techengi__32C52A79D8C1459C");

                entity.ToTable("techenginee_MIS_Student");

                entity.HasIndex(e => e.Email, "UQ__techengi__A9D105344E3F3741")
                    .IsUnique();

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TechengineeMisSubject>(entity =>
            {
                entity.HasKey(e => e.SubjectCode)
                    .HasName("PK__techengi__9F7CE1A861D59701");

                entity.ToTable("techenginee_MIS_Subject");

                entity.Property(e => e.SubjectName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TechengineeMisTeacher>(entity =>
            {
                entity.HasKey(e => e.TeacherId)
                    .HasName("PK__techengi__EDF25944462FB3DD");

                entity.ToTable("techenginee_MIS_Teacher");

                entity.HasIndex(e => e.Email, "UQ__techengi__A9D105347C61BD3C")
                    .IsUnique();

                entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                // Configure the relationship with Subject
                entity.HasOne(d => d.Subject)                  // Question has one Subject
                    .WithMany()                                // Subject can have many Questions
                    .HasForeignKey(d => d.SubjectCode)         // Foreign key in Question table
                    .IsRequired();                             // Subject is required for a Question
            });

            modelBuilder.Entity<ManageProduct>(entity =>
            {
                entity.HasKey(e => e.ProductID)
                    .HasName("PK__stripe_i__B40CC6ED");

                entity.ToTable("stripe_integration_products"); //table names

                entity.Property(e => e.ProductID)
                    .HasColumnName("ProductID");

                //entity.Property(e => e.ProductName).HasMaxLength(50);
                //entity.Property(e => e.Descriptions).HasMaxLength(50);
                //entity.Property(e => e.Quantity);
                //entity.Property(e => e.ImagePath).HasMaxLength(500);
                //entity.Property(e => e.CreatedOn);
                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Descriptions)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity);

                entity.Property(e => e.ImagePath)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime");
            });


            modelBuilder.Entity<TechengineeMisUserType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__techengi__516F0395DF30A8FB");

                entity.ToTable("techenginee_MIS_UserType");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.UserType)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
