using System;
using System.Linq;
using static YadgNet.HtmlTags;

namespace YadgNet;

public sealed record MemberPageBuilder(INamedDocMember Member)
{
    public string BackToMembersButtonText { get; init; }
    public string Build(string back)
        =>
        p(a($"../../{back}", $"{BackToMembersButtonText}")) +
        
        p(Member switch
            {
                DocMethod method =>
                    $"Method {h1(method.Name)}" +
                    (
                    method.Overloads.Count() == 1

                    ?

                    p(
                        $"{h3(MemberListBuilder.SplitWrapWordsMethod(method.Name + method.Overloads.First().Parameters))} Method"
                    ) +
                    p(new DescriptionFromXmlBuilder(method.Overloads.First().Description, "../").Build())

                    :

                    p(
                        $"Method {h1(method.Name)} and its overloads"
                    ) +

                    ul("yadg-list-2",
                        method.Overloads.Select(
                            overload =>
                                h3(MemberListBuilder.SplitWrapWordsMethod(method.Name + overload.Parameters)) +
                                p(new DescriptionFromXmlBuilder(overload.Description, "../").Build())
                        )
                    )
                    )
                ,
                DocProperty property =>
                    $"<a name='{property.Name}'></a>" +
                    p(
                        $"{h3(property.Name)} Property"
                    ) +
                    p(new DescriptionFromXmlBuilder(property.Description, "../").Build()),
                DocField field =>
                    $"<a name='{field.Name}'></a>" +
                    p(
                        $"{h3(field.Name)} Field"
                    ) +
                    p(new DescriptionFromXmlBuilder(field.Description, "../").Build()),
                _ => throw new Exception()
            } 
        );
}
