/*
 * This file is subject to the terms and conditions defined in
 * file 'license.txt', which is part of this source code package.
 */



using System;
using System.Collections.Generic;
using SteamKit2.Internal;
using SteamKit2.Unified.Internal;

namespace SteamKit2
{
    /// <summary>
    /// This handler is used for interacting with remote storage and user generated content.
    /// </summary>
    public sealed partial class SteamCloud : ClientMsgHandler
    {
        private List<JobID> _clientFileDownloadJobs = new List<JobID>();
        private List<JobID> _clientBeginFileUploadJobs = new List<JobID>();
        private List<JobID> _clientCommitFileJobs = new List<JobID>();

        Dictionary<EMsg, Action<IPacketMsg>> dispatchMap;

        internal SteamCloud()
        {
            dispatchMap = new Dictionary<EMsg, Action<IPacketMsg>>
            {
                { EMsg.ClientUFSGetUGCDetailsResponse, HandleUGCDetailsResponse },
                { EMsg.ClientUFSGetSingleFileInfoResponse, HandleSingleFileInfoResponse },
                { EMsg.ClientUFSGetFileListForAppResponse, HandleGetFileListForAppResponse },
                { EMsg.ClientUFSShareFileResponse, HandleShareFileResponse },
                { EMsg.ServiceMethodResponse, HandleServiceMethodResponse }
            };
        }

        /// <summary>
        /// Requests details for a specific item of user generated content from the Steam servers.
        /// Results are returned in a <see cref="UGCDetailsCallback"/>.
        /// The returned <see cref="AsyncJob{T}"/> can also be awaited to retrieve the callback result.
        /// </summary>
        /// <param name="ugcId">The unique user generated content id.</param>
        /// <returns>The Job ID of the request. This can be used to find the appropriate <see cref="UGCDetailsCallback"/>.</returns>
        public AsyncJob<UGCDetailsCallback> RequestUGCDetails(UGCHandle ugcId)
        {
            if (ugcId == null)
            {
                throw new ArgumentNullException(nameof(ugcId));
            }

            var request = new ClientMsgProtobuf<CMsgClientUFSGetUGCDetails>(EMsg.ClientUFSGetUGCDetails);
            request.SourceJobID = Client.GetNextJobID();

            request.Body.hcontent = ugcId;

            this.Client.Send(request);

            return new AsyncJob<UGCDetailsCallback>(this.Client, request.SourceJobID);
        }

        /// <summary>
        /// Request a list of files stored in the cloudfor a target App Id
        /// Results are returned in a <see cref="DepotKeyCallback"/> callback.
        /// The returned <see cref="AsyncJob{T}"/> can also be awaited to retrieve the callback result.
        /// </summary>
        /// <param name="appid">The AppID to request the file list for.</param>
        /// <returns>The Job ID of the request. This can be used to find the appropriate <see cref="DepotKeyCallback"/>.</returns>
        public AsyncJob<GetFileListForAppCallback> GetFileListForApp(uint appid = 0)
        {
            var request = new ClientMsgProtobuf<CMsgClientUFSGetFileListForApp>(EMsg.ClientUFSGetFileListForApp);
            request.SourceJobID = Client.GetNextJobID();

            var appList = new List<uint>();
            appList.Add(appid);
            request.Body.apps_to_query = appList;

            Client.Send(request);

            return new AsyncJob<GetFileListForAppCallback>(Client, request.SourceJobID);
        }

        /// <summary>
        /// Requests details for a specific file in the user's Cloud storage.
        /// Results are returned in a <see cref="SingleFileInfoCallback"/>.
        /// The returned <see cref="AsyncJob{T}"/> can also be awaited to retrieve the callback result.
        /// </summary>
        /// <param name="appid">The app id of the game.</param>
        /// <param name="filename">The path to the file being requested.</param>
        /// <returns>The Job ID of the request. This can be used to find the appropriate <see cref="SingleFileInfoCallback"/>.</returns>
        public AsyncJob<SingleFileInfoCallback> GetSingleFileInfo(uint appid, string filename)
        {
            var request = new ClientMsgProtobuf<CMsgClientUFSGetSingleFileInfo>(EMsg.ClientUFSGetSingleFileInfo);
            request.SourceJobID = Client.GetNextJobID();

            request.Body.app_id = appid;
            request.Body.file_name = filename;

            this.Client.Send(request);

            return new AsyncJob<SingleFileInfoCallback>(this.Client, request.SourceJobID);
        }

