using OutputDocsParser;
using System;
using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Xml;

var parsed = DocsParser.Parse(@"D:\main\vs_prj\AngouriMath\AngouriMath\Sources\AngouriMath\bin\Release\netstandard2.0\AngouriMath.xml");

parsed.Name = "AngouriMath Almanac";

var built = parsed.Build();

new WebsiteBuilder(new PageSaver()).Build(built);

public sealed class PageSaver : IPageSave
{
    public void Save(string path, string text)
    {
        var finalPath = Path.Combine(@"D:\main\vs_prj\AngouriMath\tests\MyDocsTestDest", path) ?? throw new NullReferenceException();
        // var finalPath = Path.Combine(@"D:\main\vs_prj\AngouriMath\AngouriMathSite\_generator\content\docs", path);
        Directory.CreateDirectory(Path.GetDirectoryName(finalPath) ?? throw new NullReferenceException());
        File.WriteAllText(finalPath, text);
        Console.WriteLine($"Writing to {finalPath}");
    }
}