using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class SaveData
    {
        public int level;
        public float experience;

        public SaveData(int level, float experience)
        {
            this.level = level;
            this.experience = experience;
        }
    }
}