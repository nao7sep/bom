﻿using System.Text;

namespace yyBom
{
    public class SpecifiedEncodingModel
    {
        public PathType? PathType { get; set; }

        public string? Path { get; set; }

        public string? EncodingName { get; set; }

        public Encoding? Encoding { get; set; }
    }
}
