using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using SimpleBank.Models;

namespace SimpleBank.Services
{
    // Сервис для работы с JSON-файлами (сохранение и загрузка данных)
    public class JsonDataService
    {
        private const string DefaultFileName = "bank_data.json";

        // Метод получения пути к файлу данных
        public string GetDataFilePath()
        {
            return Path.Combine(Environment.CurrentDirectory, DefaultFileName);
        }

        // Метод сохранения данных в JSON
        public void SaveToJson(BankData data, string filePath)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Данные для сохранения не могут быть null");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));
            }

            try
            {
                // Создание объекта DataContractJsonSerializer
                DataContractJsonSerializer jsonSerializer = 
                    new DataContractJsonSerializer(typeof(BankData));
                
                // Создание файлового потока для записи
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                
                try
                {
                    // Сериализация объекта в поток
                    jsonSerializer.WriteObject(fileStream, data);
                }
                finally
                {
                    // Закрытие потока
                    fileStream.Close();
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new IOException($"Директория не найдена: {filePath}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException($"Нет доступа к файлу: {filePath}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Ошибка при записи в файл: {filePath}", ex);
            }
        }

        // Метод загрузки данных из JSON
        public BankData LoadFromJson(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));
            }

            // Проверка существования файла
            if (!File.Exists(filePath))
            {
                // Если файл не существует, возвращаем пустые данные
                return new BankData();
            }

            try
            {
                // Создание объекта DataContractJsonSerializer
                DataContractJsonSerializer jsonSerializer = 
                    new DataContractJsonSerializer(typeof(BankData));
                
                // Создание файлового потока для чтения
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                
                try
                {
                    // Десериализация из потока (требуется преобразование типа)
                    BankData data = (BankData)jsonSerializer.ReadObject(fileStream);
                    
                    // Проверяем, что данные не null
                    if (data == null)
                    {
                        return new BankData();
                    }

                    return data;
                }
                finally
                {
                    // Закрытие потока
                    fileStream.Close();
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new IOException($"Файл не найден: {filePath}", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new IOException($"Директория не найдена: {filePath}", ex);
            }
            catch (SerializationException ex)
            {
                throw new IOException($"Ошибка при десериализации файла. Возможно, файл поврежден: {filePath}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException($"Нет доступа к файлу: {filePath}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Ошибка при чтении файла: {filePath}", ex);
            }
        }
    }
}

