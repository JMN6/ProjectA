using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogParser : MonoBehaviour
{
    private static readonly string folderName = "Dialog/";
    [SerializeField] private string tsvFileName = "data";

    private void Start()
    {
        Convert(tsvFileName);
    }

    public static List<DialogRow> Convert(string fileName)
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

        List<DialogRow> dataList = new List<DialogRow>();

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string[] values = lines[i].Split('\t');

            if (values.Length != 5)
            {
                Debug.LogWarning($"Line {i + 1} skipped due to invalid column count.");
                continue;
            }

            DialogRow row = new DialogRow
            {
                Number = int.Parse(values[0]),
                Pic1Number= int.Parse(values[1]),
                Pic2Number= int.Parse(values[2]),
                Text = values[3],
                Talker = int.Parse(values[4])
            };

            dataList.Add(row);
        }

        return dataList;
    }
}
