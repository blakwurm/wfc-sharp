using WfcLib;

namespace WfcHost;

public static class Utils
{
    public static (WfcGenerator, Card[]) BuildWfc()
    {
        var wfc = new WfcGenerator();
        var cards = new Card[16];

        cards[4] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {1,1,2,1}
        );
        cards[6] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {1,2,2,1}
        );
        cards[14] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {1,2,2,2}
        );
        cards[12] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {1,1,2,2}
        );
        cards[5] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {2,1,2,1}
        );
        cards[7] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {2,2,2,1}
        );
        cards[15] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {2,2,2,2}
        );
        cards[13] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {2,1,2,2}
        );
        cards[1] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {2,1,1,1}
        );
        cards[3] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {2,2,1,1}
        );
        cards[11] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {2,2,1,2}
        );
        cards[9] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {2,1,1,2}
        );
        cards[0] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {1,1,1,1}
        );
        cards[2] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {1,2,1,1}
        );
        cards[10] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {1,2,1,2}
        );
        cards[8] = Card.WrapCard(
            wfc.CreateCard(),
            new[] {1,1,1,2}
        );
        Card.ConstrainCards(wfc);

        return (wfc, cards);
    }

    public static T[] Slice<T>(this T[,] arr, int dimension, int index)
    {
        var length = arr.GetLength(dimension);
        var result = new T[length];
        for (var i = 0; i < length; i++)
        {
            result[i] = dimension == 0 ? arr[i, index] : arr[index, i];
        }

        return result;
    }
}