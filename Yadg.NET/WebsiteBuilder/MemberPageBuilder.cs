using System;
using System.Linq;
using static YadgNet.HtmlTags;

namespace YadgNet;

public sealed record MemberPageBuilder(INamedDocMember Member)
{
    public string IconNextToMemberInfo { get; init; } =
        "<span class=\"iconify\" data-icon=\"octicon:diamond-16\"></span>&nbsp;";
        
    public string BackToMembersButtonText { get; init; }
    public string Build(string back)
        =>
        p(a($"../../{back}", $"{BackToMembersButtonText}")) +
        
        p(Member switch
            {
                DocMethod method =>
                    (
                    method.Overloads.Count() == 1

                    ?

                    p(
                        h1(MemberListBuilder.SplitWrapWordsMethod(method.Name + method.Overloads.First().Parameters)) +
                        p($"{IconNextToMemberInfo}Method (no overloads)")
                    ) +
                    p(new DescriptionFromXmlBuilder(method.Overloads.First().Description, "../").Build())

                    :

                    p(
                        h1(method.Name) +
                        p($"{IconNextToMemberInfo}Method with {method.Overloads.Count()} overloads")
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
                    p(
                        h1(property.Name) +
                        p($"{IconNextToMemberInfo}Property")
                    )
                    +
                    p(new DescriptionFromXmlBuilder(property.Description, "../").Build()),
                DocField field =>
                    p(
                        h1(field.Name) +
                        p($"{IconNextToMemberInfo}Field")
                    )
                    +
                    p(new DescriptionFromXmlBuilder(field.Description, "../").Build()),
                _ => throw new Exception()
            } 
        );
}
