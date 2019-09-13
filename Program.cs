using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace publish
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("Usage: publish source.md blogRootDirectory");
        return;
      }

      var blogName = GetBlogName(args[0]);
      var blogPath = GetBlogPath(args[0]);

      var blogRoot = args[1];
      if (blogRoot.Substring(blogRoot.Length) == @"\")
        blogRoot = blogRoot[0..^2];
      if (!Directory.Exists(blogRoot))
      {
        Console.WriteLine($"ERROR: Blog root directory does not exist at {blogRoot}");
        return;
      }
      
      var postText = GetBlogPost(args[0]);
      if (string.IsNullOrWhiteSpace(postText))
        return;

      var newPost = FixImagePaths(postText, blogName, blogRoot);

      WritePostFile(newPost, blogName, blogRoot);
      CopyImages(blogName, blogPath, blogRoot);

      Console.WriteLine($"Blog post {args[0]} published to {blogRoot} as {blogName}");
    }

    private static string GetBlogPath(string v)
    {
      var blogPath = v;
      if (blogPath.Contains("\\"))
      {
        var parts = blogPath.Split('\\');
        blogPath = blogPath.Replace(parts[parts.Length - 1], "");
      }
      else
      {
        blogPath = @".\";
      }
      return blogPath;
    }

    private static string GetBlogName(string v)
    {
      var blogName = v;
      if (blogName.Contains("\\"))
      {
        var parts = blogName.Split('\\');
        blogName = parts[parts.Length - 1];
      }
      blogName = blogName.Replace(" ", "-");
      blogName = $"{DateTime.Now.ToString("yyyy-MM-dd")}-{blogName}";
      return blogName;
    }

    private static string GetBlogPost(string filename)
    {
      try
      {
        return File.ReadAllText(filename);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"ERROR: Can not read input file - {ex.Message}");
        return string.Empty;
      }
    }

    private static string FixImagePaths(string postText, string v1, string v2)
    {
      var pattern = @"\!\[(?:.*)\]\((?:.*)\)";
      v1 = v1.Replace(".md", "");
      var newPath = $"{{{{ site.url }}}}/assets/{v1}/";
      var match = Regex.Match(postText, pattern, RegexOptions.Multiline);
      while (match.Success)
      {
        if (!match.ToString().Contains("://"))
        {
          var original = match.ToString();
          var newText = original.Replace("](", $"]({newPath}");
          postText = postText.Replace(original, newText);
        }
        match = match.NextMatch();
      }
      if (!postText.Contains("featured-image: https"))
        postText = postText.Replace("featured-image: ", $"featured-image: {{{{ site.url }}}}/assets/{v1}/");
      return postText;
    }

    private static void WritePostFile(string postText, string v1, string v2)
    {
      File.WriteAllText($"{v2}\\_posts\\{v1}", postText);
    }

    private static void CopyImages(string blogName, string sourcePath, string blogRoot)
    {
      blogRoot = blogRoot + @"\assets\" + blogName.Replace(".md", "") + @"\";
      if (!Directory.Exists(blogRoot))
        Directory.CreateDirectory(blogRoot);

      var images = new List<string>();
      images.AddRange(Directory.GetFiles(sourcePath, "*.png"));
      images.AddRange(Directory.GetFiles(sourcePath, "*.jpg"));
      images.AddRange(Directory.GetFiles(sourcePath, "*.gif"));
      foreach (var item in images)
      {
        var fileName = item;
        if (fileName.Contains("\\"))
        {
          var parts = fileName.Split('\\');
          fileName = parts[^1];
        }
        File.Copy(item, blogRoot + fileName, true);
      }
    }
  }
}
