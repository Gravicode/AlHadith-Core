using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hadith.Migrator.Models;

public partial class HadithDbContext : DbContext
{
    public HadithDbContext()
    {
    }

    public HadithDbContext(DbContextOptions<HadithDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bookmark> Bookmarks { get; set; }

    public virtual DbSet<Crawled> Crawleds { get; set; }

    public virtual DbSet<Hadith> Hadiths { get; set; }

    public virtual DbSet<HadithChapter> HadithChapters { get; set; }

    public virtual DbSet<HadithContent> HadithContents { get; set; }

    public virtual DbSet<HadithIndex> HadithIndices { get; set; }

    public virtual DbSet<HadithPage> HadithPages { get; set; }

    public virtual DbSet<SqliteSequence> SqliteSequences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=HadithDB;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => e.Idx).HasName("Bookmark_PK");

            entity.ToTable("Bookmark", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.Idx)
                .HasComment("TRIAL")
                .HasColumnName("idx");
            entity.Property(e => e.ChapterNo).HasComment("TRIAL");
            entity.Property(e => e.HadithId).HasComment("TRIAL");
            entity.Property(e => e.HadithNo).HasComment("TRIAL");
            entity.Property(e => e.PageNo).HasComment("TRIAL");
            entity.Property(e => e.Title)
                .HasComment("TRIAL")
                .HasColumnType("text");
            entity.Property(e => e.Trial788)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("TRIAL788");
        });

        modelBuilder.Entity<Crawled>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Crawled__3214EC27FF447EC8");

            entity.ToTable("Crawled");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasColumnType("ntext");
            entity.Property(e => e.Url).IsUnicode(false);
        });

        modelBuilder.Entity<Hadith>(entity =>
        {
            entity.HasKey(e => e.HadithId).HasName("PK__Hadith__CC026034B72316DB");

            entity.ToTable("Hadith");

            entity.Property(e => e.HadithId).HasColumnName("HadithID");
            entity.Property(e => e.About).HasColumnType("ntext");
            entity.Property(e => e.Arabic).HasColumnType("ntext");
            entity.Property(e => e.Name)
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(350)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HadithChapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId).HasName("PK__HadithCh__0893A34A32A32D13");

            entity.ToTable("HadithChapter");

            entity.Property(e => e.ChapterId).HasColumnName("ChapterID");
            entity.Property(e => e.ChapterNoStr)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HadithId).HasColumnName("HadithID");
            entity.Property(e => e.Intro).HasColumnType("ntext");
            entity.Property(e => e.Title).IsUnicode(false);
            entity.Property(e => e.TitleArabic).HasColumnType("ntext");
        });

        modelBuilder.Entity<HadithContent>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__HadithCo__2907A87E59F02F32");

            entity.ToTable("HadithContent");

            entity.Property(e => e.ContentId).HasColumnName("ContentID");
            entity.Property(e => e.BookRef)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ContentArabic).HasColumnType("ntext");
            entity.Property(e => e.ContentEnglish).IsUnicode(false);
            entity.Property(e => e.ContentIndonesia).IsUnicode(false);
            entity.Property(e => e.ContentUrdu).HasColumnType("ntext");
            entity.Property(e => e.Grade).HasColumnType("ntext");
            entity.Property(e => e.HadithId).HasColumnName("HadithID");
            entity.Property(e => e.Narated).IsUnicode(false);
            entity.Property(e => e.OtherRef)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Reference).IsUnicode(false);
            entity.Property(e => e.SanadBottom).HasColumnType("ntext");
            entity.Property(e => e.SanadTop).HasColumnType("ntext");
            entity.Property(e => e.UrlRef).IsUnicode(false);
            entity.Property(e => e.Uscref)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("USCRef");
        });

        modelBuilder.Entity<HadithIndex>(entity =>
        {
            entity.HasKey(e => e.IndexId).HasName("PK__HadithIn__40BC8AA158829A21");

            entity.ToTable("HadithIndex");

            entity.Property(e => e.IndexId).HasColumnName("IndexID");
            entity.Property(e => e.ArabicName).HasColumnType("ntext");
            entity.Property(e => e.HadithId).HasColumnName("HadithID");
            entity.Property(e => e.Name).IsUnicode(false);
        });

        modelBuilder.Entity<HadithPage>(entity =>
        {
            entity.HasKey(e => e.PageId).HasName("PK__HadithPa__C565B124CAAB8B6C");

            entity.ToTable("HadithPage");

            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.HadithId).HasColumnName("HadithID");
            entity.Property(e => e.Title).IsUnicode(false);
            entity.Property(e => e.TitleArabic).HasColumnType("ntext");
        });

        modelBuilder.Entity<SqliteSequence>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("sqlite_sequence", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.Name)
                .HasComment("TRIAL")
                .HasColumnType("text")
                .HasColumnName("name");
            entity.Property(e => e.Seq)
                .HasComment("TRIAL")
                .HasColumnType("text")
                .HasColumnName("seq");
            entity.Property(e => e.Trial788)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("TRIAL788");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
