namespace TaskEngine.Assets;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class LoadAudioAttribute : System.Attribute
{
    public string Name { get; }
    public string BundleName { get; }
    public string FileName { get; }

    public LoadAudioAttribute(string name, string bundleName, string clipName)
    {
        Name = name;
        BundleName = bundleName;
        FileName = clipName;
    }
}