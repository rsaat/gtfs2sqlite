using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Gtfs2Sqlite
{
	class MainClass
	{
		public static void Main (string[] args)
		{
		    var ct = System.Threading.Thread.CurrentThread;
            ct.CurrentCulture = new CultureInfo("en-US");
			if (args.Length == 2) {
				var processor = new GTFSProcessor ();
				processor.Package (args [0], args [1]);
			} else {
				Console.WriteLine ("Gtfs2Sqlite usage: Gtfs2Sqlite.exe <gtfs.zip> <output.db>");
			}

		    Console.WriteLine("");
            Console.WriteLine("Conversion End. Type any key to close the program");
		    Console.ReadKey();
		}
	}
}
