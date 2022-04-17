using Mediatek86.metier;
using Mediatek86.modele;
using Mediatek86.vue;
using System;
using System.Collections.Generic;


namespace Mediatek86.controleur
{
    /// <summary>
    /// Classe d'instance du Controleur
    /// </summary>
    internal class Controle
    {
        private readonly List<Livre> lesLivres;
        private readonly List<Dvd> lesDvd;
        private readonly List<Revue> lesRevues;
        private readonly List<Categorie> lesRayons;
        private readonly List<Categorie> lesPublics;
        private readonly List<Categorie> lesGenres;
        private readonly List<Categorie> lesSuivis;

        private readonly String lesFinAbo;
        private Utilisateur user;

        /// <summary>
        /// fenetre de connexion
        /// </summary>
        private readonly FrmAuthentification frmAuth;
        private FrmMediatek frmMediatek;

        /// <summary>
        /// Constructeur de la classe Controle
        /// Ouverture de la fenêtre 
        /// Recupération des différents libelles
        /// </summary>
        public Controle()
        {

            lesLivres = Dao.GetAllLivres();
            lesDvd = Dao.GetAllDvd();
            lesRevues = Dao.GetAllRevues();
            lesGenres = Dao.GetAllGenres();
            lesRayons = Dao.GetAllRayons();
            lesPublics = Dao.GetAllPublics();
            lesSuivis = Dao.GetAllSuivis();
            lesFinAbo = Dao.GetEndingAbonnement();

            frmAuth = new FrmAuthentification(this);
            frmAuth.ShowDialog();

        }


        #region Connexion
        /// <summary>
        /// Methode de vérification de connexion, si vrai,
        /// ouverture de la fenetre FrmGestion
        /// </summary>
        /// <param name="log"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public Boolean Authentification(string login, string pwd)
        {
            user = Dao.Authentification(login, pwd);

            if (user != null)
            {
                frmAuth.Hide();

                frmMediatek = new FrmMediatek(this);
                AccesServices();

                return true;
            }

            return false;

        }

        /// <summary>
        /// Methode de gestion des acces aux onglets de l'application selon le service de l'utilisateur 
        /// </summary>
        private void AccesServices()
        {
            frmMediatek.Text = "Gestion Médiathèque: " + user.Label;
            switch (user.IdService)
            {
                case 4: //Admin            
                case 1: //Administratif  
                    frmMediatek.AfficheFinAbo();
                    frmMediatek.ShowDialog();

                    break;

                case 2: //Prêts
                    frmMediatek.AccesServicePrets();
                    frmMediatek.ShowDialog();
                    break;

                case 3: //Culture
                    frmMediatek.AfficheServiceCulture();
                    frmMediatek.Close();
                    break;

                default: //None
                    frmMediatek.AfficheServiceNone();
                    frmMediatek.Close();
                    break;
            }

        }
        #endregion 


        #region Abonnement

        /// <summary>
        /// Methode du controleur d'appel de la methode GetAllRevues
        /// Getter sur la chaine lesFinAbo
        /// </summary>
        /// <returns>Chaine de liste d'idRevue</returns>
        public String GetEndingAbonnement()
        {
            return lesFinAbo;
        }

        /// <summary>
        /// Methode du controleur accédant à la méthode GetEndingTitleDate
        /// Permet la récupération de la liste des revues en fin d'abonnements
        /// </summary>
        /// <returns>La chaine d'alerte à afficher</returns>
        public String GetEndingTitleDate()
        {
            Dictionary<String, String> dictFinAbo = Dao.GetEndingTitleDate(lesFinAbo);
            string strList = "";

            foreach (var item in dictFinAbo)
            {
                strList += item.Key + " termine le : " + item.Value + ".\n";
            }
            return strList;
        }


        /// <summary>
        /// Methode du controleur accédant à la méthode GetAllCommandesRevue
        /// Permet la récupération de la liste des abonnements d'une revue
        /// </summary>
        /// <param name="idRevue">Identifiant d'une revue</param>
        /// <returns>La liste des abonnements d'une revue</returns>
        public List<Abonnement> GetAllAbonnemmentRevue(string idRevue)
        {
            return Dao.GetAllAbonnemmentRevue(idRevue);
        }


        /// <summary>
        /// Methode de controle de création d'une commande d'abonnement
        /// Permet la creation d'une commande de revue dans la DB
        /// </summary>
        /// <param name="abo">Un identifiant d'abonnement d'une revue</param>
        /// <returns>Vrai si CreerAbonnement retourne Vrai</returns>
        public bool CreerAbonnement(Abonnement abo)
        {
            return Dao.CreerAbonnement(abo);
        }

