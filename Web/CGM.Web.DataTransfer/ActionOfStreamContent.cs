using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Web.DataTransfer
{
    public class ActionOfStreamContent : HttpContent
    {
        private readonly Action<Stream> _actionOfStream;
     
        public ActionOfStreamContent(Action<Stream> actionOfStream)
        {
            if (actionOfStream == null)
           {
              throw new ArgumentNullException("actionOfStream");
           }
    
          _actionOfStream = actionOfStream;
       }
   
       protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
       {
          return Task.Factory.StartNew(
               (obj) =>
               {
                   Stream target = obj as Stream;
                 _actionOfStream(target);
              },
              stream);
      }
   
       protected override bool TryComputeLength(out long length)
       {
           // We can't know how much the Action<Stream> is going to write
           length = -1;
           return false;
      }
   }
}
