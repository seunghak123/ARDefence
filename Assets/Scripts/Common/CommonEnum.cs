#region ResourceType
public enum E_ANIMATION_EVENT
{
    CREATE_OBJECT,          //오브젝트 생성 이벤트
    CREATE_PARTICLE,        //파티클 생성 이벤트
    PLAY_SOUND,             //사운드 플레이 이벤트
    CAMERA_ACATION,         //카메라 이벤트

}
public enum E_SOUND_TYPE
{
    BGM_SOUND,
    VOICE_SOUND,
    FBX_SOUND,
}
#endregion ResourceType
#region EnviromentType
public enum E_APPLICATION_STATE
{
    APPLICATION_START,
    APPLICATION_UPDATE,
    REQUEST_PERMISSION,
    USER_LOGIN,
    BUNDLE_UPDATE,
    GAME_RESOURCE_LOAD,
    INAPP_UPDATE,
    TITLE,
}
public enum E_APPLICATION_PERMISSION_TYPE
{
    //퍼미션 타입
    STORAGE_PERMISSION,
    CAMERA_PERMISSION,
    END,
}
#endregion EnviromentType
#region UserInfoType
public enum E_LOGIN_TYPE
{
    GUEST_LOGIN,
    GOOGLE_LOGIN,
    APPLE_LOGIN,
}
public enum E_LANGUAGE_TYPE
{
    //Default 는 한국어
    KOREAN,
    ENGLISH,
}
#endregion UserInfoType
#region UserDataType
public enum E_ITEM_TYPE
{
    GOLD,
    CRYSTALS,
    VALID_CRYSTALS,
}
#endregion UserDataType

public enum E_STOREITEM_TYPE
{
    CONSUME,
    NON_CONSUME,
    SUBSCRIPTION,
}
//서버에 보낼 행동 타입
public enum E_OBJECT_ACTION_TYPE
{
    ATTACK_OTHER,
    MOVE_SELF,
    MOVE_OTHER,
    HEAL_OTHER,
    DEFENCE_SELF,
}

#region IngameType
public enum E_INGAME_TEAM_TYPE
{
    NONE,
    //pvp용 팀
    ALPAH_TEAM,
    BETA_TEAM,

    //인던용 팀
    USER_TEAM,
    MONSTER_TEAM,
    TEAM_TYPE_END,
}
//움직임은 크게, 이동 공격, 먹히기전 스턴 , 이벤트 액션-> 이벤트 액션은 등록한 액션 실행
public enum E_INGAME_AI_TYPE
{
    NONE,
    UNIT_IDLE,
    UNIT_MOVE,
    UNIT_ATTACK,
    UNIT_HIT,
    UNIT_DEAD,
    UNIT_EVENT,

    END,
}
public enum E_STATVALUE_TYPE
{
    ATTACK_VALUE,
    ATTACK_RANGE,
    DEFENCE_VALUE,
    HP_VALUE,
    MAX_HP_VALUE,
    SHIELD_VALUE,
    HP_DRAIN_VALUE,
    CRITICAL_PERCENT_VALUE,
}
#endregion IngameType