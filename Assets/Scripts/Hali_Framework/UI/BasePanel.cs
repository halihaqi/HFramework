using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hali_Framework
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePanel : MonoBehaviour
    {
        //UI组件容器，物体名对应物体挂载的所有UI组件
        private Dictionary<string, List<UIBehaviour>> _controlDic = new Dictionary<string, List<UIBehaviour>>();
        private Dictionary<string, AnimationClip> _panelAnimDic;

        //面板渐变速度(0,1)
        private const float FADE_SPEED = 0.1f;

        protected virtual void Awake()
        {
            //一开始搜索常用UI组件添加到容器中
            FindChildrenControls<Button>();
            FindChildrenControls<Image>();
            FindChildrenControls<Text>();
            FindChildrenControls<Toggle>();
            FindChildrenControls<Slider>();
        }

        /// <summary>
        /// 搜索所有子物体的特定UI组件并添加进字典容器中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void FindChildrenControls<T>() where T : UIBehaviour
        {
            //搜索所有子物体的组件
            T[] controls = this.GetComponentsInChildren<T>();
            for (int i = 0; i < controls.Length; i++)
            {
                //添加组件进字典
                string objName = controls[i].gameObject.name;

                if (_controlDic.ContainsKey(objName))
                    _controlDic[objName].Add(controls[i]);
                else
                    _controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
            
                //组件添加事件监听
                if(controls[i] is Button btn)
                {
                    btn.onClick.AddListener(() =>
                    {
                        OnClick(objName);
                    });
                }
                if (controls[i] is Toggle tog)
                {
                    tog.onValueChanged.AddListener((isToggle) =>
                    {
                        OnToggleValueChanged(objName, isToggle);
                    });
                }
                if (controls[i] is Slider sld)
                {
                    sld.onValueChanged.AddListener((val) =>
                    {
                        OnSliderValueChanged(objName, val);
                    });
                }
                if (controls[i] is InputField ifd)
                {
                    ifd.onValueChanged.AddListener((val) =>
                    {
                        OnInputFieldValueChanged(objName, val);
                    });
                }
            }
        }

        /// <summary>
        /// 获得物体挂载的UI组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="name">物体名</param>
        /// <returns></returns>
        public T GetControl<T>(string name) where T : UIBehaviour
        {
            //每个物体只会挂载一个同种类的组件，所以不会重复
            if (_controlDic.ContainsKey(name))
            {
                for (int i = 0; i < _controlDic[name].Count; i++)
                {
                    if (_controlDic[name][i] is T)
                        return _controlDic[name][i] as T;
                }
            }
            print(name + " No this UIControl");
            return null;
        }

        #region 子类重写方法
        /// <summary>
        /// 面板显示时调用
        /// </summary>
        /// <param name="isFade">是否开启渐变</param>
        public virtual void OnShow(bool isFade = true)
        {
            if (isFade)
            {
                StopAllCoroutines();
                StartCoroutine(FadeInOut(true));
            }

        }

        /// <summary>
        /// 面板隐藏时调用
        /// </summary>
        /// <param name="isFade">是否开启渐变</param>
        public virtual void OnHide(bool isFade = true)
        {
            if (isFade)
            {
                StopAllCoroutines();
                StartCoroutine(FadeInOut(false));
            }

        }

        protected virtual void OnClick(string btnName)
        {

        }

        protected virtual void OnToggleValueChanged(string togName, bool isToggle)
        {

        }

        protected virtual void OnSliderValueChanged(string sldName, float val)
        {

        }

        protected virtual void OnInputFieldValueChanged(string inputName, string val)
        {

        }
        #endregion

        /// <summary>
        /// 面板渐变显示隐藏协程
        /// </summary>
        /// <param name="isIn">是否显示</param>
        /// <returns></returns>
        IEnumerator FadeInOut(bool isIn)
        {
            CanvasGroup canvasGroup = this.GetComponent<CanvasGroup>();
            if (isIn)
            {
                canvasGroup.alpha = 0;
                while(canvasGroup.alpha < 1)
                {
                    canvasGroup.alpha += FADE_SPEED;
                    yield return null;
                }
            }
            else
            {
                canvasGroup.alpha = 1;
                while (canvasGroup.alpha > 0)
                {
                    canvasGroup.alpha -= FADE_SPEED;
                    yield return null;
                }
            }
        }
    }
}