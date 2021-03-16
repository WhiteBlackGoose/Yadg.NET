﻿using System;
using System.Text;
using static YadgNet.HtmlTags;

namespace YadgNet
{
    public sealed class DescriptionFromXmlBuilder
    {
        private readonly string xml;
        private readonly string upPath;
        public DescriptionFromXmlBuilder(string xml, string upPath)
            => (this.xml, this.upPath) = (xml, upPath);

        private static string ReplaceTag(string before, string tag, string src)
            => src
                .Replace($"<{tag}", $"{before}<div class='yadg-{tag}'")
                .Replace($"</{tag}>", $"</div>");

        private static (string inside, int firstId, int lastId)? Unjail(string before, string after, string src)
        {
            var id1 = src.IndexOf(before);
            if (id1 == -1)
                return null;
            var id2 = src.IndexOf(after, id1 + before.Length);
            if (id2 == -1)
                return null;
            var inside = src[(id1 + before.Length)..id2];
            return (inside, id1, id2 + after.Length);
        }

        private static string ReplaceAttributedOpenCloseTag(
            Func<string, string> attrDelegate,
            string tag, string attr, string src)
        {
            if (Unjail($"<{tag} {attr}=\"", "\"", src) is not { } pair)
                return src;
            var (inside, firstId, _) = pair;
            if (inside.Contains('>') || inside.Contains('<'))
                return src;
            var before = attrDelegate(inside);
            if (Unjail($"<{tag} {attr}=\"{inside}\">", $"</{tag}>", src) is not { } insideTextPair)
                return src;
            var (insideText, _, finalId) = insideTextPair;
            return src[..firstId] +
                before + div(insideText, $"yadg-{tag}") +
                ReplaceAttributedOpenCloseTag(attrDelegate,
                tag, attr, src[finalId..]);
        }

        private static string ReplaceAttributedCloseTag(
            Func<string, string> attrDelegate,
            string tag, string attr, string src)
        {
            if (Unjail($"<{tag} {attr}=\"", "\"/>", src) is not { } pair)
                return src;
            var (inside, firstId, lastId) = pair;
            if (inside.Contains('>') || inside.Contains('<'))
                return src[..(firstId + 2)] + ReplaceAttributedCloseTag(attrDelegate, tag, attr, src[(firstId + 2)..]); ;
            var replacement = attrDelegate(inside);
            return src[..firstId] + replacement +
                ReplaceAttributedCloseTag(attrDelegate, tag, attr, src[lastId..]);
        }

        private string FindWhiteSpace(string src, int until)
        {
            var sb = new StringBuilder();
            var len = 0;
            while (until - len >= 0 && src[until - len] == ' ')
            {
                len++;
                sb.Append(" ");
            }
            return sb.ToString();
        }

        private string CodeToPreCode(string src)
        {
            if (src.Contains("x + 8 - 4"))
                Console.WriteLine();
            var unjailed = Unjail("<code>", "</code>", src);
            if (unjailed is not { } unjailedNotNull)
                return src;
            var (inside, first, last) = unjailedNotNull;
            inside = inside.Replace("<br>", "\n");
            inside = inside.Replace("\r\n", "\n");
            inside = inside.Replace("\n" + FindWhiteSpace(src, first - 1), "\n");
            if (inside.StartsWith('\n'))
                inside = inside[1..];
            if (inside.EndsWith('\n'))
                inside = inside.Substring(0, inside.Length - 1);
            return src.Substring(0, first) + $"<pre><code>{inside}</code></pre>" + CodeToPreCode(src.Substring(last));
        }

        public string Build()
            => 
            ReplaceTag(
                p(b("Example")),
                "example",
                ReplaceTag(
                    p(b("Returns")),
                    "returns",
                    ReplaceTag(
                        p(b("Summary")),
                        "summary",
                        ReplaceAttributedOpenCloseTag(
                            tName => p(b($"Type parameter \"{tName}\"")),
                            "typeparam",
                            "name",
                            ReplaceAttributedOpenCloseTag(
                                pName => p(b($"Parameter \"{pName}\"")),
                                "param",
                                "name",
                                ReplaceAttributedCloseTag(
                                    cref => a(upPath + WebsiteBuilder.GetLinkByName(cref), NameParser.LastFold(cref)),
                                    "see",
                                    "cref",
                                    ReplaceAttributedCloseTag(
                                        href => a(href, href),
                                        "a",
                                        "href",
                                        CodeToPreCode(
                                            xml
                                            .Replace("\" />", "\"/>")
                                            // .Replace("\n", "<br>")
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );
    }
}
