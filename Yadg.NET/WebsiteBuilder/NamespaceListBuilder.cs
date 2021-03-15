using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YadgNet.HtmlTags;

namespace YadgNet
{
    public sealed class NamespaceListBuilder
    {
        public string MainPageName { get; set; } = "";
        private readonly DocAssembly assembly;
        public NamespaceListBuilder(DocAssembly assembly)
            => this.assembly = assembly;

        public string Build()
            =>
            h2_centered(MainPageName) +

            hr() +

            p(
                ul(
                    assembly.Namespaces.Select(
                        nspace => a(nspace.Name + ".html", nspace.Name)
                        )
                )
            );
    }
}
