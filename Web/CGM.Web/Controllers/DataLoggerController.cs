using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CGM.Communication.Interfaces;
using CGM.Web.Hubs;
using Microsoft.AspNetCore.Mvc;


namespace CGM.Web.Controllers
{
    public class DataLoggerController : Controller
    {
        private readonly DataLoggerHub dataLoggerHub;

        public ICgmTask CgmTask { get; }

        public DataLoggerController(DataLoggerHub dataLoggerHub, ICgmTask cgmTask)
        {
            this.dataLoggerHub = dataLoggerHub;
            CgmTask = cgmTask;
            cgmTask.StatusChanged += CgmTask_StatusChanged;
          
        }

        private void CgmTask_StatusChanged(object sender, EventArgs e)
        {
            SendStatus();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Start()
        {

            if (CgmTask.Status==Communication.Common.TaskStatusEnum.Stopped)
            {
                CgmTask.Start(); 
            }
           return SendStatus();

        }

        [HttpPost]
        public ActionResult Stop()
        {
          
            if (CgmTask.Status == Communication.Common.TaskStatusEnum.Running)
            {
                CgmTask.Stop();
            }
            return SendStatus();
        
        }
        [HttpPost]
        public ActionResult Restart()
        {
           
            if (CgmTask.Status == Communication.Common.TaskStatusEnum.Running)
            {
                CgmTask.Stop();
                CgmTask.Start();
            }
            else
            {
                CgmTask.Start();
            }
          return  SendStatus();
     
        }

        private ActionResult SendStatus()
        {
            dataLoggerHub.SendStatus(CgmTask.Status.ToString());
            return Json(CgmTask.Status.ToString());
        }
    }
}