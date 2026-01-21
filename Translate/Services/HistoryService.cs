using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Translate.Data;
using Translate.Models;

namespace Translate.Services
{
    /// <summary>
    /// Service for managing translation history records.
    /// </summary>
    public class HistoryService
    {
        private readonly IDbContextFactory<TranslateDbContext> _contextFactory;
        private readonly SettingsService _settings;

        public HistoryService(IDbContextFactory<TranslateDbContext> contextFactory, SettingsService settings)
        {
            _contextFactory = contextFactory;
            _settings = settings;
            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            using var context = _contextFactory.CreateDbContext();
            context.Database.EnsureCreated();
        }

        /// <summary>
        /// Adds a new translation record to history.
        /// </summary>
        public async Task AddRecordAsync(
            string sourceText,
            string targetText,
            string sourceLanguageCode,
            string targetLanguageCode,
            string serviceName)
        {
            await using var context = _contextFactory.CreateDbContext();

            var record = new TranslationHistoryItem
            {
                Timestamp = DateTime.Now,
                SourceText = sourceText,
                TargetText = targetText,
                SourceLanguageCode = sourceLanguageCode,
                TargetLanguageCode = targetLanguageCode,
                ServiceName = serviceName
            };

            context.TranslationHistory.Add(record);
            await context.SaveChangesAsync();

            // Limit history count
            await TrimHistoryAsync(context);
        }

        /// <summary>
        /// Gets all translation history records ordered by timestamp descending.
        /// </summary>
        public async Task<List<TranslationHistoryItem>> GetAllAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.TranslationHistory
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Deletes a specific history record by ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await using var context = _contextFactory.CreateDbContext();
            var record = await context.TranslationHistory.FindAsync(id);
            if (record != null)
            {
                context.TranslationHistory.Remove(record);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Clears all history records.
        /// </summary>
        public async Task ClearAllAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            context.TranslationHistory.RemoveRange(context.TranslationHistory);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Keeps only the most recent MaxHistoryCount records.
        /// </summary>
        private async Task TrimHistoryAsync(TranslateDbContext context)
        {
            var maxCount = _settings.MaxHistoryCount;
            var count = await context.TranslationHistory.CountAsync();
            if (count > maxCount)
            {
                var recordsToDelete = await context.TranslationHistory
                    .OrderBy(h => h.Timestamp)
                    .Take(count - maxCount)
                    .ToListAsync();

                context.TranslationHistory.RemoveRange(recordsToDelete);
                await context.SaveChangesAsync();
            }
        }
    }
}
