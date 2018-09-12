using System;
using NLog;
using ToDoApplication.Code;
using ToDoApplication.Models;
using TodoData.Models.Task;
using System.Web.Mvc;

namespace ToDoApplication.Controllers
{
    //[Route("api/[controller]")]
    [RoutePrefix("api/Task")]
    [AllowAnonymous]
    //[Authorize]
    public class TaskController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Достать задачу по идентификатору
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        public object Get(long id)
        {
            logger.Log(LogLevel.Debug, $"TaskController.Get({id})");
            try
            {
                return $"Works! {id}";
                //return TaskManager.GetTask(id);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.Get({id}) - {ex}");
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// Достать все задачи по пользователю
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        public object GetAll(long id)
        {
            logger.Log(LogLevel.Debug, $"TaskController.GetAll({id})");

            try
            {
                return TaskManager.GetAllTasksByUser(id);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.GetAll({id}) - {ex}");
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// Создать задачу
        /// </summary>
        [HttpPut]
        public object Create(Task task)
        {
            logger.Log(LogLevel.Debug, $"TaskController.Create({task})"); //object to json

            try
            {
                var newTask = TaskManager.SaveOrUpdate(task);
                return new Response(0, "Success"); //добавить объект в response
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.Create({task}) - {ex}"); //object to json
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// Редактировать задачу
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public object Update(Task task)
        {
            logger.Log(LogLevel.Debug, $"TaskController.Update({task})"); //object to json

            try
            {
                var newGroup = TaskManager.SaveOrUpdate(task);
                return new Response(0, "Success"); //добавить объект в response
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.Update({task}) - {ex}"); //object to json
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// Удалить группу
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public object Delete(Task task)
        {
            logger.Log(LogLevel.Debug, $"TaskController.Delete({task})"); //object to json

            try
            {
                TaskManager.Delete(task);
                return new Response(0, "Success");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.Delete({task}) - {ex}"); //object to json
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }
    }
}