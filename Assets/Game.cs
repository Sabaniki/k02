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

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void InitGame() {
        ResetValues();
    }

    private void ResetValues() {
        // キャンバスの大きさを設定します
        gc.SetResolution(720, 1280);
        cardGenerator = new CardGenerator();
        cardName = "";
    }


    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame() {
        // 起動からの経過時間を取得します
        if ( /*gc.GetPointerFrameCount(0) == 1 && */!cardGenerator.IsComplete) {
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
    public readonly List<string> cardNames;
    public int Money { private set; get; }
    public bool IsComplete { private set; get; }
    public readonly Dictionary<string, int> CardHistory;

    public CardGenerator(int initMoney = 10000) {
        cardNamesAndRarity = new Dictionary<string, float> {
            {"A", 0.05f}, {"B", 0.05f}, {"C", 0.05f}, {"D", 0.05f}, {"E", 0.05f},
            {"F", 0.15f}, {"G", 0.15f}, {"H", 0.15f}, {"I", 0.15f}, {"J", 0.15f}
        };
        cardNames = new List<string>();
        IsComplete = false;
        Money = initMoney;
        CardHistory = new Dictionary<string, int>();
        foreach (var cardName in cardNamesAndRarity.Keys) {
            cardNames.Add(cardName);
            CardHistory.Add(cardName, 0);
        }
    }
    
    [CanBeNull]
    private string Choose(){
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
        IsComplete = CardHistory.Values.All(count => count > 0);
        return cardName;
    }
}