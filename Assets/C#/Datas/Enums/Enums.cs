using System;
using Unity.VisualScripting;

public enum ETypeGame
{
    usual = 0,
    ThrowIn = 1,
    Transferable = 2
}

public enum ESuit
{
    HEART = 0,
    TILE = 1,
    CLOVERS = 2,
    PIKES = 3
}

public enum EStyle
{
    Base,
    Russian,
    Erotic,
    Forest,
    Sea,
    Scary,
    Heroes,
    Fallout,
    Cars
}

public enum EStatus
{
    Null,
    Fold,
    Grab,
    Pass
}

public enum ESortTrump
{
    General,
    Left,
    Right
}

enum CardOrderMethod
{
    SuitThenKind,
    KindThenSuit,
}

public enum EScreens
{
    StartScreen,
    PolicyScreen,
    SignInScreen_NameAvatar,
    SignInScreen,
    MenuScreen,
    ShopScreen,
    RatingScreen,
    CollectionsScreen,
    RewardsScreen,
    SettingsScreen,
    SkinsScreen,
    LoginScreen,
    Count
}

public enum ERole
{
    main = 0,
    firstThrower = 1,
    thrower = 2
}

public enum ENominal
{
    TWO = 0,
    THREE = 1,
    FOUR = 2,
    FIVE = 3,
    SIX = 4,
    SEVEN = 5,
    EIGHT = 6,
    NINE = 7,
    TEN = 8,
    JACK = 9,
    QUEEN = 10,
    KING = 11,
    ACE = 12,
    COUNT = 13
}
public enum LastMove
{
    folding = 0,
    grabbing = 1
}