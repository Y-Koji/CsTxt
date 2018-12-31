using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CsTxt.Block
{
    public class UsingBlock : BlockBase, IBlock
    {
        private static readonly string REGEX = "@using [A-Za-z0-9\\.;]+(\r\n)?";

        public override BlockType Type => BlockType.Using;

        public override string Content { get; protected set; }

        public static IBlock Parse(string text)
        {
            if (Regex.IsMatch(text, REGEX))
            {
                var result = Regex.Match(text, REGEX).Value;

                return new UsingBlock
                {
                    Content = result,
                };
            }

            return Null;
        }

        public override string ToCSharpCode()
        {
            return Content.TrimStart('@').Trim();
        }

        public string GetNamespace()
        {
            return Content.Trim().TrimStart('@').TrimEnd(';').Replace("using ", string.Empty);
        }
    }
}
