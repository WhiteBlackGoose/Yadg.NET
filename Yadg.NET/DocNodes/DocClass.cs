using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YadgNet
{
    public sealed class DocClassBuilder
    {
        public string? Name { get; set; }

        public string Description { get; set; } = "";

        public SortedDictionary<string, DocMemberBuilder> Members { get; } = new();

        public DocClass Build()
            => new DocClass(Name ?? throw new NullReferenceException(), Description ?? throw new NullReferenceException(), Members.Values.Select(c => c.Build()).ToArray());

        public DocPropertyBuilder InsertProperty(string name)
        {
            if (Members.TryGetValue(name, out var res))
                return (DocPropertyBuilder)res;
            var prop = new DocPropertyBuilder();
            prop.Name = name;
            Members[name] = prop;
            return prop;
        }

        public DocMethodBuilder InsertMethod(string name)
        {
            if (Members.TryGetValue(name, out var res))
                return (DocMethodBuilder)res;
            var method = new DocMethodBuilder();
            method.Name = name;
            Members[name] = method;
            return method;
        }
    }

    public sealed record DocClass(string Name, string Description, IEnumerable<DocMember> Members);
}
