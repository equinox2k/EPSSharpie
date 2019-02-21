using System;
using System.IO;

namespace com.softhub.ts
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


	public class PropertyProfile : Profile
	{

		private Properties properties;

		public PropertyProfile() : this(new Properties())
		{
		}

		public PropertyProfile(Properties properties)
		{
			this.properties = properties;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PropertyProfile(java.io.File file) throws java.io.FileNotFoundException, java.io.IOException
		public PropertyProfile(File file) : this(new Properties())
		{
			properties.load(new FileStream(file, FileMode.Open, FileAccess.Read));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PropertyProfile(String path) throws java.io.FileNotFoundException, java.io.IOException
		public PropertyProfile(string path) : this(new File(path))
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.File file, String title) throws java.io.FileNotFoundException, java.io.IOException
		public virtual void save(File file, string title)
		{
			properties.store(new FileStream(file, FileMode.Create, FileAccess.Write), title);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(String path, String title) throws java.io.FileNotFoundException, java.io.IOException
		public virtual void save(string path, string title)
		{
			save(new File(path), title);
		}

		public virtual void setString(string key, string value)
		{
			properties.setProperty(key, value);
		}

		public virtual string getString(string key, string defaultValue)
		{
			return properties.getProperty(key, defaultValue);
		}

		public virtual void setInteger(string key, int value)
		{
			properties.setProperty(key, value.ToString());
		}

		public virtual int getInteger(string key, int defaultValue)
		{
			int result = defaultValue;
			string s = properties.getProperty(key);
			if (!string.ReferenceEquals(s, null))
			{
				result = int.Parse(s);
			}
			return result;
		}

		public virtual void setFloat(string key, float value)
		{
			properties.setProperty(key, value.ToString());
		}

		public virtual float getFloat(string key, float defaultValue)
		{
			float result = defaultValue;
			string s = properties.getProperty(key);
			if (!string.ReferenceEquals(s, null))
			{
				result = Convert.ToSingle(s);
			}
			return result;
		}

	}

}