using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Communication.MiniMed;
using CGM.Communication.Patterns;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Responses.Patterns;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Tasks;

namespace CGM.Communication
{
    public abstract class BaseContext //: IDisposable
    {

        protected CommunicationBlock _communicationBlock;
        protected IDevice Device;
        protected ILogger Logger = ApplicationLogging.CreateLogger<BaseContext>();


        public SerializerSession Session { get; set; } = new SerializerSession();

        public BaseContext(IDevice device)
        {
            this.Device = device;
        }

        protected async Task StartCommunicationStandardResponse(AstmStart request, CancellationToken cancelToken)
        {
            _communicationBlock = new CommunicationBlock();
            _communicationBlock.Request = request;
            _communicationBlock.ExpectedResponses.Add(new SendMessageResponsePattern());
            _communicationBlock.ExpectedResponses.Add(new RecieveMessageResponsePattern());

            await StartCommunication(cancelToken);

        }

        protected async Task StartCommunication(AstmStart request, CancellationToken cancelToken)
        {
            _communicationBlock = new CommunicationBlock();
            _communicationBlock.Request = request;
            await StartCommunication(cancelToken);

        }

        protected async Task StartCommunication(AstmStart request, IReportPattern responsePattern, CancellationToken cancelToken)
        {
            _communicationBlock = new CommunicationBlock();
            _communicationBlock.Request = request;
            _communicationBlock.ExpectedResponses.Add(responsePattern);
            await StartCommunication(cancelToken);

        }

        protected async Task StartCommunication(CommunicationBlock communicationBlock, CancellationToken cancelToken)
        {
            _communicationBlock = communicationBlock;
            await StartCommunication(cancelToken);
        }

        private async Task StartCommunication(CancellationToken cancelToken)
        {
            try
            {
                await _communicationBlock.StartCommunication(Device, Session, cancelToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                _communicationBlock = null;

                //throw;
            }


        }

    }
}
