using System;
using System.Threading;
using System.Threading.Tasks;
using Network.Extensions;
using Network.Models.Clans;
using Newtonsoft.Json;
using UnityEngine;

namespace Network.Core
{
    /// <summary>Clan API for UI. SnapshotUpdated and Invalidated run on Unity's main thread.</summary>
    public sealed class ClanHandler : IClientHandler
    {
        private readonly SemaphoreSlim requests = new SemaphoreSlim(1, 1);
        private Client client;
        private int charId;
        private volatile bool left;
        private int refreshRunning;
        private int refreshWanted;
        public ClanDetailDto Current { get; private set; }
        public event Action<ClanDetailDto> SnapshotUpdated;
        public event Action<ClanChangedDto> Invalidated;
        public MessageTag[] HandleTag => new[] { MessageTag.ClanChanged };

        public void OnEnter()
        {
            client = Client.Instance;
            charId = client.CharacterData.Id;
            client.TcpClient.ReconnectedSuccess += RequestRefresh;
            RequestRefresh();
        }
        public void OnLeave()
        {
            left = true;
            client.TcpClient.ReconnectedSuccess -= RequestRefresh;
            Current = null;
            SnapshotUpdated = null;
            Invalidated = null;
        }
        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
            NetworkManager.Instance.Invoke(() => { Current = null; SnapshotUpdated?.Invoke(null); });
        }
        public void Handle(Message<MessageTag> message)
        {
            var change = JsonConvert.DeserializeObject<ClanChangedDto>(message.Reader.Read());
            if (change == null || left) return;
            NetworkManager.Instance.Invoke(() => { if (!left) Invalidated?.Invoke(change); });
            RequestRefresh();
        }
        public Task<ClanResponse> SearchAsync(string search, long afterId = 0, int limit = 20) =>
            QueryAsync(new ClanQueryRequest { Query = ClanQueryType.Search, Search = search, AfterId = afterId, Limit = limit });

        public Task<ClanResponse> QueryAsync(ClanQueryRequest request)
        {
            if (request.RequestId == Guid.Empty) request.RequestId = Guid.NewGuid();
            return SendAsync(MessageTag.ClanQuery, request.RequestId, JsonConvert.SerializeObject(request));
        }
        // Retain this request (including RequestId) on timeout and retry it unchanged.
        public async Task<ClanResponse> ExecuteAsync(ClanCommandRequest request)
        {
            if (request.RequestId == Guid.Empty) request.RequestId = Guid.NewGuid();
            var result = await SendAsync(MessageTag.ClanCommand, request.RequestId, JsonConvert.SerializeObject(request));
            if (result.Success)
            {
                await OnMainThread(() =>
                {
                    if (result.LinhThach.HasValue) client.CharacterData.Stats.LinhThach = result.LinhThach.Value;
                });
                RequestRefresh();
            }
            return result;
        }
        public async Task<ClanResponse> RefreshAsync()
        {
            var result = await QueryAsync(new ClanQueryRequest { Query = ClanQueryType.Mine });
            if (result.Success)
                await OnMainThread(() => { Current = result.Detail; SnapshotUpdated?.Invoke(Current); });
            return result;
        }
        private async Task<ClanResponse> SendAsync(MessageTag tag, Guid requestId, string json)
        {
            await requests.WaitAsync();
            try
            {
                EnsureSession();
                var reader = await client.RequestWait(tag, json);
                var response = JsonConvert.DeserializeObject<ClanResponse>(reader.Read());
                EnsureSession();
                if (response == null || response.RequestId != requestId)
                    throw new InvalidOperationException("Clan response does not match request; retry with the same RequestId.");
                return response;
            }
            finally
            {
                // The transport correlates replies by tag; keep only one Clan request in flight.
                await Task.Delay(220);
                requests.Release();
            }
        }
        private void EnsureSession()
        {
            if (left || client.IsLogout || client.CharacterData == null || client.CharacterData.Id != charId)
                throw new InvalidOperationException("Character session changed.");
        }
        private Task OnMainThread(Action action)
        {
            var completion = new TaskCompletionSource<bool>();
            NetworkManager.Instance.Invoke(() =>
            {
                try { EnsureSession(); action(); completion.TrySetResult(true); }
                catch (Exception ex) { completion.TrySetException(ex); }
            });
            return completion.Task;
        }
        private void RequestRefresh()
        {
            if (left) return;
            Interlocked.Exchange(ref refreshWanted, 1);
            if (Interlocked.CompareExchange(ref refreshRunning, 1, 0) == 0) _ = RefreshLoop();
        }
        private async Task RefreshLoop()
        {
            try
            {
                while (!left && Interlocked.Exchange(ref refreshWanted, 0) == 1)
                {
                    await Task.Delay(250);
                    await RefreshAsync();
                }
            }
            catch (Exception ex)
            {
                NetworkManager.Instance.Invoke(() => Debug.LogWarning("Clan sync: " + ex.Message));
            }
            finally
            {
                Interlocked.Exchange(ref refreshRunning, 0);
                if (!left && Volatile.Read(ref refreshWanted) == 1) RequestRefresh();
            }
        }
    }
}
