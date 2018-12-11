using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ToDoApplication.Code;
using ToDoApplication.Models;
using ToDoApplication.Models.Request;

namespace ToDoApplication.Controllers
{
    //[RoutePrefix("api/Task")]
    //[RoutePrefix("api")]
    //[RoutePrefix("Task")]
    [Authorize]
    public class TaskController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Достать задачу по идентификатору
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        //[RequireHttps]
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
        [HttpGet]
        public object GetAll()
        {
            var id = User.Identity.GetUserId<long>();
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
        [HttpGet]
        public object GetAllShared()
        {
            var id = User.Identity.GetUserId<long>();
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
                var id = User.Identity.GetUserId<long>();
                var newTask = TaskManager.Save(task, id);
                return Json(newTask, JsonRequestBehavior.AllowGet); //добавить объект в response
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
                var id = User.Identity.GetUserId<long>();
                var newGroup = TaskManager.Update(task, id);
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
        /// Добавить подзадачу
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object AddSubTask(AddSubtaskRequest request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.AddSubTask({request})"); //object to json

            try
            {
                //var result = TaskManager.AddSubtask(request);

                //if (result != null) return new HttpStatusCodeResult(200);
                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.AddSubTask({request}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object AddComment(AddCommentRequest request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.AddComment({request})"); //object to json

            try
            {
                var result = TaskManager.AddComment(request, User.Identity.GetUserId<long>());

                if (result != null) return new HttpStatusCodeResult(200);
                return new HttpStatusCodeResult(400);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.AddComment({request}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public object AddSubTasks(List<AddSubtaskRequest> request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.AddSubTask({request})"); //object to json

            try
            {
                foreach (var item in request)
                {
                    //var result = TaskManager.AddSubtask(item);
                    //if (result == null) return new HttpStatusCodeResult(400);
                }

                return new HttpStatusCodeResult(200);

            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.AddSubTask({request}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object AddComments(List<AddCommentRequest> request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.AddComment({request})"); //object to json

            try
            {
                foreach (var item in request)
                {
                    var result = TaskManager.AddComment(item, User.Identity.GetUserId<long>());
                    if (result == null) return new HttpStatusCodeResult(400);
                }
                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.AddComment({request}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object AddUser(AddUserRequest request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.AddUser({request})"); //object to json

            try
            {
                var result = TaskManager.AddUser(request, User.Identity.GetUserId<long>());

                if (result != null) return new HttpStatusCodeResult(200);
                return new HttpStatusCodeResult(400);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.AddUser({request}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object AddUsers(List<AddUserRequest> request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.AddUser({request})"); //object to json

            try
            {
                foreach (var user in request)
                {
                    var result = TaskManager.AddUser(user, User.Identity.GetUserId<long>());
                    if (result == null) return new HttpStatusCodeResult(400);
                }
                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.AddUser({request}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Добавить подзадачу
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object DeleteSubTask(AddSubtaskRequest request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.DeleteSubTask({request})"); //object to json

            try
            {
                //var result = TaskManager.DeleteSubtask(request);

                //if (result) return new HttpStatusCodeResult(200);
                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.DeleteSubTask({request}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Добавить подзадачу
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object DeleteSubTasks(List<AddSubtaskRequest> request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.DeleteSubTask({request})"); //object to json

            try
            {
                foreach (var task in request)
                {
                    //var result = TaskManager.DeleteSubtask(task);
                    //if (!result) return new HttpStatusCodeResult(400);
                }

                return new HttpStatusCodeResult(200);

            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.DeleteSubTask({request}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object UpdateRepetiotion(IterationModule request)
        {
            logger.Log(LogLevel.Debug, $"TaskController.UpdateRepetiotion({request})"); //object to json

            try
            {
                var result = TaskManager.UpdateRepetition(request);

                if (result != null) return new HttpStatusCodeResult(200);
                return new HttpStatusCodeResult(400);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.UpdateRepetiotion({request}) - {ex}"); //object to json
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

        [HttpPost]
        public object AddFavorite(long taskId)
        {
            logger.Log(LogLevel.Debug, $"TaskController.AddFavorite({taskId})"); //object to json

            try
            {
                var result = TaskManager.AddFavorite(taskId, User.Identity.GetUserId<long>());

                if (result) return new HttpStatusCodeResult(200);
                return new HttpStatusCodeResult(400);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.AddFavorite({taskId}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object FindTask(string name)
        {
            logger.Log(LogLevel.Debug, $"TaskController.FindTask({name})"); //object to json

            try
            {
                var result = TaskManager.FindTask(User.Identity.GetUserId<long>(), name);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.FindTask({name}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object GetTaskForMonth()
        {
            var id = User.Identity.GetUserId<long>();
            logger.Log(LogLevel.Debug, $"TaskController.GetTaskForMonth({id})");

            try
            {
                return Json(TaskManager.GetAllForMonth(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.GetTaskForMonth({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object GetTaskForDay()
        {
            var id = User.Identity.GetUserId<long>();
            logger.Log(LogLevel.Debug, $"TaskController.GetTaskForDay({id})");

            try
            {
                return Json(TaskManager.GetAllForToday(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.GetTaskForDay({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object GetTaskForGroup(long groupId)
        {
            var id = User.Identity.GetUserId<long>();
            logger.Log(LogLevel.Debug, $"TaskController.GetTaskForGroup({id})");

            try
            {
                return Json(TaskManager.GetAllTasksByGroup(groupId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.GetTaskForGroup({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object GetFavorite()
        {
            var id = User.Identity.GetUserId<long>();
            logger.Log(LogLevel.Debug, $"TaskController.GetFavorite({id})");

            try
            {
                return Json(TaskManager.GetFavorite(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"TaskController.GetFavorite({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}