using OpenTK;
using OpenTK.Input;
using System;

namespace TKTools.Context.Input
{
	public class KeyboardWatch
	{
		public KeyboardWatch()
		{
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
		public float X
		{
			get { return Mouse.current.X; }
		}
		public float Y
		{
			get { return Mouse.current.Y; }
		}
		public Vector2 Position
		{
			get { return new Vector2(X, Y); }
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