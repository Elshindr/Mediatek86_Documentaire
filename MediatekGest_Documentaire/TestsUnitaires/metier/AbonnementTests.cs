using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mediatek86.metier.Tests
{
    [TestClass()]
    public class AbonnementTests
    {
        private const string idCommande = "00002";
        private static DateTime dateCommande = DateTime.Now;
        private const double montant = 23;
        private const string idSuivi = "1";
        private const string label = "en cours";
        private const string idRevue = "00002";
        private static DateTime dateFinAbonnement = DateTime.Now;

        private readonly Abonnement abo = new Abonnement(idCommande, dateCommande, montant, idSuivi, label, idRevue, dateFinAbonnement);

        [TestMethod()]
        public void AbonnementTest()
        {

            Assert.AreEqual(idCommande, abo.IdCommande, "devrait réussir : idCommande valorisé");
            Assert.AreEqual(idRevue, abo.IdRevue, "devrait réussir : idRevue valorisé");
            Assert.AreEqual(idSuivi, abo.IdSuivi, "devrait réussir : idSuivi valorisé");
            Assert.AreEqual(label, abo.Label, "devrait réussir : label valorisé");
            Assert.AreEqual(montant, abo.Montant, "devrait réussir : montant valorisé");
            Assert.AreEqual(dateCommande, abo.DateCommande, "devrait réussir : dateCommande valorisé");
            Assert.AreEqual(dateFinAbonnement, abo.DateFinAbonnement, "devrait réussir : dateFinAbonnement valorisé");

        }
    }
}