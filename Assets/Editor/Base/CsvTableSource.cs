using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvTableSource : ITableSource
{
    private readonly string path;

    public CsvTableSource(string path)
    {
        this.path = path;
    }

    public List<Dictionary<string, string>> LoadRows()
    {
        // 1. 檔案不存在 → LogError（含路徑），回傳空清單
        if (!File.Exists(path))
        {
            Debug.LogError($"[CsvTableSource] 找不到 CSV 檔案：{path}");
            return new List<Dictionary<string, string>>();
        }
        // 2. File.ReadAllText 讀檔
        string text = File.ReadAllText(path);
        // 3. 交給 CsvParser.Parse 並回傳
        return CsvParser.Parse(text);
    }
}
