using FindRects;
using System.Collections.Generic;

public static class GlobalMembers
{
		/** Find a biggest rectangle in a given two-dimensional array
			@param data: two-dimensional array, value 0 is empty area, any non-zero value is considered for searching
			@param width: a width of data array
			@param height: a height of data array
		*/
		public static Rect findBiggest(byte input, int width, int height)
		{
			return findBiggest(input, width, height, width);
		}

		/** Split an area into rectangles, trying to cover as much area as possible
			@param minLength: minimal length of the rectangle side, can be 1 to count every element in the array
			@param output: output array of width x height * 2, where each two elements are width/height of a rectangle's top-left corner, or 0 if the area is inside a rectangle
			@return total area size of all rectangles, covered by rectangles with side length = maxLength or bigger,
		*/
		public static long findAll(byte input, int width, int height, int minLength, ref ushort output)
		{
			return findRectsInArea(input, width, minLength, output, new Rect(0, 0, width, height));
		}

	internal static void update_cache(List<int> c, int n, int M, int rowWidth, byte[] input)
	{
		for (int m = 0; m != M; ++m)
		{
			if (input[n * rowWidth + m] != 0)
			{
				c[m]++;
			}
			else
			{
				c[m] = 0;
			}
		}
	}

	internal static Rect findBiggest(byte input, int M, int N, int rowWidth)
	{

		Pair best_ll = new Pair(0, 0); // Lower-left corner
		Pair best_ur = new Pair(-1, -1); // Upper-right corner
		int best_area = 0;
		int best_perimeter = 0;

		List<int> c = new List<int>(M + 1); // Cache
		List<Pair> s = new List<Pair>(); // Stack
		s.Capacity = M + 1;

		int m;
		int n;

		/* Main algorithm: */
		for (n = 0; n != N; ++n)
		{
			int open_width = 0;
			update_cache(c, n, M, rowWidth, input);
			for (m = 0; m != M + 1; ++m)
			{
				if (c[m] > open_width)
				{ // Open new rectangle?
					s.Add(new Pair(m, open_width));
					open_width = c[m];
				}
				else if (c[m] < open_width)
				{ // Close rectangle(s)?
					int m0;
					int w0;
					int area;
					int perimeter;
					do
					{
						m0 = s[s.Count - 1].x;
						w0 = s[s.Count - 1].y;
						s.RemoveAt(s.Count - 1);
						area = open_width * (m - m0);
						perimeter = open_width + (m - m0);
						/* If the area is the same, prefer squares over long narrow rectangles,
							it finds more rectangles this way when calling findAll() with minLength == 2 or more */
						if (area > best_area || (area == best_area && perimeter < best_perimeter))
						{
							best_area = area;
							best_perimeter = perimeter;
							best_ll.x = m0;
							best_ll.y = n;
							best_ur.x = m - 1;
							best_ur.y = n - open_width + 1;
						}
						open_width = w0;
					} while (c[m] < open_width);
					open_width = c[m];
					if (open_width != 0)
					{
						s.Add(new Pair(m0, w0));
					}
				}
			}
		}
		return new Rect(best_ll.x, Math.Max(0, best_ur.y), 1 + best_ur.x - best_ll.x, 1 + best_ll.y - best_ur.y);
	}

	//#define CHECK_BOTH_WAYS 1 /* This will make the algorithm terribly slow, with a factorial complexity */

	/** Find biggest rectangle, then recursively search area to the left/right and to the top/bottom of that rectangle
		for smaller rectangles, and choose the one that covers biggest area.
		@return biggest area size, covered by rectangles with side length = maxLength or bigger,
		@param search: limit searching for the following area
		@param output: may be NULL, then the function will only return the area size
	*/

