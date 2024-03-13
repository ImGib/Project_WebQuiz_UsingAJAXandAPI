using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Models
{
    public partial class QuizAPIContext : DbContext
    {
        public QuizAPIContext()
        {
        }

        public QuizAPIContext(DbContextOptions<QuizAPIContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<History> Histories { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Initial Catalog=QuizAPI; User ID=sa;Password=giabao5102;Encrypt=false;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.Answerno);

                entity.ToTable("ANSWER");

                entity.Property(e => e.Answerno).HasColumnName("ANSWERNO");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.Iscorect).HasColumnName("ISCORECT");

                entity.Property(e => e.Questionno).HasColumnName("QUESTIONNO");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.QuestionnoNavigation)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.Questionno)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ANSWER_QUESTION");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Categoryno);

                entity.ToTable("CATEGORY");

                entity.Property(e => e.Categoryno).HasColumnName("CATEGORYNO");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("TITLE");
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.HasKey(e => e.Htrno)
                    .HasName("PK_HISTORY_1");

                entity.ToTable("HISTORY");

                entity.Property(e => e.Htrno).HasColumnName("HTRNO");

                entity.Property(e => e.Answerno).HasColumnName("ANSWERNO");

                entity.Property(e => e.Questionno).HasColumnName("QUESTIONNO");

                entity.Property(e => e.Subjectno).HasColumnName("SUBJECTNO");

                entity.Property(e => e.Testno).HasColumnName("TESTNO");

                entity.Property(e => e.Username)
                    .HasMaxLength(20)
                    .HasColumnName("USERNAME")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Questionno);

                entity.ToTable("QUESTION");

                entity.Property(e => e.Questionno).HasColumnName("QUESTIONNO");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Subjectno).HasColumnName("SUBJECTNO");

                entity.HasOne(d => d.SubjectnoNavigation)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.Subjectno)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QUESTION_SUBJECT");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.Subjectno);

                entity.ToTable("SUBJECT");

                entity.Property(e => e.Subjectno).HasColumnName("SUBJECTNO");

                entity.Property(e => e.Categoryno).HasColumnName("CATEGORYNO");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.Image).HasColumnName("IMAGE");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("TITLE");

                entity.HasOne(d => d.CategorynoNavigation)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.Categoryno)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SUBJECT_CATEGORY");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.ToTable("USER");

                entity.Property(e => e.Username)
                    .HasMaxLength(20)
                    .HasColumnName("USERNAME")
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("EMAIL")
                    .IsFixedLength();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("FIRST_NAME");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("LAST_NAME");

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .HasColumnName("PASSWORD")
                    .IsFixedLength();

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(16)
                    .HasColumnName("PHONENUMBER")
                    .IsFixedLength();

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnName("ROLE")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("((1))");

                entity.HasMany(d => d.Subjectnos)
                    .WithMany(p => p.Usernames)
                    .UsingEntity<Dictionary<string, object>>(
                        "Enroll",
                        l => l.HasOne<Subject>().WithMany().HasForeignKey("Subjectno").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ENROLL_SUBJECT"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("Username").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ENROLL_USER"),
                        j =>
                        {
                            j.HasKey("Username", "Subjectno");

                            j.ToTable("ENROLL");

                            j.IndexerProperty<string>("Username").HasMaxLength(20).HasColumnName("USERNAME").IsFixedLength();

                            j.IndexerProperty<int>("Subjectno").HasColumnName("SUBJECTNO");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
