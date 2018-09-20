using CGM.Communication.Common.Serialize;
using CGM.Communication.DTO;
using CGM.Communication.Extensions;
using CGM.Communication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace CGM.Web.Controllers
{
    [Route("api")]
    public class SessionController : Controller
    {
        [HttpPost("session")]
        public ActionResult session()
        {
            var stream = Request.Body;
            IFormatter formatter = new BinaryFormatter();
            try
            {
                SessionDTO dto = formatter.Deserialize(stream) as SessionDTO;
                var session = dto.GetSerializerSession();
                RunBehaviors(session);
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }


        }

        private void RunBehaviors(SerializerSession session)
        {
            var behaviors = CommonServiceLocator.ServiceLocator.Current.GetInstance<ISessionBehaviors>();
            var _token = new CancellationToken();

            foreach (var item in behaviors.SessionBehaviors)
            {
                try
                {
                    item.ExecuteTask(session, _token).TimeoutAfter(15000);
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }
    }
}