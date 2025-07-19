using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneRow
{
    public int Number;
    public float XPos;
    public float YPos;
    public float FontSize;
    public int Align;
    public string Pic;
    public string Text;
}

public class CutSceneParser
{
    private static readonly string folderName = "CutScene/";

    public static List<CutSceneRow> Convert(string fileName)
    {
        TextAsset tsvFile = Resources.Load<TextAsset>(folderName + fileName);
        if (tsvFile == null)
        {
            Debug.LogError($"TSV file not found: {folderName + fileName}");
            return null;
        }


        string[] lines = tsvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1)
        {
            Debug.LogWarning("TSV contains no data.");
            return null;
        }

        List<CutSceneRow> dataList = new List<CutSceneRow>();

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string[] values = lines[i].Split('\t');

            if (values.Length != 7)
            {
                Debug.LogWarning($"Line {i + 1} skipped due to invalid column count.");
                continue;
            }

            CutSceneRow row = new CutSceneRow
            {
                Number = int.Parse(values[0]),
                XPos = float.Parse(values[1]),
                YPos = float.Parse(values[2]),
                FontSize = int.Parse(values[3]),
                Align = int.Parse(values[4]),
                Pic = values[5],
                Text = values[6].Replace("\\\\n", "\n")
            };

            dataList.Add(row);
        }

        return dataList;
    }
}
