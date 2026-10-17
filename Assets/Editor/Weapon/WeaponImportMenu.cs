using UnityEditor;

public static class WeaponImportMenu
{
    [MenuItem("Tools/Tables/Import Weapon")]
    static void Run()
    {
        var source = new CsvTableSource("Assets/Data/Tables/Weapon.csv");
        new WeaponImporter(source).Import();
    }
}
