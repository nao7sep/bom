﻿using System.Text;
using yyLib;

namespace bom
{
    internal class Program
    {
        static void Main (string [] args)
        {
            try
            {
                string xIgnoredPathsFileName = "IgnoredPaths.txt",
                    xIgnoredPathsFilePath = yyAppDirectory.MapPath (xIgnoredPathsFileName);

                if (File.Exists (xIgnoredPathsFilePath) == false)
                    File.WriteAllLines (xIgnoredPathsFilePath,
                        [
                            @"DirectoryPath: C:\Directory",
                            "DirectoryName: Directory",
                            @"FilePath: C:\File.ext",
                            "FileName: File.ext",
                            "FileExtension: .ext",

                            "DirectoryNamePrefix: Prefix-",
                            "DirectoryNameSuffix: -Suffix",
                            "FileNamePrefix: Prefix-",
                            "FileNameSuffix: -Suffix"
                        ],
                        Encoding.UTF8);

                List <IgnoredPathInfo> xIgnoredPaths = [];

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

                    // Now some prefixes and suffixes may be shared by directories and files.
                    /* if (xIgnoredPaths.Any (x => x.Path!.Equals (xValue, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine ($"Duplicate value in {xIgnoredPathsFileName}: {xLine}");
                        return;
                    } */

                    if (xKey.Equals ("DirectoryPath", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFilePath (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.DirectoryPath, Path = xValue });

                    else if (xKey.Equals ("DirectoryName", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFileName (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.DirectoryName, Path = xValue });

                    else if (xKey.Equals ("FilePath", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFilePath (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.FilePath, Path = xValue });

                    else if (xKey.Equals ("FileName", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFileName (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.FileName, Path = xValue });

                    else if (xKey.Equals ("FileExtension", StringComparison.OrdinalIgnoreCase) && Validator.IsFileExtension (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.FileExtension, Path = xValue });

                    else if (xKey.Equals ("DirectoryNamePrefix", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFileName (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.DirectoryNamePrefix, Path = xValue });

                    else if (xKey.Equals ("DirectoryNameSuffix", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFileName (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.DirectoryNameSuffix, Path = xValue });

                    else if (xKey.Equals ("FileNamePrefix", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFileName (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.FileNamePrefix, Path = xValue });

                    else if (xKey.Equals ("FileNameSuffix", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFileName (xValue))
                        xIgnoredPaths.Add (new IgnoredPathInfo { PathType = PathType.FileNameSuffix, Path = xValue });

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
                    xSpecifiedEncodingsFilePath = yyAppDirectory.MapPath (xSpecifiedEncodingsFileName);

                if (File.Exists (xSpecifiedEncodingsFilePath) == false)
                    File.WriteAllLines (xSpecifiedEncodingsFilePath,
                        [
                            @"FilePath: C:\File.ext | ASCII",
                            "FileName: File.ext | ASCII",
                            "FileExtension: .ext | shift_jis"
                        ],
                        Encoding.UTF8);

                List <SpecifiedEncodingInfo> xSpecifiedEncodings = [];

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

                    // Still an effective check.
                    if (xSpecifiedEncodings.Any (x => x.Path!.Equals (xValue, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine ($"Duplicate value in {xSpecifiedEncodingsFileName}: {xLine}");
                        return;
                    }

                    try
                    {
                        if (xKey.Equals ("FilePath", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFilePath (xValue))
                        {
                            xSpecifiedEncodings.Add (new SpecifiedEncodingInfo { PathType = PathType.FilePath, Path = xValue, EncodingName = xEncodingName, Encoding = Encoding.GetEncoding (xEncodingName) });
                            continue;
                        }

                        if (xKey.Equals ("FileName", StringComparison.OrdinalIgnoreCase) && Validator.IsDirectoryOrFileName (xValue))
                        {
                            xSpecifiedEncodings.Add (new SpecifiedEncodingInfo { PathType = PathType.FileName, Path = xValue, EncodingName = xEncodingName, Encoding = Encoding.GetEncoding (xEncodingName) });
                            continue;
                        }

                        if (xKey.Equals ("FileExtension", StringComparison.OrdinalIgnoreCase) && Validator.IsFileExtension (xValue))
                        {
                            xSpecifiedEncodings.Add (new SpecifiedEncodingInfo { PathType = PathType.FileExtension, Path = xValue, EncodingName = xEncodingName, Encoding = Encoding.GetEncoding (xEncodingName) });
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

                // -----------------------------------------------------------------------------

                void LoadFile (FileInfo file, List <DetectedPathInfo> detectedPaths)
                {
                    // We dont need to load the same file twice.
                    if (detectedPaths.Any (x => x.PathType == PathType.FilePath && x.Path!.Equals (file.FullName, StringComparison.OrdinalIgnoreCase)))
                        return;

                    // Could be one line of code, but I dont see much reason to modify what's already working. :$

                    if (xIgnoredPaths.Any (x => x.PathType == PathType.FilePath && x.Path!.Equals (file.FullName, StringComparison.OrdinalIgnoreCase)))
                    {
                        detectedPaths.Add (new DetectedPathInfo { PathType = PathType.FilePath, Path = file.FullName, IsIgnored = true });
                        return;
                    }

                    if (xIgnoredPaths.Any (x => x.PathType == PathType.FileName && x.Path!.Equals (file.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        detectedPaths.Add (new DetectedPathInfo { PathType = PathType.FilePath, Path = file.FullName, IsIgnored = true });
                        return;
                    }

                    if (xIgnoredPaths.Any (x => x.PathType == PathType.FileExtension && x.Path!.Equals (file.Extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        detectedPaths.Add (new DetectedPathInfo { PathType = PathType.FilePath, Path = file.FullName, IsIgnored = true });
                        return;
                    }

                    if (xIgnoredPaths.Any (x => x.PathType == PathType.FileNamePrefix && file.Name.StartsWith (x.Path!, StringComparison.OrdinalIgnoreCase)))
                    {
                        detectedPaths.Add (new DetectedPathInfo { PathType = PathType.FilePath, Path = file.FullName, IsIgnored = true });
                        return;
                    }

                    if (xIgnoredPaths.Any (x => x.PathType == PathType.FileNameSuffix && file.Name.EndsWith (x.Path!, StringComparison.OrdinalIgnoreCase)))
                    {
                        detectedPaths.Add (new DetectedPathInfo { PathType = PathType.FilePath, Path = file.FullName, IsIgnored = true });
                        return;
                    }

                    DetectedPathInfo xDetectedPath = new () { PathType = PathType.FilePath, Path = file.FullName, IsIgnored = false };

                    try
                    {
                        byte [] xBytes = new byte [3];
                        int xReadByteCount;

                        using FileStream xFileStream = file.OpenRead ();
                        xReadByteCount = xFileStream.Read (xBytes, 0, 3);

                        if (xReadByteCount == 3 && xBytes [0] == 0xEF && xBytes [1] == 0xBB && xBytes [2] == 0xBF)
                        {
                            xDetectedPath.StartsWithUtf8Bom = true;
                            xDetectedPath.DetectedOrSpecifiedEncoding = Encoding.UTF8;
                        }

                        else
                        {
                            xDetectedPath.StartsWithUtf8Bom = false;

                            xDetectedPath.DetectedOrSpecifiedEncoding = xSpecifiedEncodings.FirstOrDefault (x =>
                            {
                                // I choose not to expect a case where a certain extension is associated with a certain encoding
                                //     AND a certain file with the same extension must be re-associated with a different encoding with its full path.

                                return (x.PathType == PathType.FilePath && x.Path!.Equals (file.FullName, StringComparison.OrdinalIgnoreCase)) ||
                                    (x.PathType == PathType.FileName && x.Path!.Equals (file.Name, StringComparison.OrdinalIgnoreCase)) ||
                                    (x.PathType == PathType.FileExtension && x.Path!.Equals (file.Extension, StringComparison.OrdinalIgnoreCase));
                            })?.
                            Encoding;
                        }

                        detectedPaths.Add (xDetectedPath);
                    }

                    catch
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine ("Failed to load file: " + file.FullName);
                        Console.ResetColor ();
                    }
                }

                const string xSpaces = "                                                                                "; // 80 spaces.

                void LoadDirectory (DirectoryInfo directory, List <DetectedPathInfo> detectedPaths)
                {
                    try
                    {
                        // Not loading the same directory twice.
                        if (detectedPaths.Any (x => x.PathType == PathType.DirectoryPath && x.Path!.Equals (directory.FullName, StringComparison.OrdinalIgnoreCase)))
                            return;

                        if (xIgnoredPaths.Any (x => x.PathType == PathType.DirectoryPath && x.Path!.Equals (directory.FullName, StringComparison.OrdinalIgnoreCase)))
                        {
                            detectedPaths.Add (new DetectedPathInfo { PathType = PathType.DirectoryPath, Path = directory.FullName, IsIgnored = true });
                            return;
                        }

                        if (xIgnoredPaths.Any (x => x.PathType == PathType.DirectoryName && x.Path!.Equals (directory.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            detectedPaths.Add (new DetectedPathInfo { PathType = PathType.DirectoryPath, Path = directory.FullName, IsIgnored = true });
                            return;
                        }

                        if (xIgnoredPaths.Any (x => x.PathType == PathType.DirectoryNamePrefix && directory.Name.StartsWith (x.Path!, StringComparison.OrdinalIgnoreCase)))
                        {
                            detectedPaths.Add (new DetectedPathInfo { PathType = PathType.DirectoryPath, Path = directory.FullName, IsIgnored = true });
                            return;
                        }

                        if (xIgnoredPaths.Any (x => x.PathType == PathType.DirectoryNameSuffix && directory.Name.EndsWith (x.Path!, StringComparison.OrdinalIgnoreCase)))
                        {
                            detectedPaths.Add (new DetectedPathInfo { PathType = PathType.DirectoryPath, Path = directory.FullName, IsIgnored = true });
                            return;
                        }

                        string xFirstPart = "Loading directory: " + directory.Name;
                        Console.Write ($"{xFirstPart}{xSpaces.AsSpan (xFirstPart.Length)}\r");

                        foreach (DirectoryInfo xSubdirectory in directory.GetDirectories ())
                            LoadDirectory (xSubdirectory, detectedPaths);

                        foreach (FileInfo xFile in directory.GetFiles ())
                            LoadFile (xFile, detectedPaths);

                        detectedPaths.Add (new DetectedPathInfo { PathType = PathType.DirectoryPath, Path = directory.FullName, IsIgnored = false });
                    }

                    catch
                    {
                        // Displays an error message and continues.

                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine ("Failed to load directory: " + directory.FullName);
                        Console.ResetColor ();
                    }
                }

                List <DetectedPathInfo> xDetectedPaths = [];

                foreach (string xPath in xPaths.Select (x => x.Path))
                {
                    if (Directory.Exists (xPath))
                        LoadDirectory (new DirectoryInfo (xPath), xDetectedPaths);

                    else // if (File.Exists (xPath)) // Either a directory or a file exists.
                        LoadFile (new FileInfo (xPath), xDetectedPaths);
                }

                string xFirstPart = "Loaded all directories and files.";
                Console.WriteLine ($"{xFirstPart}{xSpaces.AsSpan (xFirstPart.Length)}");

                var xOrderedDetectedPaths = xDetectedPaths.OrderBy (x => x.Path, StringComparer.OrdinalIgnoreCase).ToList ();

                // -----------------------------------------------------------------------------

                StringBuilder xOutput = new ();

                var xIgnoredDetectedDirectoryPaths = xOrderedDetectedPaths.Where (x => x.PathType == PathType.DirectoryPath && x.IsIgnored == true).ToList ();

                if (xIgnoredDetectedDirectoryPaths.Count > 0)
                {
                    xOutput.AppendLine ("[Ignored Directories]");
                    xOutput.AppendLine (string.Join (Environment.NewLine, xIgnoredDetectedDirectoryPaths.Select (x => x.Path)));
                }

                var xIgnoredDetectedFilePaths = xOrderedDetectedPaths.Where (x => x.PathType == PathType.FilePath && x.IsIgnored == true).ToList ();

                if (xIgnoredDetectedFilePaths.Count > 0)
                {
                    if (xOutput.Length > 0)
                        xOutput.AppendLine ();

                    xOutput.AppendLine ("[Ignored Files]");
                    xOutput.AppendLine (string.Join (Environment.NewLine, xIgnoredDetectedFilePaths.Select (x => x.Path)));
                }

                var xDetectedDirectoryPaths = xOrderedDetectedPaths.Where (x => x.PathType == PathType.DirectoryPath && x.IsIgnored == false).ToList ();

                if (xDetectedDirectoryPaths.Count > 0)
                {
                    if (xOutput.Length > 0)
                        xOutput.AppendLine ();

                    xOutput.AppendLine ("[Detected Directories]");
                    xOutput.AppendLine (string.Join (Environment.NewLine, xDetectedDirectoryPaths.Select (x => x.Path)));
                }

                var xDetectedFilePaths = xOrderedDetectedPaths.Where (x => x.PathType == PathType.FilePath && x.IsIgnored == false).ToList ();

                if (xDetectedFilePaths.Count > 0)
                {
                    if (xOutput.Length > 0)
                        xOutput.AppendLine ();

                    xOutput.AppendLine ("[Detected Files]");

                    xOutput.AppendLine (string.Join (Environment.NewLine, xDetectedFilePaths.Select (x =>
                        $"{x.Path}, UTF-8 BOM: {x.StartsWithUtf8Bom}, Encoding: {(x.DetectedOrSpecifiedEncoding != null ? x.DetectedOrSpecifiedEncoding.EncodingName : yyString.GetVisibleString (null))}")));
                }

                string xOutputFilePath = Path.Join (yySpecialDirectories.Desktop, $"bom-{yyFormatter.ToRoundtripFileNameString (DateTime.UtcNow)}.txt");
                File.WriteAllText (xOutputFilePath, xOutput.ToString (), Encoding.UTF8);

                // -----------------------------------------------------------------------------

                // I've tried a few different implementations.
                // Simply showing the file paths that require attention is convenient for me.

                var xPathsToBeChecked = xDetectedFilePaths.Where (x => x.StartsWithUtf8Bom == false && x.DetectedOrSpecifiedEncoding == null).
                    Select (y => y.Path).
                    ToList ();

                if (xPathsToBeChecked.Count > 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine (string.Join (Environment.NewLine, xPathsToBeChecked));
                    Console.ResetColor ();
                }

                else Console.WriteLine ("Everything is fine.");
            }

            catch (Exception xException)
            {
                yyLogger.Default.TryWriteException (xException);
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
