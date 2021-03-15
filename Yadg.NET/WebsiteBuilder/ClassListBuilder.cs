using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YadgNet.HtmlTags;

namespace YadgNet
{
    public sealed class ClassListBuilder
    {
        public string BackToNamespacesButtonText { get; set; } = "&#8592;";
        private DocNamespace nspace;
        public ClassListBuilder(DocNamespace nspace)
            => this.nspace = nspace;

        public string Build()
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
                                new DescriptionFromXmlBuilder(cls.Description).Build()
                    )
                )
            );
    }
}
