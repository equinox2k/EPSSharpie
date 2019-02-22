using System;
using ToastScriptNet;

namespace com.softhub.ps.device
{
	/// <summary>
	/// Copyright 1998 by Christian Lehner.
	/// 
	/// This file is part of ToastScript.
	/// 
	/// ToastScript is free software; you can redistribute it and/or modify
	/// it under the terms of the GNU General Public License as published by
	/// the Free Software Foundation; either version 2 of the License, or
	/// (at your option) any later version.
	/// 
	/// ToastScript is distributed in the hope that it will be useful,
	/// but WITHOUT ANY WARRANTY; without even the implied warranty of
	/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	/// GNU General Public License for more details.
	/// 
	/// You should have received a copy of the GNU General Public License
	/// along with ToastScript; if not, write to the Free Software
	/// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
	/// </summary>

	using CharacterShape = com.softhub.ps.graphics.CharacterShape;
	using DisplayList = com.softhub.ps.graphics.DisplayList;
	using Reusable = com.softhub.ps.graphics.Reusable;
	using Cache = com.softhub.ps.util.Cache;
	using CharWidth = com.softhub.ps.util.CharWidth;

	public class CacheDevice : DisplayListDevice
	{

		private Cache cache = new Cache(2048);
		private Device target;
		private string character;
		private CharWidth charWidth;
		private Rectangle2D bounds;

		/// <summary>
		/// Create a new cache device.
		/// </summary>
		public CacheDevice()
		{
		}

		/// <returns> a newly created display list </returns>
		protected internal override DisplayList createDisplayList()
		{
			return new CharacterShape();
		}

		/// <returns> the device name </returns>
		public override string Name
		{
			get
			{
				return "cachedevice";
			}
		}

		/// <summary>
		/// Set the device target. </summary>
		/// <param name="target"> the target device </param>
		public virtual Device Target
		{
			set
			{
				this.target = value;
			}
		}

		/// <summary>
		/// Set the character width. </summary>
		/// <param name="cw"> the character width </param>
		public virtual CharWidth CharWidth
		{
			set
			{
				this.charWidth = value;
			}
		}

		/// <summary>
		/// Set the character bounds. </summary>
		/// <param name="bounds"> the character bounding box </param>
		public virtual Rectangle2D CharBounds
		{
			set
			{
				this.bounds = value;
			}
		}

		/// <returns> the resolution in dots per inch </returns>
		public override float Resolution
		{
			get
			{
				return target.Resolution;
			}
		}

		/// <returns> the scale factor </returns>
		public override float Scale
		{
			get
			{
				return target.Scale;
			}
		}

		/// <returns> the default matrix </returns>
		public override AffineTransform DefaultMatrix
		{
			get
			{
				return target.DefaultMatrix;
			}
		}

		/// <returns> the current cache size </returns>
		public virtual int CacheSize
		{
			get
			{
				return cache.Size;
			}
		}

		/// <returns> the current cache size </returns>
		public virtual int MaxCacheSize
		{
			get
			{
				return cache.MaximumSize;
			}
			set
			{
				cache.MaximumSize = value;
			}
		}


		/// <summary>
		/// Clear the cache.
		/// </summary>
		public virtual void clearCache()
		{
			cache.clear();
		}

		/// <summary>
		/// Flush the cache device. </summary>
		/// <param name="info"> the font info </param>
		/// <param name="ctm"> the current transformation matrix </param>
		public virtual void flush(FontInfo info, AffineTransform ctm)
		{
			try
			{
				if (!string.ReferenceEquals(character, null))
				{
					displayList.trimToSize();
					CharacterShape charShape = (CharacterShape) displayList;
					charShape.CharCode = character;
					charShape.CharWidth = charWidth;
					charShape.normalize(ctm);
					CacheEntry key = new CacheEntry(info, character, charWidth, charShape);
					cache.put(key, key); // TODO: redundant, we should use set instead of map
					show(charShape, ctm);
				}
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine("warning: " + ex);
			}
			finally
			{
				character = null;
				charWidth = null;
				bounds = null;
			}
		}

		/// <summary>
		/// Show the cached object. </summary>
		/// <param name="info"> the font info </param>
		/// <param name="character"> the encoded character </param>
		public virtual CharWidth showCachedCharacter(FontInfo info, AffineTransform ctm, Point2D curpt, string character)
		{
			this.character = character;
			CacheEntry key = new CacheEntry(info, character);
			CacheEntry val = (CacheEntry) cache.get(key);
			if (val == null)
			{
				return null;
			}
			ctm.translate(curpt.X, curpt.Y);
			AffineTransform ftm = info.FontMatrix;
			ctm.concatenate(ftm);
			show(val.Object, ctm);
			return val.CharWidth.transform(ftm);
		}

		/// <summary>
		/// Show the cached object. </summary>
		/// <param name="obj"> the cached object </param>
		public override void show(Reusable obj, AffineTransform xform)
		{
			target.show(obj, xform);
		}

		/// <returns> the cliprect </returns>
		protected internal override Rectangle2D cliprect()
		{
			return bounds;
		}

		/// <returns> the device size </returns>
		public override Dimension Size
		{
			get
			{
				Dimension d = target.Size;
				float s = Resolution * Scale / 72f;
				int wbits = range(s * (float) bounds.Width, 1, d.width);
				int hbits = range(s * (float) bounds.Height, 1, d.height);
				return new Dimension(wbits, hbits);
			}
		}

		/// <summary>
		/// Assert round(val) is between min and max. </summary>
		/// <param name="val"> the value </param>
		/// <param name="min"> the minimum value </param>
		/// <param name="max"> the maximum value </param>
		/// <returns> value rounded in interval min <= val <= max </returns>
		private static int range(float val, int min, int max)
		{
			int ival = (int)Math.Round(val, MidpointRounding.AwayFromZero);
			if (ival < min)
			{
				return min;
			}
			if (ival > max)
			{
				return max;
			}
			return ival;
		}

	}

}