using System.Collections.Generic;
using UnityEngine;

public class OrbitWeapon : IWeapon
{
    [SerializeField] float startAngle = 90f;   // 第一片的起始角度(90 = 正上方)

    readonly List<Transform> blades = new();
    float currentRadius;
    int currentAmount;

    bool isActive;        // true = 刀刃出現中, false = 冷卻中
    float phaseTimer;     // 目前這個階段已經過的時間

    void Start()
    {
        currentRadius = data.range;
        currentAmount = data.amount;
        isActive = true;

        Rebuild();
    }

    /// <summary>清掉全部刀刃,依 currentAmount 重新平分生成</summary>
    void Rebuild()
    {
        // 先清舊的。用 list 記錄刀刃,不靠遍歷子物件,
        // 因為 Destroy 到幀尾才生效,同一幀重生時舊刀刃還在子物件裡
        foreach (Transform blade in blades)
        {
            if (blade != null) Destroy(blade.gameObject);
        }
        blades.Clear();

        if (currentAmount <= 0)
        {
            Debug.LogWarning($"{data.weaponName} 的刀刃數量是 {currentAmount},沒有刀刃可生成");
            return;
        }

        for (int i = 0; i < currentAmount; i++)
        {
            // 平分圓周:3 片 = 相隔 120 度
            float angle = (startAngle + 360f / currentAmount * i) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

            GameObject obj = Instantiate(data.projectilePrefab, transform);
            obj.transform.localPosition = dir * currentRadius;
            obj.GetComponent<ShurikenBlade>().Initialize(data.damage, data.hitInterval);
            obj.SetActive(isActive);   // 冷卻中升級的話,新刀刃也保持隱藏

            blades.Add(obj.transform);
        }
    }

    /// <summary>設定新的軌道半徑</summary>
    public void SetRadius(float newRadius)
    {
        currentRadius = newRadius;

        foreach (Transform blade in blades)
        {
            // normalized = 保留原本方向,只改長度
            blade.localPosition = blade.localPosition.normalized * currentRadius;
        }
    }

    public void AddRadius(float amount) => SetRadius(currentRadius + amount);

    /// <summary>設定刀刃數量,並重新平分</summary>
    public void SetAmount(int newAmount)
    {
        currentAmount = newAmount;
        Rebuild();
    }

    public void AddAmount(int amount) => SetAmount(currentAmount + amount);

    // 不呼叫 base.Update():這裡用自己的「出現 → 冷卻」週期
    protected override void Update()
    {
        phaseTimer += Time.deltaTime;

        if (isActive)
        {
            // 負號 = 順時針;想逆時針就拿掉負號
            transform.Rotate(0, 0, -data.speed * Time.deltaTime);

            // lifetime <= 0 視為永久存在
            if (data.lifetime > 0 && phaseTimer >= data.lifetime)
                SetBladesActive(false);
        }
        else if (phaseTimer >= data.cooldown)
        {
            SetBladesActive(true);
        }
    }

    void SetBladesActive(bool active)
    {
        isActive = active;
        phaseTimer = 0f;

        foreach (Transform blade in blades)
        {
            blade.gameObject.SetActive(active);
        }
    }

    // 環繞武器沒有「發動攻擊」這個動作,傷害由 ShurikenBlade 處理
    protected override void Attack() { }
}
