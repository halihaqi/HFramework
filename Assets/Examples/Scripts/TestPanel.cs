using Hali_Framework;
using UnityEngine.UI;

namespace Example
{
    public class TestPanel : PanelBase
    {
        private Text _input;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _input = GetControl<Text>("input");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _input.text = "乐乐乐";
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            _input.text = "得得得";
        }
    }
}