using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomConversions
{
    public class Percent
    {
        public static float Convert(float value)
        {
            if (value < 0f)
            {
                value = 0f;
            }
            else if (value > 100f)
            {
                value = 100f;
            }

            return value;
        }
    }
}