using UnityEngine;

namespace HFramework
{
    internal class InputManager : HModule, IInputManager
    {
        private const string HORIZONTAL_MOVE = "Horizontal";
        private const string VERTICAL_MOVE = "Vertical";
        private const string HORIZONTAL_LOOK = "Mouse X";
        private const string VERTICAL_LOOK = "Mouse Y";
        private const string MOUSE_SCROLL = "Mouse ScrollWheel";

        private Vector2 _move = Vector2.zero;
        private Vector2 _look = Vector2.zero;
        
        internal override int Priority => 8;


        //是否开启输入检测
        private bool _enabled = false;

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        internal override void Init()
        {
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            InputUpdate();
        }

        internal override void Shutdown()
        {
        }

        /// <summary>
        /// 检查Key输入
        /// </summary>
        /// <param name="key"></param>
        private void KeyCheck(KeyCode key)
        {
            if (Input.GetKeyDown(key))
                HEntry.EventMgr.TriggerEvent(ClientEvent.GET_KEY_DOWN, key);
            else if(Input.GetKey(key))
                HEntry.EventMgr.TriggerEvent(ClientEvent.GET_KEY, key);
            else if(Input.GetKeyUp(key))
                HEntry.EventMgr.TriggerEvent(ClientEvent.GET_KEY_UP, key);
        }

        private void MouseCheck(int key)
        {
            if(Input.GetMouseButtonDown(key))
                HEntry.EventMgr.TriggerEvent(ClientEvent.GET_MOUSE_BUTTON_DOWN, key);
            else if(Input.GetMouseButton(key))
                HEntry.EventMgr.TriggerEvent(ClientEvent.GET_MOUSE_BUTTON, key);
            else if(Input.GetMouseButtonUp(key))
                HEntry.EventMgr.TriggerEvent(ClientEvent.GET_MOUSE_BUTTON_UP, key);
        }

        /// <summary>
        /// 在GlobalMono每帧调用
        /// </summary>
        private void InputUpdate()
        {
            if (!_enabled)
                return;
            
            KeyCheck(KeyCode.W);
            KeyCheck(KeyCode.A);
            KeyCheck(KeyCode.S);
            KeyCheck(KeyCode.D);
            
            KeyCheck(KeyCode.Tab);
            KeyCheck(KeyCode.Escape);
            KeyCheck(KeyCode.Space);

            MouseCheck(0);
            MouseCheck(1);
            MouseCheck(2);

            _move.x = Input.GetAxis(HORIZONTAL_MOVE);
            _move.y = Input.GetAxis(VERTICAL_MOVE);
            _look.x = Input.GetAxis(HORIZONTAL_LOOK);
            _look.y = Input.GetAxis(VERTICAL_LOOK);
            HEntry.EventMgr.TriggerEvent(ClientEvent.GET_MOVE, _move);
            HEntry.EventMgr.TriggerEvent(ClientEvent.GET_LOOK, _look);
            
            HEntry.EventMgr.TriggerEvent(ClientEvent.GET_MOUSE_SCROLL, Input.GetAxis(MOUSE_SCROLL));
        }
    }
}
