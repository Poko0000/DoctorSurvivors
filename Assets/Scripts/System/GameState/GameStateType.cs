// 遊戲的大階段。給 UI 判斷「現在該顯示什麼」用
// 注意:LevelUp、Pause 不是狀態,它們是疊在 Playing 上面的暫停請求
public enum GameStateType
{
    None,
    Playing,
    GameOver,
    Victory
}
