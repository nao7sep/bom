namespace yyBom
{
    public static class Validator
    {
        public static bool IsDirectoryOrFilePath (string str) => Path.IsPathFullyQualified (str);

        public static bool IsDirectoryOrFileName (string str) => str.Contains (Path.DirectorySeparatorChar) == false && str.Contains (Path.AltDirectorySeparatorChar) == false;

        public static bool IsFileExtension (string str) => str.Length >= 2 && str.StartsWith ('.');
    }
}
