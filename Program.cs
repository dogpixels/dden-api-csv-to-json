using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace dden_csv_to_json
{
    class Program
    {
        static void Main(string[] args)
        {
            Result result = new Result();
            CsvHelper.Configuration.CsvConfiguration csvHelperConfig = new CsvHelper.Configuration.CsvConfiguration
            (
                cultureInfo: CultureInfo.InvariantCulture
            );

            string path = $"dealers_appdata_ef24.csv";
            string outpath = $"{path}.json";

            string art_placeholder = $"placeholder.jpg";
            string preview_placeholder = $"placeholder_preview.jpg";

            if (!File.Exists(path))
            {
                Console.WriteLine($"csv not found: \"{path}\". Aborting.");
                Environment.Exit(1);
            }

            Console.WriteLine($"found csv file: \"{path}\".");

            // read csv
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, csvHelperConfig))
            {
                csv.Configuration.BadDataFound = x =>
                {
                    Console.WriteLine($"bad data on line {x.Row}: {x.RawRecord}");
                };

                csv.Configuration.ReadingExceptionOccurred = exception =>
                {
                    Console.Write(exception);
                    return false;
                };

                csv.Configuration.Delimiter = ";";

                var records = csv.GetRecords<Artist>();
                result.artists = records.ToList();
            }

            // add image file names
            foreach (Artist artist in result.artists)
            {
                artist.thumbnail = preview_placeholder;
                artist.image = art_placeholder;

                if (File.Exists($"images/art_{artist.reg}_preview.jpg"))
                {
                    artist.thumbnail = $"art_{artist.reg}_preview.jpg";
                }

                if (File.Exists($"images/art_{artist.reg}.jpg"))
                {
                    artist.image = $"art_{artist.reg}.jpg";
                }
            }

            // write json
            string json = JsonSerializer.Serialize(result);

            Console.WriteLine($"[result] {json}");

            if (File.Exists(outpath))
                File.Delete(outpath);

            File.WriteAllText($"{path}.json", json);

            Console.WriteLine("Done.");
        }
    }

    public class Result
    {
        public List<Artist> artists { get; set; } = new List<Artist>();
    }

    public class Artist
    {
        [Index(0)]
        public string reg { get; set; }
        [Index(1)]
        public string nick { get; set; }
        [Index(2)]
        public string displayname { get; set; }
        [Index(3)]
        public string website { get; set; }
        [Index(4)]
        public string merchandise { get; set; }
        //[Index(5)]
        //private string attends_thu { get; set; }
        //[Index(6)]
        //private string attends_fri { get; set; }
        //[Index(7)]
        //private string attends_sat { get; set; }
        //[Index(8)]
        //public string allowUseOfData { get; set; }
        //[Index(9)]
        //public string afterdark { get; set; }
        [Index(10)]
        public string description { get; set; }
        //[Index(11)]
        //private string aboutTheArtist { get; set; }
        //[Index(12)]
        //private string aboutTheArt { get; set; }
        //[Index(13)]
        //private string web { get; set; }
        //[Index(14)]
        //private string twitter { get; set; }
        //[Index(15)]
        //private string telegram { get; set; }
        //[Index(16)]
        //private string discord { get; set; }
        //[Index(17)]
        //private string tweet { get; set; }
        //[Index(18)]
        //public string artPreviewCaption { get; set; }
        [Index(19)]
        public string thumbnail { get; set; }
        //[Index(20)]
        //private string artistImg { get; set; }
        [Index(21)]
        public string image { get; set; }
        //[Index(22)]
        //private string catPrints { get; set; }
        //[Index(23)]
        //private string catArtwork { get; set; }
        //[Index(24)]
        //private string catFursuit { get; set; }
        //[Index(25)]
        //private string catCommissions { get; set; }
        //[Index(26)]
        //private string catMisc { get; set; }
    }
}
