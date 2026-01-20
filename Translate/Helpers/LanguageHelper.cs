namespace Translate.Helpers
{
    /// <summary>
    /// Helper methods for language code operations.
    /// </summary>
    public static class LanguageHelper
    {
        /// <summary>
        /// Available language codes for translation.
        /// </summary>
        public static readonly string[] LanguageCodes = new[] { "zh", "en", "ja", "ko" };

        /// <summary>
        /// Converts a language code to its full language name.
        /// </summary>
        public static string CodeToLanguage(string code)
        {
            return code switch
            {
                "en" => "English",
                "zh" => "Chinese",
                "ja" => "Japanese",
                "ko" => "Korean",
                _ => code
            };
        }

        public const string PromptTemplate =
            "You are a professional {SOURCE_LANG} ({SOURCE_CODE}) to {TARGET_LANG} ({TARGET_CODE}) translator. " +
            "Your goal is to accurately convey the meaning and nuances of the original {SOURCE_LANG} text " +
            "while adhering to {TARGET_LANG} grammar, vocabulary, and cultural sensitivities. " +
            "Produce only the {TARGET_LANG} translation, without any additional explanations or commentary. " +
            "Please translate the following {SOURCE_LANG} text into {TARGET_LANG}:\n\n{TEXT}";
    }
}