        /// <summary>
        /// Request a list of files stored in the cloudfor a target App Id
        /// Results are returned in a <see cref="DepotKeyCallback"/> callback.
        /// The returned <see cref="AsyncJob{T}"/> can also be awaited to retrieve the callback result.
        /// </summary>
        /// <param name="appid">The AppID to request the file list for.</param>
        /// <returns>The Job ID of the request. This can be used to find the appropriate <see cref="DepotKeyCallback"/>.</returns>
        public AsyncJob<ClientFileDownloadCallback> ClientFileDownload(uint appid, string fileName)
        {
            var request = new ServiceCallMsgProtobuf<CCloud_ClientFileDownload_Request>(EMsg.ServiceMethodCallFromClient);
            request.SourceJobID = Client.GetNextJobID();

            request.Body.appid = appid;
            request.Body.filename = fileName;
            request.TargetJobName = SteamCloudServiceJobConstants.ClientFileDownload;

            lock (_clientFileDownloadJobs)
            {
                _clientFileDownloadJobs.Add(request.SourceJobID);
            }

            Client.Send(request);

            return new AsyncJob<ClientFileDownloadCallback>(Client, request.SourceJobID);
        }

        /// <summary>
        /// Request a list of files stored in the cloudfor a target App Id
        /// Results are returned in a <see cref="DepotKeyCallback"/> callback.
        /// The returned <see cref="AsyncJob{T}"/> can also be awaited to retrieve the callback result.
        /// </summary>
        /// <param name="appid">The AppID to request the file list for.</param>
        /// <returns>The Job ID of the request. This can be used to find the appropriate <see cref="DepotKeyCallback"/>.</returns>
        public AsyncJob<ClientBeginFileUploadCallback> RequestFileUpload(uint appID, string fileName, byte[] fileData, bool compress = false)
        {
            byte[] compressedData = compress ? ZipUtil.Compress(fileData) : fileData;

            var request = new ServiceCallMsgProtobuf<CCloud_ClientBeginFileUpload_Request>(EMsg.ServiceMethodCallFromClient);
            request.SourceJobID = Client.GetNextJobID();

            request.Body.appid = appID;
            request.Body.is_shared_file = false;
            request.Body.can_encrypt = true;
            request.Body.filename = fileName;
            request.Body.file_size = (uint)compressedData.Length;
            request.Body.raw_file_size = (uint)fileData.Length;
            request.Body.file_sha = CryptoHelper.SHAHash(fileData);
            request.Body.time_stamp = DateUtils.DateTimeToUnixTime(DateTime.UtcNow);
            request.TargetJobName = SteamCloudServiceJobConstants.ClientBeginFileUpload;

            lock (_clientBeginFileUploadJobs)
            {
                _clientBeginFileUploadJobs.Add(request.SourceJobID);
            }

            Client.Send(request);

            return new AsyncJob<ClientBeginFileUploadCallback>(Client, request.SourceJobID);
        }

        /// <summary>
        /// Request a list of files stored in the cloudfor a target App Id
        /// Results are returned in a <see cref="DepotKeyCallback"/> callback.
        /// The returned <see cref="AsyncJob{T}"/> can also be awaited to retrieve the callback result.
        /// </summary>
        /// <param name="appid">The AppID to request the file list for.</param>
        /// <returns>The Job ID of the request. This can be used to find the appropriate <see cref="DepotKeyCallback"/>.</returns>
        public AsyncJob<ClientCommitFileUploadCallback> CommitFileUpload(uint appID, string fileName, byte[] fileData, bool succeeded = false)
        {
            var request = new ServiceCallMsgProtobuf<CCloud_ClientCommitFileUpload_Request>(EMsg.ServiceMethodCallFromClient);
            request.SourceJobID = Client.GetNextJobID();

            request.Body.appid = appID;
            request.Body.transfer_succeeded = succeeded;
            request.Body.filename = fileName;
            request.Body.file_sha = CryptoHelper.SHAHash(fileData);
            request.TargetJobName = SteamCloudServiceJobConstants.ClientCommitFileUpload;

            lock (_clientCommitFileJobs)
            {
                _clientCommitFileJobs.Add(request.SourceJobID);
            }

            Client.Send(request);

            return new AsyncJob<ClientCommitFileUploadCallback>(Client, request.SourceJobID);
        }

