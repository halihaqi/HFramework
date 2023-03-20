namespace HFramework
{
    /// <summary>
    /// 事件名枚举类
    /// </summary>
    public enum ClientEvent
    {
        //Input
        GET_KEY_DOWN,
        GET_KEY_UP,
        GET_KEY,
        GET_MOUSE_BUTTON_DOWN,
        GET_MOUSE_BUTTON_UP,
        GET_MOUSE_BUTTON,
        GET_MOVE,
        GET_LOOK,
        GET_MOUSE_SCROLL,
        
        //Scene
        SCENE_LOADING,
        SCENE_LOAD_COMPLETE,
    }
}