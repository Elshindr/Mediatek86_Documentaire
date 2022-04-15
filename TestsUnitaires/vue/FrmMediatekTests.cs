using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mediatek86.vue.Tests
{
    [TestClass()]
    public class FrmMediatekTests
    {

        // Varibles test ParutionDansAbonnement
        private DateTime dateFin = DateTime.Parse("14/03/2008");
        private DateTime dateParu = DateTime.Parse("12/03/2008");
        private DateTime dateCommande = DateTime.Parse("10/03/2008");

        /// <summary>
        /// Methode qui vérifie si la date d'achat de l'exemplaire est comprise entre la date de la commande et la date de fin d'abonnement
        /// </summary>
        /// <param name="dateCommande">Date de la commande</param>
        /// <param name="dateFin">Date de fin d'abonnement</param>
        /// <param name="dateParution">Date de parution d'un nuemro de revue</param>
        /// <returns>Vrai si dateParution est entre les 2 autres dates</returns>
        private bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFin, DateTime dateParution)
        {
            if ((dateCommande < dateParution && dateParution < dateFin) || dateParution == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// TestMethode pour ParutionDansAbonnementTest
        /// </summary>
        [TestMethod()]
        public void ParutionDansAbonnementTest()
        {

            ParutionDansAbonnement(dateCommande, dateFin, dateParu);
            Assert.AreEqual(true, ParutionDansAbonnement(dateCommande, dateFin, dateParu));
            Assert.AreEqual(false, ParutionDansAbonnement(dateParu, dateFin, dateCommande));
            Assert.AreEqual(false, ParutionDansAbonnement(dateCommande, dateParu, dateFin));
        }

        [TestMethod()]
        public void UpdateCmdDVDTest()
        {

        }

        [TestMethod()]
        public void UpdateCmdDvdTest()
        {

        }

        [TestMethod()]
        public void UpdateCmdLivresTest()
        {

        }

        [TestMethod()]
        public void RemplirComboCategorieTest()
        {

        }


    }
}