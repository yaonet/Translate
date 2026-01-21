using System;

namespace Translate.Models
{
    /// <summary>
    /// Entity representing a translation history record.
    /// </summary>
    public class TranslationHistoryItem
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string SourceText { get; set; } = string.Empty;
        public string TargetText { get; set; } = string.Empty;
        public string SourceLanguageCode { get; set; } = string.Empty;
        public string TargetLanguageCode { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
    }
}
