using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Data
{
    public static class SaveSystem
    {
        private static string s_Path = Application.dataPath + "save001.dat";

        public static void Save(SaveData saveData)
        {
            string data = JsonUtility.ToJson(saveData);

            File.WriteAllText(s_Path, data);
        }

        public static SaveData Load()
        {
            if (File.Exists(s_Path))
            {
                string data = File.ReadAllText(s_Path);
                return JsonUtility.FromJson<SaveData>(data);
            }

            return null;
        }
    }
}