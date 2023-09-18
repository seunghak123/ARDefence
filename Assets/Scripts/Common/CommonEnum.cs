
public enum E_ANIMATION_EVENT
{
    CREATE_OBJECT,          //오브젝트 생성 이벤트
    CREATE_PARTICLE,        //파티클 생성 이벤트
    PLAY_SOUND,             //사운드 플레이 이벤트
    CAMERA_ACATION,         //카메라 이벤트
    
}
public enum E_LOGIN_TYPE
{
    GUEST_LOGIN,
    GOOGLE_LOGIN,
    APPLE_LOGIN,
}
public enum E_SOUND_TYPE
{
    BGM_SOUND,
    VOICE_SOUND,
    FBX_SOUND,
}
public enum E_LANGUAGE_TYPE
{
    //Default 는 한국어
    KOREAN,
    ENGLISH,
}

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