using CGM.Communication;
using CGM.Communication.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Patterns;
using CGM.Communication.Common;
using CGM.Communication.MiniMed.Requests.Standard;
using CGM.Communication.MiniMed.Responses.Patterns;
using System.Linq;
using CGM.Communication.MiniMed.Infrastructur;
using System.Diagnostics;

namespace CGM.Communication.MiniMed
{
    public class MiniMedContext : BaseContext
    {
        private IStateRepository _stateRepository;

        public MiniMedContext(IDevice device, SerializerSession session, IStateRepository stateRepository) : base(device)
        {
            this.Session = session;
            _stateRepository = stateRepository;
        }


        public async Task<BayerStickInfoResponse> GetDeviceInformationAsync(CancellationToken cancelToken)
        {
            try
            {

                await StartCollectDeviceInfoAsync(cancelToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
            finally
            {
                await CloseAsync(cancelToken);
            }


            return Session.Device;
        }


        public async Task<SerializerSession> GetSessionAsync(CancellationToken cancelToken)
        {

            List<Func<Task>> tasks = new List<Func<Task>>();
            tasks.Add(() => StartCollectPumpSettingsAsync(cancelToken));
            return await CallPumpWithActions(tasks, cancelToken);
        }

        public async Task<SerializerSession> GetPumpSessionAsync(CancellationToken cancelToken)
        {


            List<Func<Task>> tasks = new List<Func<Task>>();
            tasks.Add(() => StartPumpTimeAsync(cancelToken));
            tasks.Add(() => StartCollectPumpDataAsync(cancelToken));

            if (Session.Settings.IncludeHistory)
            {
                tasks.Add(() => StartReadHistory(cancelToken));
            }



            return await CallPumpWithActions(tasks, cancelToken);
        }

        public async Task<SerializerSession> GetOnlyCnlSessionAsync(CancellationToken cancelToken)
        {
            return await CallPumpWithActions(null, false, cancelToken);
        }

        public async Task<SerializerSession> GetPumpConfigurationAsync(CancellationToken cancelToken)
        {

            List<Func<Task>> tasks = new List<Func<Task>>();
            tasks.Add(() => StartBasalPatternAsync(cancelToken));
            tasks.Add(() => StartGetSetting(AstmSendMessageType.DEVICE_CHARACTERISTICS_REQUEST, cancelToken));
            tasks.Add(() => StartGetSetting(AstmSendMessageType.DEVICE_STRING_REQUEST, cancelToken));
            tasks.Add(() => StartGetSetting(AstmSendMessageType.READ_BOLUS_WIZARD_BG_TARGETS_REQUEST, cancelToken));
            tasks.Add(() => StartGetSetting(AstmSendMessageType.READ_BOLUS_WIZARD_CARB_RATIOS_REQUEST, cancelToken));
            tasks.Add(() => StartGetSetting(AstmSendMessageType.READ_BOLUS_WIZARD_SENSITIVITY_FACTORS_REQUEST, cancelToken));

            return await CallPumpWithActions(tasks, cancelToken);
        }

        private async Task<SerializerSession> CallPumpWithActions(List<Func<Task>> tasks, CancellationToken cancelToken)
        {
            return await CallPumpWithActions(tasks, true, cancelToken);
        }

        private async Task<SerializerSession> CallPumpWithActions(List<Func<Task>> tasks, bool lookForPump, CancellationToken cancelToken)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                cancelToken.ThrowIfCancellationRequested();
                await StartCollectDeviceInfoAsync(cancelToken);
                Logger.LogInformation($"Call pump with CNL: {this.Session.Device.SerialNumber} ");

                try
                {
                    cancelToken.ThrowIfCancellationRequested();
                    await BeginModesAsync(cancelToken);

                    try
                    {
                        cancelToken.ThrowIfCancellationRequested();
                        await OpenConnectionAsync(cancelToken);

                        try
                        {
                            cancelToken.ThrowIfCancellationRequested();
                            await StartCollectPumpSettingsAsync(cancelToken);

                            if (lookForPump)
                            {
                                try
                                {
                                    cancelToken.ThrowIfCancellationRequested();
                                    await StartChannelNegoationAsync(cancelToken);

                                    try
                                    {
                                        cancelToken.ThrowIfCancellationRequested();
                                        await BeginEHSMAsync(cancelToken);

                                        try
                                        {
                                            if (tasks != null)
                                            {
                                                foreach (var item in tasks)
                                                {
                                                    cancelToken.ThrowIfCancellationRequested();
                                                    await item();

                                                }
                                            }

                                        }
                                        catch (Exception e)
                                        {

                                            Logger.LogError(e.Message);
                                        }
                                    }
                                    catch (Exception e)
                                    {

                                        Logger.LogError(e.Message);
                                    }
                                    finally
                                    {
                                        if (Device.IsConnected)
                                        {
                                            await EndEHSMAsync(cancelToken);
                                        }

                                    }


                                }
                                catch (Exception e)
                                {

                                    Logger.LogError(e.Message);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e.Message);
                        }

                    }
                    catch (Exception e)
                    {

                        Logger.LogError(e.Message);
                    }
                    finally
                    {
                        if (Device.IsConnected)
                        {
                            await CloseConnectionAsync(cancelToken);
                        }
                    }
                }
                catch (Exception e)
                {

                    Logger.LogError(e.Message);
                }
                finally
                {
                    if (Device.IsConnected)
                    {
                        await EndModesAsync(cancelToken);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
            finally
            {
                if (Device.IsConnected)
                {
                    await CloseAsync(cancelToken);
                }

            }


            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
          ts.Hours, ts.Minutes, ts.Seconds,
          ts.Milliseconds / 10);

            Logger.LogInformation($"Pumpsession-time: {elapsedTime}");

            //try
            //{
            //    if (Session.PumpDataHistory.MultiPacketHandlers != null && Session.PumpDataHistory.MultiPacketHandlers.Count > 0)
            //    {
            //        Session.PumpDataHistory.GetHistoryEvents();
            //    }
            //}
            //catch (Exception e)
            //{
            //    Logger.LogError(e.Message);
            //}

            return Session;
        }

        private async Task StartCollectDeviceInfoAsync(CancellationToken cancelToken)
        {
            //if in bad state.....
            //await CloseAsync(cancelToken);


            Logger.LogInformation("Getting CNL deviceInformation");
            CommunicationBlock block = new CommunicationBlock();
            block.TimeoutSeconds = Session.Settings.TimeoutSeconds;;
            block.Request = new AstmStart("X");
            //block.Request = new AstmStart("W");
            //expected responses for the request
            block.ExpectedResponses.Add(new ReportPattern(new byte[] { 0x00, 0x41, 0x42, 0x43 }, 0));
            block.ExpectedResponses.Add(new EnqOREotkPattern());
            //Start Communication 
            await this.StartCommunication(block, cancelToken);

            if (!cancelToken.IsCancellationRequested)
            {
                if (string.IsNullOrEmpty(this.Session.Device.SerialNumber))
                {
                    //if in bad state.....
                    //await CloseAsync(cancelToken);
                    this.Session.NeedResetCommunication = true;
                    throw new Exception("Could not communicate with CNL. Please unplug CNL and plug it in again to reset CNL-communication. ");
                }
                else
                {
                    //Get previous saved parameters Or set this session if device do not exsist
                    _stateRepository.GetOrSetSessionAndSettings(Session);
                    //using (CgmUnitOfWork uow = new CgmUnitOfWork())
                    //{
                    //    uow.Device.GetOrSetSessionAndSettings(Session);
                    //}
                }
            }








        }

        private async Task BeginModesAsync(CancellationToken cancelToken)
        {
            Logger.LogInformation("Enter ControlMode");
            await StartCommunication(new NAKMessage(), new EnqOREotkPattern(), cancelToken);
            await StartCommunication(new ENQMessage(), new EnqORAckkPattern(), cancelToken);

            Logger.LogInformation("Enter PassthroughMode");
            await StartCommunication(new AstmStart("W|"), new ACKPattern(), cancelToken);
            await StartCommunication(new AstmStart("Q|"), new ACKPattern(), cancelToken);
            await StartCommunication(new AstmStart("1|"), new ACKPattern(), cancelToken);
        }

        private async Task OpenConnectionAsync(CancellationToken cancelToken)
        {
            Logger.LogInformation($"Open Connection");

            if (this.Session.Device != null && this.Session.Device.HMACbyte != null)
            {
                //Logger.LogInformation($"OpenConnection HMAC: {BitConverter.ToString(this.Session.Device.HMACbyte)}");
                await StartCommunication(Session.GetOpenConnectionRequest(),
                    new OpenConnectionResponsePattern(),
                    cancelToken);
                Logger.LogInformation($"Connection opened");
            }
            else
            {
                throw new Exception("HMACbyte from CNL is null.");
            }

        }

        private async Task StartCollectPumpSettingsAsync(CancellationToken cancelToken)
        {
            //test this....
            if (this.Session.LinkMac == null && this.Session.PumpMac == null)
            {
                Logger.LogInformation($"Getting linkmac/Pumpmac");
                await StartCommunication(Session.GetReadInfoRequest(),
                        new ReadInfoResponsePattern(),
                        cancelToken);
                Logger.LogInformation($"Got LinkMac: {BitConverter.ToString(this.Session.LinkMac)} AND PumpMac: {BitConverter.ToString(this.Session.PumpMac)}");


                if (this.Session.LinkMac == null && this.Session.PumpMac == null)
                {
                    throw new Exception("Error getting Linkmac/Pumpmac");
                }
                else
                {
                    //save macs
                    _stateRepository.AddUpdateSessionToDevice(Session);
                    //using (CgmUnitOfWork uow = new CgmUnitOfWork())
                    //{
                    //    uow.Device.AddUpdateSessionToDevice(Session);
                    //}
                }

            }



            if (this.Session.LinkKey == null)
            {

                Logger.LogInformation($"Getting linkkey");
                await StartCommunication(Session.GetLinkKeyRequest(),
        new LinkKeyResponsePattern(),
        cancelToken);


                if (this.Session.LinkKey == null)
                {
                    throw new Exception("Error getting linkkey");
                }
                else
                {
                    Logger.LogInformation($"Got LinkKey: {BitConverter.ToString(this.Session.LinkKey)}");
                    //save LinkKey
                    _stateRepository.AddUpdateSessionToDevice(Session);
                    //using (CgmUnitOfWork uow = new CgmUnitOfWork())
                    //{
                    //    uow.Device.AddUpdateSessionToDevice(Session);
                    //}
                }

            }


            if (this.Session.LinkMac == null || this.Session.PumpMac == null)
            {
                throw new Exception($"Could not get linkmac/pumpmac: {this.Session.LinkMac}/{this.Session.PumpMac}");
            }

        }

        private async Task StartChannelNegoationAsync(CancellationToken cancelToken)
        {
            //IEEE 802.15.4 Channel ID
            //0x0e - Channel 14 - 2420MHz
            //0x11 - Channel 17 - 2435MHz
            //0x14 - Channel 20 - 2450MHz
            //0x17 - Channel 23 - 2465MHz
            //0x1a - Channel 26 - 2480MHz
            List<byte> channels = new List<byte>() { 0x1a, 0x17, 0x14, 0x0e, 0x11 };
            //short list of channels. 
            //channel 23: observed loosing connection between sensor and¨pump when on channel 23. NOT GOOD.
            //channel 26: never seen a connection on this channel. remove to save loop-time.
            //the above channels removed from list to save loop-time and increase stability.

            //if (this.Session.RadioChannel==0x17)
            //{
            //    this.Session.RadioChannel = 0x00;
            //}
            //List<byte> channels = new List<byte>() { 0x1a,  0x14, 0x0e, 0x11 };

            //and then NOT.... because of no connections....
            byte lastChannel = this.Session.RadioChannel;


            if (this.Session.RadioChannelConfirmed && this.Session.RadioChannel == 0x00)
            {
                this.Session.RadioChannelConfirmed = false;
            }


            if (this.Session.RadioChannel != 0x00)
            {
                byte trie = this.Session.RadioChannel;
                this.Session.RadioChannel = 0x00;
                Logger.LogInformation($"Looking for pump. Channel: {trie} (Last used)");
                await StartCommunicationStandardResponse(Session.GetChannelRequest(trie), cancelToken);
                if (this.Session.RadioChannel == 0x00)
                {
                    Logger.LogInformation($"No connection on Channel {trie}");
                }
            }


            if (this.Session.RadioChannel == 0x00)
            {

                if (lastChannel != 0x00)
                {
                    channels.Remove(lastChannel);
                }

                foreach (var item in channels)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    Logger.LogInformation($"Looking for pump. Channel: {item}");
                    await StartCommunicationStandardResponse(Session.GetChannelRequest(item), cancelToken);

                    if (this.Session.RadioChannel != 0x00)
                    {
                        break;
                    }
                    else
                    {
                        Logger.LogInformation($"No connection on Channel {item}");
                    }

                }
            }

            if (this.Session.RadioRSSI == 0 && this.Session.RadioChannel != 0x00)
            {
                Logger.LogInformation($"Signal on Radiochannel {this.Session.RadioChannel.ToString()} is too weak ({this.Session.RadioRSSI}%)");
                this.Session.RadioChannel = 0x00;
            }

            if (this.Session.RadioChannel == 0x00)
            {
                this.Session.RadioChannelConfirmed = false;
                throw new Exception("Could not find RadioChannel/Pump.");
            }
            else
            {
                this.Session.RadioChannelConfirmed = true;
                Logger.LogInformation($"Connected on radiochannel {this.Session.RadioChannel.ToString()}. ({this.Session.RadioRSSI}%)");
                //save LinkKey
                _stateRepository.AddUpdateSessionToDevice(Session);
                //using (CgmUnitOfWork uow = new CgmUnitOfWork())
                //{
                //    uow.Device.AddUpdateSessionToDevice(Session);
                //}
            }


            Logger.LogTrace(this.Session.GetParametersAsString());
        }

        private async Task BeginEHSMAsync(CancellationToken cancelToken)
        {
            Logger.LogInformation("Begin EHSM");
            await StartCommunication(Session.GetBeginEHSM(),
                    new SendMessageResponsePattern(),
                    cancelToken);
        }

        private async Task EndEHSMAsync(CancellationToken cancelToken)
        {

            Logger.LogInformation("End EHSM");
            await StartCommunication(Session.GetEndEHSM(),
                    new SendMessageResponsePattern(),
                    cancelToken);
        }

        private async Task StartBasalPatternAsync(CancellationToken cancelToken)
        {
            //Dictionary<int, List<UnitStartTime>> current = this.Session.BasalPatterns;
            //this.Session.BasalPatterns = new Dictionary<int, List<UnitStartTime>>();

            Logger.LogInformation("Getting BasalPatterns");
            ////check for 8 PumpBasal
            for (int i = 1; i <= 8; i++)
            {
                await StartCommunicationStandardResponse(Session.GetPumpBasalPattern(i), cancelToken);
            }


            //TODO: check if new basalPatterns, maybe save to SQLite and publish to profile on nightscout, if changed (but not here)

        }

        private async Task StartPumpTimeAsync(CancellationToken cancelToken)
        {

            Logger.LogInformation("Getting Pumptime");
            await StartCommunicationStandardResponse(Session.GetSetting(AstmSendMessageType.TIME_REQUEST), cancelToken);

            if (Session.PumpTime != null && Session.PumpTime.PumpDateTime.HasValue)
            {
                Logger.LogInformation($"Got pumptime: {Session.PumpTime.PumpDateTime.Value.ToString()}");
            }
            else
            {
                throw new Exception("PumpDate has no value.");
            }
        }

        private async Task StartCollectPumpDataAsync(CancellationToken cancelToken)
        {

            Logger.LogInformation("Getting Pumpstatus");
            await StartCommunicationStandardResponse(Session.GetSetting(AstmSendMessageType.READ_PUMP_STATUS_REQUEST), cancelToken);
            if (Session.Status.Count > 0)
            {
                Logger.LogInformation($"Got pumpstatus: {Session.Status.Last().ToString()}");
                Logger.LogTrace($"Decrypted bytes: {BitConverter.ToString(Session.Status.Last().AllBytes)}");
            }
        }

        private async Task StartReadHistory(CancellationToken cancelToken)
        {
            await StartReadHistoryByType(HistoryDataTypeEnum.Sensor, cancelToken);
            await StartReadHistoryByType(HistoryDataTypeEnum.Pump, cancelToken);
        }

        private async Task StartReadHistoryByType(HistoryDataTypeEnum historytype, CancellationToken cancelToken)
        {
            await StartReadHistoryInfoAsync(historytype, cancelToken);
            await StartReadHistoryAsync(historytype, cancelToken);
            await StartReadHistoryEvents(cancelToken);

        }

        private async Task StartReadHistoryEvents(CancellationToken cancelToken)
        {
            await StartMultiPacketAsync(new byte[] { 0x00, 0xff }, cancelToken);
            await EndMultiPacketAsync(new byte[] { 0x01, 0xff }, cancelToken);

            if (Session.PumpDataHistory.CurrentMultiPacketHandler != null && Session.PumpDataHistory.CurrentMultiPacketHandler.WaitingForSegment)
            {
                if (!cancelToken.IsCancellationRequested && !this._communicationBlock.Erorrs)
                {
                    await StartReadHistoryEvents(cancelToken);
                }

            }
        }

        private async Task StartReadHistoryInfoAsync(HistoryDataTypeEnum historytype, CancellationToken cancelToken)
        {
            Logger.LogInformation($"ReadHistoryInfo: {historytype.ToString()}");
            await StartCommunicationStandardResponse(Session.GetReadHistoryInfo(historytype), cancelToken);

            if (Session.PumpDataHistory.MultiPacketHandlers.Count(e => e.ReadInfoResponse.HistoryDataType == historytype) == 0)
            {
                throw new Exception("Error reading historyInfo");
            }
        }

        private async Task StartGetSetting(AstmSendMessageType type, CancellationToken cancelToken)
        {

            DateTime lastReadDateTime = DateTime.Now;
            Logger.LogInformation(type.ToString());
            await StartCommunicationStandardResponse(Session.GetSetting(type), cancelToken);
        }

        private async Task StartReadHistoryAsync(HistoryDataTypeEnum historytype, CancellationToken cancelToken)
        {
            Logger.LogInformation($"ReadHistory: {historytype.ToString()}");

            int expectedSize = this.Session.PumpDataHistory.GetSize(historytype);

            CommunicationBlock communicationBlock = new CommunicationBlock();
            communicationBlock.Request = Session.GetReadHistory(historytype, expectedSize);
            communicationBlock.ExpectedResponses.Add(new SendMessageResponsePattern());
            communicationBlock.ExpectedResponses.Add(new RecieveMessageResponsePattern());
            communicationBlock.ExpectedResponses.Add(new RecieveMessageResponsePattern());
            communicationBlock.ExpectedResponses.Add(new RecieveMessageResponsePattern());


            await StartCommunication(communicationBlock, cancelToken);

        }

        private async Task StartMultiPacketAsync(byte[] bytes, CancellationToken cancelToken)
        {
            CommunicationBlock communicationBlock = new CommunicationBlock();
            communicationBlock.Request = Session.GetMultiPacket(bytes);
            communicationBlock.ExpectedResponses.Add(new SendMessageResponsePattern());

            if (Session.PumpDataHistory.CurrentMultiPacketHandler == null)
            {
                throw new Exception($"Error in getting InitiateMultiPacketTransferResponse. CurrentMultiPacketHandler is not set.");
            }

            if (Session.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment == null)
            {
                throw new Exception($"Error in getting InitiateMultiPacketTransferResponse. CurrentSegment is not set.");
            }

            int expectedMessages = Session.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment.Init.PacketsToFetch;

            for (int i = 0; i < expectedMessages; i++)
            {
                communicationBlock.ExpectedResponses.Add(new RecieveMessageResponsePattern());
            }

            communicationBlock.TimeoutSeconds = (int)Math.Ceiling((Decimal)(expectedMessages / 4));
            //communicationBlock.LogDataRecieved = false;
            if (communicationBlock.TimeoutSeconds < 5)
            {
                communicationBlock.TimeoutSeconds = 5;
            }

            Logger.LogInformation($"MultiPacket Start- expecting {expectedMessages} packets.");
            await StartCommunication(communicationBlock, cancelToken);

            //get  missing segments
            //await CheckSegments(cancelToken);


        }

        private async Task CheckSegments(CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }
            if (Session.PumpDataHistory.CurrentMultiPacketHandler != null && Session.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment != null)
            {
                var segment = Session.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment;
                Logger.LogInformation($"MultiPacket - got {segment.Packets.Count} messages.");
                var list = segment.GetMissingSegments();
                if (list.Count > 0)
                {
                    Logger.LogInformation($"MultiPacket - Missing {list.Count} message(s).");
                    foreach (var item in list)
                    {
                        Logger.LogInformation($"MultiPacket - Getting missing message number {item}.");
                        CommunicationBlock communicationBlock2 = new CommunicationBlock();
                        communicationBlock2.Request = Session.GetMissingSegments((ushort)item, 1);
                        communicationBlock2.ExpectedResponses.Add(new SendMessageResponsePattern());
                        communicationBlock2.ExpectedResponses.Add(new RecieveMessageResponsePattern());
                        await StartCommunication(communicationBlock2, cancelToken);
                    }
                    //check again
                    await CheckSegments(cancelToken);
                }

            }

        }

        private async Task EndMultiPacketAsync(byte[] bytes, CancellationToken cancelToken)
        {


            Logger.LogInformation("End MultiPacket");

            CommunicationBlock communicationBlock = new CommunicationBlock();
            communicationBlock.Request = Session.GetMultiPacket(bytes);


            communicationBlock.ExpectedResponses.Add(new SendMessageResponsePattern());
            communicationBlock.ExpectedResponses.Add(new RecieveMessageResponsePattern());
            communicationBlock.ExpectedResponses.Add(new RecieveMessageResponsePattern());
            communicationBlock.ExpectedResponses.Add(new RecieveMessageResponsePattern());


            await StartCommunication(communicationBlock, cancelToken);
        }

        private async Task EndModesAsync(CancellationToken cancelToken)
        {

            Logger.LogInformation("End PassthroughMode");
            await StartCommunication(new AstmStart("W|"), new ACKPattern(), cancelToken);
            await StartCommunication(new AstmStart("Q|"), new ACKPattern(), cancelToken);
            await StartCommunication(new AstmStart("0|"), new ACKPattern(), cancelToken);
        }

        private async Task CloseConnectionAsync(CancellationToken cancelToken)
        {
            Logger.LogInformation($"Closing connection");
            await StartCommunication(Session.GetCloseConnectionRequest(), new AnyResponsePattern(), cancelToken);
        }

        private async Task CloseAsync(CancellationToken cancelToken)
        {
            Logger.LogInformation("Close CNL");
            //sometimes we get a enq and sometimes not......
            await StartCommunication(new EOTMessage(), new ReportPattern(new byte[] { 001, ASCII.ENQ }, 4), cancelToken);
            //await StartCommunication(new AstmStart(new byte[] { 0x15 }), new ReportPattern(new byte[] { 001, ASCII.EOT }, 4), cancelToken);
            //await StartCommunication(new AstmStart(new byte[] { 0x05 }), new ReportPattern(new byte[] { 001, 0x06 }, 4), cancelToken);
        }

    }
}
