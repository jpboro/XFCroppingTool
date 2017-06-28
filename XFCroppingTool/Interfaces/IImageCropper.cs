using System;
using Xamarin.Forms;

namespace XFCroppingTool.Interfaces
{
	public interface IImageCropper
	{
		ImageSource CropImage(ImageSource imageSource, Rectangle cropRect, Size maxSize);
	}
}
