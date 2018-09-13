using System;
using NLog;
using ToDoApplication.Code;
using ToDoApplication.Models;
using TodoData.Models.Group;
using System.Web.Mvc;

namespace ToDoApplication.Controllers
{
    [RoutePrefix("api/Group")]
    [AllowAnonymous]
    //[Authorize]
    public class GroupController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Достать группу по идентификатору
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Get(long id)
        {
            logger.Log(LogLevel.Debug, $"GroupController.Get({id})");
            try
            {
                return Json(GroupManager.GetGroup(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.Get({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Достать все группы по пользователю
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetAll(long id)
        {
            logger.Log(LogLevel.Debug, $"GroupController.GetAll({id})");

            try
            {
                return Json(GroupManager.GetAllGroupsByUser(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.GetAll({id}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Создать группу
        /// </summary>
        [HttpPut]
        public object Create(Group group)
        {
            logger.Log(LogLevel.Debug, $"GroupController.Create({group})"); //object to json

            try
            {
                var newGroup = GroupManager.SaveOrUpdate(group);
                return Json(new Response(0, "Success"), JsonRequestBehavior.AllowGet); //добавить объект в response
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.Create({group}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Редактировать группу
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public object Update(Group group)
        {
            logger.Log(LogLevel.Debug, $"GroupController.Update({group})"); //object to json

            try
            {
                var newGroup = GroupManager.SaveOrUpdate(group);
                return Json(new Response(0, "Success"), JsonRequestBehavior.AllowGet); //добавить объект в response
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.Update({group}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Удалить группу
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public object Delete(Group group)
        {
            logger.Log(LogLevel.Debug, $"GroupController.Delete({group})"); //object to json

            try
            {
                GroupManager.Delete(group);
                return Json(new Response(0, "Success"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.Delete({group}) - {ex}"); //object to json
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}