using System;
using System.Collections.Generic;
using backend.Models;
using MySql.Data.MySqlClient;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace backend.Data
{
    public class FileUploadScreenContext
    {

        private string fileFolder = "files";
        private string contentSplit = "base64,";
        private string dataBaseName = "DOCUMENT_MANAGEMENT";
        private string connectionString = "server=localhost;userid=root;password=fathimaadmin;database=";
        private string trainingCommand = "select * from DOCUMENT_MANAGEMENT.training";
        private string companyCommand = "select * from DOCUMENT_MANAGEMENT.company";
        private string insertToDataTable = "INSERT INTO DOCUMENT_MANAGEMENT.trainingdetails_data(`Training_index`,`Filepath`,`minimum_version`,`file_name`) values(";
        private string insertToHeaderTable = "INSERT INTO DOCUMENT_MANAGEMENT.trainingdetails_header(`Company_ID`,`Version`,`Training_ID`) values(";
        private string getIndexCmd = "select training_index from DOCUMENT_MANAGEMENT.trainingdetails_header where training_id =";
        private string companyId = "company_id";
        private string equals = "=";
        private string and = "and";
        IWebHostEnvironment environment;
        public FileUploadScreenContext(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        private string FilePath(String fileName)
        {
            string webRootPath = environment.WebRootPath;
            string filesPath = Path.Combine(webRootPath, fileFolder);
            string fileNamesave = $"{Path.GetFileNameWithoutExtension(FileName)}_{Guid.NewGuid().ToString()}{Path.GetExtension(FileName)}";
            string path = filesPath + "\\" + fileNamesave;
            return path;
        }
        public void SaveFileInServer(string fileContent, string filePath)
        {
            String[] contents = FileContent.Split(contentSplit);
            byte[] data = Convert.FromBase64String(contents[1]);
            using (System.IO.FileStream stream = System.IO.File.Create(path))
            {
                stream.Write(data, 0, data.Length);
            }
        }
        private int GetTrainingIndex(FileModel model)
        {

            MySqlConnection connection = new MySqlConnection(connectionString + dataBaseName);
            connection.Open();
            IDictionary<string, string> companyMap = GetCompanyNameToIdMap(connection);
            IDictionary<string, string> trainingMap = GetTrainingNameToIdMap(connection);

            var trainingindexCommand = $"{getIndexCmd}{trainingMap[model.Training.ToUpper()]}{and} {companyId} {equals} {companyMap[model.Company.ToUpper()]}{ and} {version}{equals}{model.Version}";

            int trainingindex = -1;

            using (var getTrainingIndexCmd = new MySqlCommand(trainingindexCommand, connection))
            {
                using (MySqlDataReader reader = getTrainingIndexCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trainingindex = reader.GetInt32(0);
                    }
                }
            }
            return trainingindex;
        }

        private void FileUploadToDataTable(FileModel model, int trainingIndex)
        {
            //connection
            var uploadatindexCommand = $"{insertToDataTable}{ trainingIndex} ,{path},{model.MinVersion},{ model.FileName})";
            using var insertTrainingData = new MySqlCommand(uploadatindexCommand, connection);
            insertTrainingData.ExecuteNonQuery();
        }
        private void InsertInHeaderTable(FileModel model)
        {
            var companyId = companyMap[model.Company.ToUpper()];
            var trainingId = trainingMap[model.Training.ToUpper()];

            var uploadHeader = $"{insertToHeaderTable}{companyId},{ model.Version} ,{ trainingId })";
            var indexReadBack = -1;
            using var insertTrainingHeaderCommand = new MySqlCommand(uploadHeader, connection);
            insertTrainingHeaderCommand.ExecuteNonQuery();
            var trainingIndex = GetTrainingIndex(model);
            FileUploadToDataTable(model, trainingIndex);
        }

        public void fileSave(FileModel fileModel)
        {
            string filePath = FilePath(fileModel.FileName);
            SaveFileInServer(fileModel.FileContent, filePath);
            var trainingIndex = GetTrainingIndex(fileModel);
            if (trainingindex >= 0)
            {
                FileuploadToDataTable(fileModel, trainingIndex);
            }
            else
            {

                InsertInHeaderTable(fileModel);
            }
        }

        //private function that maps company name to the id
        private IDictionary<string, string> GetCompanyNameToIdMap(MySqlConnection connection)
        {
            IDictionary<string, string> companyMap = new Dictionary<string, string>();
            using (MySqlCommand readAllCompaniesCommand = connection.CreateCommand())
            {
                readAllCompaniesCommand.CommandText = companyCommand;
                using (var reader = readAllCompaniesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader.GetInt32(0).ToString();
                        string name = reader.GetString(1).ToString();
                        companyMap.Add(name, id);
                    }
                }
            }
            return companyMap;
        }

        //private function that maps training name to the id
        private IDictionary<string, string> GetTrainingNameToIdMap(MySqlConnection connection)
        {
            IDictionary<string, string> trainingMap = new Dictionary<string, string>();
            using (MySqlCommand readAllTrainingCommand = connection.CreateCommand())
            {
                readAllTrainingCommand.CommandText = trainingCommand;
                using (var reader = readAllTrainingCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader.GetInt32(0).ToString();
                        string name = reader.GetString(1).ToString();
                        trainingMap.Add(name, id);
                    }
                }
            }
            return trainingMap;
        }
    }
}