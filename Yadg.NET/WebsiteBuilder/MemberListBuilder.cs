using System;
using System.Linq;
using static YadgNet.HtmlTags;


namespace YadgNet
{
    public sealed class MemberListBuilder
    {
        public string BackToClassesButtonText { get; set; } = "&#8592; Back to list of classes";
        private readonly DocClass cls;
        public MemberListBuilder(DocClass cls)
            => this.cls = cls;

        public string Build(string back)
            =>
            h2_centered(cls.Name) +

            hr() +

            p(a($"../{back}", "&#8592; Back to list of classes")) +

            h3("Description") +

            new DescriptionFromXmlBuilder(cls.Description).Build() +

            h3("Members") +

            p(
                ul(
                    cls.Members.Select(
                        member
                            => member switch
                            {
                                DocMethod method =>
                                    method.Overloads.Count() == 1

                                    ?

                                    p(
                                        $"Method {method.Name + method.Overloads.First().Parameters}"
                                    ) +
                                    p(new DescriptionFromXmlBuilder(method.Overloads.First().Description).Build())

                                    :

                                    p(
                                        $"Method {method.Name} and its overloads"
                                    ) +

                                    ul(
                                        method.Overloads.Select(
                                            overload =>
                                                p($"{method.Name}{overload.Parameters}") +
                                                p(new DescriptionFromXmlBuilder(overload.Description).Build())
                                        )
                                    )

                                ,
                                DocProperty property =>
                                    p(
                                        $"Property {property.Name}"
                                    ) +
                                    p(new DescriptionFromXmlBuilder(property.Description).Build()),
                                _ => throw new Exception()
                            }
                    )
                )
            );
    }
}
