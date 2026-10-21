using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

public class WeaponImporter
{
    private const string AssetFolder = "Assets/Data/Weapons";

    private readonly ITableSource source;

    private struct WeaponRowData
    {
        public string weaponName;
        public float damage, cooldown, hitInterval, speed, lifetime, range;
        public int amount, maxLevel;
    }

    public WeaponImporter(ITableSource source)
    {
        this.source = source;
    }

    private static string GetAssetPath(string id)
    {
        return $"{AssetFolder}/WeaponData_{id}.asset";
    }

    public WeaponData GetOrCreateAsset(string id, out bool created)
    {
        string path = GetAssetPath(id);
        WeaponData asset = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
        // 嘗試載入
        if(asset)
        {
            created = false;
            return asset;
        }
        
        //建立新Weapon
        asset = ScriptableObject.CreateInstance<WeaponData>();
        AssetDatabase.CreateAsset(asset, path);
        created = true;
        return asset;
    }

    private static bool TryGetFloat(Dictionary<string, string> row, string key, out float value)
    {
        value = 0f;
        // row 沒有這個 key → LogWarning 提示缺欄位，回傳 false
        if (!row.TryGetValue(key, out string text))
        {
            Debug.LogWarning($"[WeaponImporter] 缺少欄位：{key}");
            return false;
        }
         if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            Debug.LogWarning($"[WeaponImporter] 欄位 {key} 的值 \"{text}\" 不是數字");
            return false;
        }

        return true;
    }

    private static bool TryGetInt(Dictionary<string, string> row, string key, out int value)
    {
        value = 0;
        // row 沒有這個 key → LogWarning 提示缺欄位，回傳 false
        if (!row.TryGetValue(key, out string text))
        {
            Debug.LogWarning($"[WeaponImporter] 缺少欄位：{key}");
            return false;
        }
        // int.TryParse(row[key], NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
        //    失敗 → LogWarning 提示「欄位 key 的值 xxx 不是數字」，回傳 false
         if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
        {
            Debug.LogWarning($"[WeaponImporter] 欄位 {key} 的值 \"{text}\" 不是整數");
            return false;
        }
        
        return true;
    }

    private static bool TryParseRow(Dictionary<string, string> row, out WeaponRowData data)
    {
        data = default;

        if (!row.TryGetValue("weaponName", out data.weaponName))
        {
            Debug.LogWarning("[WeaponImporter] 缺少欄位：weaponName");
            return false;
        }
        // damage、cooldown、speed、lifetime、range 是 float
        if (!TryGetFloat(row, "damage", out data.damage)) return false;
        if (!TryGetFloat(row, "cooldown", out data.cooldown)) return false;
        // hitInterval 為選填欄位:表裡還沒加這欄時用預設值 0.5,不讓整列被略過
        if (row.ContainsKey("hitInterval"))
        {
            if (!TryGetFloat(row, "hitInterval", out data.hitInterval)) return false;
        }
        else
        {
            data.hitInterval = 0.5f;
        }
        if (!TryGetFloat(row, "speed", out data.speed)) return false;
        if (!TryGetFloat(row, "lifetime", out data.lifetime)) return false;
        if (!TryGetFloat(row, "range", out data.range)) return false;
        // amount、maxLevel 是 int（用 TryGetInt）
        if (!TryGetInt(row, "amount", out data.amount)) return false;
        if (!TryGetInt(row, "maxLevel", out data.maxLevel)) return false; 

        return true;
    }

    private static void Apply(WeaponData asset, WeaponRowData data)
    {
        asset.weaponName = data.weaponName;
        asset.damage = data.damage;
        asset.cooldown = data.cooldown;
        asset.hitInterval = data.hitInterval;
        asset.speed = data.speed;
        asset.lifetime = data.lifetime;
        asset.range = data.range;
        asset.amount = data.amount;
        asset.maxLevel = data.maxLevel;
        EditorUtility.SetDirty(asset);
    }

    public void Import()
    {
        var rows = source.LoadRows();
        if (rows.Count == 0)
        {
            Debug.LogWarning("[WeaponImporter] 沒有任何資料列，匯入中止。");
            return;
        }

        int updated = 0, created = 0, skipped = 0;
        var seenIds = new HashSet<string>();

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];

            if (!row.TryGetValue("id", out string id) || string.IsNullOrWhiteSpace(id))
            {
                Debug.LogWarning($"[WeaponImporter] 第 {i + 2} 列沒有 id，已略過。");
                skipped++;
                continue;
            }

            // 重複 id 檢查
            if (!seenIds.Add(id))
            {
                Debug.LogWarning($"[WeaponImporter] 第 {i + 2} 列的 id \"{id}\" 與前面重複，已略過。");
                skipped++;
                continue;
            }
            // 解析（失敗就略過，此時還沒建立任何檔案）
            if (!TryParseRow(row, out var data))
            {
                Debug.LogWarning($"[WeaponImporter] 第 {i + 2} 列（id: {id}）資料有誤，已略過。");
                skipped++;
                continue;
            }
            // 驗證都通過了，才找到或建立 asset
            var asset = GetOrCreateAsset(id, out bool isNew);
            // 寫入數值
            Apply(asset, data);

            // 統計
            if (isNew)
            {
                created++;
                if (asset.projectilePrefab == null || asset.weaponPrefab == null)
                {
                    Debug.LogWarning($"[WeaponImporter] 新建的 {id} 還沒指派 Projectile Prefab 或 Weapon Prefab。", asset);
                }
            }
            else
            {
                updated++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[WeaponImporter] 完成：更新 {updated}、新建 {created}、略過 {skipped}");
    }

}