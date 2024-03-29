﻿using System;
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

        internal static string SplitWrapWordsMethod(string method)
            => method.Replace(",", ",&#8203;").Replace("(","&#8203;(").Replace(".", ".&#8203;");

        public string Build(string back)
            =>
            h2_centered(cls.Name) +

            hr() +

            p(a($"../{back}", "&#8592; Back to list of classes")) +

            h2("Description") +

            new DescriptionFromXmlBuilder(cls.Description, "../").Build() +

            h2("Members") +

            p(
                ul("yadg-member yadg-list",
                    cls.Members.Select(
                        member
                            => member switch
                            {
                                DocMethod method =>
                                    (
                                    method.Overloads.Count() == 1

                                    ?

                                    p(
                                        a($"./{cls.Name}/{method.Name}.html", h3(SplitWrapWordsMethod(method.Name + method.Overloads.First().Parameters))) + "Method"
                                    )

                                    :

                                    p(
                                        a($"./{cls.Name}/{method.Name}.html", h3(method.Name)) + "Method and its overloads"
                                    )
                                    )
                                ,
                                DocProperty property =>
                                    p(
                                        a($"./{cls.Name}/{property.Name}.html", h3(property.Name)) + "Property"
                                    ),
                                DocField field =>
                                    p(
                                        a($"./{cls.Name}/{field.Name}.html", h3(field.Name)) + "Field"
                                    ),
                                _ => throw new Exception()
                            }
                    )
                )
            );
    }
}
