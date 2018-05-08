/*
 * This file is subject to the terms and conditions defined in
 * file 'license.txt', which is part of this source code package.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SteamKit2.Internal;
using SteamKit2.Unified.Internal;

namespace SteamKit2
{
    public sealed partial class SteamCloud
    {
        public enum SteamCloudHttpMethod
        {
            Invalid = 0,
            Get,
            Head,
            Post,
            Put,
            Delete,
            Options
        }

        public sealed class HttpHeaders
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public sealed class CloudFileUploadBlockInfo
        {
            public string UrlHost;

            public string UrlPath;

            public bool UseHttps;

            public SteamCloudHttpMethod HttpMethod;

            public List<HttpHeaders> RequestHeaders;
        }

        public sealed class UFSFileInfo
        {
            public uint AppID;

            public string FileName;

            public byte[] FileHash;

            public ulong TimeStamp;

            public bool ExplicitDelete;

            public uint PlatformsToSync;

            public uint PathPrefixIndex;

            public uint RawFileSize;
        }

        /// <summary>
        /// This callback is recieved in response to calling <see cref="RequestUGCDetails"/>.
        /// </summary>
        public sealed class UGCDetailsCallback : CallbackMsg
        {
            /// <summary>
            /// Gets the result of the request.
            /// </summary>
            public EResult Result { get; private set; }

            /// <summary>
            /// Gets the App ID the UGC is for.
            /// </summary>
            public uint AppID { get; private set; }
            /// <summary>
            /// Gets the SteamID of the UGC's creator.
            /// </summary>
            public SteamID Creator { get; private set; }

            /// <summary>
            /// Gets the URL that the content is located at.
            /// </summary>
            public string URL { get; private set; }

            /// <summary>
            /// Gets the name of the file.
            /// </summary>
            public string FileName { get; private set; }
            /// <summary>
            /// Gets the size of the file.
            /// </summary>
            public uint FileSize { get; private set; }


            internal UGCDetailsCallback(JobID jobID, CMsgClientUFSGetUGCDetailsResponse msg)
            {
                JobID = jobID;

                Result = (EResult)msg.eresult;

                AppID = msg.app_id;
                Creator = msg.steamid_creator;

                URL = msg.url;

                FileName = msg.filename;
                FileSize = msg.file_size;
            }
        }

        /// <summary>
        /// This callback is recieved in response to calling <see cref="GetSingleFileInfo"/>.
        /// </summary>
        public sealed class SingleFileInfoCallback : CallbackMsg
        {
            /// <summary>
            /// Gets the result of the request.
            /// </summary>
            public EResult Result { get; private set; }

            /// <summary>
            /// Gets the App ID the file is for.
            /// </summary>
            public uint AppID { get; private set; }
            /// <summary>
            /// Gets the file name request.
            /// </summary>
            public string FileName { get; private set; }

            /// <summary>
            /// Gets the SHA hash of the file.
            /// </summary>
            public byte[] SHAHash { get; private set; }

            /// <summary>
            /// Gets the timestamp of the file.
            /// </summary>
            public DateTime Timestamp { get; private set; }
            /// <summary>
            /// Gets the size of the file.
            /// </summary>
            public uint FileSize { get; private set; }

            /// <summary>
            /// Gets if the file was explicity deleted by the user.
            /// </summary>
            public bool IsExplicitDelete { get; private set; }

            internal SingleFileInfoCallback(JobID jobID, CMsgClientUFSGetSingleFileInfoResponse msg)
            {
                JobID = jobID;

                Result = (EResult)msg.eresult;

                AppID = msg.app_id;
                FileName = msg.file_name;
                SHAHash = msg.sha_file;
                Timestamp = DateUtils.DateTimeFromUnixTime(msg.time_stamp);
                FileSize = msg.raw_file_size;
                IsExplicitDelete = msg.is_explicit_delete;
            }
        }
        /// <summary>
        /// This callback is recieved in response to calling <see cref="ShareFile"/>.
        /// </summary>
        public sealed class ShareFileCallback : CallbackMsg
        {
            /// <summary>
            /// Gets the result of the request.
            /// </summary>
            public EResult Result { get; private set; }

            /// <summary>
            /// Gets the resulting UGC handle.
            /// </summary>
            public ulong UGCId { get; private set; }

            internal ShareFileCallback(JobID jobID, CMsgClientUFSShareFileResponse msg)
            {
                JobID = jobID;

                Result = (EResult)msg.eresult;

                UGCId = msg.hcontent;
            }
        }

        /// <summary>
        /// This callback is recieved in response to calling <see cref="GetSingleFileInfo"/>.
        /// </summary>
        public sealed class GetFileListForAppCallback : CallbackMsg
        {
            public List<UFSFileInfo> Files { get; private set; }

            public List<string> PathPrefixes { get; private set; }

            internal GetFileListForAppCallback(JobID jobID, CMsgClientUFSGetFileListForAppResponse msg)
            {
                JobID = jobID;

                Files = msg.files
                  .Select(file => new UFSFileInfo()
                  {
                      AppID = file.app_id,
                      FileName = file.file_name,
                      FileHash = file.sha_file,
                      TimeStamp = file.time_stamp,
                      ExplicitDelete = file.is_explicit_delete,
                      PlatformsToSync = file.platforms_to_sync,
                      PathPrefixIndex = file.path_prefix_index,
                      RawFileSize = file.raw_file_size
                  })
                  .ToList();

                PathPrefixes = msg.path_prefixes;
            }
        }

        /// <summary>
        /// This callback is recieved in response to calling <see cref="GetSingleFileInfo"/>.
        /// </summary>
        public sealed class ClientFileDownloadCallback : CallbackMsg
        {
            public uint AppId { get; private set; }

            public string UrlHost { get; private set; }

            public string UrlPath { get; private set; }

            public bool UseHttps { get; private set; }

            public uint FileSize { get; private set; }

            public uint RawFileSize { get; private set; }

            public List<HttpHeaders> RequestHeaders = new List<HttpHeaders>();

            public byte[] ShaFile { get; private set; }

            public ulong TimeStamp { get; private set; }

            public bool Encrypted { get; private set; }

            internal ClientFileDownloadCallback(JobID jobID, CCloud_ClientFileDownload_Response msg)
            {
                JobID = jobID;

                AppId = msg.appid;

                UrlHost = msg.url_host;

                UrlPath = msg.url_path;

                UseHttps = msg.use_https;

                FileSize = msg.file_size;

                RawFileSize = msg.raw_file_size;

                RequestHeaders = msg.request_headers
                  .Select(header => new HttpHeaders()
                  {
                      Name = header.name,
                      Value = header.value
                  })
                  .ToList();

                Encrypted = msg.encrypted;

                ShaFile = msg.sha_file;

                TimeStamp = msg.time_stamp;

            }
        }

        /// <summary>
        /// This callback is recieved in response to calling <see cref="GetSingleFileInfo"/>.
        /// </summary>
        public sealed class ClientBeginFileUploadCallback : CallbackMsg
        {
            public List<CloudFileUploadBlockInfo> BlockRequests;

            public List<ClientCloudFileUploadBlockDetails> BlockRequests2;

            public bool EncryptFile { get; private set; }

            internal ClientBeginFileUploadCallback(JobID jobID, CCloud_ClientBeginFileUpload_Response msg)
            {
                JobID = jobID;

                BlockRequests = msg.block_requests
                  .Select(request => new CloudFileUploadBlockInfo()
                  {
                      UrlHost = request.url_host,
                      UrlPath = request.url_path,
                      UseHttps = request.use_https,
                      HttpMethod = (SteamCloudHttpMethod)request.http_method,
                      RequestHeaders = request.request_headers
                          .Select(header => new HttpHeaders
                          {
                              Name = header.name,
                              Value = header.value
                          })
                          .ToList()
                    })
                  .ToList();

                EncryptFile = msg.encrypt_file;
            }
        }
        /// <summary>
        /// This callback is recieved in response to calling <see cref="GetSingleFileInfo"/>.
        /// </summary>
        public sealed class ClientCommitFileUploadCallback : CallbackMsg
        {
            /// <summary>
            /// Gets the result of the upload request, whether the file is committed to the server.
            /// </summary>
            public bool FileCommitted { get; private set; }

            internal ClientCommitFileUploadCallback(JobID jobID, CCloud_ClientCommitFileUpload_Response msg)
            {
                JobID = jobID;

                FileCommitted = msg.file_committed;
            }
        }
    }
}
