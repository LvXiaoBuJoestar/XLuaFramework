public class AppConst
{
    public const string bundleExtension = ".ab";
    public const string FileListName = "filelist.txt";

    public const string ResourcesUrl = "http://127.0.0.1/AssetBundles";

    public static GameMode GameMode;
}

public enum GameMode
{
    Editor,
    Package,
    Update
}
