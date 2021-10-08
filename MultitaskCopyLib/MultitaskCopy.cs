using System;
using System.IO;
using System.Threading.Tasks;

namespace MultitaskCopyLib
{
    public static class MultitaskCopy
    {
        private static readonly int _maxCountThread = 10;

        /// <summary>
        /// Копировать файл.
        /// </summary>
        /// <param name="sourcePath">Полное имя файла источника.</param>
        /// <param name="distancePath">Полное имя нового файла.</param>
        /// <param name="countThread">Количество потоков чтения файла.</param>
        /// <param name="isCopyBlock">true - копировать блоками | false - копировать по байтово</param>
        /// <para>Исключения:</para>
        /// <para><see cref="ArgumentException"/></para>
        /// <para><see cref="FileNotFoundException"/></para>
        /// <para><see cref="Exception"/></para>
        public static void Copy(string sourcePath, string distancePath, int countThread, bool isCopyBlock = true)
        {
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException("Ошибка в пути файла-источника");
            }

            if (countThread > _maxCountThread || countThread < 1)
            {
                throw new ArgumentException($"Ошибка в количестве потоков. Должно быть не более {_maxCountThread}");
            }

            if (string.IsNullOrWhiteSpace(distancePath))
            {
                distancePath = Path.Combine(
                    Path.GetDirectoryName(sourcePath),
                    $"{Path.GetFileNameWithoutExtension(sourcePath)}_copy" +
                    $"{Path.GetExtension(sourcePath)}");
            }

            try
            {
                var packs = Split(sourcePath, countThread, isCopyBlock);
                Concat(packs, distancePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Pack[] Split(string fileName, int countThread, bool isCopyBlock = true)
        {
            var data = new Pack[countThread];        
            var length = new FileInfo(fileName).Length;

            if (length < countThread)
            {
                throw new Exception($"Файл пустой или его длина меньше количества запускаемых потоков: {countThread}");
            }

            var part = length / countThread;
            var remains = length % countThread;

            Parallel.For(0, countThread, i =>
            {
                var packLength = (i < countThread - 1) ? part : part + remains;
                data[i] = isCopyBlock
                    ? CopyBlockToPack(fileName, part * i, packLength, i)
                    : CopyToPack(fileName, part * i, packLength, i);
            });

            return data;
        }

        private static Pack CopyToPack(string fileName, long start, long packLength, int id)
        {
            var temp = new byte[packLength];
            using var file = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            file.BaseStream.Seek(start, SeekOrigin.Begin);

            for (int i = 0; i < packLength; i++)
            {
                temp[i] = file.ReadByte();
            }

            return new Pack(temp, id);
        }

        private static Pack CopyBlockToPack(string fileName, long start, long packLength, int id)
        {
            if (packLength > int.MaxValue)
            {
                throw new ArgumentException("Размер пакета не должен быть больше 2 147 483 647 байт");
            }

            using var file = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            file.BaseStream.Seek(start, SeekOrigin.Begin);
            var data = file.ReadBytes((int)packLength);

            return new Pack(data, id);
        }

        private static void Concat(Pack[] pack, string fileName)
        {
            using var file = new BinaryWriter(File.Open(fileName, FileMode.Create));
            for (int i = 0; i < pack.Length; i++)
            {
                file.Write(pack[i].Content);
            }
        }
    }
}