        /// <summary>
        /// Methode du controleur accedant à la methode SupprimerAboRevue
        /// Permet la suppression d'une commande d'une revue
        /// </summary>
        /// <param name="idCommande">Identifiant d'une commande</param>
        /// <returns>Vrai si SupprimerAboRevue retourne Vrai</returns>
        public bool SupprimerAboRevue(string idCommande)
        {
            return Dao.SupprimerAboRevue(idCommande);
        }

        /// <summary>
        /// Methode du controleur accedant à la methode GetDateParution
        /// Permet de récupérer la date de parution de la table exemplaire
        /// </summary>
        /// <param name="idRevue">Identifiant d'une revue</param>
        /// <returns>Date de Parution d'une revue</returns>
        public DateTime GetDateParution(string idRevue)
        {
            return Dao.GetDateParution(idRevue);
        }

        #endregion

        #region Commande de document Livre DVD
        /// <summary>
        /// Methode de controle de création d'une commande
        /// Permet la creation d'une commande d'un Livre ou Dvd
        /// </summary>
        /// <param name="commande">Une commande d'un livre ou Dvd</param>
        /// <returns>True si la commande est executée</returns>
        public bool CreerCommande(CommandeDocument commande)
        {
            return Dao.CreerCommande(commande);
        }


        /// <summary>
        /// Methode du controleur accedant à la methode SupprimerCmdDocument
        /// Permet la suppression d'une commande d'un Livre ou Dvd
        /// </summary>
        /// <param name="idCommande">Un identifiant de commande</param>
        /// <returns>Vrai si SupprimerCmdDocument retourne vrai</returns>
        public bool SupprimerCmdDocument(string idCommande)
        {
            return Dao.SupprimerCmdDocument(idCommande);
        }


        /// <summary>
        /// Methode du controleur accedant à la méthode UpdateCmdDocument
        /// Permet la mise à jours du suivi de commande d'un Livre ou Dvd
        /// </summary>
        /// <param name="idCommande">Un identifiant de commande</param>
        /// <param name="idSuivi">Un identifiant de suivi</param>
        /// <returns>Vrai si UpdateCmdDocument retourne vrai</returns>
        public bool UpdateCmdDocument(string idCommande, string idSuivi)
        {
            return Dao.UpdateCmdDocument(idCommande, idSuivi);
        }


        /// <summary>
        /// Methode du controleur accedant à la méthode GetLastIdCommande
        /// Permet la récupération de id Max de la table commande
        /// </summary>
        /// <returns>L'identifiant de commande de la commande en cours</returns>
        public int GetLastIdCommande()
        {
            return Dao.GetLastIdCommande();
        }


        /// <summary>
        /// Methode du controleur accedant à la méthode GetAllCommandesDocument
        /// Permet la récupération de la liste de commande d'un livre ou Dvd
        /// </summary>
        /// <returns>Collection d'objets de Livre ou de Dvd</returns>
        public List<CommandeDocument> GetAllCommandesDocument(string idDocument)
        {
            return Dao.GetAllCommandesDocument(idDocument);
        }

        #endregion

        #region Getter Categorie
        /// <summary>
        /// Methode du controleur d'appel de la methode GetAllSuivis
        /// Getter sur la liste des Suivis
        /// </summary>
        /// <returns>Collection d'objets Suivi</returns>
        public List<Categorie> GetAllSuivis()
        {
            return lesSuivis;
        }


        /// <summary>
        /// Methode du controleur d'appel de la methode GetAllGenres
        /// Getter sur la liste des genres
        /// </summary>
        /// <returns>Collection d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return lesGenres;
        }


        /// <summary>
        /// Methode du controleur d'appel de la methode GetAllPublics
        /// Getter ur la liste des rayons
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return lesRayons;
        }


        /// <summary>
        /// Methode du controleur d'appel de la methode GetAllPublics
        /// Getter sur la liste des publics
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return lesPublics;
        }

        #endregion

        #region Getter Documents
        /// <summary>
        /// Methode du controleur d'appel de la methode GetAllLivres
        /// Getter sur la liste des livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return lesLivres;
        }


        /// <summary>
        /// Methode du controleur d'appel de la methode GetAllDvd
        /// Getter sur la liste des Dvd
        /// </summary>
        /// <returns>Collection d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return lesDvd;
        }


        /// <summary>
        /// Methode du controleur d'appel de la methode GetAllRevues
        /// Getter sur la liste des revues
        /// </summary>
        /// <returns>Collection d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return lesRevues;
        }

        #endregion

        #region Exemplaire

        /// <summary>
        /// Methode du controleur d'appel de la methode GetExemplairesRevue
        /// Permet la récupèration des exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return Dao.GetExemplairesRevue(idDocuement);
        }


        /// <summary>
        /// Methode du controleur d'appel de la methode CreerExemplaire
        /// Permet la création d'un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return Dao.CreerExemplaire(exemplaire);
        }
        #endregion
    }

}

