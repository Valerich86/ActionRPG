

using System;

public static class StaticData 
{
    public static Action<string> OnHintChanged;
    public static Action<string, int> OnGlobalHintChanged;
    public static Action<ItemController> OnItemPicked;
    public static Action<ItemSO> OnCellEnter;
    public static Action OnCellExit;
    public static Action<EnemyController> OnEnemyDying;
    public static Action<int> OnArrowAmountChanged;

    public static Role PlayerRole;
}
