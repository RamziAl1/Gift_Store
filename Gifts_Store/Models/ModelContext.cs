using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace Gifts_Store.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

	public virtual DbSet<ContactUsEntry> ContactUsEntries { get; set; }
	public virtual DbSet<AboutU> AboutUs { get; set; }

    public virtual DbSet<Adminn> Adminns { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<Gift> Gifts { get; set; }

    public virtual DbSet<GiftMaker> GiftMakers { get; set; }

    public virtual DbSet<GiftSender> GiftSenders { get; set; }

    public virtual DbSet<Home> Homes { get; set; }

    public virtual DbSet<Orderr> Orderrs { get; set; }

    public virtual DbSet<Rolee> Rolees { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<Userr> Userrs { get; set; }

    public virtual DbSet<VisaInfo> VisaInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("USER ID=JOR18_USER312;PASSWORD=Ddollar1@;DATA SOURCE=94.56.229.181:3488/traindb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("JOR18_USER312")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<ContactUsEntry>(entity =>
        {
			entity.HasKey(e => e.Id).HasName("SYS_C00382398");

			entity.ToTable("CONTACT_US_ENTRY");

			entity.Property(e => e.Id)
				.ValueGeneratedOnAdd()
				.HasColumnType("NUMBER(38)")
				.HasColumnName("ID");
			entity.Property(e => e.AdminId)
				.HasColumnType("NUMBER(38)")
				.HasColumnName("ADMIN_ID");
			entity.Property(e => e.Name)
	            .HasMaxLength(50)
	            .IsUnicode(false)
	            .HasColumnName("NAME");
			entity.Property(e => e.Email)
	            .HasMaxLength(50)
	            .IsUnicode(false)
	            .HasColumnName("EMAIL");
			entity.Property(e => e.Subject)
	            .HasMaxLength(20)
	            .IsUnicode(false)
	            .HasColumnName("SUBJECT");
			entity.Property(e => e.Message)
	            .HasMaxLength(500)
	            .IsUnicode(false)
	            .HasColumnName("MESSAGE");

			entity.HasOne(d => d.Admin).WithMany(p => p.ContactUsEntries)
				.HasForeignKey(d => d.AdminId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_CONTACTUSENTRIES_ADMIN");
		});


		modelBuilder.Entity<AboutU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380354");

            entity.ToTable("ABOUT_US");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.AdminId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ADMIN_ID");
            entity.Property(e => e.IntroText)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("INTRO_TEXT");

            entity.HasOne(d => d.Admin).WithMany(p => p.AboutUs)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ABOUTUS_ADMIN");
        });

        modelBuilder.Entity<Adminn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380333");

            entity.ToTable("ADMINN");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Profit)
                .HasDefaultValueSql("0")
                .HasColumnType("FLOAT")
                .HasColumnName("PROFIT");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Adminns)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ADMIN_USERID");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380331");

            entity.ToTable("CATEGORY");

            entity.HasIndex(e => e.CategoryName, "UQ_CATEGORY_CATEGORYNAME").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CATEGORY_NAME");
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380360");

            entity.ToTable("CONTACT_US");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.AdminId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ADMIN_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("MOBILE_NUMBER");
            entity.Property(e => e.PoBox)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("PO_BOX");

            entity.HasOne(d => d.Admin).WithMany(p => p.ContactUs)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CONTACTUS_ADMIN");
        });

        modelBuilder.Entity<Gift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380345");

            entity.ToTable("GIFT");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CATEGORY_ID");
            entity.Property(e => e.GiftMakerId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("GIFT_MAKER_ID");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Price)
                .HasColumnType("FLOAT")
                .HasColumnName("PRICE");
            entity.Property(e => e.Sale)
                .HasColumnType("NUMBER")
                .HasColumnName("SALE");
            entity.Property(e => e.AddedDate)
                .HasColumnType("DATE")
                .HasColumnName("DATE_ADDED");
            entity.Property(e => e.Quantity)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("QUANTITY")
                .HasDefaultValueSql("1");

            entity.HasOne(d => d.Category).WithMany(p => p.Gifts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_GIFT_CATEGORYID");

            entity.HasOne(d => d.GiftMaker).WithMany(p => p.Gifts)
                .HasForeignKey(d => d.GiftMakerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_GIFT_GIFTMAKERID");
        });

        modelBuilder.Entity<GiftMaker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380336");

            entity.ToTable("GIFT_MAKER");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CATEGORY_ID");
            entity.Property(e => e.Profit)
                .HasDefaultValueSql("0")
                .HasColumnType("FLOAT")
                .HasColumnName("PROFIT");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Category).WithMany(p => p.GiftMakers)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MAKER_CATEGORYID");

            entity.HasOne(d => d.User).WithMany(p => p.GiftMakers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MAKER_USERID");
        });

        modelBuilder.Entity<GiftSender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380340");

            entity.ToTable("GIFT_SENDER");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.GiftSenders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SENDER_USERID");
        });

        modelBuilder.Entity<Home>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380351");

            entity.ToTable("HOME");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.AdminId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ADMIN_ID");
            entity.Property(e => e.LogoPath)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("LOGO_PATH");
			entity.Property(e => e.BackgroundPath)
				.HasMaxLength(200)
				.IsUnicode(false)
				.HasColumnName("BACKGROUND_PATH");
			entity.Property(e => e.SiteName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SITE_NAME");
            entity.Property(e => e.WelcomeText)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WELCOME_TEXT");

            entity.HasOne(d => d.Admin).WithMany(p => p.Homes)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_HOME_ADMIN");
        });

        modelBuilder.Entity<Orderr>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380376");

            entity.ToTable("ORDERR");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.ExpectedArrivalDate)
                .HasColumnType("DATE")
                .HasColumnName("EXPECTED_ARRIVAL_DATE");
            entity.Property(e => e.GiftId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("GIFT_ID");
            entity.Property(e => e.GiftSenderId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("GIFT_SENDER_ID");
            entity.Property(e => e.HasArrived)
                .HasPrecision(1)
                .HasDefaultValueSql("0")
                .HasColumnName("HAS_ARRIVED");
            entity.Property(e => e.PaymentMade)
                .HasPrecision(1)
                .HasDefaultValueSql("0")
                .HasColumnName("PAYMENT_MADE");
            entity.Property(e => e.OrderDate)
                .HasColumnType("DATE")
                .HasColumnName("ORDER_DATE");
            entity.Property(e => e.Quantity)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("QUANTITY");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("FLOAT")
                .HasColumnName("TOTAL_PRICE");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("'pending'")
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Gift).WithMany(p => p.Orderrs)
                .HasForeignKey(d => d.GiftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDER_GIFTID");

            entity.HasOne(d => d.GiftSender).WithMany(p => p.Orderrs)
                .HasForeignKey(d => d.GiftSenderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDER_GIFTSENDERID");
        });

        modelBuilder.Entity<Rolee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380319");

            entity.ToTable("ROLEE");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380363");

            entity.ToTable("TESTIMONIAL");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Messege)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MESSEGE");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("'pending'")
                .HasColumnName("STATUS");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TESTIMONIAL_USERID");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380324");

            entity.ToTable("USER_LOGIN");

            entity.HasIndex(e => e.Email, "UQ_USERLOGIN_EMAIL").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ_USERLOGIN_USERNAME").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.RoleId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROLE_ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");
            entity.Property(e => e.UserName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("USER_NAME");

            entity.HasOne(d => d.Role).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_USERLOGIN_ROLEID");

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_USERLOGIN_USERID");
        });

        modelBuilder.Entity<Userr>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380316");

            entity.ToTable("USERR");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Fname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FNAME");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.Lname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LNAME");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("'pending'\n")
                .HasColumnName("STATUS");
        });

        modelBuilder.Entity<VisaInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C00380393");

            entity.ToTable("VISA_INFO");

            entity.HasIndex(e => e.CardNumber, "UQ_VISAINFO_CARDNUMBER").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Balance)
                .HasDefaultValueSql("0\n")
                .HasColumnType("FLOAT")
                .HasColumnName("BALANCE");
            entity.Property(e => e.CardHolderName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CARD_HOLDER_NAME");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CARD_NUMBER");
            entity.Property(e => e.Ccv)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CCV");
            entity.Property(e => e.ExpireDate)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRE_DATE");
        });
        modelBuilder.HasSequence("DEPARTMENT_SEQUENCE");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
