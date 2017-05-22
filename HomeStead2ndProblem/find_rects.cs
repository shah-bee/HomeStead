using System;
using System.Collections.Generic;

namespace FindRects
{

	/** Input aray type, typically the smallest integral type */
	/** Output array type, which consists of the width and height of any given rectangle at this point */
//C++ TO C# CONVERTER NOTE: Enums must be named in C#, so the following enum has been named AnonymousEnum:
	public enum AnonymousEnum
	{
		OUTPUT_MAX_LENGTH = USHRT_MAX
	}

	/** A rectangle, (x,y) is the upper-left corner, coordinates start at (0,0) */
	public class Rect
	{
		public int x;
		public int y;
		public int w;
		public int h;
		public Rect(int x, int y, int w, int h)
		{
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}
		/** Returns true if a point (x,y) is inside this rectangle */
		public bool isPointInside(int x, int y)
		{
			return x >= this.x != 0 && x < this.x + this.w && y >= this.y != 0 && y < this.y + this.h;
		}
	}

}



namespace FindRects
{

/* Algorithm was taken from here: http://stackoverflow.com/a/20039017/624766
   http://www.drdobbs.com/database/the-maximal-rectangle-problem/184410529
*/

public class Pair
{
	public int x;
	public int y;
	public Pair(int a, int b)
	{
		this.x = a;
		this.y = b;
	}
}

} // namespace
