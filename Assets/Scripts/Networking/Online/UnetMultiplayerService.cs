namespace OnlineService {
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;
    using System.Collections.Generic;
    using System.Linq;

    public class UnetMultiplayerService : IMultiplayerService {
        private class LobbySnapshot : ILobbySnapshot {
            public string name {
                get {
                    return m.name;
                }
            }
            public LobbyType type {
                get {
                    return m.isPrivate ? LobbyType.FriendsOnly : LobbyType.Public;
                }
            }
            public int currentSize {
                get {
                    return m.currentSize;
                }
            }
            public bool hasPassword {
                get {
                    return false; // can't determine
                }
            }
            private readonly MatchInfoSnapshot m;
            private readonly IConnectionSurrogate connectionSurrogate;
            private Promise<ILobby> promise;
            private readonly int domain;

            public LobbySnapshot (MatchInfoSnapshot m, IConnectionSurrogate connectionSurrogate, int domain) {
                this.m = m;
                this.connectionSurrogate = connectionSurrogate;
                this.domain = domain;
            }

            public Future<ILobby> Join (string password) {
                this.promise = new Promise<ILobby>();
                string publicAddrData = null;
                foreach (var info in m.directConnectInfos) {
                    if (!string.IsNullOrEmpty(info.publicAddress)) {
                        publicAddrData = info.publicAddress;
                        break;
                    }
                }
                if (connectionSurrogate != null && publicAddrData != null) {
                    NetworkManager.singleton.matchMaker.JoinMatch(m.networkId, password, "", "", 0, 0, (success, extendedInfo, match) => {
                        if (success) {
                            connectionSurrogate.GetJoinAddress(publicAddrData).OnReturn((addr) => {
                                var manager = NetworkManager.singleton;
                                int colonIndex = addr.IndexOf(':');
                                if (colonIndex == -1) {
                                    promise.Error(null);
                                    return;
                                }
                                manager.networkAddress = addr.Substring(0, colonIndex);
                                manager.networkPort = int.Parse(addr.Substring(colonIndex + 1));
                                manager.StartClient(); // TODO only show network lobby after connecting
                                promise.Return(new Lobby(match, name, false, domain, connectionSurrogate));
                            }).OnError((info) => {
                                promise.Error("surrogate error: " + info);
                            });
                        } else {
                            UnityEngine.Debug.LogError("Join Failed: " + extendedInfo);
                            promise.Error("Join Failed: " + extendedInfo);
                        }
                    });
                } else {
                    NetworkManager.singleton.matchMaker.JoinMatch(m.networkId, password, "", "", 0, 0, OnJoin);
                }
                return this.promise;
            }

            private void OnJoin (bool success, string extendedInfo, MatchInfo match) {
                if (success) {
                    NetworkManager.singleton.StartClient(match);
                    promise.Return(new Lobby(match, name, false, domain, connectionSurrogate));
                } else {
                    UnityEngine.Debug.LogError("Join Failed: " + extendedInfo);
                    promise.Error("Join Failed: " + extendedInfo);
                }
            }
        }

        private class Lobby : ILobby {
            private readonly MatchInfo match;
            public bool isHost { get; private set; }
            public string name { get; private set; }
            private readonly int domain;

            private readonly IConnectionSurrogate connectionSurrogate;

            public Lobby (MatchInfo match, string name, bool isHost, int domain, IConnectionSurrogate connectionSurrogate) {
                this.match = match;
                this.isHost = isHost;
                this.name = name;
                this.domain = domain;
                this.connectionSurrogate = connectionSurrogate;
            }

            public Future Leave () {
                var leavePromise = new Promise();
                if (isHost) {
                    NetworkManager.singleton.matchMaker.DestroyMatch(match.networkId, domain, leavePromise.CallbackHandler);
                    if (relayKeeperHostId != -1) {
                        NetworkTransport.RemoveHost(relayKeeperHostId);
                        relayKeeperHostId = -1;
                    }
                    NetworkManager.singleton.StopHost();
                } else {
                    NetworkManager.singleton.matchMaker.DropConnection(match.networkId, match.nodeId, domain, leavePromise.CallbackHandler);
                }
                if (connectionSurrogate != null) {
                    connectionSurrogate.Deactivate();
                    connectionSurrogate.Activate();
                }
                return leavePromise;
            }

            public Future Lock () {
                var lockPromise = new Promise();
                if (isHost) {
                    if (connectionSurrogate != null) {
                        if (relayKeeperHostId != -1) {
                            byte error;
                            NetworkTransport.DisconnectNetworkHost(relayKeeperHostId, out error);
                            NetworkTransport.RemoveHost(relayKeeperHostId);
                            relayKeeperHostId = -1;
                            var err = (NetworkError)error;
                            if (err != NetworkError.Ok) {
                                UnityEngine.Debug.LogError(err);
                            }
                        }
                        NetworkManager.singleton.matchMaker.DestroyMatch(match.networkId, domain, lockPromise.CallbackHandler);
                    } else {
                        NetworkManager.singleton.matchMaker.SetMatchAttributes(match.networkId, false, domain, lockPromise.CallbackHandler);
                    }
                } else {
                    lockPromise.Error("not the host");
                }
                return lockPromise;
            }
        }

        private IConnectionSurrogate connectionSurrogate;

        public UnetMultiplayerService (IConnectionSurrogate connectionSurrogate = null) {
            this.connectionSurrogate = connectionSurrogate;
        }

        public void Activate () {
            if (NetworkManager.singleton.matchMaker == null) {
                NetworkManager.singleton.StartMatchMaker();
                if (connectionSurrogate != null) {
                    connectionSurrogate.Activate();
                }
            }
        }

        public void Deactivate () {
            if (NetworkManager.singleton.matchMaker != null) {
                NetworkManager.singleton.StopMatchMaker();
                if (connectionSurrogate != null) {
                    connectionSurrogate.Deactivate();
                }
            }
        }

        public void Update () {
            if (connectionSurrogate != null) {
                connectionSurrogate.Update();
            }
        }

        private Promise<ILobby> createPromise;

        public Future<ILobby> CreateLobby (string name, int size, string userID, LobbyType lobbyType, string password, int domain) {
            Activate();
            this.createPromise = new Promise<ILobby>();
            if (connectionSurrogate == null) {
                NetworkManager.singleton.matchMaker.CreateMatch(name, (uint)size, lobbyType == LobbyType.Public, password, "", "", 0, domain, (success, extendedInfo, match) => OnMatchCreated(success, extendedInfo, match, name, domain));
            } else {
                var manager = NetworkManager.singleton;
                manager.StartHost();
                connectionSurrogate.GetHostData(manager.networkAddress, NetworkTransport.GetHostPort(NetworkServer.serverHostId)).OnReturn((hostData) => {
                    NetworkManager.singleton.matchMaker.CreateMatch(name, (uint)size, lobbyType == LobbyType.Public, password, hostData, "", 0, domain, (success, extendedInfo, match) => {
                        if (success) {
                            ConnectRelayToKeepInList(match);
                            createPromise.Return(new Lobby(match, name, true, domain, connectionSurrogate));
                        } else {
                            UnityEngine.Debug.LogError("Match Creation Failed: " + extendedInfo);
                            createPromise.Error("Match Creation Failed: " + extendedInfo);
                        }
                    });
                }).OnError((info) => {
                    manager.StopHost();
                    createPromise.Error("surrogate error:" + info);
                });
            }
            return this.createPromise;
        }

        private Promise<List<ILobbySnapshot>> requestPromise;

        public Future<List<ILobbySnapshot>> RequestLobbies (int resultSize, string filter, int domain) {
            Activate();
            this.requestPromise = new Promise<List<ILobbySnapshot>>();
            NetworkManager.singleton.matchMaker.ListMatches(0, resultSize, filter, true, 0, domain, (success, extendedInfo, matches) => OnMatchList(success, extendedInfo, matches, domain));
            return this.requestPromise;
        }

        public Future<List<ILobbySnapshot>> RequestUserLobbies (List<string> activeFriendList, bool includeInviteOnly, int domain) {
            var promise = new Promise<List<ILobbySnapshot>>();
            promise.Return(new List<ILobbySnapshot>());
            return promise;
        }

        private void OnMatchCreated (bool success, string extendedInfo, MatchInfo match, string name, int domain) {
            if (success) {
                NetworkGameManager.singleton.StartHost(match);
                createPromise.Return(new Lobby(match, name, true, domain, connectionSurrogate));
            } else {
                UnityEngine.Debug.LogError("Match Creation Failed: " + extendedInfo);
                createPromise.Error("Match Creation Failed: " + extendedInfo);
            }
        }

        private void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> matches, int domain) {
            if (success) {
                this.requestPromise.Return(matches.Select(m => new LobbySnapshot(m, connectionSurrogate, domain) as ILobbySnapshot).ToList());
            } else {
                UnityEngine.Debug.LogError("List Matches Failed: " + extendedInfo);
                this.requestPromise.Error("List Matches Failed: " + extendedInfo);
            }
        }

        static private int relayKeeperHostId = -1;

        private void ConnectRelayToKeepInList (MatchInfo info) {
            byte error;
            if (relayKeeperHostId == -1) {
                relayKeeperHostId = NetworkTransport.AddHost(NetworkServer.hostTopology);
            } else {
                NetworkTransport.DisconnectNetworkHost(relayKeeperHostId, out error);
            }
            NetworkTransport.ConnectAsNetworkHost(relayKeeperHostId, info.address, info.port, info.networkId, Utility.GetSourceID(), info.nodeId, out error);
            var err = (NetworkError)error;
            if (err != NetworkError.Ok) {
                UnityEngine.Debug.LogError(err);
            }
        }
    }

    static class ResponseDelegateHelper {
        static public void CallbackHandler (this Promise promise, bool success, string extendedInfo) {
            if (success) {
                promise.Return();
            } else {
                promise.Error(extendedInfo);
            }
        }
    }
}
