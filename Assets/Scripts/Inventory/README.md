# Backpack Hero 風格背包系統 — Unity 實作說明

## 檔案總覽

| 檔案 | 職責 |
|---|---|
| `ItemShape.cs` | 定義物品佔用格子的形狀,支援旋轉 |
| `ItemData.cs` | 物品的 ScriptableObject 資料 + 執行期實例 `ItemInstance` |
| `BackpackGridModel.cs` | 純邏輯層:碰撞檢查、放置、移除、找空位(不依賴 UI,方便測試) |
| `BackpackGridUI.cs` | 畫出格線背景、螢幕座標轉格子座標、放置預覽高亮 |
| `DraggableItemUI.cs` | 掛在每個物品圖示上,處理拖曳、右鍵旋轉、放置判定 |
| `BackpackManager.cs` | 整合範例:生成物品並放入背包 |

## Unity 編輯器設定步驟

1. **建立 Canvas**
   - `Screen Space - Overlay` 或 `Camera` 皆可,記得記錄 `rootCanvas` 要拖進 Inspector。

2. **建立背包面板**
   - 新增一個空的 `Panel`(RectTransform),掛上 `BackpackGridUI.cs`
   - 設定 `width`、`height`(例如 8x6)、`cellSize`(例如 64)
   - `cellHighlightPrefab`:做一個簡單的正方形 `Image`,顏色半透明,拖進去當預覽格子

3. **建立物品 UI Prefab**
   - 一個帶 `Image`(顯示 icon)的 UI GameObject
   - 掛上 `DraggableItemUI.cs`
   - 記得加上 `CanvasGroup`(腳本裡沒有會自動加,但先手動加更保險)
   - 存成 Prefab,拖進 `BackpackManager.itemUIPrefab`

4. **建立物品資料**
   - 在 Project 視窗右鍵 → `Create > Backpack > Item Data`
   - 設定名稱、圖示、`shapeType`(Single / L形 / 2x2 等)
   - 這些拖進 `BackpackManager.startingItems`

5. **建立 BackpackManager**
   - 場景中新增空物件,掛上 `BackpackManager.cs`
   - 把上面設定的 `gridUI`、`rootCanvas`、`itemUIPrefab`、`startingItems` 都拖進去
   - 執行遊戲,應該就能看到物品出現、可拖曳、右鍵旋轉、拖到重疊處會顯示紅色無法放置

## 核心邏輯重點

- **形狀系統**:每個物品不是佔 1 格,而是一組相對座標(`ItemShape`),旋轉用 `(x,y) -> (-y,x)` 矩陣運算再正規化座標,避免出現負值。
- **邏輯與畫面分離**:`BackpackGridModel` 完全不碰 UI,你可以直接寫 Unit Test 驗證碰撞邏輯,之後也方便做「存檔/讀檔」(只存 `ItemData` 引用 + 座標 + 旋轉狀態即可)。
- **拖曳判定流程**:`OnBeginDrag` 先把物品從邏輯網格移除(避免自己卡自己)→ `OnDrag` 即時顯示放置預覽 → `OnEndDrag` 嘗試放置,失敗則自動找空位或退回。

## 延伸功能建議(Backpack Hero 的特色機制)

Backpack Hero 最大的特色其實不只是格狀庫存,而是「**相鄰物品互相產生效果**」(例如藥水放在武器旁邊會加成)。要做這個,建議:

1. 在 `ItemData` 加上效果定義,例如:
   ```csharp
   public List<AdjacencyEffect> adjacencyEffects;
   ```
2. 在 `BackpackGridModel` 加一個 `GetNeighbors(ItemInstance item)` 方法,掃描該物品佔用格子四周(或緊鄰)的其他物品。
3. 每次 `TryPlace` 成功後,重新計算一次全背包的效果加成(可以用事件 `OnLayoutChanged` 通知戰鬥系統重算數值)。

- **形狀更複雜的物品**:目前只提供幾種範例形狀,你可以改成用 `bool[,]` 手動畫格子(用一個自訂 Inspector 讓設計師直接在編輯器裡點格子畫形狀,這是比較進階但更直覺的做法)。
- **音效/動畫**:放置成功/失敗、旋轉時可以掛 `AudioSource.PlayOneShot` 和簡單的 `transform` 動畫補間。

有需要的話,我可以再幫你做「相鄰效果系統」或「格子形狀自訂編輯器」的部分。
