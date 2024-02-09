using System.Text;

namespace yyBom
{
    public class DetectedPathModel
    {
        public PathType? PathType { get; set; }

        public string? Path { get; set; }

        public bool? IsIgnored { get; set; }

        // In the open source world, it's safe to limit the encoding to UTF-8 only.
        // Code is mostly written in ASCII.
        // No need to double the file sizes using UTF-16.
        public bool? StartsWithUtf8Bom { get; set; }

        /// <summary>
        /// Not null ONLY if the UTF-8 BOM is detected OR the encoding is specified by the SpecifiedEncodings.txt file.
        /// </summary>
        public Encoding? DetectedOrSpecifiedEncoding { get; set; }

        /// <summary>
        /// Null if the UTF-8 BOM is not detected AND no encoding is specified.
        /// </summary>
        public string? FileContents { get; set; }
    }
}
