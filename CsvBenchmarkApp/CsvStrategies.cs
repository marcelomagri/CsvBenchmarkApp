using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

public static class CsvStrategies
{
    public static DataTable ReadWithStreamReader(string path, string colSep)
    {
        var dt = new DataTable();
        using var reader = new StreamReader(path);
        bool isHeader = true;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var cols = line.Split(colSep);

            if (isHeader)
            {
                foreach (var col in cols)
                    dt.Columns.Add(col.Trim());
                isHeader = false;
            }
            else
                dt.Rows.Add(cols);
        }

        return dt;
    }

    public static DataTable ReadWithAllText(string path, string lineSep, string colSep)
    {
        var dt = new DataTable();
        var lines = File.ReadAllText(path).Split(new[] { lineSep }, StringSplitOptions.None);
        bool isHeader = true;

        foreach (var line in lines)
        {
            var cols = line.Split(colSep);
            if (isHeader)
            {
                foreach (var col in cols)
                    dt.Columns.Add(col.Trim());
                isHeader = false;
            }
            else
                dt.Rows.Add(cols);
        }

        return dt;
    }

    public static DataTable ReadWithParallel(string path, string colSep)
    {
        var dt = new DataTable();
        var lines = File.ReadLines(path).ToList();
        var header = lines.First().Split(colSep);
        foreach (var col in header)
            dt.Columns.Add(col.Trim());

        var rows = lines.Skip(1).AsParallel().Select(l => l.Split(colSep)).ToList();
        foreach (var row in rows)
            dt.Rows.Add(row);

        return dt;
    }

    public static DataTable ReadWithMemoryMapped(string path, string colSep)
    {
        var dt = new DataTable();
        using var mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
        using var stream = mmf.CreateViewStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        bool isHeader = true;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var cols = line.Split(colSep);
            if (isHeader)
            {
                foreach (var col in cols)
                    dt.Columns.Add(col.Trim());
                isHeader = false;
            }
            else
                dt.Rows.Add(cols);
        }

        return dt;
    }

    public static DataTable ReadWithSpan(string path, string lineSep, string colSep)
    {
        var dt = new DataTable();
        byte[] bytes = File.ReadAllBytes(path);
        Span<byte> span = bytes;

        byte line = Encoding.UTF8.GetBytes(lineSep)[0];
        byte col = Encoding.UTF8.GetBytes(colSep)[0];

        var buffer = new StringBuilder();
        var row = new List<string>();
        bool isHeader = true;

        foreach (var b in span)
        {
            if (b == col)
            {
                row.Add(buffer.ToString());
                buffer.Clear();
            }
            else if (b == line)
            {
                row.Add(buffer.ToString());
                buffer.Clear();

                if (isHeader)
                {
                    foreach (var colName in row)
                        dt.Columns.Add(colName.Trim());
                    isHeader = false;
                }
                else
                    dt.Rows.Add(row.ToArray());

                row.Clear();
            }
            else
                buffer.Append((char)b);
        }

        return dt;
    }
}
