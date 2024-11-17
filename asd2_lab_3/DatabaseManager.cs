using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asd2_lab_3
{
    public class DatabaseManager
    {
        private const string DataFile = "DataFile.dat";
        private const string IndexFile = "IndexFile.dat";

        public DatabaseManager()
        {
            if (!File.Exists(DataFile)) File.Create(DataFile).Close();
            if (!File.Exists(IndexFile)) File.Create(IndexFile).Close();
        }

        // Додавання запису
        public bool AddRecord(int key, string data)
        {
            var indexRecords = ReadIndexFile();

            if (indexRecords.Any(r => r.Key == key))
                return false;

            long position = AppendDataToFile(key, data);

            indexRecords.Add(new Record { Key = key, Data = position.ToString() });
            WriteIndexFile(indexRecords);

            return true;
        }

        // Пошук запису
        public Record SearchRecord(int key)
        {
            var indexRecords = ReadIndexFile();

            // Бінарний пошук
            int left = 0;
            int right = indexRecords.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                if (indexRecords[mid].Key == key)
                {
                    return ReadDataFromFile(long.Parse(indexRecords[mid].Data));
                }

                if (indexRecords[mid].Key < key)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            return null;
        }

        // Видалення запису
        public bool DeleteRecord(int key)
        {
            var indexRecords = ReadIndexFile();
            var indexRecord = indexRecords.FirstOrDefault(r => r.Key == key);

            if (indexRecord == null) return false;

            indexRecords.Remove(indexRecord);
            WriteIndexFile(indexRecords);

            return true;
        }

        // Редагування запису
        public bool EditRecord(int key, string newData)
        {
            var indexRecords = ReadIndexFile();
            var indexRecord = indexRecords.FirstOrDefault(r => r.Key == key);

            if (indexRecord == null) return false;

            long position = long.Parse(indexRecord.Data);
            UpdateDataInFile(position, key, newData);

            return true;
        }

        // Методи роботи з файлами

        private long AppendDataToFile(int key, string data)
        {
            using (var stream = new FileStream(DataFile, FileMode.Append, FileAccess.Write))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                long position = stream.Position;

                writer.Write(key);

                byte[] dataBytes = Encoding.UTF8.GetBytes(data.PadRight(100, ' '));
                writer.Write(dataBytes);

                return position;
            }
        }

        private Record ReadDataFromFile(long position)
        {
            using (var stream = new FileStream(DataFile, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                stream.Seek(position, SeekOrigin.Begin);

                int key = reader.ReadInt32();

                byte[] dataBytes = reader.ReadBytes(100);
                string data = Encoding.UTF8.GetString(dataBytes).Trim();

                return new Record { Key = key, Data = data };
            }
        }

        private void UpdateDataInFile(long position, int key, string newData)
        {
            using (var stream = new FileStream(DataFile, FileMode.Open, FileAccess.Write))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                stream.Seek(position, SeekOrigin.Begin);

                writer.Write(key);

                byte[] dataBytes = Encoding.UTF8.GetBytes(newData.PadRight(100, ' '));
                writer.Write(dataBytes);
            }
        }

        public List<Record> ReadIndexFile()
        {
            var indexRecords = new List<Record>();

            using (var stream = new FileStream(IndexFile, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                    int key = reader.ReadInt32();
                    long position = reader.ReadInt64();
                    indexRecords.Add(new Record { Key = key, Data = position.ToString() });
                }
            }

            return indexRecords.OrderBy(r => r.Key).ToList();
        }

        private void WriteIndexFile(List<Record> indexRecords)
        {
            using (var stream = new FileStream(IndexFile, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(stream))
            {
                foreach (var record in indexRecords.OrderBy(r => r.Key))
                {
                    writer.Write(record.Key);
                    writer.Write(long.Parse(record.Data));
                }
            }
        }

        public void DeleteAllRecords()
        {
            File.WriteAllText(DataFile, string.Empty);
            File.WriteAllText(IndexFile, string.Empty);
        }
    }
}
