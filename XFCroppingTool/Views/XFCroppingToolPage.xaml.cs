using Xamarin.Forms;

namespace XFCroppingTool
{
    public partial class XFCroppingToolPage : ContentPage
    {
        public XFCroppingToolPage()
        {
            InitializeComponent();
			this.BindingContext = new CropperViewModel();
        }
    }
}
