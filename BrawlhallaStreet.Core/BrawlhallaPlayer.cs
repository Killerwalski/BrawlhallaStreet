using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BrawlhallaStreet.Core
{
    [BsonIgnoreExtraElements]
    public class BrawlhallaPlayer
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("brawlhalla_id")]
        public int BrawlhallaId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("xp")]
        public int Xp { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("xp_percentage")]
        public double XpPercentage { get; set; }

        [JsonProperty("games")]
        public int Games { get; set; }

        [JsonProperty("wins")]
        public int Wins { get; set; }

        [JsonProperty("damagebomb")]
        public string Damagebomb { get; set; }

        [JsonProperty("damagemine")]
        public string Damagemine { get; set; }

        [JsonProperty("damagespikeball")]
        public string Damagespikeball { get; set; }

        [JsonProperty("damagesidekick")]
        public string Damagesidekick { get; set; }

        [JsonProperty("hitsnowball")]
        public int Hitsnowball { get; set; }

        [JsonProperty("kobomb")]
        public int Kobomb { get; set; }

        [JsonProperty("komine")]
        public int Komine { get; set; }

        [JsonProperty("kospikeball")]
        public int Kospikeball { get; set; }

        [JsonProperty("kosidekick")]
        public int Kosidekick { get; set; }

        [JsonProperty("kosnowball")]
        public int Kosnowball { get; set; }

        [JsonProperty("legends")]
        public IList<Legend> Legends { get; set; }

        [JsonProperty("clan")]
        public Clan Clan { get; set; }
    }
    public class Legend
    {
        [JsonProperty("legend_id")]
        public int LegendId { get; set; }

        [JsonProperty("legend_name_key")]
        public string LegendNameKey { get; set; }

        [JsonProperty("damagedealt")]
        public string Damagedealt { get; set; }

        [JsonProperty("damagetaken")]
        public string Damagetaken { get; set; }

        [JsonProperty("kos")]
        public int Kos { get; set; }

        [JsonProperty("falls")]
        public int Falls { get; set; }

        [JsonProperty("suicides")]
        public int Suicides { get; set; }

        [JsonProperty("teamkos")]
        public int Teamkos { get; set; }

        [JsonProperty("matchtime")]
        public int Matchtime { get; set; }

        [JsonProperty("games")]
        public int Games { get; set; }

        [JsonProperty("wins")]
        public int Wins { get; set; }

        [JsonProperty("damageunarmed")]
        public string Damageunarmed { get; set; }

        [JsonProperty("damagethrownitem")]
        public string Damagethrownitem { get; set; }

        [JsonProperty("damageweaponone")]
        public string Damageweaponone { get; set; }

        [JsonProperty("damageweapontwo")]
        public string Damageweapontwo { get; set; }

        [JsonProperty("damagegadgets")]
        public string Damagegadgets { get; set; }

        [JsonProperty("kounarmed")]
        public int Kounarmed { get; set; }

        [JsonProperty("kothrownitem")]
        public int Kothrownitem { get; set; }

        [JsonProperty("koweaponone")]
        public int Koweaponone { get; set; }

        [JsonProperty("koweapontwo")]
        public int Koweapontwo { get; set; }

        [JsonProperty("kogadgets")]
        public int Kogadgets { get; set; }

        [JsonProperty("timeheldweaponone")]
        public int Timeheldweaponone { get; set; }

        [JsonProperty("timeheldweapontwo")]
        public int Timeheldweapontwo { get; set; }

        [JsonProperty("xp")]
        public int Xp { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("xp_percentage")]
        public double XpPercentage { get; set; }
    }

    public class Clan
    {
        [JsonProperty("clan_name")]
        public string ClanName { get; set; }

        [JsonProperty("clan_id")]
        public int ClanId { get; set; }

        [JsonProperty("clan_xp")]
        public string ClanXp { get; set; }

        [JsonProperty("personal_xp")]
        public int PersonalXp { get; set; }
    }

    
}