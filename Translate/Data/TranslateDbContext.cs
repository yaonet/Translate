using Microsoft.EntityFrameworkCore;
using Translate.Models;

namespace Translate.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for translation history.
    /// </summary>
    public class TranslateDbContext : DbContext
    {
        public DbSet<TranslationHistoryItem> TranslationHistory { get; set; } = null!;

        public TranslateDbContext(DbContextOptions<TranslateDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TranslationHistoryItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.SourceText).IsRequired();
                entity.Property(e => e.TargetText).IsRequired();
                entity.Property(e => e.SourceLanguageCode).HasMaxLength(10);
                entity.Property(e => e.TargetLanguageCode).HasMaxLength(10);
                entity.Property(e => e.ServiceName).HasMaxLength(50);
                
                // Index for faster querying by timestamp
                entity.HasIndex(e => e.Timestamp);
            });
        }
    }
}
