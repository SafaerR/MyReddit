using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MyReddit.Models
{
    public partial class dbSiteContext : DbContext
    {
        public dbSiteContext()
        {
        }

        public dbSiteContext(DbContextOptions<dbSiteContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<User> Users { get; set; }
 //----méthodes de manupilation de la base de données de

        //ajout nv user
        public void AddNewUser(User user){
            Add<User>(user);
            SaveChanges();

        }

        //ajout d,un nouveau link
         public void AddNewLink(Post post){
            Add<Post>(post);
            SaveChanges();

        }

        //---recuperer tous les links d'un user
        public List<Post> getLinkForUser(int userid){ 
             return Posts.Where(p => p.UserId == userid).ToList();
             }      

        //---afficher les infos du link cliqué
          public List<Post> getInfosLink(int linkid){ 
             return Posts.Where(p=>p.Id == linkid).ToList();
             }      
        //ajout nouveau comment
        public void AddNewComment(Comment comment){
            Add<Comment>(comment);
            SaveChanges();

        }
        //update le link:
        public void UpdateLink(Post post) {
             Update<Post>(post);
               SaveChanges();
         }

    //delete tt d'abors le comment d'un link a supprimer
          public void DeleteComment(Comment comment) {         
                
             Remove<Comment>(comment);
               SaveChanges();
         }  
         //delete link:
         
         public void DeleteLink(Post post) {
             //delete tt d'abors le comment d'un link a supprimer
               
             Remove<Post>(post);
               SaveChanges();
         } 

    //recuperer tous les commentaires d'un lien
    public List<Comment> getCommentForLink(int idlink){ 
             return Comments.Where(c=>c.PostId == idlink).ToList();
             }       

     /*    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=rsafae-mysql-db.mysql.database.azure.com;port=3306;database=dbSite;uid=rsafae@rsafae-mysql-db;pwd=Aspnet2021;sslmode=Preferred", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.32-mysql"));
            }
        } */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment");

                entity.HasIndex(e => e.PostId, "PostID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.PostId)
                    .HasColumnType("int(11)")
                    .HasColumnName("PostID");

                entity.Property(e => e.PublicationDate).HasColumnType("date");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("UserID");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("comment_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("comment_ibfk_1");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("post");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.DownVote).HasColumnType("int(11)");

                entity.Property(e => e.Link).HasMaxLength(100);

                entity.Property(e => e.PublicationDate).HasColumnType("date");

                entity.Property(e => e.Upvote).HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("post_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Email).HasMaxLength(30);

                entity.Property(e => e.Pwd).HasMaxLength(30);

                entity.Property(e => e.UserName).HasMaxLength(30);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
