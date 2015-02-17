using System;
using ServiceStack.DataAnnotations;

namespace Gtfs2Sqlite
{
    public class ShapeCompressed
	{
		[PrimaryKey]
		public string shape_id { get; set; }
		public byte[] Coordinates { get; set; }
	}
}

