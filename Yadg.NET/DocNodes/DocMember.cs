using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YadgNet
{
    public abstract class DocMemberBuilder
    {
        public abstract DocMember Build();
    }

    public sealed class DocMethodBuilder : DocMemberBuilder
    {
        public string? Name { get; set; }
        public SortedDictionary<string, DocOverload> Overloads { get; } = new();
        public override DocMethod Build() => new DocMethod(Overloads.Values, Name ?? throw new NullReferenceException());
        public DocOverload InsertOverload(string pars, string description)
            => Overloads.TryGetValue(pars, out var res) ? res : Overloads[pars] = new DocOverload(pars, description);
    }

    public sealed class DocPropertyBuilder : DocMemberBuilder
    {
        public string? Name { get; set; }
        public string Description { get; set; } = "";
        public override DocProperty Build() => new DocProperty(Name ?? throw new NullReferenceException(), Description ?? throw new NullReferenceException());
    }

    public abstract record DocMember;

    public sealed record DocMethod(IEnumerable<DocOverload> Overloads, string Name) : DocMember;

    public sealed record DocOverload(string Parameters, string Description);

    public sealed record DocProperty(string Name, string Description) : DocMember;
}
