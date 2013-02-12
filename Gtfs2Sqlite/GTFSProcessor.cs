using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Ionic;
using Ionic.Zip;
using LINQtoCSV;
using Mono.Data.Sqlite;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using Gtfs2Sqlite.Entities;
using Gtfs2Sqlite;

namespace Gtfs2Sqlite
{
	public class GTFSProcessor
	{
		private Dictionary<string, Action<String, Stream>> _readers;

		public GTFSProcessor ()
		{
			_readers = new Dictionary<string, Action<String, Stream>> (){
			{"agency.txt",Process<Agency>},
			{"stops.txt",Process<Stop>},
			{"routes.txt",Process<Route>},
			{"trips.txt",Process<Trip>},
			{"stop_times.txt",ProcessStopTime},
			{"calendar.txt",Process<Calendar>},
			{"calendar_dates.txt",Process<CalendarDate>},
			{"fare_attributes.txt",Process<FareAttribute>},
			{"fare_rules.txt",Process<FareRule>},
			{"shapes.txt",ProcessShape},
			{"frequencies.txt",Process<Frequency>},
			{"transfers.txt",Process<Transfer>},
			{"feed_info.txt",Process<FeedInfo>}
		};
		}

		public void Package (string sourceUrl, string outputDb)
		{
			//var wc = new WebClient ();
			//var file = wc.OpenRead (sourceUrl);
			//var buffer = new byte[file.Length];
			//file.Read (buffer, 0, buffer.Length);
			try {
				using (var zipFile = ZipFile.Read(sourceUrl)) {
					foreach (var entry in zipFile.Entries) {
						using (var reader  =  entry.OpenReader()) {
							using (var ms = new MemoryStream(reader.ReadFully())) {
								_readers [entry.FileName] (outputDb, ms);
							}
						}

					}
				}

				Process (outputDb, SurrogateKeyRegistry<Headsign>.GetAll ());
				Process (outputDb, SurrogateKeyRegistry<StopDescription>.GetAll ());
				Process (outputDb, SurrogateKeyRegistry<StopName>.GetAll ());

			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
				Console.WriteLine (ex.StackTrace);
			}
		}

		private void Process<T> (string connection, Stream stream) where T:class, new()
		{
			var objs = new CsvContext ().Read<T> (new StreamReader (stream)).ToArray ();
			Process (connection, objs);
		}

		private void ProcessStopTime (string connection, Stream stream)
		{
			var objs = new CsvContext ().Read<StopTime> (new StreamReader (stream)).ToArray ();
			var stopTimes =
				objs.Where (k => k.trip.HasValue)
				.ToLookup (j => j.trip)
				.Select (l => new TripStops (){
  				TripId = l.Key,
				Stops = l.OrderBy(k=>k.sequence)
					.SelectMany(r=> BitConverter.GetBytes(r.stop_id)
					            .Concat(BitConverter.GetBytes(r.departure))
						            ).ToArray()
			}).ToArray ();

			Process (connection, stopTimes);
		}

		///<summary>
		/// Packs the shape into a smaller hunk (coordinates are stored in binary) 
		///</summary>
		private void ProcessShape (string connection, Stream stream)
		{
			var objs = new CsvContext ().Read<Shape> (new StreamReader (stream)).ToArray ();
			var saveObjects = objs.ToLookup (k => k.shape_id)
				.Select (shapeGroup => new PackedShape{ ShapeName = shapeGroup.Key, 
					Coordinates = shapeGroup
						.OrderBy(l=>l.shape_pt_sequence)
						.SelectMany(shape=> BitConverter.GetBytes(shape.shape_pt_lat)
							.Concat(BitConverter.GetBytes(shape.shape_pt_lon))).ToArray()
				}).ToArray ();
			Process (connection, saveObjects);
		}


		private void Process<T> (string connection, IEnumerable<T> objs) where T:class, new()
		{
			Console.WriteLine ("Processing: " + typeof(T).Name); 
			var factory = new OrmLiteConnectionFactory (connection, SqliteDialect.Provider);
			using (var db = factory.OpenDbConnection ()) {
				using (var tranny = db.BeginTransaction()) {
					db.CreateTable<T> (true);
					db.InsertAll (objs);
					tranny.Commit ();
				}
			}
		}

	}
}

