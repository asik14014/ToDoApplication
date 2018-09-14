using System;
using NLog;
using ToDoApplication.Code;
using ToDoApplication.Models;
using TodoData.Models.Group;
using TodoData.Models.Task;
using System.Web.Mvc;

namespace ToDoApplication.Controllers
{
    [RoutePrefix("api/Share")]
    //[Authorize]
    [AllowAnonymous]
    public class ShareController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        public object Task(long taskId, long userId, int type)
        {
            logger.Log(LogLevel.Debug, $"ShareController.Task({taskId}, {userId})");
            try
            {
                return Json(ShareManager.ShareTask(taskId, userId, type), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ShareController.Task({taskId}, {userId}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public object Group(long groupId, long userId, int type)
        {
            logger.Log(LogLevel.Debug, $"ShareController.Group({groupId}, {userId})");
            try
            {
                return Json(ShareManager.ShareGroup(groupId, userId, type), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ShareController.Group({groupId}, {userId}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public object DeleteTask(long taskId, long userId)
        {
            logger.Log(LogLevel.Debug, $"ShareController.DeleteTask({taskId}, {userId})");
            try
            {
                if (ShareManager.UnshareTask(taskId, userId))
                    return new HttpStatusCodeResult(200);
                else
                    return new HttpStatusCodeResult(404);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ShareController.DeleteTask({taskId}, {userId}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public object DeleteGroup(long groupId, long userId)
        {
            logger.Log(LogLevel.Debug, $"ShareController.DeleteGroup({groupId}, {userId})");
            try
            {
                if (ShareManager.UnshareGroup(groupId, userId))
                    return new HttpStatusCodeResult(200);
                else
                    return new HttpStatusCodeResult(404);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ShareController.DeleteGroup({groupId}, {userId}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}