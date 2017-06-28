using System;
using System.IO;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using XFCroppingTool.Interfaces;
using XFCroppingTool.iOS.CustomControls;

[assembly: Dependency(typeof(iOSImageCropper))]
namespace XFCroppingTool.iOS.CustomControls
{
	public class iOSImageCropper : IImageCropper
	{
public ImageSource CropImage(ImageSource imageSource, Xamarin.Forms.Rectangle cropRect, Xamarin.Forms.Size MaxSize)
{
	UIImage CurrentBitmap = null;

	if (imageSource.GetType().Equals(typeof(FileImageSource)))
	{
		CurrentBitmap = new UIImage(((FileImageSource)imageSource).File);

	}

	if (imageSource.GetType().Equals(typeof(StreamImageSource)))
	{
		StreamImageSource iSource = (StreamImageSource)imageSource;
		Stream s = iSource.Stream(System.Threading.CancellationToken.None).Result;
		MemoryStream ms = new MemoryStream();
		s.Position = 0;
		s.CopyTo(ms);
		NSData data = NSData.FromArray(ms.ToArray());
		CurrentBitmap = UIImage.LoadFromData(data);
	}


	if (CurrentBitmap != null)
	{
		
		CurrentBitmap = ImageUtil.Rotate(CurrentBitmap);

		var CroppedImage = CurrentBitmap.CGImage.WithImageInRect(new CGRect((float)cropRect.X, (float)cropRect.Y, (float)cropRect.Width, (float)cropRect.Height));
		if (CroppedImage != null)
		{

			CurrentBitmap = UIImage.FromImage(CroppedImage);

			if (CurrentBitmap.CGImage.Width > MaxSize.Width || CurrentBitmap.CGImage.Height > MaxSize.Height)
			{
				double bAspect = (double)CurrentBitmap.CGImage.Width / CurrentBitmap.CGImage.Height;
				double imageAspect = (double)MaxSize.Width / MaxSize.Height;
				double bsH = 0;
				double bsW = 0;
				if (bAspect < imageAspect)
				{
					//bs = bHeight / imageHeight;
					bsH = MaxSize.Height;
					bsW = MaxSize.Height * bAspect;
				}
				else
				{
					//bs = bWidth / imageWidth;
					bsH = MaxSize.Width / bAspect;
					bsW = MaxSize.Width;
				}
				CroppedImage = CurrentBitmap.StretchableImage((int)bsW, (int)bsH).CGImage;
				CurrentBitmap = UIImage.FromImage(CroppedImage);
				//CroppedImage = Bitmap.CreateScaledBitmap(CurrentBitmap, (int)bsW, (int)bsH, true);
			}

			//imgImage.Image = CurrentBitmap;

			//NSData imageData = CurrentBitmap.AsPNG();

			NSData imageData = CurrentBitmap.AsJPEG(0.5f);

			Byte[] imgByteArray = new Byte[imageData.Length];

			System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, imgByteArray, 0, Convert.ToInt32(imageData.Length));

			MemoryStream ms = new MemoryStream(imgByteArray);

			//cropperView.Origin = new PointF(imageView.Frame.Left, imageView.Frame.Top);

			return ImageSource.FromStream(() => { return ms; });
		}
		else { return null; }
	}
	else { return null; }

	}
	}
}
