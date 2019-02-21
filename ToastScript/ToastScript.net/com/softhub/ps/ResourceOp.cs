namespace com.softhub.ps
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

	internal sealed class ResourceOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"defineresource", "undefineresource", "findresource", "resourcestatus", "resourceforall", "findencoding"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(ResourceOp));
		}

		internal static void defineresource(Interpreter ip)
		{
			Any category = ip.ostack.pop(Types_Fields.STRING | Types_Fields.NAME);
			Any instance = popInstance(ip, category.ToString());
			Any key = ip.ostack.pop();
			DictType resources = ip.Resources;
			DictType dict = (DictType) resources.get(category);
			if (dict == null)
			{
				DictType catdict = (DictType) resources.get("Category");
				dict = (DictType) catdict.get("Generic");
			}
			DictType statusdict = ip.StatusDict;
			Any handler = statusdict.get("define" + category + "Resource");
			if (handler != null)
			{
				handler.cvx();
				ip.ostack.pushRef(instance);
				ip.ostack.pushRef(key);
				ip.estack.run(ip, handler);
				if (ip.ostack.popBoolean())
				{
					ip.ostack.pushRef(category);
					ip.ostack.pushRef(instance);
					ip.ostack.pushRef(key);
					throw new Stop(Stoppable_Fields.UNDEFINEDRESOURCE, category.ToString());
				}
			}
			dict.put(ip.vm, key, instance);
			ip.ostack.push(instance);
		}

		internal static void undefineresource(Interpreter ip)
		{
			string category = ip.ostack.popString();
			Any key = ip.ostack.pop();
			DictType resources = ip.Resources;
			resources.remove(ip.vm, key);
		}

		internal static void findresource(Interpreter ip)
		{
			Any category = ip.ostack.pop(Types_Fields.STRING | Types_Fields.NAME);
			Any key = ip.ostack.pop();
			DictType resources = ip.Resources;
			DictType cat = (DictType) resources.get(category);
			if (cat == null)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESOURCE, category.ToString());
			}
			Any instance = cat.get(key);
			if (instance != null)
			{
				ip.ostack.push(instance);
			}
			else if (!handleFindResource(ip, category.ToString(), key))
			{
				// handler has altered the stack -> restore
				ip.ostack.pushRef(key);
				ip.ostack.pushRef(category);
				throw new Stop(Stoppable_Fields.UNDEFINEDRESOURCE, category.ToString());
			}
		}

		internal static void findencoding(Interpreter ip)
		{
			Any encoding = ip.ostack.pop(Types_Fields.STRING | Types_Fields.NAME);
			DictType resources = ip.Resources;
			DictType cat = (DictType) resources.get("Encoding");
			ArrayType array = (ArrayType) cat.get(encoding, Types_Fields.ARRAY);
			if (array != null)
			{
				ip.ostack.push(array);
			}
			else if (!handleFindResource(ip, "Encoding", encoding))
			{
				// handler has altered the stack -> restore
				ip.ostack.pushRef(encoding);
				throw new Stop(Stoppable_Fields.UNDEFINEDRESOURCE, encoding.ToString());
			}
		}

		internal static void resourceforall(Interpreter ip)
		{
			string category = ip.ostack.popString();
			Any scratch = ip.ostack.pop();
			ArrayType proc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			Any template = ip.ostack.pop();
		}

		internal static void resourcestatus(Interpreter ip)
		{
			Any category = ip.ostack.pop(Types_Fields.STRING | Types_Fields.NAME);
			Any key = ip.ostack.pop();
			DictType resources = ip.Resources;
			DictType cat = (DictType) resources.get(category);
			if (cat == null)
			{
				throw new Stop(Stoppable_Fields.UNDEFINED, category.ToString());
			}
			Any rsrc = cat.get(category);
			if (rsrc != null)
			{
				// TODO: implement correct status
				ip.ostack.pushRef(new IntegerType(0)); // status
				ip.ostack.pushRef(new IntegerType(0)); // size
				ip.ostack.push(BoolType.TRUE);
			}
			else
			{
				ip.ostack.push(BoolType.FALSE);
			}
		}

		private static Any popInstance(Interpreter ip, string category)
		{
			Any instance;
			if ("Font".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.DICT);
			}
			else if ("Encoding".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.ARRAY);
			}
			else if ("Form".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.DICT);
			}
			else if ("Pattern".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.DICT);
			}
			else if ("ProcSet".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.DICT);
			}
			else if ("ColorSpace".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.ARRAY);
			}
			else if ("Halftone".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.DICT);
			}
			else if ("ColorRendering".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.DICT);
			}
			else if ("Filter".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.NAME | Types_Fields.STRING);
			}
			else if ("ColorSpaceFamily".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.NAME | Types_Fields.STRING);
			}
			else if ("Emulator".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.NAME | Types_Fields.STRING);
			}
			else if ("IODevice".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.STRING);
			}
			else if ("ColorRenderingType".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.INTEGER);
			}
			else if ("FMapType".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.INTEGER);
			}
			else if ("FontType".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.INTEGER);
			}
			else if ("FormType".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.INTEGER);
			}
			else if ("HalftoneType".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.INTEGER);
			}
			else if ("ImageType".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.INTEGER);
			}
			else if ("PatternType".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.INTEGER);
			}
			else if ("Category".Equals(category))
			{
				instance = ip.ostack.pop(Types_Fields.DICT);
			}
			else
			{
				instance = ip.ostack.pop();
			}
			return instance;
		}

		internal static bool handleFindResource(Interpreter ip, string category, Any key)
		{
			DictType statusdict = ip.StatusDict;
			Any handler = statusdict.get("find" + category + "Resource");
			bool found = false;
			if (handler != null)
			{
				handler.cvx();
				ip.ostack.pushRef(key);
				ip.estack.run(ip, handler);
				found = ip.ostack.popBoolean();
			}
			return found;
		}

	}

}