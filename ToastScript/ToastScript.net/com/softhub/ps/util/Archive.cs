using System;
using System.IO;

namespace com.softhub.ps.util
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


	public class Archive
	{

		private string name;
		private string path;
		private ZipFile zipfile;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Archive(String name) throws java.io.IOException
		public Archive(string name)
		{
			this.name = name;
			int i, j = 0, n = name.Length;
			for (i = 0; i < n; i++)
			{
				char ch = name[i];
				if (ch == Path.DirectorySeparatorChar || ch == '/')
				{
					if (i > j)
					{
						string s = name.Substring(j, i - j);
						if (zipfile == null)
						{
							if (s.EndsWith(".zip", StringComparison.Ordinal))
							{
								zipfile = new ZipFile(name.Substring(0, i));
							}
							else if (s.EndsWith(".jar", StringComparison.Ordinal))
							{
								zipfile = new JarFile(name.Substring(0, i));
							}
							if (zipfile != null && i < n)
							{
								path = name.Substring(i + 1, n - (i + 1));
								break;
							}
						}
					}
					j = i + 1;
				}
			}
			if (zipfile == null || string.ReferenceEquals(path, null))
			{
				throw new IOException();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream openStream() throws java.io.IOException
		public virtual Stream openStream()
		{
			ZipEntry entry = zipfile.getEntry(path);
			if (entry == null)
			{
				throw new IOException(path + " not found in " + name);
			}
			return zipfile.getInputStream(entry);
		}

	}

}