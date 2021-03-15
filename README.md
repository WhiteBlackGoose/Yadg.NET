## Yadg.NET

Yet Another Documentation Generator for .NET. Unlike others, it parses the output folder of a given project. Also,
it's not a program, it's a library (but easily can be used as a program).

## How to use

Set the output file.
<img src="img.png">

Then, here's an example of the first step:

```cs
var parsedBuilder = DocsParser.Parse(@"some_path\bin\Release\netstandard2.0\AngouriMath.xml")
```

With this builder you can change whatever you want right there. Once you've done all you wanted, build it to 
an immutable documentation tree:

```cs
var built = parsedBuilder.Build();
```

Alright, now build the pages

```cs
new WebsiteBuilder(new PageSaver()).Build(built);
```

It does NOT build a ready-to-use website. Make sure to go over those files and wrap them with your header
and footer. At least, 100% customizable.