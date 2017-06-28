using System;
using CoreGraphics;
using UIKit;

namespace XFCroppingTool.iOS.CustomControls
{
	public class CropView : UIView
	{
		UIPanGestureRecognizer gestPan;
		UIPinchGestureRecognizer gestPinch;

		private CGPoint _origin;
		private CGSize _size;
		//private float _initCropperSize = 100;

		private nfloat _initCropperSize = 100;


		private nfloat _scaleFactor;
		public event EventHandler RectChange;

		private CGRect _croplimit;
		public CGRect CropLimit
		{
			get { return _croplimit; }
			set { _croplimit = value; }
		}
		public CGRect CropValue
		{
			get
			{

				return new CGRect(_origin, _size);
				//return new RectangleF((int)_posX, (int)_posY, (int)(_posX + (_scaleFactor * _initCropperSize)), (int)(_posY + (_scaleFactor * _initCropperSize)));
			}
		}


		public CropView()
		{

			this._origin = new CGPoint(0, 0);
			this._size = new CGSize(0, 0);
			this._scaleFactor = 1;
			this.Alpha = 0.4f;
			nfloat dx = 0;
			nfloat dy = 0;
			gestPan = new UIPanGestureRecognizer((UIPanGestureRecognizer gesture) =>
				{
			//                    var gesture = new UIPanGestureRecognizer();

			if ((gesture.State == UIGestureRecognizerState.Began || gesture.State == UIGestureRecognizerState.Changed) && (gesture.NumberOfTouches == 1))
					{
						var p0 = gesture.LocationInView(this);

						if (dx == 0)
							dx = p0.X - this._origin.X;

						if (dy == 0)
							dy = p0.Y - this._origin.Y;

						var p1 = new CGPoint(p0.X - dx, p0.Y - dy);

				//rectCrop.Frame = new RectangleF(p0, new SizeF(rectCrop.Frame.Width, rectCrop.Frame.Height));
				this._origin = p1;
						RectChange(this, EventArgs.Empty);
						this.SetNeedsDisplay();
					}
					else if (gesture.State == UIGestureRecognizerState.Ended)
					{
						dx = 0;
						dy = 0;
					}
				});

			//            gestPan = new UIPanGestureRecognizer();
			//
			//            gestPan.AddTarget(() => HandlePan(gestPan));

			nfloat s0 = 1;

			//            gestPinch = new UIPinchGestureRecognizer();
			//            gestPinch.AddTarget(() => HandlePinch(gestPinch));
			//            var gesturePinch = new UIPinchGestureRecognizer();

			gestPinch = new UIPinchGestureRecognizer((UIPinchGestureRecognizer gesturePinch) =>
				{

			//                nfloat s = gesture.Scale;
			nfloat s = gesturePinch.Scale;
					nfloat ds = (nfloat)Math.Abs(s - s0);
					nfloat sf = 0;
					const float rate = 0.5f;

					if (s >= s0)
					{
						sf = 1 + ds * rate;
					}
					else if (s < s0)
					{
						sf = 1 - ds * rate;
					}
					s0 = s;

					_scaleFactor *= sf;
			//this._size = new SizeF(this._size.Width * sf, this._size.Height * sf);
			RectChange(this, EventArgs.Empty);
					this.SetNeedsDisplay();
			//                if (gesture.State == UIGestureRecognizerState.Ended)
			if (gesturePinch.State == UIGestureRecognizerState.Ended)
					{
						s0 = 1;
					}
				});
			this.AddGestureRecognizer(gestPan);
			this.AddGestureRecognizer(gestPinch);
			this.SetNeedsDisplay();
		}

		private void HandlePan(UIPanGestureRecognizer gesture)
		{
			nfloat dx = 0;
			nfloat dy = 0;

			if ((gesture.State == UIGestureRecognizerState.Began || gesture.State == UIGestureRecognizerState.Changed) && (gesture.NumberOfTouches == 1))
			{
				var p0 = gesture.LocationInView(this);

				if (dx == 0)
					dx = p0.X - this._origin.X;

				if (dy == 0)
					dy = p0.Y - this._origin.Y;

				var p1 = new CGPoint(p0.X - dx, p0.Y - dy);

				//rectCrop.Frame = new RectangleF(p0, new SizeF(rectCrop.Frame.Width, rectCrop.Frame.Height));
				this._origin = p1;
				RectChange(this, EventArgs.Empty);
				this.SetNeedsDisplay();
			}
			else if (gesture.State == UIGestureRecognizerState.Ended)
			{
				dx = 0;
				dy = 0;
			}

		}

		private void HandlePinch(UIPinchGestureRecognizer gesture)
		{
			nfloat s0 = 1;

			nfloat s = gesture.Scale;
			nfloat ds = (nfloat)Math.Abs(s - s0);
			nfloat sf = 0;
			const float rate = 0.5f;

			if (s >= s0)
			{
				sf = 1 + ds * rate;
			}
			else if (s < s0)
			{
				sf = 1 - ds * rate;
			}
			s0 = s;

			_scaleFactor *= sf;
			//this._size = new SizeF(this._size.Width * sf, this._size.Height * sf);
			RectChange(this, EventArgs.Empty);
			this.SetNeedsDisplay();
			if (gesture.State == UIGestureRecognizerState.Ended)
			{
				s0 = 1;
			}

		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			if (_size.Width == 0)
			{
				_scaleFactor = 1.5f;
				_origin.X = (this.Frame.Width - (_initCropperSize * _scaleFactor)) / 2;
				_origin.Y = (this.Frame.Height - (_initCropperSize * _scaleFactor)) / 2;

				RectChange(this, EventArgs.Empty);

			}

			_size.Width = _initCropperSize * _scaleFactor;
			_size.Height = _initCropperSize * _scaleFactor;

			if (_croplimit != null)
			{
				if (_origin.X < _croplimit.Left) _origin.X = _croplimit.Left;
				if (_origin.Y < _croplimit.Top) _origin.Y = _croplimit.Top;
				if (_origin.X + (_initCropperSize * _scaleFactor) > _croplimit.Right) _origin.X = _croplimit.Right - (_initCropperSize * _scaleFactor);
				if (_origin.Y + (_initCropperSize * _scaleFactor) > _croplimit.Bottom) _origin.Y = _croplimit.Bottom - (_initCropperSize * _scaleFactor);
			}

			RectChange(this, EventArgs.Empty);

			using (CGContext g = UIGraphics.GetCurrentContext())
			{
				g.SetFillColor(UIColor.Red.CGColor);
				g.FillRect(rect);


				g.SetBlendMode(CGBlendMode.Normal);
				g.SetStrokeColor(UIColor.Yellow.CGColor);
				var path2 = new CGPath();
				path2.AddRect(new CGRect(_origin, _size));
				g.AddPath(path2);
				g.DrawPath(CGPathDrawingMode.Stroke);

				g.SetBlendMode(CGBlendMode.Clear);
				UIColor.Clear.SetColor();

				var path = new CGPath();
				path.AddRect(new CGRect(_origin, _size));

				g.AddPath(path);
				g.DrawPath(CGPathDrawingMode.Fill);


			}

		}
	}
}
