using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mediatek86.vue.Tests
{
    [TestClass()]
    public class FrmMediatekTests
    {

        // Varibles test ParutionDansAbonnement
        private readonly DateTime dateParu = DateTime.Parse("11/04/2022");
        private readonly DateTime dateCommande = DateTime.Parse("01/04/2022");
        private readonly DateTime dateFin = DateTime.Parse("03/04/2022");


        /// <summary>
        /// Methode qui vérifie si la date d'achat de l'exemplaire est comprise entre la date de la commande et la date de fin d'abonnement
        /// </summary>
        /// <param name="dateCommande">Date de la commande</param>
        /// <param name="dateFin">Date de fin d'abonnement</param>
        /// <param name="dateParution">Date de parution d'un nuemro de revue</param>
        /// <returns>Vrai si dateParution est entre les 2 autres dates</returns>
#pragma warning disable S1172 // Unused method parameters should be removed
        private bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFin, DateTime dateParution)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            if ((dateCommande < dateParution && dateParution < dateFin) || dateParution == DateTime.MinValue)
            {
                return false;
            }
            return true;
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
        public void UpdateCmdDvd(string idCommande, string idSuivi)
        {


        }

        [TestMethod()]
        public void UpdateCmdDVDTest()
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