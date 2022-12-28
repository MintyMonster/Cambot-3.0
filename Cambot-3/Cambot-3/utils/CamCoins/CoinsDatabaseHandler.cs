using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.utils.CamCoins
{
    public class CoinsDatabaseHandler
    {
        private static readonly CamCoinsEntities _db = new CamCoinsEntities();
        private static Dictionary<ulong, CoinsPlayerObject> players = new Dictionary<ulong, CoinsPlayerObject>();

        // Instancing
        private static CoinsDatabaseHandler _instance;
        private CoinsDatabaseHandler() { }
        public static CoinsDatabaseHandler Instance { get { return _instance ?? new CoinsDatabaseHandler(); } }

        // Private functions
        private bool PlayerExists(ulong id) => players.ContainsKey(id);
        private void CreatePlayer(IInteractionContext context) =>
            players.Add(context.User.Id, new CoinsPlayerObject()
            {
                UserId = context.User.Id,
                Username = context.User.Username,
                Money = 1500,
                CamCoins = 0,
                Rigs = 0,
                GPUs = 0,
                NetWorth = 1500,
                Health = 100,
                Food = 100,
                Water = 100,
                Happiness = 100
            });

        private async void PushToDatabase(CoinsPlayerObject player) =>
            await _db.CamCoins.AddAsync(new CamCoinsDatabaseModel()
            {
                UserId = player.UserId,
                Username = player.Username,
                Money = player.Money,
                CamCoins = player.CamCoins,
                Rigs = player.Rigs,
                GPUs = player.GPUs,
                NetWorth = player.NetWorth,
                Health = player.Health,
                Food = player.Food,
                Water = player.Water,
                Happiness = player.Happiness,
            });

        // Adding functions
        private void AddMoney(ulong id, double amount) => players[id].Money += amount;
        private void AddCamCoins(ulong id, double amount) => players[id].CamCoins += amount;
        private void AddRig(ulong id, int amount) => players[id].Rigs += amount;
        private void AddGpu(ulong id, int amount) => players[id].GPUs += amount;
        private void AddNetWorth(ulong id, double amount) => players[id].NetWorth += amount;
        private void AddHealth(ulong id, int amount) => players[id].Health += amount;
        private void AddFood(ulong id, int amount) => players[id].Food += amount;
        private void AddWater(ulong id, int amount) => players[id].Water += amount;
        private void AddHappiness(ulong id, int amount) => players[id].Happiness += amount;

        // Setting functions
        private void SetMoney(ulong id, double amount) => players[id].Money = amount;
        private void SetCamCoins(ulong id, double amount) => players[id].CamCoins = amount;
        private void SetRigs(ulong id, int amount) => players[id].Rigs = amount;
        private void SetGpus(ulong id, int amount) => players[id].GPUs = amount;
        private void SetNetWorth(ulong id, double amount) => players[id].NetWorth = amount;
        private void SetHealth(ulong id, int amount) => players[id].Health = amount;
        private void SetFood(ulong id, int amount) => players[id].Food = amount;
        private void SetWater(ulong id, int amount) => players[id].Water = amount;
        private void SetHappiness(ulong id, int amount) => players[id].Happiness = amount;

        // Removing functions
        private void RemoveMoney(ulong id, double amount) => players[id].Money -= amount;
        private void RemoveCamCoins(ulong id, double amount) => players[id].CamCoins -= amount;
        private void RemoveRigs(ulong id, int amount) => players[id].Rigs -= amount;
        private void RemoveGpus(ulong id, int amount) => players[id].GPUs -= amount;
        private void RemoveNetWorth(ulong id, double amount) => players[id].NetWorth -= amount;
        private void RemoveHealth(ulong id, int amount) => players[id].NetWorth -= amount;
        private void RemoveFood(ulong id, int amount) => players[id].Food -= amount;
        private void RemoveWater(ulong id, int amount) => players[id].Water -= amount;
        private void RemoveHappiness(ulong id, int amount) => players[id].Happiness -= amount;

        // Getters
        private double GetPlayerMoney(ulong id) => PlayerExists(id) ? players[id].Money : 1500;
        private double GetPlayerCamCoins(ulong id) => PlayerExists(id) ? players[id].CamCoins : 0;
        private int GetPlayerRigs(ulong id) => PlayerExists(id) ? players[id].Rigs : 0;
        private int GetPlayerGpus(ulong id) => PlayerExists(id) ? players[id].GPUs : 0;
        private double GetPlayerNetWorth(ulong id) => PlayerExists(id) ? players[id].NetWorth : 0;
        private int GetPlayerHealth(ulong id) => PlayerExists(id) ? players[id].Health : 0;
        private int GetPlayerFood(ulong id) => PlayerExists(id) ? players[id].Food : 0;
        private int GetPlayerWater(ulong id) => PlayerExists(id) ? players[id].Water : 0;
        private int GetPlayerHappiness(ulong id) => PlayerExists(id) ? players[id].Happiness : 0;
        private int GetPlayerCount() => players.Count;

        


    }
}
