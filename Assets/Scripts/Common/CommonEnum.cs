
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
//앱 구동 순서
//앱 구동,
//매니페스트(권한 요청)
//앱 업데이트
//번들 업데이트,
//결제 데이터 업데이트(인앱 데이터 갱신)
//로그인 
public enum E_APPLICATION_STATE
{
    APPLICATION_START,
    APPLICATION_UPDATE,
    REQUEST_PERMISSION,
    BUNDLE_UPDATE,
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