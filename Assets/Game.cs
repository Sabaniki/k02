using System.Collections.Generic;
using System.Linq;
using Sequence = System.Collections.IEnumerator;

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
        cardGenerator = new CardGenerator(
            new List<string> {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J"
            });
        cardName = "";
    }


    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame() {
        // 起動からの経過時間を取得します
        if (gc.GetPointerFrameCount(0) == 1 && !cardGenerator.IsComplete) {
            cardName = cardGenerator.DrawCard();
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

        foreach (var (item, index) in cardGenerator.CardHistory.Select((item, index) => (item, index)))
            gc.DrawString($"{item.Key}: {item.Value.ToString()}", 60, 120 + index * 40);

        if (cardGenerator.IsComplete)
            gc.DrawString("complete!!", 60, 520);
    }
}


public class CardGenerator {
    private readonly List<string> cardNames;
    public int Money { private set; get; }
    public bool IsComplete { private set; get; }
    public readonly IDictionary<string, int> CardHistory;

    public CardGenerator(List<string> cardNames, int initMoney = 10000) {
        this.cardNames = cardNames;
        IsComplete = false;
        Money = initMoney;
        CardHistory = new Dictionary<string, int>();
        foreach (var cardName in cardNames)
            CardHistory.Add(cardName, 0);
    }

    public string DrawCard() {
        Money -= 100;
        if (Money <= 0) return null;
        var random = new System.Random().Next(0, cardNames.Count);
        var cardName = cardNames[random];
        CardHistory[cardName]++;
        IsComplete = CardHistory.Values.All(count => count > 0);
        return cardNames[random];
    }
}