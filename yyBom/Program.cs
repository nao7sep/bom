using System.Text;
using yyLib;

namespace yyBom
{
    internal class Program
    {
        static void Main (string [] args)
        {
            try
            {
                string xIgnoredPathsFileName = "IgnoredPaths.txt",
                    xIgnoredPathsFilePath = yyApplicationDirectory.MapPath (xIgnoredPathsFileName);

                if (File.Exists (xIgnoredPathsFilePath) == false)
                    File.WriteAllLines (xIgnoredPathsFilePath,
                        [
                            @"Directory: C:\Directory",
                            @"File: C:\File.ext",
                            "Extension: .ext"
                        ],
                        Encoding.UTF8);

                List <IgnoredPathModel> xIgnoredPaths = new ();

                foreach (string xLine in File.ReadAllLines (xIgnoredPathsFilePath, Encoding.UTF8).
                    Where (x => string.IsNullOrWhiteSpace (x) == false && x.TrimStart ().StartsWith ("//", StringComparison.Ordinal) == false)) // Inefficient, but it's usually a small file.
                {
                    if (xLine.Contains (':', StringComparison.Ordinal) == false)
                    {
                        Console.WriteLine ($"Invalid line in {xIgnoredPathsFileName}: {xLine}");
                        return;
                    }

                    string xKey = xLine.Substring (0, xLine.IndexOf (':', StringComparison.Ordinal)).Trim (),
                        xValue = xLine.Substring (xLine.IndexOf (':', StringComparison.Ordinal) + 1).Trim ();

                    if (xIgnoredPaths.Any (x => x.Path!.Equals (xValue, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine ($"Duplicate value in {xIgnoredPathsFileName}: {xLine}");
                        return;
                    }

                    if (xKey.Equals ("Directory", StringComparison.OrdinalIgnoreCase) && Path.IsPathFullyQualified (xValue))
                        xIgnoredPaths.Add (new IgnoredPathModel { PathType = PathType.Directory, Path = xValue });

                    else if (xKey.Equals ("File", StringComparison.OrdinalIgnoreCase) && Path.IsPathFullyQualified (xValue))
                        xIgnoredPaths.Add (new IgnoredPathModel { PathType = PathType.File, Path = xValue });

                    else if (xKey.Equals ("Extension", StringComparison.OrdinalIgnoreCase) && xValue.StartsWith ('.') && xValue.Length >= 2) // Minimal validation.
                        xIgnoredPaths.Add (new IgnoredPathModel { PathType = PathType.Extension, Path = xValue });

                    else
                    {
                        Console.WriteLine ($"Invalid line in {xIgnoredPathsFileName}: {xLine}");
                        return;
                    }
                }

                // -----------------------------------------------------------------------------

                // To use encodings like shift_jis.
                Encoding.RegisterProvider (CodePagesEncodingProvider.Instance);

                // Somebody is too lazy to optimize the following code.

                string xSpecifiedEncodingsFileName = "SpecifiedEncodings.txt",
                    xSpecifiedEncodingsFilePath = yyApplicationDirectory.MapPath (xSpecifiedEncodingsFileName);

                if (File.Exists (xSpecifiedEncodingsFilePath) == false)
                    File.WriteAllLines (xSpecifiedEncodingsFilePath,
                        [
                            @"File: C:\File.ext | ASCII",
                            "Extension: .ext | shift_jis"
                        ],
                        Encoding.UTF8);

                List <SpecifiedEncodingModel> xSpecifiedEncodings = new ();

                foreach (string xLine in File.ReadAllLines (xSpecifiedEncodingsFilePath, Encoding.UTF8).
                    Where (x => string.IsNullOrWhiteSpace (x) == false && x.TrimStart ().StartsWith ("//", StringComparison.Ordinal) == false))
                {
                    int xColonIndex = xLine.IndexOf (':', StringComparison.Ordinal),
                        xPipeIndex = xLine.IndexOf ('|', StringComparison.Ordinal);

                    if (xColonIndex < 0 || xPipeIndex < 0 || xColonIndex > xPipeIndex)
                    {
                        Console.WriteLine ($"Invalid line in {xSpecifiedEncodingsFileName}: {xLine}");
                        return;
                    }

                    string xKey = xLine.Substring (0, xColonIndex).Trim (),
                        xValue = xLine.Substring (xColonIndex + 1, xPipeIndex - xColonIndex - 1).Trim (),
                        xEncodingName = xLine.Substring (xPipeIndex + 1).Trim ();

                    if (xSpecifiedEncodings.Any (x => x.Path!.Equals (xValue, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine ($"Duplicate value in {xSpecifiedEncodingsFileName}: {xLine}");
                        return;
                    }

                    try
                    {
                        if (xKey.Equals ("File", StringComparison.OrdinalIgnoreCase) && Path.IsPathFullyQualified (xValue))
                        {
                            xSpecifiedEncodings.Add (new SpecifiedEncodingModel { PathType = PathType.File, Path = xValue, EncodingName = xEncodingName, Encoding = Encoding.GetEncoding (xEncodingName) });
                            continue;
                        }

                        if (xKey.Equals ("Extension", StringComparison.OrdinalIgnoreCase) && xValue.StartsWith ('.') && xValue.Length >= 2)
                        {
                            xSpecifiedEncodings.Add (new SpecifiedEncodingModel { PathType = PathType.Extension, Path = xValue, EncodingName = xEncodingName, Encoding = Encoding.GetEncoding (xEncodingName) });
                            continue;
                        }
                    }

                    catch
                    {
                        // Most likely, the encoding name is invalid.
                    }

                    Console.WriteLine ($"Invalid line in {xSpecifiedEncodingsFileName}: {xLine}");
                    return;
                }

                // -----------------------------------------------------------------------------

                if (args.Length == 0)
                {
                    Console.WriteLine ("Usage: Drag and drop the directories and files to be checked onto the executable file.");
                    return;
                }

                List <(string Path, bool IsValid, bool Exists)> xPaths = args.Select (x => (x, Path.IsPathFullyQualified (x), Directory.Exists (x) || File.Exists (x))).ToList ();

                if (xPaths.Any (x => x.IsValid == false))
                {
                    Console.WriteLine ($"The following paths are invalid:{Environment.NewLine}{string.Join (Environment.NewLine, xPaths.Where (x => x.IsValid == false).Select (x => $"    {x.Path}"))}");
                    return;
                }

                if (xPaths.Any (x => x.Exists == false))
                {
                    Console.WriteLine ($"The following paths do not exist:{Environment.NewLine}{string.Join (Environment.NewLine, xPaths.Where (x => x.Exists == false).Select (x => $"    {x.Path}"))}");
                    return;
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException.ToString ().TrimEnd ());
            }

            finally
            {
                Console.Write ("Press any key to close this window: ");
                Console.ReadKey (true);
                Console.WriteLine ();
            }
        }
    }
}
