using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mediatek86.metier.Tests
{
    [TestClass()]
    public class AbonnementTests
    {
        readonly string idCommande = "00002";
        readonly DateTime dateCommande = DateTime.Now;
        readonly double montant = 23;
        readonly string idSuivi = "1";
        readonly string label = "en cours";
        readonly string idRevue = "00002";
        readonly DateTime dateFinAbonnement = DateTime.Now;

        [TestMethod()]
        public void AbonnementTest()
        {
            Abonnement abo = new Abonnement(idCommande, dateCommande, montant, idSuivi, label, idRevue, dateFinAbonnement);
            Abonnement expabo = new Abonnement("00002", DateTime.Now, 23, "1", "en cours", "00002", DateTime.Now);

            Assert.AreEqual(expabo, abo);
            Assert.AreSame(expabo, abo);
        }
    }
}