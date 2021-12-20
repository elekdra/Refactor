using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using backend.Data;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    //file upload class
    public class FileUploadScreenController : ControllerBase
    {
        FileUploadScreenContext fileUploadScreenContext = new FileUploadScreenContext();
        IWebHostEnvironment environment;
        public FileUploadScreenController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        //function to check whether the file already exists with the unique combination key in our database if user try to upload a new file( upload mode )
        [HttpGet]
        [Route("FileCheck")]
        public bool GetFileCheck(string fileParameters)
        {
            try
            {
                var data = fileUploadScreenContext.FileStatus(fileParameters);
                return data;
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                return false;
            }
        }

        // function to save the file with details in our database 
        [HttpPut]
        [Route("Save")]
        public IActionResult FileSave([FromBody] FileModel model)
        {
            var data = fileUploadScreenContext.fileSavetoDb(model, environment);
            return Ok();
        }

        // it will delete the file from trainingdetails_data database
        [HttpGet()]
        [Route("Delete")]
        public String FileDelete(string file)
        {
            try
            {
                var data = fileUploadScreenContext.fileDeletefromDb(file);
                return "file deleted successfully";
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                return "file not deleted";
            }
        }
        // it will load all the available files currently exists in the database
        [HttpGet]
        [Route("Defaults")]
        public IList<FileModel> GetDefaults(string initialize)
        {
            var filemodels = fileUploadScreenContext.retrieveFilesInDb();
            return filemodels;
        }
    }
}