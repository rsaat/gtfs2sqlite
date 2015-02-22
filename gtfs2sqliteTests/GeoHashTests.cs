using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtfs2Sqlite.Util;
using NUnit.Framework;

namespace gtfs2sqliteTests
{
     [TestFixture]
    public class GeoHashTests
    {
        const double TestLat = -23.480017;
        const double TestLong = -46.603199;
        const string GeohashTest = "6gyf7t8dfz84";
        const int Precision = 13;

         [Test]
         public void EncodeTest()
         {
             string hash = Geohash.Encode(TestLat, TestLong, Precision);
             Assert.That(hash, Is.EqualTo(GeohashTest));
         }

         [Test]
         public void DecodeTest()
         {
              var coordinates = Geohash.Decode(GeohashTest);
              Assert.That(coordinates[0], Is.EqualTo(TestLat).Within(0.0000001));
              Assert.That(coordinates[1], Is.EqualTo(TestLong).Within(0.0000001));
         }


         [Test]
         public void CalculateAdjacentTest()
        {
            //Result from 
            //http://openlocation.org/geohash/geohash-js/
            //-23.480048 -46.603211 6gyf7t8df3z1 6
            //  LEFT(geohash,6) IN ('6gyf7w','6gyf7s','6gyf7v','6gyf7m','6gyf7q','6gyf7y','6gyf7u','6gyf7k','6gyf7t')
            // -23.48, -46.603 [w:2.32km, h:1.65km] (3.85km2)
           
            // Calculate hash with full precision
            string hash = Geohash.Encode(-23.480048, -46.603211);

         
            // Print neighbours
             hash = hash.Substring(0, 6);
             var top = Geohash.CalculateAdjacent(hash, Geohash.Direction.Top);
             var left = Geohash.CalculateAdjacent(hash, Geohash.Direction.Left);
             var right = Geohash.CalculateAdjacent(hash, Geohash.Direction.Right);
             var bottom = Geohash.CalculateAdjacent(hash, Geohash.Direction.Bottom);
             var topleft = Geohash.CalculateAdjacent(top, Geohash.Direction.Left);
             var topright = Geohash.CalculateAdjacent(top, Geohash.Direction.Right);
             var bottomright = Geohash.CalculateAdjacent(bottom, Geohash.Direction.Right);
             var bottomleft = Geohash.CalculateAdjacent(bottom, Geohash.Direction.Left);

             Debug.WriteLine("{0} \t: {1}", "T", top);
             Debug.WriteLine("{0} \t: {1}", "L", left);
             Debug.WriteLine("{0} \t: {1}", "R", right);
             Debug.WriteLine("{0} \t: {1}", "B", bottom);

             var coordTl = Geohash.Decode(topleft);
             var coordTr = Geohash.Decode(topright);
             var coordBr = Geohash.Decode(bottomright);
             var coordBl = Geohash.Decode(bottomleft);

             Debug.WriteLine("{0} \t: {1} Lat:{2} Long{3}", "TL", topleft, coordTl[0], coordTl[1]);
             Debug.WriteLine("{0} \t: {1} Lat:{2} Long{3}", "TR", topright, coordTr[0], coordTr[1]);
             Debug.WriteLine("{0} \t: {1} Lat:{2} Long{3}", "BR", bottomright, coordBr[0], coordBr[1]);
             Debug.WriteLine("{0} \t: {1} Lat:{2} Long{3}", "BL", bottomleft, coordBl[0], coordBl[1]);


             Assert.That(hash, Is.EqualTo("6gyf7t"));

             Assert.That(top, Is.EqualTo("6gyf7w"));
             Assert.That(left, Is.EqualTo("6gyf7m"));
             Assert.That(right, Is.EqualTo("6gyf7v"));
             Assert.That(bottom, Is.EqualTo("6gyf7s"));


             Assert.That(topleft, Is.EqualTo("6gyf7q"));
             Assert.That(topright, Is.EqualTo("6gyf7y"));
             Assert.That(bottomright, Is.EqualTo("6gyf7u"));
             Assert.That(bottomleft, Is.EqualTo("6gyf7k"));

        }
    }
}
