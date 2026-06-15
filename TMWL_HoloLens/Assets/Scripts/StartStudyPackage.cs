using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class StartStudyPackage
{
    public static int packageID = 111;
    public int participantID;
    public int visID; // 0 - Halo, 1 - Flicker, 2 - Diegetic, 3 - Non-Diegetic
    public int studyRunID; // 0 - Measurement, 1-3 - Training, 4-5 - Measurement, 11 - Expert

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(packageID);
                writer.Write(participantID);
                writer.Write(visID);
                writer.Write(studyRunID);
            }
            return m.ToArray();
        }
    }

    public static StartStudyPackage Deserialize(byte[] data)
    {
        StartStudyPackage result = new StartStudyPackage();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                reader.ReadInt32();//read package id

                result.participantID = reader.ReadInt32();
                result.visID = reader.ReadInt32();
                result.studyRunID = reader.ReadInt32();
            }
        }
        return result;
    }
}
