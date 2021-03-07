#r "nuget: FSharp.Data,4.0.1"
(**
# HTML Parser

[View this documentation in a .NET Interactive notebook ![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/fsprojects/FSharp.Data/gh-pages?filepath=library/HtmlParser.ipynb)

This article demonstrates how to use the HTML Parser to parse HTML files.

The HTML parser takes any fragment of HTML, uri or a stream and trys to parse it into a DOM. 
The parser is based on the [HTML Living Standard](http://www.whatwg.org/specs/web-apps/current-work/multipage/index.html#contents)
Once a document/fragment has been parsed, a set of extension methods over the HTML DOM elements allow you to extract information from a web page
independently of the actual HTML Type provider.
*)
open FSharp.Data
(**
The following example uses Google to search for `FSharp.Data` then parses the first set of
search results from the page, extracting the URL and Title of the link.

To achieve this we must first parse the webpage into our DOM. We can do this using
the `HtmlDocument.Load` method. This method will take a URL and make a synchronous web call
to extract the data from the page. Note: an asynchronous variant `HtmlDocument.AsyncLoad` is also available  
*)
let results = HtmlDocument.Load("http://www.google.co.uk/search?q=FSharp.Data")
(**
Now that we have a loaded HTML document we can begin to extract data from it. 
Firstly we want to extract all of the anchor tags `a` out of the document, then
inspect the links to see if it has a `href` attribute. If it does, extract the value,
which in this case is the url that the search result is pointing to, and additionally the 
`InnerText` of the anchor tag to provide the name of the web page for the search result
we are looking at. 
*)
let links = 
    results.Descendants ["a"]
    |> Seq.choose (fun x -> 
           x.TryGetAttribute("href")
           |> Option.map (fun a -> x.InnerText(), a.Value())
    )
    |> Seq.toList
(**
Now that we have extracted our search results you will notice that there are lots of
other links to various Google services and cached/similar results. Ideally we would 
like to filter these results as we are probably not interested in them.
At this point we simply have a sequence of Tuples, so F# makes this trivial using `Seq.filter`
and `Seq.map`.
*)
let searchResults =
    links
    |> List.filter (fun (name, url) -> 
                    name <> "Cached" && name <> "Similar" && url.StartsWith("/url?"))
    |> List.map (fun (name, url) -> name, url.Substring(0, url.IndexOf("&sa=")).Replace("/url?q=", ""))
(**
Putting this all together yields the following:
```
[("fsprojects/FSharp.Data: F# Data: Library for Data Access - GitHubgithub.com › fsharp › FSharp",
  "https://github.com/fsharp/FSharp.Data");
 ("FSharp.Data: Data Access Made Simplefsprojects.github.io › FSharp",
  "http://fsprojects.github.io/FSharp.Data/");
 ("FSharp.Data.TypeProvidersfsprojects.github.io › FSharp.Data.TypeProviders",
  "https://fsprojects.github.io/FSharp.Data.TypeProviders/");
 ("FSharp.Data 4.0.1 - NuGet Gallerywww.nuget.org › packages › FSharp",
  "https://www.nuget.org/packages/FSharp.Data");
 ("Guide - Data Access | The F# Software Foundationfsharp.org › guides › data-access",
  "https://fsharp.org/guides/data-access/");
 ("Guide - Data Science | The F# Software Foundationfsharp.org › guides › data-science",
  "https://fsharp.org/guides/data-science/");
 ("Working with data frames in F# - FsLabfslab.org › Deedle › frame",
  "https://fslab.org/Deedle/frame.html");
 ("Using F# for rapid data analysis | Compositional ITwww.compositional-it.com › news-blog › using-f-for-rapid-data-analysis",
  "https://www.compositional-it.com/news-blog/using-f-for-rapid-data-analysis/");
 ("Type Providers - F# | Microsoft Docsdocs.microsoft.com › Docs › .NET › F# guide",
  "https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/");
 ("How to read in csv with FSharp.Data - Stack Overflowstackoverflow.com › questions › how-to-read-in-csv-with-fsharp-data",
  "https://stackoverflow.com/questions/63759765/how-to-read-in-csv-with-fsharp-data");
 ("Learn more",
  "http://support.google.com/websearch%3Fp%3Dws_settings_location%26hl%3Den");
 ("Sign in",
  "https://accounts.google.com/ServiceLogin%3Fcontinue%3Dhttp://www.google.co.uk/search%253Fq%253DFSharp.Data%26hl%3Den")]
```

*)

