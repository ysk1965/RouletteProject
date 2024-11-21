public static class Define
{
    public const string ATLAS_UI_MAIN = "UI_Main";
    public const string ATLAS_SOCIAL = "SocialProfile";

    public static string GetFilePath(FilePath filePath)
    {
        switch (filePath)
        {
            case FilePath.PoupPath:
                return "UI/Popup/";
            case FilePath.CharacterPath:
                return "Character/";
            case FilePath.CharacterNamePath:
                return "Character/Name/";
            case FilePath.StagePath:
                return "Stage/";
        }

        return string.Empty;
    }
}
