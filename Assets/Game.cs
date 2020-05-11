using System;
using System.Collections.Generic;
using System.Linq;
using Sequence = System.Collections.IEnumerator;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

//using Random = System.Random;

/// <summary>
/// ゲームクラス。
/// 学生が編集すべきソースコードです。
/// </summary>
public sealed class Game : GameBase {
    // 変数の宣言
    private CardGenerator cardGenerator;
    public string cardName;

    private readonly Dictionary<string, float> cardNamesAndRarity = new Dictionary<string, float> {
        {"LR", 0.01f}, {"UR", 0.05f}, {"SSR", 0.075f}, {"SR", 0.10f}, {"HR", 0.15f},
        {"R", 0.20f}, {"HN", 0.25f}, {"N", 0.165f}
    };

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void InitGame() {
        ResetValues();
    }

    private void ResetValues() {
        // キャンバスの大きさを設定します
        gc.SetResolution(720, 1280);
        cardGenerator = new CardGenerator(cardNamesAndRarity);
        cardName = "";
    }


    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame() {
        // 起動からの経過時間を取得します
        if (gc.GetPointerFrameCount(0) == 1 && !cardGenerator.IsComplete) {
            cardName = cardGenerator.DrawCard() ?? "no card";
        }

        if (gc.GetPointerFrameCount(0) >= 120) {
            ResetValues();
        }
    }

    /// <summary>
    /// 描画の処理
    /// </summary>
    public override void DrawGame() {
        // 画面を白で塗りつぶします
        gc.ClearScreen();
        gc.SetColor(0, 0, 0);
        gc.SetFontSize(36);
        gc.DrawString($"money: {cardGenerator.Money}", 60, 40);
        gc.DrawString($"new: {cardName}", 60, 80);

        gc.Random(0, 10);

        foreach (var (item, index) in cardGenerator.CardHistory.Select((item, index) => (item, index)))
            gc.DrawString($"{item.Key}: {item.Value.ToString()}", 60, 120 + index * 40);

        if (cardGenerator.IsComplete)
            gc.DrawString("complete!!", 60, 520);
    }
}


public class CardGenerator {
    private readonly Dictionary<string, float> cardNamesAndRarity;
    public int Money { private set; get; }
    public bool IsComplete { private set; get; }
    public readonly Dictionary<string, int> CardHistory;

    public CardGenerator(Dictionary<string, float> cardNamesAndRarity ,int initMoney = 10000) {
        this.cardNamesAndRarity = cardNamesAndRarity;
        IsComplete = false;
        Money = initMoney;
        CardHistory = new Dictionary<string, int>();
        foreach (var cardName in cardNamesAndRarity.Keys) {
            CardHistory.Add(cardName, 0);
        }
    }

    [CanBeNull]
    private string Choose() {
        var total = cardNamesAndRarity.Sum(elem => elem.Value);
        var randomPoint = Random.value * total;
        foreach (var elem in cardNamesAndRarity) {
            if (!(randomPoint < elem.Value))
                randomPoint -= elem.Value;
            else
                return elem.Key;
        }

        return null;
    }

    [CanBeNull]
    public string DrawCard() {
        if (Money <= 0) return null;
        Money -= 100;
        var cardName = Choose();
        if (cardName == null) return null;
        CardHistory[cardName]++;
        IsComplete = CardHistory
            .Where(pair => pair.Key == "LR" || pair.Key == "UR")
            .Any(pair => pair.Value >= 2);
        return cardName;
    }
}