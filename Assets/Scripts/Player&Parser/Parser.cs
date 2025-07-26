using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataParseAndPlayer
{
    public class DataRow
    {
        public int Number { get; set; }

        public DataRow() { }

        public virtual void SetValue(string[] datas)
        {
            Number = int.Parse(datas[0]);
        }
    }

    public class Parser<T> where T : DataRow, new()
    {
        public static List<T> Convert(string _folderName, string _fileName)
        {
            TextAsset tsvFile = Resources.Load<TextAsset>($"{_folderName}/{_fileName}");
            if (tsvFile == null)
            {
                Debug.LogError($"TSV file not found: {_folderName}/{_fileName}");
                return null;
            }


            string[] lines = tsvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length <= 1)
            {
                Debug.LogWarning("TSV contains no data.");
                return null;
            }

            List<T> dataList = new List<T>();

            for (int i = 1; i < lines.Length; i++) // skip header
            {
                string[] values = lines[i].Split('\t');

                T row = new T();
                row.SetValue(values);

                dataList.Add(row);
            }

            return dataList;
        }
    }

}