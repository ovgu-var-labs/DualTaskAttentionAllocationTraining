using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SwitchPrimTaskPackage : MonoBehaviour
{
    public static int packageID = 333;
    public int[] primTaskNum = new int[8];
    public int[] primHighlightsIndices = new int[4];

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(packageID);

                for (int i = 0; i < primTaskNum.Length; i++)
                {
                    writer.Write(primTaskNum[i]);
                }
                for(int i = 0; i < primHighlightsIndices.Length; i++)
                {
                    writer.Write(primHighlightsIndices[i]);
                }
            }
            return m.ToArray();
        }
    }

    public static SwitchPrimTaskPackage Deserialize(byte[] data)
    {
        SwitchPrimTaskPackage result = new SwitchPrimTaskPackage();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                reader.ReadInt32();//read package id

                for (int i = 0; i < result.primTaskNum.Length; i++)
                {
                    result.primTaskNum[i] = reader.ReadInt32();
                }

                for (int i = 0; i < result.primHighlightsIndices.Length; i++)
                {
                    result.primHighlightsIndices[i] = reader.ReadInt32();
                }
            }
        }
        return result;
    }
}
