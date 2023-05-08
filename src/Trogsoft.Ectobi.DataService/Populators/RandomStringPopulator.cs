using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Populators
{
    [DisplayName("Random String Generator")]
    [PopulatorReturnType(PopulatorReturnType.String)]
    public class RandomStringPopulator : PopulatorBase
    {
        private const string OPT_LOWERCASE = "lowerCase";
        private const string OPT_UPPERCASE = "upperCase";
        private const string OPT_NUMBERS = "numbers";
        private const string OPT_SYMBOLS = "symbols";
        private const string OPT_LENGTH = "length";

        public override IList<PopulatorOption> GetOptions()
        {
            return new List<PopulatorOption>
            {
                new PopulatorOption(OPT_LOWERCASE, "Include lower case characters", PopulatorOptionType.Checkbox),
                new PopulatorOption(OPT_UPPERCASE, "Include upper case characters", PopulatorOptionType.Checkbox),
                new PopulatorOption(OPT_NUMBERS, "Include numbers", PopulatorOptionType.Checkbox),
                new PopulatorOption(OPT_SYMBOLS, "Include symbols", PopulatorOptionType.Checkbox),
                new PopulatorOption(OPT_LENGTH, "Length", PopulatorOptionType.Integer),
            };
        }

        public override string GetString(Dictionary<string, string> options)
        {
            bool lowerCase = true;
            bool upperCase = true;
            bool numbers = true;
            bool symbols = false;
            int length = 16;

            if (options.ContainsKey(OPT_LOWERCASE)) bool.TryParse(options[OPT_LOWERCASE], out lowerCase);
            if (options.ContainsKey(OPT_UPPERCASE)) bool.TryParse(options[OPT_UPPERCASE], out upperCase);
            if (options.ContainsKey(OPT_NUMBERS)) bool.TryParse(options[OPT_NUMBERS], out numbers);
            if (options.ContainsKey(OPT_SYMBOLS)) bool.TryParse(options[OPT_SYMBOLS], out symbols);
            if (options.ContainsKey(OPT_LENGTH)) int.TryParse(options[OPT_LENGTH], out length);

            var lowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz";
            var upperCaseCharacters = lowerCaseCharacters.ToUpperInvariant();
            var numberCharacters = "0123456789";
            var symbolCharacters = "!$%^&*#@";

            string charSet = "";

            if (lowerCase) charSet += lowerCaseCharacters;
            if (upperCase) charSet += upperCaseCharacters;
            if (numbers) charSet += numberCharacters;
            if (symbols) charSet += symbolCharacters;

            StringBuilder result = new StringBuilder();

            var i = 0;
            while (i < length)
            {
                var c = RandomNumberGenerator.GetInt32(0, charSet.Length);
                result.Append(charSet[c]);
                i++;
            }

            return result.ToString();

        }
    }
}
