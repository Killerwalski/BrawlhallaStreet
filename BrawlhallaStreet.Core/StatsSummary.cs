using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core
{
    public class StatsSummary
    {
        public string PlayerName { get; set; }
        public int WeaponOneDamage { get; set; }
        public int WeaponTwoDamage { get; set; }
        public int UnarmedDamage { get; set; }
        public int GadgetDamage { get; set; }
        public int ThrownItemDamage { get; set; }
        public int TotalDamage { get; set; }
        public int DamageTaken { get; set; }
        public int WeaponOneKo { get; set; }
        public int WeaponTwoKo { get; set; }
        public int UnarmedKo { get; set; }
        public int GadgetKo { get; set; }
        public int ThrownItemKo { get; set; }
        public int TotalKo { get; set; }
        public int Falls { get; set; }
        public int Suicides { get; set; }
        public int TeamKills { get; set; }
        public int GamesPlayed { get; set; }

        public override string ToString()
        {
            return $"Player *** {PlayerName} *** Damage Summary for {GamesPlayed} game{ (GamesPlayed == 1 ? "" : "s") } \n WeaponOne: {WeaponOneDamage}  \t WeaponTwo: {WeaponTwoDamage} \t Unarmed: {UnarmedDamage} \n" +
                $"Gadgets: {GadgetDamage} \t Thrown Item: {ThrownItemDamage} \t Total Damage: {TotalDamage} \t Damage Taken: {DamageTaken}\n" + 
                $"KO Summary\n WeaponOne: {WeaponOneKo} \t WeaponTwo: {WeaponTwoKo} \t Unarmed: {UnarmedKo} \n" + 
                $"Gadgets: {GadgetKo} \t Thrown Item: {ThrownItemKo} \t Total KO: {TotalKo} \t Falls: {Falls} \n" + 
                $"LULS\n Suicides: {Suicides} \t Team KO: {TeamKills} ";
        }
    }
}
