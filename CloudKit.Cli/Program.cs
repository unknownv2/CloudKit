using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using SteamKit2;

namespace CloudKit.Cli
{
    class Program
    {
        static SteamClient steamClient;
        static CallbackManager manager;

        static SteamUser steamUser;
        static SteamCloud steamCloud;

        static bool isRunning;

        static string user, pass;
        static string authCode, twoFactorAuth;

        // Directory to write downloaded files to
        static string saveDataDir = "Saves";

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Client: No username and password specified!");
                return;
            }

            // save our logon details
            user = args[0];
            pass = args[1];

            // create our steamclient instance
            steamClient = new SteamClient();
            // create the callback manager which will route callbacks to function calls
            manager = new CallbackManager(steamClient);

            // get the steamuser handler, which is used for logging on after successfully connecting
            steamUser = steamClient.GetHandler<SteamUser>();

            steamCloud = steamClient.GetHandler<SteamCloud>();

            // register a few callbacks we're interested in
            // these are registered upon creation to a callback manager, which will then route the callbacks
            // to the functions specified
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

            // this callback is triggered when the steam servers wish for the client to store the sentry file
            manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);

            isRunning = true;

            Console.WriteLine("Connecting to Steam...");

            // initiate the connection
            steamClient.Connect();

            // create our callback handling loop
            while (isRunning)
            {
                // in order for the callbacks to get routed, they need to be handled by the manager
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected to Steam! Logging in '{0}'...", user);

            byte[] sentryHash = null;
            if (File.Exists("sentry.bin"))
            {
                // if we have a saved sentry file, read and sha-1 hash it
                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                sentryHash = CryptoHelper.SHAHash(sentryFile);
            }

            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = user,
                Password = pass,

                // in this sample, we pass in an additional authcode
                // this value will be null (which is the default) for our first logon attempt
                AuthCode = authCode,

                // if the account is using 2-factor auth, we'll provide the two factor code instead
                // this will also be null on our first logon attempt
                TwoFactorCode = twoFactorAuth,

                // our subsequent logons use the hash of the sentry file as proof of ownership of the file
                // this will also be null for our first (no authcode) and second (authcode only) logon attempts
                SentryFileHash = sentryHash,
            });
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            // after recieving an AccountLogonDenied, we'll be disconnected from steam
            // so after we read an authcode from the user, we need to reconnect to begin the logon flow again

            Console.WriteLine("Disconnected from Steam, reconnecting in 5...");

            Thread.Sleep(TimeSpan.FromSeconds(5));

            steamClient.Connect();
        }

        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            bool isSteamGuard = callback.Result == EResult.AccountLogonDenied;
            bool is2FA = callback.Result == EResult.AccountLoginDeniedNeedTwoFactor;

            if (isSteamGuard || is2FA)
            {
                Console.WriteLine("This account is SteamGuard protected!");

                if (is2FA)
                {
                    Console.Write("Please enter your 2 factor auth code from your authenticator app: ");
                    twoFactorAuth = Console.ReadLine();
                }
                else
                {
                    Console.Write("Please enter the auth code sent to the email at {0}: ", callback.EmailDomain);
                    authCode = Console.ReadLine();
                }

                return;
            }

            if (callback.Result != EResult.OK)
            {
                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);

                isRunning = false;
                return;
            }

            Console.WriteLine("Successfully logged on!");

            // at this point, we'd be able to perform actions on Steam
        }

        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
        }

        static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            Console.WriteLine("Updating sentryfile...");

            // write out our sentry file
            // ideally we'd want to write to the filename specified in the callback
            // but then this sample would require more code to find the correct sentry file to read during logon
            // for the sake of simplicity, we'll just use "sentry.bin"

            int fileSize;
            byte[] sentryHash;
            using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);
                fileSize = (int)fs.Length;

                fs.Seek(0, SeekOrigin.Begin);
                using (var sha = SHA1.Create())
                {
                    sentryHash = sha.ComputeHash(fs);
                }
            }

            // inform the steam servers that we're accepting this sentry file
            steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash,
            });

            Console.WriteLine("Done!");
        }

        static string FormatUrl(string host, string path)
        {
            return $"https://{host}{path}";
        }

        static async void DownloadFiles(uint appID)
        {
            if (steamCloud == null)
            {
                return;
            }

            var fileListInfo = await steamCloud.GetFileListForApp(appID);

            foreach (var file in fileListInfo.Files)
            {
                var fileInfo = await steamCloud.GetSingleFileInfo(file.AppID, file.FileName);

                Console.WriteLine("Filename is: " + fileInfo.FileName);

                var downloadFileInfo = await steamCloud.ClientFileDownload(fileInfo.AppID, fileInfo.FileName);

                using (var client = new HttpClient())
                {
                    foreach (var header in downloadFileInfo.RequestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }

                    var fileData = await client.GetByteArrayAsync(FormatUrl(downloadFileInfo.UrlHost, downloadFileInfo.UrlPath));

                    var fileName = Path.Combine(saveDataDir, file.AppID.ToString(), fileInfo.FileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                    File.WriteAllBytes(fileName,
                        downloadFileInfo.RawFileSize != downloadFileInfo.FileSize ? ZipUtil.Decompress(fileData) : fileData);
                }
            }
        }

        static async void UploadFileTest(uint appId)
        {
            string testFileName = "savedata_test.bin";

            var testFileData = new byte[] { 0x1, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            var beginFileUploadResp = await steamCloud.RequestFileUpload(appId, testFileName, testFileData);

            if (beginFileUploadResp.BlockRequests.Count == 0)
            {
                CommitFile(appId, testFileName, testFileData, false);

                Console.WriteLine("File already exists in cloud");

                return;
            }

            using (var uploadClient = new HttpClient())
            {
                try
                {
                    using (HttpContent reqBody = new ByteArrayContent(testFileData))
                    {

                        foreach (var blockRequest in beginFileUploadResp.BlockRequests)
                        {
                            foreach (var header in blockRequest.RequestHeaders)
                            {
                                var headerName = header.Name;
                                var headerValue = header.Value;

                                Console.WriteLine("Adding header: {0}", headerName);

                                switch (headerName)
                                {
                                    case "Content-Type":
                                    case "Content-Length":
                                        reqBody.Headers.Add(headerName, headerValue);
                                        break;
                                    case "Content-Disposition":
                                        var disposition = headerValue;
                                        reqBody.Headers.Add(headerName, disposition.EndsWith(';') ? disposition.TrimEnd(';') : disposition);
                                        break;
                                    default:
                                        uploadClient.DefaultRequestHeaders.Add(headerName, headerValue);
                                        break;
                                }
                            }

                            var uri = FormatUrl(blockRequest.UrlHost, blockRequest.UrlPath);

                            var uploadResult = await uploadClient.PutAsync(uri, reqBody);
                            if (uploadResult.StatusCode == System.Net.HttpStatusCode.Created)
                            {
                                Console.WriteLine("File uploaded to cloud. Now committing...");
                            }
                            else
                            {
                                CommitFile(appId, testFileName, testFileData, false);

                                Console.WriteLine("File failed uploaded to cloud");

                                return;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    CommitFile(appId, testFileName, testFileData, false);

                    Console.WriteLine("File uploaded to cloud failed with {0}", exception.ToString());

                    return;
                }

                CommitFile(appId, testFileName, testFileData, true);

                Console.WriteLine("Successfully uploaded and committed file!");
            }
        }

        static async void CommitFile(uint appID, string fileName, byte[] fileData, bool commit)
        {
            await steamCloud.CommitFileUpload(appID, fileName, fileData, commit);
        }
    }
}