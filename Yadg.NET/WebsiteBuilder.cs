﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YadgNet
{
    public interface IPageSave
    {
        public void Save(string path, string text);
    }

    public sealed class WebsiteBuilder
    {
        private readonly IPageSave saver;
        public WebsiteBuilder(IPageSave saver)
            => this.saver = saver;

        public void Build(DocAssembly assembly)
        {
            saver.Save("namespaces.html", BuildNamespaceList(assembly));
            foreach (var nspace in assembly.Namespaces)
                saver.Save(nspace.Name + ".html", BuildClassList(nspace));

            foreach (var nspace in assembly.Namespaces)
                foreach (var cls in nspace.Classes)
                    saver.Save($"{nspace.Name}/{cls.Name}.html", BuildMemberList(cls, nspace.Name + ".html"));
        }

        private static string BuildNamespaceList(DocAssembly assembly)
            =>
            h2_centered(assembly.Name) +

            hr() +

            p("Here we describe all the namespaces and outer classes within the assembly") +

            p(
                ul(
                    assembly.Namespaces.Select(
                        nspace => a(nspace.Name + ".html", nspace.Name)
                        )
                )
            );

        private static string BuildClassList(DocNamespace nspace)
            =>
            h2_centered(nspace.Name) +

            hr() +

            p(a("namespaces.html", "&#8592; Back to list of namespaces")) +

            p($"Classes within the {nspace.Name} namespace") +

            p(
                ul(
                    nspace.Classes.Select(
                        cls
                            => 
                                (cls.Members.Count() > 0
                                ?
                                a($"{nspace.Name}/{cls.Name}.html", cls.Name)
                                :
                                cls.Name
                                ) + 
                                DescriptionFromXml(cls.Description)
                    )
                )
            );

        private static string BuildMemberList(DocClass cls, string back)
            =>
            h2_centered(cls.Name) +

            hr() +

            p(a($"../{back}", "&#8592; Back to list of classes")) +

            h3("Description") +

            DescriptionFromXml(cls.Description) +

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
                                    p(DescriptionFromXml(method.Overloads.First().Description))

                                    :

                                    p(
                                        $"Method {method.Name} and its overloads"
                                    ) +

                                    ul(
                                        method.Overloads.Select(
                                            overload =>
                                                p($"{method.Name}{overload.Parameters}") +
                                                p(DescriptionFromXml(overload.Description))
                                        )
                                    )

                                ,
                                DocProperty property =>
                                    p(
                                        $"Property {property.Name}"
                                    ) +
                                    p(DescriptionFromXml(property.Description)),
                                _ => throw new Exception()
                            }
                    )
                )
            );

        private static string tag(string text, string tag)
            => $"<{tag}>{text}</{tag}>";

        private static string tagClass(string text, string tag, string @class)
            => $"<{tag} class=\"{@class}\">{text}</{tag}>";

        private static string p(string text)
            => tag(text, "p");
        private static string ul(IEnumerable<string> points)
            => tag(string.Join("", points.Select(c => li(c))), "ul");
        private static string li(string text)
            => tag(text, "li");
        private static string h1(string text)
            => tag(text, "h1");
        private static string h2(string text)
            => tag(text, "h2");
        private static string h2_centered(string text)
            => tagClass(text, "h2", "centered");
        private static string h3(string text)
            => tag(text, "h3");
        private static string hr() => "<hr/>";
        private static string a(string href, string text)
            => $"<a href=\"{href}\">{text}</a>";


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
                return xml;
            return xml.Substring(0, nextId) + ">Link</a>" + FixLinks(xml.Substring(nextId + 2));
        }
        private static string DescriptionFromXml(string xml)
        {
            xml = FixLinks(xml);
            return xml;
        }
    }
}
