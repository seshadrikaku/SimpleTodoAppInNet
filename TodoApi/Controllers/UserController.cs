using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TodoApi.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private List<TodoItem> taskList = new();
        private const string FilePath = "taskList.json";

        [HttpPost("ToDo")]
        public IActionResult ToDoList(TodoItem User)
        {
            this.readFile();
            User.Id = GenerateUniqueId();


            taskList.Add(User);
            SaveTaskListToJson();
            return Ok("Todo is Added successfully");
        }



        [HttpGet("ToDo")]
        public IActionResult GetTodoList()
        {
            try
            {
                this.readFile();
                return Ok(taskList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        [HttpDelete("ToDo/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                this.readFile();

                TodoItem itemToRemove = taskList.Find(item => item.Id == id);
                if (itemToRemove != null)
                {
                    taskList.Remove(itemToRemove);
                    string updatedJson = JsonSerializer.Serialize(taskList, new JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(FilePath, updatedJson);
                    return Ok("Deleted successfully");
                }
                else
                {
                    return NotFound("Item not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPut("ToDo/{id}")]
        public IActionResult Update(int id, TodoItem updatedTodo)
        {
            try
            {
                //taskList = JsonSerializer.Deserialize<List<TodoItem>>(json);
                this.readFile();
                TodoItem existingItem = taskList.Find(item => item.Id == id);
                if (existingItem != null)
                {
                    existingItem.task = updatedTodo.task;
                    existingItem.date = updatedTodo.date;

                    string updatedJson = JsonSerializer.Serialize(taskList, new JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(FilePath, updatedJson);

                    return Ok("Updated successfully");
                }
                else
                {
                    return NotFound("Item not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private int GenerateUniqueId()
        {
            int maxId = 0;
            foreach (TodoItem item in taskList)
            {
                if (item.Id > maxId)
                {
                    maxId = item.Id;
                }
            }
            return maxId + 1;
        }

        [NonAction]
        private void readFile()
        {
            taskList.Clear();
            try
            {
                string json = System.IO.File.ReadAllText(FilePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    taskList = JsonSerializer.Deserialize<List<TodoItem>>(json);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [NonAction]
        private void SaveTaskListToJson()
        {
            var json = JsonSerializer.Serialize(taskList);
            System.IO.File.WriteAllText(FilePath, json);
        }
    }
}
