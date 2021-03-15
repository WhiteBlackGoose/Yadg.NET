using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YadgNet
{
    public sealed class PageSaver : IPageSave
    {
        private string rootPath;
        public PageSaver(string path)
            => this.rootPath = path;
        public void Save(string path, string text)
        {
            var finalPath = Path.Combine(rootPath, path) ?? throw new NullReferenceException();
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath) ?? throw new NullReferenceException());
            File.WriteAllText(finalPath, text);
            Console.WriteLine($"Writing to {finalPath}");
        }
    }
}
