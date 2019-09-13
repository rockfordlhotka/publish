using System;
using System.IO;

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

      var blogRoot = args[1];
      if (blogRoot.Substring(blogRoot.Length) == @"\")
        blogRoot = blogRoot[0..^2];
      if (!Directory.Exists(blogRoot))
      {
        Console.WriteLine($"Blog root directory does not exist at {blogRoot}");
        return;
      }
      
      var postText = GetBlogPost(args[0]);
      if (string.IsNullOrWhiteSpace(postText))
        return;

      FixImagePaths(postText, args[0], blogRoot);

      CopyPostFile(args[0], blogRoot);
      CopyImages(args[0], blogRoot);

      Console.WriteLine($"Blog post {args[0]} published to {blogRoot}");
    }

    private static string GetBlogPost(string filename)
    {
      try
      {
        return File.ReadAllText(filename);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Can not read input file - {ex.Message}");
        return string.Empty;
      }
    }

    private static void FixImagePaths(string postText, string v1, string v2)
    {
      // find all ![]() patterns
      // if () is a file (not a url) rewrite to v2/assets/v1
      // find and fixup featured-image path to v2/assets/v1
    }

    private static void CopyPostFile(string v1, string v2)
    {
      // copy to v2/posts
    }

    private static void CopyImages(string v1, string v2)
    {
      // copy to v2/assets/v1
    }
  }
}
