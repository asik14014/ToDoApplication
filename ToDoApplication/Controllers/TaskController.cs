using System;
using NLog;
using ToDoApplication.Code;
using ToDoApplication.Models;
using TodoData.Models.Task;
using System.Web.Mvc;
using ToDoApplication.Models.Request;

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
                return Json(TaskManager.GetTask(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.Get({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
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
                return Json(TaskManager.GetAllTasksByUser(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.GetAll({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Достать все shared задачи по пользователю
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        public object GetAllShared(long id)
        {
            logger.Log(LogLevel.Debug, $"TaskController.GetAllShared({id})");

            try
            {
                return Json(TaskManager.GetAllSharedTasksByUser(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.GetAllShared({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Создать задачу
        /// </summary>
        [HttpPut]
        public object Create(TaskRequest task)
        {
            logger.Log(LogLevel.Debug, $"TaskController.Create({task})"); //object to json

            try
            {
                var newTask = TaskManager.Save(task);
                return Json(new Response(0, "Success"), JsonRequestBehavior.AllowGet); //добавить объект в response
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.Create({task}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Редактировать задачу
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public object Update(TaskRequest task)
        {
            logger.Log(LogLevel.Debug, $"TaskController.Update({task})"); //object to json

            try
            {
                var newGroup = TaskManager.Update(task);
                return Json(new Response(0, "Success"), JsonRequestBehavior.AllowGet); //добавить объект в response
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.Update({task}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Удалить группу
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public object Delete(long taskId)
        {
            logger.Log(LogLevel.Debug, $"TaskController.Delete({taskId})"); //object to json

            try
            {
                TaskManager.Delete(taskId);
                return Json(new Response(0, "Success"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.Delete({taskId}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}