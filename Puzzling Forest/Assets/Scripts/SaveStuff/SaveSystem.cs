using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// The save method will take a LevelProgress and write it to a static location as a binary file.
/// The load method will deserialize the binary file back into a LevelProgress and return it.
/// </summary>
public static class SaveSystem
{
    static string file_name = "/levelProgress.save";

    public static void SaveLevelProgress (LevelProgress progress)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + file_name;
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, progress);
        stream.Close();
    }

    public static LevelProgress LoadLevelProgress()
    {
        string path = Application.persistentDataPath + file_name;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            LevelProgress progress = formatter.Deserialize(stream) as LevelProgress;
            stream.Close();

            return progress;
        }
        else
        {
            Debug.LogError("Save fle not found in " + path);
            return null;
        }
    }
}