	internal static long findRectsInArea(byte input, int rowWidth, int minLength, ushort[] output, Rect search)
	{
		if (search.w < minLength || search.h < minLength)
		{
			return 0; // We reached a size limit
		}
		Rect biggest = findBiggest(input + search.y * rowWidth + search.x, search.w, search.h, rowWidth);

		if (biggest.w < minLength || biggest.h < minLength)
		{
			return 0; // No rectangles here
		}
		biggest.x += search.x;
		biggest.y += search.y;
		if (biggest.w > AnonymousEnum.OUTPUT_MAX_LENGTH)
		{
			biggest.w = (int)AnonymousEnum.OUTPUT_MAX_LENGTH;
		}
		if (biggest.h > AnonymousEnum.OUTPUT_MAX_LENGTH)
		{
			biggest.h = (int)AnonymousEnum.OUTPUT_MAX_LENGTH;
		}
		/* We got two choices to split remaining area into four rectangular regions, where (B) is the biggest rectangle:
			****|***|***				************
			****|***|***				************
			****BBBBB***				----BBBBB---
			****BBBBB***		vs		****BBBBB***
			****BBBBB***				----BBBBB---
			****|***|***				************
			We are not filling the output array in the first recursive call, it's just for determining the resulting area size
		*/

		if (output != null)
		{
			for (int y = biggest.y; y < biggest.y + biggest.h; y++)
			{
				for (int x = biggest.x; x < biggest.x + biggest.w; x++)
				{
					output[(y * rowWidth + x) * 2] = 0;
					output[(y * rowWidth + x) * 2 + 1] = 0;
				}
			}
			output[(biggest.y * rowWidth + biggest.x) * 2] = biggest.w;
			output[(biggest.y * rowWidth + biggest.x) * 2 + 1] = biggest.h;
		}

	#if CHECK_BOTH_WAYS
		long splitHorizArea = findRectsInArea(input, rowWidth, minLength, null, new Rect(search.x, search.y, biggest.x - search.x, search.h)) + findRectsInArea(input, rowWidth, minLength, null, new Rect(biggest.x + biggest.w, search.y, search.x + search.w - biggest.x - biggest.w, search.h)) + findRectsInArea(input, rowWidth, minLength, null, new Rect(biggest.x, search.y, biggest.w, biggest.y - search.y)) + findRectsInArea(input, rowWidth, minLength, null, new Rect(biggest.x, biggest.y + biggest.h, biggest.w, search.y + search.h - biggest.y - biggest.h));

		long splitVertArea = findRectsInArea(input, rowWidth, minLength, null, new Rect(search.x, search.y, search.w, biggest.y - search.y)) + findRectsInArea(input, rowWidth, minLength, null, new Rect(search.x, biggest.y + biggest.h, search.w, search.y + search.h - biggest.y - biggest.h)) + findRectsInArea(input, rowWidth, minLength, null, new Rect(search.x, biggest.y, biggest.x - search.x, biggest.h)) + findRectsInArea(input, rowWidth, minLength, null, new Rect(biggest.x + biggest.w, biggest.y, search.x + search.w - biggest.x - biggest.w, biggest.h));

		/* Inefficiently perform the recursive call again, this time with non-NULL output array */
		if (splitHorizArea > splitVertArea)
		{
			if (output != null)
			{
	#endif
				return (long)biggest.w * biggest.h + findRectsInArea(input, rowWidth, minLength, output, new Rect(search.x, search.y, biggest.x - search.x, search.h)) + findRectsInArea(input, rowWidth, minLength, output, new Rect(biggest.x + biggest.w, search.y, search.x + search.w - biggest.x - biggest.w, search.h)) + findRectsInArea(input, rowWidth, minLength, output, new Rect(biggest.x, search.y, biggest.w, biggest.y - search.y)) + findRectsInArea(input, rowWidth, minLength, output, new Rect(biggest.x, biggest.y + biggest.h, biggest.w, search.y + search.h - biggest.y - biggest.h));
	#if CHECK_BOTH_WAYS
			}
			return splitHorizArea + (long)biggest.w * biggest.h;
		}
		else
		{
			if (output != null)
			{
				findRectsInArea(input, rowWidth, minLength, output, new Rect(search.x, search.y, search.w, biggest.y - search.y));
				findRectsInArea(input, rowWidth, minLength, output, new Rect(search.x, biggest.y + biggest.h, search.w, search.y + search.h - biggest.y - biggest.h));
				findRectsInArea(input, rowWidth, minLength, output, new Rect(search.x, biggest.y, biggest.x - search.x, biggest.h));
				findRectsInArea(input, rowWidth, minLength, output, new Rect(biggest.x + biggest.w, biggest.y, search.x + search.w - biggest.x - biggest.w, biggest.h));
			}
			return splitVertArea + (long)biggest.w * biggest.h;
		}
	#endif
	}

	public static byte[] testdata = {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0};

