using System;
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
        public string MainPageName { get; set; } = "My Documentation";
        public string BackToNamespacesButtonText { get; set; } = "&#8592; Back to list of namespaces";
        public string BackToClassesButtonText { get; set; } = "&#8592; Back to list of classes";

        private readonly IPageSave saver;
        public WebsiteBuilder(IPageSave saver)
            => this.saver = saver;

        public void Build(DocAssembly assembly)
        {
            saver.Save("namespaces.html", 
                new NamespaceListBuilder(assembly)
                { 
                    MainPageName = MainPageName 
                }
                .Build()
                );

            foreach (var nspace in assembly.Namespaces)
                saver.Save(nspace.Name + ".html", 
                    new ClassListBuilder(nspace)
                    {
                        BackToNamespacesButtonText = BackToNamespacesButtonText
                    }.Build()
                    );

            foreach (var nspace in assembly.Namespaces)
                foreach (var cls in nspace.Classes)
                    saver.Save($"{nspace.Name}/{cls.Name}.html", 
                        new MemberListBuilder(cls)
                        {
                            BackToClassesButtonText = BackToClassesButtonText
                        }.Build(nspace.Name + ".html")
                        );
        }
    }
}
