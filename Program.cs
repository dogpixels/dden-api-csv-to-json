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
            string outpath = $"dealers.json";

            string art_placeholder = $"placeholder.jpg";
            string preview_placeholder = $"placeholder_preview.jpg";

            int stat_website_overridden = 0;

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

            foreach (Artist artist in result.artists)
            {
                // add image file names
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

                if (artist.website == "" && artist.website_override != "")
                    Console.WriteLine($"Artist with no website reg but website: {artist.nick}");

                // fix website link
                if (!artist.website_override.Equals(string.Empty))
                {
                    artist.website = artist.website_override;
                    artist.website_override = string.Empty;
                    stat_website_overridden++;
                }
            }

            // write json
            string json = JsonSerializer.Serialize(result);

            //Console.WriteLine($"[result] {json}");

            if (File.Exists(outpath))
                File.Delete(outpath);

            File.WriteAllText(outpath, json);

            Console.WriteLine($"[Stat] Website overriden: {stat_website_overridden}");
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
        public string reg { get; set; }                     // @CSV Reg No.
        [Index(1)]
        public string nick { get; set; }                    // @CSV Nick
        [Index(2)]
        public string displayname { get; set; }             // @CSV Display Name
        [Index(3)]
        public string website { get; set; }                 // @CSV Website Reg
        [Index(4)]
        public string merchandise { get; set; }             // @CSV Merchandise
        //[Index(5)]
        //private string attends_thu { get; set; }          // @CSV Attends Thu
        //[Index(6)]
        //private string attends_fri { get; set; }          // @CSV Attends Fri
        //[Index(7)]
        //private string attends_sat { get; set; }          // @CSV Attends Sat
        //[Index(8)]
        //public string allowUseOfData { get; set; }        // @CSV Allows Use of Data
        //[Index(9)]
        //public string afterdark { get; set; }             // @CSV After Dark
        [Index(10)]
        public string description { get; set; }             // @CSV Short Description
        //[Index(11)]
        //private string aboutTheArtist { get; set; }       // @CSV About the Artist
        //[Index(12)]
        //private string aboutTheArt { get; set; }          // @CSV About the Art
        [Index(13)]
        public string website_override { get; set; }        // @CSV Website
        //[Index(14)]
        //private string twitter { get; set; }              // @CSV Twitter
        //[Index(15)]
        //private string telegram { get; set; }             // @CSV Telegram
        //[Index(16)]
        //private string discord { get; set; }              // @CSV Discord
        //[Index(17)]
        //private string tweet { get; set; }                // @CSV Tweet
        //[Index(18)]
        //public string artPreviewCaption { get; set; }     // @CSV Art Preview Caption
        [Index(19)]
        public string thumbnail { get; set; }               // @CSV ThumbnailImg
        //[Index(20)]
        //private string artistImg { get; set; }            // @CSV ArtistImg
        [Index(21)]
        public string image { get; set; }                   // @CSV ArtImg
        //[Index(22)]
        //private string catPrints { get; set; }            // @CSV Cat. Prints
        //[Index(23)]
        //private string catArtwork { get; set; }           // @CSV Cat. Artwork
        //[Index(24)]
        //private string catFursuit { get; set; }           // @CSV Cat. Fursuit
        //[Index(25)]
        //private string catCommissions { get; set; }       // @CSV Cat. Commissions
        //[Index(26)]
        //private string catMisc { get; set; }              // @CSV Cat. Misc
    }
}
