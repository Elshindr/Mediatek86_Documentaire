using Mediatek86.metier;
using Mediatek86.modele;
using Mediatek86.vue;
using System.Collections.Generic;


namespace Mediatek86.controleur
{
    internal class Controle
    {
        private readonly List<Livre> lesLivres;
        private readonly List<Dvd> lesDvd;
        private readonly List<Revue> lesRevues;
        private readonly List<Categorie> lesRayons;
        private readonly List<Categorie> lesPublics;
        private readonly List<Categorie> lesGenres;
        private readonly List<Categorie> lesSuivis;

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

            FrmMediatek frmMediatek = new FrmMediatek(this);
            frmMediatek.ShowDialog();
        }

        public bool SupprimerCmdLivres(string idCommande)
        {
            return Dao.SupprimerCmdLivres(idCommande);
        }

        /// <summary>
        /// Methode du controleur accedant à la méthode UpdateCmdLivre
        /// Permet la mise à jours du suivi de commande de livre
        /// </summary>
        /// <param name="idCommande"></param>
        /// <param name="idSuivi"></param>
        /// <returns></returns>
        public bool UpdateCmdLivres(string idCommande, string idSuivi)
        {
            return Dao.UpdateCmdLivres(idCommande, idSuivi);
        }

        /// <summary>
        /// Methode de controle de création d'une commande
        /// </summary>
        /// <param name="commande"></param>
        /// <returns>True si la commande est executée</returns>
        public bool CreerCommande(Commande commande)
        {
            return Dao.CreerCommande(commande);
        }

        /// <summary>
        /// Methode de controle de récupération de id Max de la table commande
        /// </summary>
        /// <returns></returns>
        public int GetLastIdCommande()
        {

            return Dao.GetLastIdCommande();
        }
        /// <summary>
        /// getter sur la liste commande d'un livre
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public List<Commande> GetAllCommandesLivre(string idDocumentlivreDvd)
        {
            return Dao.GetAllCommandesLivre(idDocumentlivreDvd);
        }


        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Collection d'objets Genre</returns>
        public List<Categorie> GetAllSuivis()
        {
            return lesSuivis;
        }


        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Collection d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return lesGenres;
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return lesLivres;
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Collection d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return lesDvd;
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Collection d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return lesRevues;
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return lesRayons;
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return lesPublics;
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return Dao.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return Dao.CreerExemplaire(exemplaire);
        }

    }

}

