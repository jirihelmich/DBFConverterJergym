using System;
using System.Text;
using DbfDataReader;
using System.Collections.Generic;

namespace DBFConverterJergym
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to DBF converter for JerGym.");

            var path = GetPath();
            var data = ExctractScheduleData(path);
            WriteCsv(path, data); 
        }

        static string GetPath() {
            string path = null;
            while (!System.IO.File.Exists(path))
            {
                Console.WriteLine("Please, enter a valid path to the DBF file:");
                path = Console.ReadLine();
            }

            return path;
        }

        static ScheduleData ExctractScheduleData(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var options = new DbfDataReaderOptions
            {
                SkipDeletedRecords = true,
                Encoding = DbfDataReader.EncodingProvider.GetEncoding(852)
            };

            var list = new List<ScheduleEntry>();

            using (var dbfDataReader = new DbfDataReader.DbfDataReader(path, options))
            {
                while (dbfDataReader.Read())
                {
                    var entry = new ScheduleEntry {
                        Day = dbfDataReader.GetString(0),
                        Hour = dbfDataReader.GetString(1),
                        Class = dbfDataReader.GetString(3),
                        Group = dbfDataReader.GetString(4),
                        SubjectShortcut = dbfDataReader.GetString(5),
                        Subject = dbfDataReader.GetString(6),
                        TeacherFirstName = dbfDataReader.GetString(9),
                        TeacherLastName = dbfDataReader.GetString(8),
                        SpaceName = dbfDataReader.GetString(10)
                    };
                    list.Add(entry);
                }
            }

            return new ScheduleData
            {
                ScheduleEntries = list
            };
        }

        static void WriteCsv(string path, ScheduleData data)
        {
            var csvPath = path + ".csv";
            var header = ";;;;;;;;;;;";

            var builder = new StringBuilder();
            builder.AppendLine(header);
            foreach (var e in data.ScheduleEntries)
            {
                builder.AppendLine($"{e.Day};{e.Hour};{e.Class};{e.Group};{e.SubjectShortcut};{e.Subject};;{e.TeacherLastName};{e.TeacherFirstName};{e.SpaceName};;;");
            }


            System.IO.File.WriteAllText(csvPath, builder.ToString(), System.Text.Encoding.GetEncoding(852));
        }
    }
}
