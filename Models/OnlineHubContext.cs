using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication1.Models
{
    public partial class OnlineHubContext : DbContext
    {
        public OnlineHubContext()
        {
        }

        public OnlineHubContext(DbContextOptions<OnlineHubContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<Level> Level { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
        public virtual DbSet<Steps> Steps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.IngredientName).HasMaxLength(50);

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Ingredient)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_Ingredient_Recipe");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.Property(e => e.LevelId).HasColumnName("LevelID");

                entity.Property(e => e.LevelName).HasMaxLength(50);
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.DateAdded).HasColumnType("smalldatetime");

                entity.Property(e => e.Image1).HasMaxLength(250);

                entity.Property(e => e.Image2).HasMaxLength(250);

                entity.Property(e => e.Image3).HasMaxLength(250);

                entity.Property(e => e.LevelId).HasColumnName("LevelID");

                entity.Property(e => e.RecipeTitle).HasMaxLength(50);

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Recipe)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK_Recipe_Level");
            });

            modelBuilder.Entity<Steps>(entity =>
            {
                entity.Property(e => e.StepsId).HasColumnName("StepsID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.StepName).HasMaxLength(50);

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Steps)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_Steps_Recipe");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
