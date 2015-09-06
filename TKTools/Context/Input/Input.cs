using OpenTK;
using OpenTK.Input;
using System;

namespace TKTools.Context.Input
{
	public enum KeyPrefix
	{
		None = 000,
		Control = 001,
		Alt = 010,
		Shift = 100,
		ControlAlt = 011,
		ControlShift = 101,
		AltShift = 110,
		ControlAltShift = 111
	}

	public class KeyboardWatch
	{
		public KeyboardWatch()
		{
		}

		public bool HasPrefix(KeyPrefix p)
		{
			int v = 0;

			if (this[Key.LControl])
				v += 1;
			if (this[Key.LAlt])
				v += 10;
			if (this[Key.LShift])
				v += 100;

			return (int)p == v;
		}

		public bool KeyPressed(Key k)
		{
			return GetKey(k) && !Keyboard.previous[k];
		}

		public bool KeyReleased(Key k)
		{
			return !GetKey(k) && Keyboard.previous[k];
		}

		public bool GetKey(Key k)
		{
			return Keyboard.current[k];
		}

		public bool this[Key k]
		{
			get { return GetKey(k); }
		}
	}

	public class MouseWatch
	{
		Camera perspectiveCamera;
		float planeDistance = 0f;

		public Camera Perspective
		{
			get { return perspectiveCamera; }
			set { perspectiveCamera = value; }
		}

		public float PlaneDistance
		{
			get { return planeDistance; }
			set { planeDistance = value; }
		}

		public float X
		{
			get { return Position.X; }
		}
		public float Y
		{
			get { return Position.Y; }
		}
		public float Wheel
		{
			get { return Mouse.current.Wheel; }
		}
		public float WheelDelta
		{
			get
			{
				return Wheel - Mouse.previous.Wheel;
			}
		}
		public Vector3 Position
		{
			get
			{
				if (perspectiveCamera == null)
					return new Vector3(ScreenPosition);
				else
				{
					Ray r = ScreenRay;

					Vector3 s = ScreenRay.Start;
					Vector3 d = ScreenRay.Direction;

					float z = s.Z - perspectiveCamera.Position.Z;
					d /= Math.Abs(d.Z);

					return s + d * (planeDistance + z);
				}
			}
		}
		public Vector2 ScreenPosition
		{
			get
			{
				return new Vector2(Mouse.current.X, Mouse.current.Y);
			}
		}

		public Ray ScreenRay
		{
			get
			{
				return perspectiveCamera.GetRayFromScreen(new Vector2(Mouse.current.X, Mouse.current.Y));
			}
		}

		public MouseWatch()
		{
		}

		public bool ButtonPressed(MouseButton mb)
		{
			return GetButton(mb) && !Mouse.previous[mb];
		}

		public bool ButtonReleased(MouseButton mb)
		{
			return !GetButton(mb) && Mouse.previous[mb];
		}

		public bool GetButton(MouseButton mb)
		{
			return Mouse.current[mb];
		}

		public bool this[MouseButton mb]
		{
			get { return GetButton(mb); }
		}
	}

	internal class Keyboard
	{
		internal static KeyboardState current, previous;
		internal static void Update()
		{
			previous = current;
			current = OpenTK.Input.Keyboard.GetState();
		}
	}

	internal class Mouse
	{
		internal struct TKMouseState
		{
			float x, y;
			MouseState state;

			internal float X
			{
				get { return x; }
			}

			internal float Y
			{
				get { return y; }
			}

			internal float Wheel
			{
				get { return state.WheelPrecise; }
			}

			public TKMouseState(float x, float y, MouseState state)
			{
				this.x = x;
				this.y = y;
				this.state = state;
			}

			public bool this[MouseButton mb]
			{
				get { return state[mb]; }
			}
		}

		internal static TKMouseState current, previous;
		internal static void Update(int x, int y, GameWindow w)
		{
			float xx = (x / (float)w.ClientSize.Width - 0.5f) * 2f;
			float yy = (y / (float)w.ClientSize.Height - 0.5f) * -2f;

			previous = current;
			current = new TKMouseState(xx, yy, OpenTK.Input.Mouse.GetState());
		}
	}
}