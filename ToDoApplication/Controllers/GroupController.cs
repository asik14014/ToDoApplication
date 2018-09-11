using System;
using NLog;
using ToDoApplication.Code;
using ToDoApplication.Models;
using TodoData.Models.Group;
using System.Web.Mvc;

namespace ToDoApplication.Controllers
{
    public class GroupController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Достать группу по идентификатору
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public object Get(long id)
        {
            logger.Log(LogLevel.Debug, $"GroupController.Get({id})");
            try
            {
                return GroupManager.GetGroup(id);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.Get({id}) - {ex}");
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// Достать все группы по пользователю
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public object GetAll(long userId)
        {
            logger.Log(LogLevel.Debug, $"GroupController.GetAll({userId})");

            try
            {
                return GroupManager.GetAllGroupsByUser(userId);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.GetAll({userId}) - {ex}");
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// Создать группу
        /// </summary>
        [HttpPut]
        [Authorize]
        public object Create(Group group)
        {
            logger.Log(LogLevel.Debug, $"GroupController.Create({group})"); //object to json

            try
            {
                var newGroup = GroupManager.SaveOrUpdate(group);
                return new Response(0, "Success"); //добавить объект в response
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.Create({group}) - {ex}"); //object to json
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// Редактировать группу
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public object Update(Group group)
        {
            logger.Log(LogLevel.Debug, $"GroupController.Update({group})"); //object to json

            try
            {
                var newGroup = GroupManager.SaveOrUpdate(group);
                return new Response(0, "Success"); //добавить объект в response
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.Update({group}) - {ex}"); //object to json
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }

        /// <summary>
        /// Удалить группу
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public object Delete(Group group)
        {
            logger.Log(LogLevel.Debug, $"GroupController.Delete({group})"); //object to json

            try
            {
                GroupManager.Delete(group);
                return new Response(0, "Success");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GroupController.Delete({group}) - {ex}"); //object to json
                //изменить http status code
                return new Response(100, ex.Message);
            }
        }
    }
}