using System.Collections.Generic;
namespace CafeteriaServer.Recommendation
{
    public static class SentimentWords
    {
        public static readonly List<string> PositiveWords = new List<string>
    {
        "delicious", "amazing", "great", "fantastic", "excellent", "good", "tasty", "wonderful", "superb", "awesome",
        "very good", "incredible", "perfect", "outstanding", "impressive", "marvelous", "fabulous", "satisfying",
        "brilliant", "splendid", "love", "enjoy", "nice", "pleased", "delightful", "high quality", "top-notch",
        "best", "phenomenal", "pleasurable", "positive", "exceptional", "beautiful", "liked", "sweet", "happy",
        "worthwhile", "superior", "magnificent", "charming", "stellar", "enchanting", "valuable"
    };

        public static readonly List<string> NegativeWords = new List<string>
    {
        "bad", "terrible", "disappointing", "awful", "poor", "tasteless", "horrible", "gross", "unpleasant",
        "mediocre", "not good", "bad taste", "subpar", "poor quality", "lacking", "dissatisfying", "unacceptable",
        "worst", "nasty", "disgusting", "dislike", "regret", "pathetic", "offensive", "inferior", "waste",
        "problematic", "boring", "terrifying", "ugly", "dreadful", "insufficient", "crummy", "shoddy", "deficient",
        "atrocious", "vile", "hate", "horrendous", "miserable", "lousy", "inferior", "frustrating", "displeased",
        "sickening", "repulsive", "rotten", "gruesome", "disturbing", "shameful"
    };

        public static readonly List<string> NegationWords = new List<string>
    {
        "not", "no", "never", "none", "neither", "without", "barely", "hardly", "scarcely", "rarely",
        "little", "few", "nothing", "nobody"
    };
    }
}