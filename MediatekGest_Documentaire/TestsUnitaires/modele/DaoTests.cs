using Mediatek86.bdd;
using Mediatek86.metier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Mediatek86.modele.Tests
{
    [TestClass()]
    public class DaoTests
    {
        /// Connection Distante
        private static readonly string server = "localhost";
        private static readonly string userid = "root";
        private static readonly string password = "";
        private static readonly string database = "mediatek86";
        private static readonly string connectionString = "server=" + server + ";user id=" + userid + ";password=" + password + ";database=" + database + ";SslMode=none";


        //Recupération de l'instance de la BDD
        readonly BddMySql curs = BddMySql.GetInstance(connectionString);

        /// <summary>
        /// Methode qui envoie une requete de ne pas faire de modification dans la base
        /// Met en place le mode Transaction
        /// </summary>
        private void BeginTransaction()
        {
            curs.ReqUpdate("SET AUTOCOMMIT=0", null);
            curs.ReqUpdate("START TRANSACTION", null);
        }

        /// <summary>
        /// Methode qui envoie une requete pour revenir en arrière
        /// Stop le mode Transaction
        /// </summary>
        private void EndTransaction()
        {
            curs.ReqUpdate("ROLLBACK", null);
        }


        /// <summary>
        /// Méthode qui envoie une requete permettant d'annuler les vérifications de contraintes
        /// </summary>
        private void AnnuleContraintes()
        {
            curs.ReqUpdate("SET FOREIGN_KEY_CHECKS=0", null);
        }
        /// <summary>
        /// Méthode qui envoie une requete permettant d'activer les vérifications de contraintes
        /// </summary>
        private void ActiveContraintes()
        {
            curs.ReqUpdate("SET FOREIGN_KEY_CHECKS=1", null);
        }

        /// <summary>
        /// Methode de test de Dao.Authentification
        /// </summary>
        [TestMethod()]
        public void AuthentificationTest()
        {
            string login = "admf_log";
            string pwd = "admf_pwd";

            Utilisateur userTestSuccess = Dao.Authentification(login, pwd);
            Assert.IsNotNull(userTestSuccess);

            string loginFail = "__";
            string pwdFail = "__";

            Assert.AreEqual(null, Dao.Authentification(loginFail, pwd), "devrait échouer : login incorrect");
            Assert.AreEqual(null, Dao.Authentification(login, pwdFail), "devrait échouer : pwd incorrect");

        }

        /// <summary>
        /// Methode de test de Dao.GetEndingAbonnement
        /// </summary>
        [TestMethod()]
        public void GetEndingAbonnementTest()
        {
            string lesRevuesEnFinAbo = Dao.GetEndingAbonnement();

            Assert.IsNotNull(lesRevuesEnFinAbo);
            Assert.IsTrue(lesRevuesEnFinAbo.Length != 0);
        }

        /// <summary>
        /// Methode de test de Dao.GetEndingTitleDate
        /// </summary>
        [TestMethod()]
        public void GetEndingTitleDateTest()
        {
            string stridrevue = "10002";
            Dictionary<string, string> dictFinAbo = Dao.GetEndingTitleDate(stridrevue);

            Assert.IsNotNull(dictFinAbo);
            Assert.IsTrue(dictFinAbo.Count != 0);
        }


        /// <summary>
        /// Methode de test de Dao.GetAllCommandesDocument
        /// </summary>
        [TestMethod()]
        public void GetAllCommandesDocumentTest()
        {
            string idDoc = "00017";
            List<CommandeDocument> lstCmdDoc = Dao.GetAllCommandesDocument(idDoc);

            Assert.IsTrue(lstCmdDoc.Count != 0);
        }

        /// <summary>
        /// Methode de test de Dao.CreerCommande
        /// </summary>
        [TestMethod()]
        public void CreerCommandeTest()
        {
            BeginTransaction();
            AnnuleContraintes();
            string id = "aaaaa";
            DateTime dateCommande = DateTime.Now;
            double montant = 23.3;
            string idSuivi = "1";
            string label = "en cours";
            string idLivreDvd = "00017";
            int nbExemplaire = 1;


            string idDoc = "00017";
            List<CommandeDocument> lstCmdDoc = Dao.GetAllCommandesDocument(idDoc);
            int nbAvantInsert = lstCmdDoc.Count;

            CommandeDocument cmdDoc = new CommandeDocument(id, dateCommande, montant, idSuivi, label, idLivreDvd, nbExemplaire);
            Dao.CreerCommande(cmdDoc);

            List<CommandeDocument> lstCmdDocApres = Dao.GetAllCommandesDocument(idDoc);
            int nbAprestInsert = lstCmdDocApres.Count;

            CommandeDocument cmdDocAdd = lstCmdDocApres.Find(doc => doc.IdCommande.Equals(id)
                && doc.Montant.Equals(montant)
                && doc.IdSuivi.Equals(idSuivi)
                && doc.Label.Equals(label)
                && doc.IdLivreDvd.Equals(idLivreDvd)
                && doc.NbExemplaire.Equals(nbExemplaire)
                );

            Assert.IsNotNull(cmdDocAdd, "devrait réussir : une commande ajoutée");
            Assert.AreEqual(nbAvantInsert + 1, nbAprestInsert, "devrait réussir : un développeur en plus");
            ActiveContraintes();
            EndTransaction();

        }


        /// <summary>
        /// Methode de test de Dao.SupprimerCmdDocument
        /// </summary>
        [TestMethod()]
        public void SupprimerCmdDocumentTest()
        {
            BeginTransaction();
            AnnuleContraintes();
            string idDoc = "00017";
            List<CommandeDocument> lstCmdDoc = Dao.GetAllCommandesDocument(idDoc);
            int nbAvant = lstCmdDoc.Count;

            if (nbAvant != 0)
            {

                string idCommande = lstCmdDoc[0].IdCommande;
                Dao.SupprimerCmdDocument(idCommande);

                lstCmdDoc = Dao.GetAllCommandesDocument(idDoc);
                int nbApres = lstCmdDoc.Count;

                CommandeDocument cmdSupp = lstCmdDoc.Find(cmd => cmd.IdCommande.Equals(idCommande));
                Assert.IsNull(cmdSupp, "devrait réussir : une commande supprimée");

                Assert.AreEqual(nbAvant - 1, nbApres);
            }


            ActiveContraintes();
            EndTransaction();
        }

        /// <summary>
        /// Methode de test de Dao.UpdateCmdDocument
        /// </summary>
        [TestMethod()]
        public void UpdateCmdDocumentTest()
        {
            BeginTransaction();
            AnnuleContraintes();
            string idDoc = "00017";
            List<CommandeDocument> lstCmdDoc = Dao.GetAllCommandesDocument(idDoc);
            int nbAvant = lstCmdDoc.Count;


            if (nbAvant != 0)
            {

                Dao.UpdateCmdDocument(lstCmdDoc[0].IdCommande, "3");

                lstCmdDoc = Dao.GetAllCommandesDocument(idDoc);
                int nbApres = lstCmdDoc.Count;

                CommandeDocument upCmdDoc = lstCmdDoc.Find(doc => doc.IdCommande.Equals(lstCmdDoc[0].IdCommande));
                if (upCmdDoc != null)
                {
                    bool identique = upCmdDoc.IdCommande.Equals(lstCmdDoc[0].IdCommande)
                        && upCmdDoc.Montant.Equals(lstCmdDoc[0].Montant)
                        && upCmdDoc.IdSuivi.Equals(lstCmdDoc[0].IdSuivi)
                        && upCmdDoc.Label.Equals(lstCmdDoc[0].Label)
                        && upCmdDoc.IdLivreDvd.Equals(lstCmdDoc[0].IdLivreDvd)
                        && upCmdDoc.NbExemplaire.Equals(lstCmdDoc[0].NbExemplaire);

                    Assert.AreEqual(true, identique);
                }
                else
                {
                    Assert.Fail("Commande perdue suite aux modifications");
                }
                Assert.AreEqual(nbAvant, nbApres);
            }

            ActiveContraintes();
            EndTransaction();
        }


        /// <summary>
        /// Methode de test GetLastIdCommande
        /// </summary>
        [TestMethod()]
        public void GetLastIdCommandeTest()
        {
            int idtest = Dao.GetLastIdCommande();
            Assert.IsNotNull(idtest);

        }

        /// <summary>
        /// Methode de test de Dao.GetAllAbonnemmentRevue
        /// </summary>
        [TestMethod()]
        public void GetAllAbonnemmentRevueTest()
        {
            string idRevue = "10002";
            List<Abonnement> lstAbo = Dao.GetAllAbonnemmentRevue(idRevue);

            Assert.IsTrue(lstAbo.Count != 0);
        }


        /// <summary>
        /// Methode de test de Dao.CreerAbonnement
        /// </summary>
        [TestMethod()]
        public void CreerAbonnementTest()
        {

            BeginTransaction();
            AnnuleContraintes();

            string idCommande = "aaaa";
            DateTime dateCommande = DateTime.Now;
            double montant = 23.3;
            string idSuivi = "1";
            string label = "en cours";
            string idRevue = "10002";
            DateTime dateFinAbonnement = DateTime.Now;

            List<Abonnement> lstAbo = Dao.GetAllAbonnemmentRevue(idRevue);
            int nbAvant = lstAbo.Count;

            Abonnement abo = new Abonnement(idCommande, dateCommande, montant, idSuivi, label, idRevue, dateFinAbonnement);
            Dao.CreerAbonnement(abo);

            lstAbo = Dao.GetAllAbonnemmentRevue(idRevue);
            int nbApres = lstAbo.Count;

            Abonnement aboAdd = lstAbo.Find(doc => doc.IdCommande.Equals(idCommande)
             && doc.Montant.Equals(montant)
             && doc.IdSuivi.Equals(idSuivi)
             && doc.Label.Equals(label));

            Assert.IsNotNull(aboAdd, "devrait réussir : une commande ajoutée");
            Assert.AreEqual(nbAvant + 1, nbApres, "devrait réussir : un abo en plus");

            ActiveContraintes();
            EndTransaction();
        }

        /// <summary>
        /// Methode de test de Dao.SupprimerAboRevue
        /// </summary>
        [TestMethod()]
        public void SupprimerAboRevueTest()
        {
            BeginTransaction();
            AnnuleContraintes();
            string idRevue = "10002";

            List<Abonnement> lstAbo = Dao.GetAllAbonnemmentRevue(idRevue);
            int nbAvant = lstAbo.Count;
            Assert.IsNotNull(lstAbo);

            if (nbAvant != 0)
            {
                string idCmdrevue = lstAbo[0].IdCommande;
                Dao.SupprimerAboRevue(idCmdrevue);

                lstAbo = Dao.GetAllAbonnemmentRevue(idRevue);
                int nbApres = lstAbo.Count;

                Abonnement abodel = lstAbo.Find(obj => obj.IdCommande.Equals(idCmdrevue));

                Assert.IsNull(abodel, "devrait réussir : un abo supprimé");
                Assert.AreEqual(nbAvant - 1, nbApres, "devrait réussir : une abo en moins");
            }

            ActiveContraintes();
            EndTransaction();
        }


        /// <summary>
        /// Methode de test de Dao.GetDateParution
        /// </summary>
        [TestMethod()]
        public void GetDateParutionTest()
        {
            string idRevue = "10002";
            DateTime date = Dao.GetDateParution(idRevue);
            Assert.IsTrue(date != DateTime.MinValue);
        }

        /// <summary>
        /// Methode de test de dao.GetAllSuivis
        /// </summary>
        [TestMethod()]
        public void GetAllSuivisTest()
        {
            List<Categorie> lstSuivi = Dao.GetAllSuivis();
            Assert.IsTrue(lstSuivi.Count != 0);
        }
        /// <summary>
        /// Methode de test de dao.GetAllGenres
        /// </summary>
        [TestMethod()]
        public void GetAllGenresTest()
        {
            List<Categorie> lstGenre = Dao.GetAllGenres();
            Assert.IsTrue(lstGenre.Count != 0);
        }

        /// <summary>
        /// Methode de test de dao.GetAllRayons
        /// </summary>
        [TestMethod()]
        public void GetAllRayonsTest()
        {
            List<Categorie> lstRayon = Dao.GetAllRayons();
            Assert.IsTrue(lstRayon.Count != 0);
        }


        /// <summary>
        /// Methode de test de dao.GetAllPublics
        /// </summary>
        [TestMethod()]
        public void GetAllPublicsTest()
        {
            List<Categorie> lstPublic = Dao.GetAllPublics();
            Assert.IsTrue(lstPublic.Count != 0);
        }

        /// <summary>
        /// Methode de test de dao.GetAllLivres
        /// </summary>
        [TestMethod()]
        public void GetAllLivresTest()
        {
            List<Livre> lstLivre = Dao.GetAllLivres();
            Assert.IsTrue(lstLivre.Count != 0);
        }


        /// <summary>
        /// Methode de test de dao.GetAllDvd
        /// </summary>
        [TestMethod()]
        public void GetAllDvdTest()
        {
            List<Dvd> lstDvd = Dao.GetAllDvd();
            Assert.IsTrue(lstDvd.Count != 0);
        }

        /// <summary>
        /// Methode de test de dao.GetAllRevues
        /// </summary>
        [TestMethod()]
        public void GetAllRevuesTest()
        {
            List<Revue> Revues = Dao.GetAllRevues();
            Assert.IsTrue(Revues.Count != 0);
        }

        /// <summary>
        /// Methode de test de Dao.GetExemplairesRevue
        /// </summary>
        [TestMethod()]
        public void GetExemplairesRevueTest()
        {
            string idRevue = "10002";
            List<Exemplaire> lstExpl = Dao.GetExemplairesRevue(idRevue);
            Assert.IsTrue(lstExpl.Count != 0);

        }

        /// <summary>
        /// Methode de test de Dao.CreerExemplaire
        /// </summary>
        [TestMethod()]
        public void CreerExemplaireTest()
        {
            BeginTransaction();
            AnnuleContraintes();

            int numero = 4444;
            DateTime dateAchat = DateTime.Now;
            string photo = "1";
            string idEtat = "1";
            string idRevue = "10002";

            List<Exemplaire> lstExpl = Dao.GetExemplairesRevue(idRevue);
            int nbAvant = lstExpl.Count;

            Exemplaire newExmpl = new Exemplaire(numero, dateAchat, photo, idEtat, idRevue);
            Dao.CreerExemplaire(newExmpl);

            lstExpl = Dao.GetExemplairesRevue(idRevue);
            int nbApres = lstExpl.Count;

            Exemplaire exmpl = lstExpl.Find(doc => doc.Numero.Equals(numero)
            && doc.Photo.Equals(photo)
            && doc.IdEtat.Equals(idEtat)
            && doc.IdDocument.Equals(idRevue));

            Assert.IsNotNull(exmpl, "devrait réussir : une commande ajoutée");
            Assert.AreEqual(nbAvant + 1, nbApres, "devrait réussir : un exemplaire en plus");

            ActiveContraintes();
            EndTransaction();
        }

        /// <summary>
        /// Methode de test de Dao.CreerExemplaire
        /// Vérifie si la date d'achat de l'exemplaire est comprise entre la date de la commande et la date de fin d'abonnement
        /// Vrai si dateParution est entre les 2 autres dates
        /// </summary>
        [TestMethod()]
        public void ParutionDansAbonnementTest()
        {
            // Varibles test ParutionDansAbonnement
            DateTime dateMin = DateTime.Parse("01/04/2022"); // data achat Exemplaire
            DateTime date = DateTime.Parse("03/04/2022"); // date Fin abo
            DateTime dateMax = DateTime.Parse("13/04/2022"); // date Parution

            //  ((dateCommande < dateParution && dateParution < dateFin) )

            Assert.AreEqual(true, Dao.ParutionDansAbonnement(dateMin, dateMax, date));
            Assert.AreEqual(false, Dao.ParutionDansAbonnement(dateMax, date, dateMin));
            Assert.AreEqual(false, Dao.ParutionDansAbonnement(dateMax, dateMin, date));
        }
    }
}