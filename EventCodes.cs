public class EventCodes
{
    public const byte PlayCardEventCode = 1;
    /*
     * string[] cards
     * int cardsInHand
     */
    public const byte DrawCardEventCode = 2;
    /*
     * int playerNo
     * string[] cards
     */
    public const byte YourTurnEvent = 3;
    /*
     * int playerNo
     */
    public const byte InitCardsEvent = 4;
    /*
     * int playerNo
     * string[] cards
     */
    public const byte ReadyEvent = 5;
    public const byte UpdateDrawCardsLeftEvent = 6;
    /*
     * int cardsLeft
     */
    public const byte BurnEvent = 7;
    /*
     * Player player "who's turn it is"
     */
    public const byte PickUpEvent = 8;
    public const byte CheckIfCanBurnEvent = 9;
    public const byte FinishedPlayingEvent = 10;
    public const byte RemovePlayerEvent = 11;
}