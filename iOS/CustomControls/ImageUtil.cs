using System;
using CoreGraphics;
using UIKit;

namespace XFCroppingTool.iOS
{
public static class ImageUtil
{
	public static UIImage Rotate(UIImage image)
	{

		UIImage res;

		using (CGImage imageRef = image.CGImage)
		{
			CGImageAlphaInfo alphaInfo = imageRef.AlphaInfo;
			CGColorSpace colorSpaceInfo = CGColorSpace.CreateDeviceRGB();
			if (alphaInfo == CGImageAlphaInfo.None)
			{
				alphaInfo = CGImageAlphaInfo.NoneSkipLast;
			}

			nint width, height;

			width = imageRef.Width;
			height = imageRef.Height;
			//                int maxSize = Math.Max(width, height);
			nint maxSize = Convert.ToInt32(Math.Max(width, height));

			if (height >= width)
			{
				width = (int)Math.Floor((double)width * ((double)maxSize / (double)height));
				height = maxSize;
			}
			else
			{
				height = (int)Math.Floor((double)height * ((double)maxSize / (double)width));
				width = maxSize;
			}


			CGBitmapContext bitmap;

			if (image.Orientation == UIImageOrientation.Up || image.Orientation == UIImageOrientation.Down)
			{
				bitmap = new CGBitmapContext(IntPtr.Zero, width, height, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
			}
			else
			{
				bitmap = new CGBitmapContext(IntPtr.Zero, height, width, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
			}

			switch (image.Orientation)
			{
				case UIImageOrientation.Left:
					bitmap.RotateCTM((float)Math.PI / 2);
					bitmap.TranslateCTM(0, -height);
					break;
				case UIImageOrientation.Right:
					bitmap.RotateCTM(-((float)Math.PI / 2));
					bitmap.TranslateCTM(-width, 0);
					break;
				case UIImageOrientation.Up:
					break;
				case UIImageOrientation.Down:
					bitmap.TranslateCTM(width, height);
					bitmap.RotateCTM(-(float)Math.PI);
					break;
			}

			bitmap.DrawImage(new CGRect(0, 0, width, height), imageRef);


			res = UIImage.FromImage(bitmap.ToImage());
			bitmap = null;

		}


		return res;
	}

	public static BitmapRectScale GetBitmapBoundsInsideImage(UIImage bitmap, UIImageView imageview)
	{
		if (bitmap != null)
		{
			nfloat imageWidth = imageview.Bounds.Width;
			nfloat imageHeight = imageview.Bounds.Height;
			nfloat imageAspect = imageWidth / imageHeight;

			//Bitmap bitmap = CurrentBitmap;
			nfloat bWidth = bitmap.CGImage.Width;
			float bHeight = bitmap.CGImage.Height;
			nfloat bAspect = bWidth / bHeight;

			CGRect CropLimit = new CGRect();

			nfloat bsH = 0;
			nfloat bsW = 0;
			nfloat bs = 0;

			if (bAspect < imageAspect)
			{
				bs = bHeight / imageHeight;
				bsH = imageHeight;
				bsW = imageHeight * bAspect;
			}
			else
			{
				bs = bWidth / imageWidth;
				bsH = imageWidth / bAspect;
				bsW = imageWidth;
			}

			CropLimit.X = (float)(imageWidth - bsW) / 2;
			CropLimit.Y = (float)(imageHeight - bsH) / 2;
			CropLimit.Width = bsW;
			CropLimit.Height = bsH;

			var ret = new BitmapRectScale();
			ret.Rect = CropLimit;
			ret.Scale = bs;
			ret.OrigHeight = (int)bHeight;
			ret.OrigWidth = (int)bWidth;

			return ret;
		}
		else
			return null;
	}
}

	public class BitmapRectScale
{
	public nfloat Scale;
	public CGRect Rect;
	public int OrigHeight;
	public int OrigWidth;
	public BitmapRectScale() { }	
	}
}
