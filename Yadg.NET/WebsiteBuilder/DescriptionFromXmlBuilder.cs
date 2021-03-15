using System;
using static YadgNet.HtmlTags;

namespace YadgNet
{
    public sealed class DescriptionFromXmlBuilder
    {
        private readonly string xml;
        private readonly string upPath;
        public DescriptionFromXmlBuilder(string xml, string upPath)
            => (this.xml, this.upPath) = (xml, upPath);

        private static string FixLinks(string xml)
        {
            if (!xml.Contains("<a"))
                return xml;
            var id = xml.IndexOf("<a");
            var nextId = xml.IndexOf("/>", id);
            if (nextId == -1)
                return xml;
            var linkInside = xml.Substring(id + 2, nextId - id - 2);
            if (linkInside.Contains('<') || linkInside.Contains('>'))
                return xml.Substring(0, id + 2) + FixLinks(xml.Substring(id + 2));
            return xml.Substring(0, nextId) + ">Link</a>" + FixLinks(xml.Substring(nextId + 2));
        }

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

        public string Build()
            => 
            ReplaceTag(
                h4("Returns"),
                "returns",
                ReplaceTag(
                    h4("Summary"),
                    "summary",
                    ReplaceAttributedOpenCloseTag(
                        tName => h4($"Type parameter {b(tName)}"),
                        "typeparam",
                        "name",
                        ReplaceAttributedOpenCloseTag(
                            pName => h4($"Parameter {b(pName)}"),
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
                                    xml.Replace("\" />", "\"/>")
                                )
                            )
                        )
                    )
                )
            );
    }
}
