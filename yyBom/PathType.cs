namespace yyBom
{
    public enum PathType
    {
        DirectoryPath,
        DirectoryName,
        FilePath,
        FileName,
        FileExtension,

        // If we casually register a lot of patterns, even if the app attempts merging them, 1) It still cant be inefficient, 2) The merging operation may cause an error.
        // Currently, I only have to deal with things like yyTodoMail.deps.json and yyTodoMail.runtimeconfig.json.
        DirectoryNamePrefix,
        DirectoryNameSuffix,
        FileNamePrefix,
        FileNameSuffix
    }
}