        /// <summary>
        /// Commit a Cloud file at the given path to make its UGC handle publicly visible.
        /// Results are returned in a <see cref="ShareFileCallback"/>.
        /// The returned <see cref="AsyncJob{T}"/> can also be awaited to retrieve the callback result.
        /// </summary>
        /// <param name="appid">The app id of the game.</param>
        /// <param name="filename">The path to the file being requested.</param>
        /// <returns>The Job ID of the request. This can be used to find the appropriate <see cref="ShareFileCallback"/>.</returns>
        public AsyncJob<ShareFileCallback> ShareFile(uint appid, string filename)
        {
            var request = new ClientMsgProtobuf<CMsgClientUFSShareFile>(EMsg.ClientUFSShareFile);
            request.SourceJobID = Client.GetNextJobID();

            request.Body.app_id = appid;
            request.Body.file_name = filename;

            this.Client.Send(request);

            return new AsyncJob<ShareFileCallback>(this.Client, request.SourceJobID);
        }

        /// <summary>
        /// Handles a client message. This should not be called directly.
        /// </summary>
        /// <param name="packetMsg">The packet message that contains the data.</param>
        public override void HandleMsg(IPacketMsg packetMsg)
        {
            if (packetMsg == null)
            {
                throw new ArgumentNullException(nameof(packetMsg));
            }

            bool haveFunc = dispatchMap.TryGetValue(packetMsg.MsgType, out var handlerFunc);

            if (!haveFunc)
            {
                // ignore messages that we don't have a handler function for
                return;
            }

            handlerFunc(packetMsg);
        }

        #region ClientMsg Handlers
        void HandleUGCDetailsResponse(IPacketMsg packetMsg)
        {
            var infoResponse = new ClientMsgProtobuf<CMsgClientUFSGetUGCDetailsResponse>(packetMsg);

            var callback = new UGCDetailsCallback(infoResponse.TargetJobID, infoResponse.Body);
            this.Client.PostCallback(callback);
        }

        void HandleSingleFileInfoResponse(IPacketMsg packetMsg)
        {
            var infoResponse = new ClientMsgProtobuf<CMsgClientUFSGetSingleFileInfoResponse>(packetMsg);

            var callback = new SingleFileInfoCallback(infoResponse.TargetJobID, infoResponse.Body);
            this.Client.PostCallback(callback);
        }

        void HandleGetFileListForAppResponse(IPacketMsg packetMsg)
        {
            var infoResponse = new ClientMsgProtobuf<CMsgClientUFSGetFileListForAppResponse>(packetMsg);

            var callback = new GetFileListForAppCallback(infoResponse.TargetJobID, infoResponse.Body);
            this.Client.PostCallback(callback);
        }

        void HandleShareFileResponse(IPacketMsg packetMsg)
        {
            var shareResponse = new ClientMsgProtobuf<CMsgClientUFSShareFileResponse>(packetMsg);

            var callback = new ShareFileCallback(shareResponse.TargetJobID, shareResponse.Body);
            this.Client.PostCallback(callback);
        }

        void HandleServiceMethodResponse(IPacketMsg packetMsg)
        {
            CallbackMsg callback = null;

            var jobId = packetMsg.TargetJobID;

            if (_clientFileDownloadJobs.Contains(jobId))
            {
                lock (_clientFileDownloadJobs)
                {
                    _clientFileDownloadJobs.Remove(jobId);
                }

                var shareResponse = new ClientMsgProtobuf<CCloud_ClientFileDownload_Response>(packetMsg);

                callback = new ClientFileDownloadCallback(shareResponse.TargetJobID, shareResponse.Body);
            }
            else if (_clientBeginFileUploadJobs.Contains(jobId))
            {
                lock (_clientBeginFileUploadJobs)
                {
                    _clientBeginFileUploadJobs.Remove(jobId);
                }

                var shareResponse = new ClientMsgProtobuf<CCloud_ClientBeginFileUpload_Response>(packetMsg);

                callback = new ClientBeginFileUploadCallback(shareResponse.TargetJobID, shareResponse.Body);
            }
            else if (_clientCommitFileJobs.Contains(jobId))
            {
                lock (_clientCommitFileJobs)
                {
                    _clientCommitFileJobs.Remove(jobId);
                }

                var shareResponse = new ClientMsgProtobuf<CCloud_ClientCommitFileUpload_Response>(packetMsg);

                callback = new ClientCommitFileUploadCallback(shareResponse.TargetJobID, shareResponse.Body);
            }

            if (callback != null)
            {
                Client.PostCallback(callback);
            }
        }
        #endregion
    }
}
