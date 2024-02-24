using UnityEngine;

public static class Constants
{
    // Scenes
    public const string MAIN_MENU_SCENE = "MainMenu";
    public const string GAME_SCENE = "GameScene";

    //
    public const string MAIN_MIXER_VOLUME = "MainMixerVolume";
    public const string MUSIC_VOLUME = "MusicVolume";
    public const string SFX_VOLUME = "SfxVolume";

    // Tags
    public const string PLAYER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";
    public const string PLAYER_PROJECTILE_TAG = "ProjectilePlayer";
    public const string ENEMY_PROJECTILE_TAG = "ProjectileEnemy";
    public const string BORDER_TOP_TAG = "BorderTop";
    public const string BORDER_BOTTOM_TAG = "BorderBottom";
    public const string BORDER_LEFT_TAG = "BorderLeft";
    public const string BORDER_RIGHT_TAG = "BorderRight";
    public const string PROJECTILE_DESTROY_BOX = "ProjectileDestroyBox";
    public const string ITEM_DROP_TAG = "ItemDrop";

    // Player Animations
    public const string THRUSTER_ANIMATION = "Thruster Animation";
    public const string THRUSTER_ANIMATION_SLOW = "Thruster Animation Slow";

    // Display Text
    public const string SPEED_UPGRADE_TEXT = "Extra Speed";
    public const string BULLET_UPGRADE_TEXT = "Extra Bullet";
}
