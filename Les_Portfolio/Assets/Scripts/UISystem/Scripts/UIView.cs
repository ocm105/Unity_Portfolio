

namespace UISystem
{
    public abstract class UIView : UILayer
    {
        public override UILayerTypes LayerType { get { return UILayerTypes.View; } }
        public string Background;
        public string Header;
        public string Footer;
        public string BgmKey = "BGM_MAIN";

        protected internal override void OnEscapePressed()
        {
            //UIManager.Instance.GoBack();
        }
    }
}
