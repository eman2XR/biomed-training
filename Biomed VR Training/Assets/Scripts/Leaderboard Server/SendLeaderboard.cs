using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CBET {
    public class Discovery {
        private readonly byte[] FIND = Encoding.ASCII.GetBytes("FIND ");
        private readonly byte[] AT = Encoding.ASCII.GetBytes(" AT ");
        
        private readonly List<IPAddress> _cache;
        private readonly byte[] _discoveryType;

        private Discovery(byte[] discoveryType) {
            this._cache = new List<IPAddress>();
            this._discoveryType = discoveryType;
        }

        /// <summary>
        /// Sends an HTTP request representing all of the provided information to all of the discovered IP address on the local network
        /// </summary>
        /// <param name="client">
        /// The HttpClient instance for this application
        /// </param>
        /// <param name="path">
        /// The path to use in the HttpRequest
        /// </param>
        /// <param name="message">
        /// The message to be sent as the body of the request
        /// </param>
        /// <exception cref="HttpRequestException"></exception>
        public Task Send(HttpClient client, string path, byte[] message) {
            var task = Task.Run(async () => {
                int count;
                lock (_cache) {
                    count = _cache.Count;
                }

                if (count == 0) {
                    await Preload();
                }

                List<Task> requests = new List<Task>();
                
                lock (_cache) {
                    foreach (var address in _cache) {
                        requests.Add(SendRequest(address, client, path, message));
                    }
                }

                foreach (var request in requests) {
                    await request;
                }
            });
            return task;
        }

        /// <summary>
        /// Sends the HTTP request asynchronously to the server
        /// </summary>
        /// <param name="address">
        /// The address to send the request to
        /// </param>
        /// <param name="client">
        /// The global client object to send the request through
        /// </param>
        /// <param name="path">
        /// The URI path for the HTTP request
        /// </param>
        /// <param name="message">
        /// The POST body of the message
        /// </param>
        /// <returns>
        /// A task for the request
        /// </returns>
        /// <exception cref="HttpRequestException">
        /// If the request gets a response of 400 instead of the expected 200
        /// </exception>
        private static Task SendRequest(IPAddress address, HttpClient client, string path, byte[] message) {
            return Task.Run(async () => {
                var uri = "http://" + address.MapToIPv4() +
                          path; //The request should end up looking something like 'http://192.168.1.10/path' for IPv6 the request would need brackets 'http://[ipv6]/path'
                var response = await client.PostAsync(uri, new ByteArrayContent(message));

                if (response.StatusCode == HttpStatusCode.BadRequest) {
                    response = await client.PostAsync(uri, new ByteArrayContent(message));
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        throw new HttpRequestException("Received response from server with status of 400");
                }
            });
        }

        /// <summary>
        /// Preloads all of the available Servers for the _discoveryType of this Discovery Object.
        /// The loading is done by broadcasting on port 8095 and waiting for at-least one response.
        /// </summary>
        /// <exception cref="TimeoutException">
        /// If it takes longer than 5 seconds to retrieve at-least one response a TimeoutException will be thrown
        /// </exception>
        public Task Preload() {
            var task = Task.Run(async () => {
                var udpClient = new UdpClient();
                udpClient.Client.EnableBroadcast = true;
                udpClient.Client.ReceiveTimeout = 1000; //Wait 1 second before timing out

                var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, 8095);

                var findMessage = FIND.Concat(_discoveryType).ToArray();

                await udpClient.SendAsync(findMessage, findMessage.Length, broadcastEndpoint);

                var requestedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                var from = new IPEndPoint(0, 0);
                while (true) {
                    try {
                        var received = udpClient.Receive(ref from);

                        if (SectionEquals(received, 0, _discoveryType) &&
                            SectionEquals(received, _discoveryType.Length, AT)) {
                            var ipOffset = (_discoveryType.Length + AT.Length);
                            var ipStr = Encoding.ASCII.GetString(received, ipOffset, received.Length - ipOffset);

                            lock (_cache) {
                                if (IPAddress.TryParse(ipStr, out var address) && !_cache.Contains(address)) 
                                    _cache.Add(address);
                            }
                        }
                    }
                    catch (SocketException) {
                        lock (_cache) {
                            if (_cache.Count != 0)
                                break;
                        }

                        if ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - requestedAt) > 5000)
                            throw new TimeoutException("Took to long to find the leaderboard server");
                    }
                }

                udpClient.Close();
            });

            return task;
        }

        /// <summary>
        /// A simple method which checks if a section of an array equals another array
        /// </summary>
        /// <param name="buffer">
        /// The array to check the section of
        /// </param>
        /// <param name="from">
        /// The offset to use for the buffer
        /// </param>
        /// <param name="msg">
        /// A byte array to test against the buffer
        /// </param>
        /// <returns>
        /// A boolean value representing whether or not the section equal based on the provided values
        /// </returns>
        private static bool SectionEquals(byte[] buffer, int from, byte[] msg) {
            if(from + msg.Length >= buffer.Length)
                return false;


            for(var x = 0; x < msg.Length; x++) {
                if(buffer[from + x] != msg[x])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new instance of this Discovery class which enables discovering servers running on the local network.
        /// </summary>
        /// <param name="type">
        /// The type of server to discover
        /// </param>
        /// <returns>
        /// A Discovery object which can be used for finding servers of the specified type than sending packets to those servers.
        /// </returns>
        public static Discovery Find(string type) {
            return new Discovery(Encoding.ASCII.GetBytes(type));
        }
    }

    /// <summary>
    /// This class represents a leader board entry
    /// </summary>
    [Serializable]
    public struct LeaderboardEntry {
        /// <summary>
        /// The name of the user
        /// </summary>
        public string Name;

        /// <summary>
        /// The time the user took in order to complete all the modules
        /// </summary>
        public string TimeTaken;

        /// <summary>
        /// The number of questions the user got correct
        /// </summary>
        public int UserScore;

        /// <summary>
        /// The maximum possible score the user could have gotten on the questions
        /// </summary>
        public int MaxScore;

        public LeaderboardEntry(string name, long timeTaken, int userScore, int maxScore) {
            this.Name = name;
            this.TimeTaken = timeTaken.ToString();
            this.UserScore = userScore;
            this.MaxScore = maxScore;
        }
    }

    public class SendLeaderboard {
        private static readonly SendLeaderboard Instance = new SendLeaderboard();

        /// <summary>
        /// The Discovery Object used to find the leaderboard server
        /// </summary>
        private readonly Discovery _discovery;

        /// <summary>
        /// The HttpClient to be used for all of the requests to the leaderboard
        /// </summary>
        private readonly HttpClient _httpClient;

        private SendLeaderboard() {
            this._discovery = Discovery.Find("LEADERBOARD");
            this._httpClient = new HttpClient();
            this._httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// Preloads the location of the leaderboard server(s) so that it is ready whenever the requests start
        /// </summary>
        public async void Preload() {
            await this._discovery.Preload();
        }

        /// <summary>
        /// Asynchronously submits a new leaderboard entry to the leaderboard server through a post request
        /// </summary>
        /// <param name="entry">
        /// The entry to send to the leaderboard server
        /// </param>
        public async void NewEntry(LeaderboardEntry entry) {
            await this._discovery.Send(this._httpClient, "/leaderboard", Encoding.UTF8.GetBytes(JsonUtility.ToJson(entry)));
        }

        /// <summary>
        /// This method is used to get the singleton instance for this class which can then be used to either preload the Leaderboard server location(s) or send an entry to the Leaderboard server(s).
        /// </summary>
        /// <returns>
        /// The singleton for the Leaderboard class
        /// </returns>
        public static SendLeaderboard Get() {
            return Instance;
        }
    }
}
