using System;
using System.Collections.Generic;
using UnityEngine;

public static class CsvParser
{
    // 回傳：每一列是一個「欄位名 → 字串值」的字典
    public static List<Dictionary<string, string>> Parse(string csvText)
    {
        var result = new List<Dictionary<string, string>>();
        // 把 csvText 切成多行（處理 \r\n 與空行）
        string[] lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        // 如果行數 < 1，直接回傳 result
        if (lines.Length < 1) return result;
        // 第一行切開成 headers，每個都 Trim()
        string[] headers = lines[0].Split(',');
        for (int i = 0; i < headers.Length; i++)
        {
            headers[i] = headers[i].Trim();
        }
        // 從第二行開始跑迴圈：
        // 切開成 cells
        // 如果 cells.Length != headers.Length → LogWarning（含行號）後 continue
        // 建立 Dictionary，headers[i] → cells[i].Trim()
        // 加進 result
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(',');

            if (cells.Length != headers.Length)
            {
                Debug.LogWarning($"[CsvParser] 第 {i + 1} 行欄位數 {cells.Length}，應為 {headers.Length}，已略過。");
                continue;
            }

            var row = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length; j++)
            {
                row[headers[j]] = cells[j].Trim();
            }

            result.Add(row);
        }
        
        return result;
    }
}
