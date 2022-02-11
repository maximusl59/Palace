using System;

public static class Toolbox
{
    public static int GetCardValue(string str) {
        int value;
        if (str == "")
            return 1;

        if (str.Length < 3) {
            if (str[1] == 'A')
                value = 14;
            else if (str[1] == 'J')
                value = 11;
            else if (str[1] == 'Q')
                value = 12;
            else if (str[1] == 'K')
                value = 13;
            else
                value = (int)Char.GetNumericValue(str[1]);
        }
        else
            value = Int16.Parse(str.Substring(1, 2));
        return value;
    }

    public static string GetCardValueString(string str) {
        string value;
        if (str == "")
            value = "0";
        else if (str.Length < 3)
            value = str.Substring(1, 1);
        else
            value = str.Substring(1, 2);
        return value;
    }

    public static bool CanPlay(string middleCard, string handCard) {
        if (middleCard == "")
            return true;

        int mid = GetCardValue(middleCard);
        int hand = GetCardValue(handCard);

        if (hand == 2 || hand == 3)
            return true;

        if (mid == 7) {
            if (hand <= 7)
                return true;
            else
                return false;
        }

        if (hand == 10)
            return true;

        if (mid <= hand)
            return true;
        
        return false;
    }

    public static string GetSpritePath(string cardName) {
        string path = "Sprites/Cards/" + cardName;
        return path;
    }

    public static string GetCardBackPath() {
        return "Sprites/Light Blue Card Back";
    }
}
