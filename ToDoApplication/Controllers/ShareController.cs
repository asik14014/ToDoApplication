using System;
using NLog;
using ToDoApplication.Code;
using ToDoApplication.Models;
using TodoData.Models.Group;
using TodoData.Models.Task;
using System.Web.Mvc;

namespace ToDoApplication.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    public class ShareController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public object Task(Task task, long userId, int type)
        {
            logger.Log(LogLevel.Debug, $"ShareController.Task({Json(task)}, {userId})");
            try
            {
                return Json(ShareManager.ShareTask(task, userId, type), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ShareController.Task({Json(task)}, {userId}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        public object Group(Group group, long userId, int type)
        {
            logger.Log(LogLevel.Debug, $"ShareController.Group({Json(group)}, {userId})");
            try
            {
                return Json(ShareManager.ShareGroup(group, userId, type), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ShareController.Group({Json(group)}, {userId}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        public object DeleteTask(Task task, long userId)
        {
            logger.Log(LogLevel.Debug, $"ShareController.DeleteTask({Json(task)}, {userId})");
            try
            {
                if (ShareManager.UnshareTask(task, userId))
                    return new HttpStatusCodeResult(200);
                else
                    return new HttpStatusCodeResult(404);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ShareController.DeleteTask({Json(task)}, {userId}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        public object DeleteGroup(Group group, long userId)
        {
            logger.Log(LogLevel.Debug, $"ShareController.DeleteGroup({Json(group)}, {userId})");
            try
            {
                if (ShareManager.UnshareGroup(group, userId))
                    return new HttpStatusCodeResult(200);
                else
                    return new HttpStatusCodeResult(404);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"ShareController.DeleteGroup({Json(group)}, {userId}) - {ex}");
                //изменить http status code
                return Json(new Response(100, ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}