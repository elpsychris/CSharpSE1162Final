using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OnlineLibrary.Models
{
    public partial class OnlineLibraryContext : DbContext
    {
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<ActivityHistory> ActivityHistory { get; set; }
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BookStatus> BookStatus { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<FavoriteList> FavoriteList { get; set; }
        public virtual DbSet<FineHistory> FineHistory { get; set; }
        public virtual DbSet<FineType> FineType { get; set; }
        public virtual DbSet<ReturnType> ReturnType { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Title> Title { get; set; }
        public virtual DbSet<TitleCategoryDetail> TitleCategoryDetail { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Server=NGUYENNTTSE6232\NGUYENNTT;Database=OnlineLibrary;uid=sa;password=12345");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Account__CA1938C03E4A00AC");

                entity.HasIndex(e => e.Id)
                    .HasName("UQ__Account__3214EC267549EB93")
                    .IsUnique();

                entity.Property(e => e.Seq).HasColumnName("SEQ");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Avatar).HasColumnType("varchar(50)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnName("ID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.LastName).HasMaxLength(20);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.Phone).HasColumnType("char(11)");

                entity.Property(e => e.Rid)
                    .HasColumnName("RID")
                    .HasColumnType("char(3)");

                entity.Property(e => e.Sex).HasColumnType("char(1)");

                entity.HasOne(d => d.R)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.Rid)
                    .HasConstraintName("FK_M_R");
            });

            modelBuilder.Entity<ActivityHistory>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Activity__CA1938C0E31BF875");

                entity.ToTable("Activity_History");

                entity.HasIndex(e => new { e.Mid, e.Bid, e.BorrowDate })
                    .HasName("UC_Person")
                    .IsUnique();

                entity.Property(e => e.Seq).HasColumnName("SEQ");

                entity.Property(e => e.Bid)
                    .IsRequired()
                    .HasColumnName("BID")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.BorrowDate)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(e => e.Mid)
                    .IsRequired()
                    .HasColumnName("MID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.ReturnDate).HasColumnType("date");

                entity.HasOne(d => d.B)
                    .WithMany(p => p.ActivityHistory)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Bid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AH_B");

                entity.HasOne(d => d.M)
                    .WithMany(p => p.ActivityHistory)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Mid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AH_M");
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.Description).HasColumnType("varchar(4000)");

                entity.Property(e => e.FirstName)
                    .HasColumnName("First Name")
                    .HasMaxLength(100);

                entity.Property(e => e.Image).HasColumnType("varchar(50)");

                entity.Property(e => e.LastName)
                    .HasColumnName("Last Name")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Book__CA1938C0E30651DC");

                entity.HasIndex(e => e.Id)
                    .HasName("UQ__Book__3214EC265D8B80DC")
                    .IsUnique();

                entity.Property(e => e.Seq).HasColumnName("SEQ");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnName("ID")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.ImportDate).HasColumnType("date");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Book)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_B_BST");

                entity.HasOne(d => d.TitleNavigation)
                    .WithMany(p => p.Book)
                    .HasForeignKey(d => d.Title)
                    .HasConstraintName("FK_B_T");
            });

            modelBuilder.Entity<BookStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__Book_Sta__C8EE2043D1280A05");

                entity.ToTable("Book_Status");

                entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .ValueGeneratedNever();

                entity.Property(e => e.StatusName).HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Category__CA1938C0C50876ED");

                entity.Property(e => e.Seq).HasColumnName("SEQ");

                entity.Property(e => e.Name).HasMaxLength(30);
            });

            modelBuilder.Entity<FavoriteList>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Favorite__CA1938C04D8F9D9F");

                entity.ToTable("Favorite_List");

                entity.Property(e => e.Seq).HasColumnName("SEQ");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.Property(e => e.TitleId).HasColumnName("TitleID");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.FavoriteList)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("FK_FL_M");

                entity.HasOne(d => d.Title)
                    .WithMany(p => p.FavoriteList)
                    .HasForeignKey(d => d.TitleId)
                    .HasConstraintName("FK_Favorite_List_Title");
            });

            modelBuilder.Entity<FineHistory>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Fine_His__CA1938C0CC61401A");

                entity.ToTable("Fine_History");

                entity.HasIndex(e => e.Aseq)
                    .HasName("UQ__Fine_His__4DF639F196DD5AEC")
                    .IsUnique();

                entity.Property(e => e.Seq).HasColumnName("SEQ");

                entity.Property(e => e.Aseq)
                    .IsRequired()
                    .HasColumnName("ASEQ");

                entity.Property(e => e.FineTypeId).HasColumnName("FineTypeID");

                entity.HasOne(d => d.AseqNavigation)
                    .WithOne(p => p.FineHistory)
                    .HasForeignKey<FineHistory>(d => d.Aseq)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_LR_AH");

                entity.HasOne(d => d.FineType)
                    .WithMany(p => p.FineHistory)
                    .HasForeignKey(d => d.FineTypeId)
                    .HasConstraintName("FK_FH_FT");
            });

            modelBuilder.Entity<FineType>(entity =>
            {
                entity.ToTable("Fine_Type");

                entity.Property(e => e.FineTypeId)
                    .HasColumnName("FineTypeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("varchar(500)");

                entity.Property(e => e.FineTypeName).HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<ReturnType>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Return_T__CA1938C03DFD5C3D");

                entity.ToTable("Return_Type");

                entity.Property(e => e.Seq).HasColumnName("SEQ");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("char(3)");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Title>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Title__CA1938C06A3FA632");

                entity.Property(e => e.Seq).HasColumnName("SEQ");

                entity.Property(e => e.Description).HasColumnType("varchar(max)");

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.Illu).HasColumnType("varchar(250)");

                entity.Property(e => e.Isbn)
                    .HasColumnName("ISBN")
                    .HasColumnType("char(13)");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Publisher).HasMaxLength(50);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Title)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK_Title_Author");

                entity.HasOne(d => d.ReturnTypeNavigation)
                    .WithMany(p => p.Title)
                    .HasForeignKey(d => d.ReturnType)
                    .HasConstraintName("FK_Title_Return_Type");
            });

            modelBuilder.Entity<TitleCategoryDetail>(entity =>
            {
                entity.HasKey(e => e.Seq)
                    .HasName("PK__Book_Cat__CA1938C0490491D2");

                entity.ToTable("Title_Category_Detail");

                entity.HasIndex(e => new { e.TitleId, e.CategoryId })
                    .HasName("UN_T_C")
                    .IsUnique();

                entity.Property(e => e.Seq).HasColumnName("SEQ");

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasColumnName("CategoryID");

                entity.Property(e => e.TitleId)
                    .IsRequired()
                    .HasColumnName("TitleID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.TitleCategoryDetail)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("TK_C_B");

                entity.HasOne(d => d.Title)
                    .WithMany(p => p.TitleCategoryDetail)
                    .HasForeignKey(d => d.TitleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Title_Category_Detail_Title");
            });
        }
    }
}