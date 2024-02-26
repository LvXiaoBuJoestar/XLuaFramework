public class AppConst
{
    public const string bundleExtension = ".ab";
    public const string FileListName = "filelist.txt";

    public static GameMode GameMode;
}

public enum GameMode
{
    Editor,
    Package,
    Update
}
