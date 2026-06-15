using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class StopStudyPackage : MonoBehaviour
{
    public static int packageID = 666;

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(packageID);
            }
            return m.ToArray();
        }
    }
}