	static int Main()
	{
		FindRects.Rect ret = FindRects.findBiggest(testdata, AnonymousEnum2.MM, AnonymousEnum2.NN);
		Console.Write("The maximal rectangle has area {0:D}.\n", ret.w * ret.h);
		Console.Write("Location: x={0:D}, y={1:D}, w={2:D}, h={3:D}\n", ret.x, ret.y, ret.w, ret.h);
		for (int y = 0; y < AnonymousEnum2.NN; y++)
		{
			for (int x = 0; x < AnonymousEnum2.MM; x++)
			{
				Console.Write(" {0:D}{1}", testdata[y * AnonymousEnum2.MM + x], ret.isPointInside(x, y) ? "*" : " ");
			}
			Console.Write("\n");
		}
		ushort[] output = new ushort[(int)AnonymousEnum2.MM * AnonymousEnum2.NN * 2];
		long area;
		int numrects;

		/* The algorithm cannot find two 3x3 rectangles in the test data,
			because it finds 6x2 rectangle first, then discards it, because the rectangle height is too small. */
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		memset(output, 0, sizeof(ushort) * AnonymousEnum2.MM * AnonymousEnum2.NN * 2);
		area = FindRects.findAll(testdata, AnonymousEnum2.MM, AnonymousEnum2.NN, 3, output);
		numrects = 0;
		Console.Write("Searching for rects with side length 3 or more:\n", area);
		for (int y = 0; y < AnonymousEnum2.NN; y++)
		{
			for (int x = 0; x < AnonymousEnum2.MM; x++)
			{
				Console.Write(" {0:D},{1:D}", output[(y * AnonymousEnum2.MM + x) * 2], output[(y * AnonymousEnum2.MM + x) * 2 + 1]);
				if (output[(y * AnonymousEnum2.MM + x) * 2] > 0)
				{
					numrects++;
				}
			}
			Console.Write("\n");
		}
		Console.Write("Found {0:D} rects with total area {1:D}\n", numrects, area);
		fflush(stdout);

//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		memset(output, 0, sizeof(ushort) * AnonymousEnum2.MM * AnonymousEnum2.NN * 2);
		area = FindRects.findAll(testdata, AnonymousEnum2.MM, AnonymousEnum2.NN, 2, output);
		numrects = 0;
		Console.Write("Searching for rects with side length 2 or more:\n", area);
		for (int y = 0; y < AnonymousEnum2.NN; y++)
		{
			for (int x = 0; x < AnonymousEnum2.MM; x++)
			{
				Console.Write(" {0:D},{1:D}", output[(y * AnonymousEnum2.MM + x) * 2], output[(y * AnonymousEnum2.MM + x) * 2 + 1]);
				if (output[(y * AnonymousEnum2.MM + x) * 2] > 0)
				{
					numrects++;
				}
			}
			Console.Write("\n");
		}
		Console.Write("Found {0:D} rects with total area {1:D}\n", numrects, area);
		fflush(stdout);

//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		memset(output, 0, sizeof(ushort) * AnonymousEnum2.MM * AnonymousEnum2.NN * 2);
		area = FindRects.findAll(testdata, AnonymousEnum2.MM, AnonymousEnum2.NN, 1, output);
		Console.Write("Searching for rects with side length 1 or more:\n", area);
		for (int y = 0; y < AnonymousEnum2.NN; y++)
		{
			for (int x = 0; x < AnonymousEnum2.MM; x++)
			{
				Console.Write(" {0:D},{1:D}", output[(y * AnonymousEnum2.MM + x) * 2], output[(y * AnonymousEnum2.MM + x) * 2 + 1]);
				if (output[(y * AnonymousEnum2.MM + x) * 2] > 0)
				{
					numrects++;
				}
			}
			Console.Write("\n");
		}
		Console.Write("Found {0:D} rects with total area {1:D}\n", numrects, area);
		fflush(stdout);

		ushort[] verifydata = new ushort[(int)AnonymousEnum2.MM * AnonymousEnum2.NN];
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		memset(verifydata, 0, sizeof(ushort) * AnonymousEnum2.MM * AnonymousEnum2.NN);

		for (int y = 0; y < AnonymousEnum2.NN; y++)
		{
			for (int x = 0; x < AnonymousEnum2.MM; x++)
			{
				int w = output[(y * AnonymousEnum2.MM + x) * 2];
				int h = output[(y * AnonymousEnum2.MM + x) * 2 + 1];
				for (int yy = y; yy < y + h; yy++)
				{
					for (int xx = x; xx < x + w; xx++)
					{
						verifydata[yy * AnonymousEnum2.MM + xx] = 1;
					}
				}
			}
		}

		Console.Write("\n");

		for (int y = 0; y < AnonymousEnum2.NN; y++)
		{
			for (int x = 0; x < AnonymousEnum2.MM; x++)
			{
				if ((verifydata[y * AnonymousEnum2.MM + x] != 0) != (testdata[y * AnonymousEnum2.MM + x] != 0))
				{
					Console.Write("\nERROR: test data does not match calculated rectangle data at x={0:D}, y={1:D}\n", x, y);
					return 1;
				}
			}
		}

		verifydata = null;
		output = null;

		return 0;
	}
}