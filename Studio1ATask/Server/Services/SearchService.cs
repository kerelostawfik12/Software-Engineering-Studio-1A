using System.Collections.Generic;
using System.Linq;

namespace Studio1BTask.Services
{
    public class SearchService
    {
        // Converts a search phrase to a list of words, handling stuff in quotation marks as one word.
        public List<string> GetWordsFromPhrase(string searchPhrase)
        {
            if (searchPhrase == "")
                return new List<string>();

            // Treat quotes the same as single words (similar to what google does)
            var quotes = searchPhrase.Split('"').Where((item, index) => index % 2 != 0).ToList();

            // Remove quote strings from the phrase as they have already been processed
            foreach (var quote in quotes)
                searchPhrase = searchPhrase.Replace('"' + quote + '"', "");

            // Separate all words in the phrase and check them separately
            var words = searchPhrase.Split(" ").ToList();
            words.RemoveAll(x => x == "");

            words = words.Concat(quotes).ToList();

            return words;
        }
    }
}