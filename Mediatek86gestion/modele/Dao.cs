using Mediatek86.bdd;
using Mediatek86.metier;
using System;
using System.Collections.Generic;

namespace Mediatek86.modele
{
    /// <summary>
    /// Classe public executant les opérations SQL sur la base de données mediatek86
    /// </summary>
    public static class Dao
    {

        private static readonly string server = "localhost";
        private static readonly string userid = "root";
        private static readonly string password = "";
        private static readonly string database = "mediatek86";
        private static readonly string connectionString = "server=" + server + ";user id=" + userid + ";password=" + password + ";database=" + database + ";SslMode=none";


        #region connexion

        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne les informations d'utilisateur selon les informations de connexion fournies (login et pwd)
        /// </summary>
        /// <param name="login">un login</param>
        /// <param name="pwd"> un password</param>
        /// <returns>Objet type Utilisateur ou null</returns>
        public static Utilisateur Authentification(string login, string pwd)
        {
            try
            {
                string label = "";
                int idUser = 0;
                int idSuivi = 0;


                string req = "Select u.idUser, u.idService, s.label";
                req += " from utilisateur u join service s on u.idService = s.idService";
                req += " where u.pwd = @pwd AND u.login = @login;";

                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@pwd", pwd},
                    { "@login", login}
              };



                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqSelect(req, parameters);

                while (curs.Read())
                {
                    idUser = curs.Field("idUser") is DBNull ? 0 : (int)curs.Field("idUser");

                    idSuivi = curs.Field("idService") is DBNull ? 0 : (int)curs.Field("idService");

                    label = curs.Field("label") is DBNull ? "" : (string)curs.Field("label");

                }
                curs.Close();

                if (idUser != 0 && idSuivi != 0 && label != "")
                {
                    Utilisateur user = new Utilisateur(idUser, idSuivi, label);
                    return user;

                }
                return null;

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message, "Echec connexion");
                return null;
            }

        }

        #endregion


        #region alerte abo
        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT d'appel de fonction stockée
        /// </summary>
        /// <returns>Chaine d'idRevues des revues en fin d'abonnement </returns>
        public static String GetEndingAbonnement()
        {
            string strFinAbo = "";

            string req = "SELECT fctEndingAbo() as id;";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);


            while (curs.Read())
            {

                strFinAbo += (string)curs.Field("id");
            }
            curs.Close();

            return strFinAbo;
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne toutes les commandes livres ou de DVD de la BDD 
        /// </summary>
        /// <param name="strIdRevues">Chaine d'identifiant de revue </param>
        /// <returns>Dictonnaire de chaines </returns>
        public static Dictionary<String, String> GetEndingTitleDate(String strIdRevues)
        {
            string[] lstlesRevues = strIdRevues.Split(',');
            DateTime dateFinAbonnement;
            Dictionary<String, String> dictFinAbo = new Dictionary<String, String>();

            foreach (var item in lstlesRevues)
            {
                string req = "SELECT Distinct titre, dateFinAbonnement FROM abonnement a JOIN document d ON (d.id = a.idRevue) ";
                req += " WHERE idRevue = @liste GROUP BY titre ";

                Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                 { "@liste", item}
              };

                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqSelect(req, parameters);

                while (curs.Read())
                {
                    string titre = (string)curs.Field("titre");
                    dateFinAbonnement = (DateTime)curs.Field("dateFinAbonnement");

                    dictFinAbo.Add(titre, dateFinAbonnement.ToShortDateString());
                }
                curs.Close();
            }
            return dictFinAbo;
        }
        #endregion


        #region Documents: Livres Dvd

        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne toutes les commandes livres ou de DVD de la BDD
        /// </summary>
        /// <param name="idDocument">Identifiant d'un livre ou Dvd</param>
        /// <returns>Liste d'objets Livre ou Dvd</returns>
        public static List<CommandeDocument> GetAllCommandesDocument(string idDocument)
        {

            List<CommandeDocument> lesCmdDoc = new List<CommandeDocument>();

            string req = "Select c.id, c.dateCommande, c.montant, c.idSuivi, cd.nbExemplaire, cd.idLivreDvd, s.label";
            req += " from commande c join suivi s on c.idSuivi = s.idSuivi";
            req += " join commandedocument cd on c.id = cd.id";
            req += " where cd.idLivreDvd = @iddoc";
            req += " order by c.dateCommande DESC;";


            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                 { "@iddoc", idDocument}
              };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);


            while (curs.Read())
            {
                string idCommande = (string)curs.Field("id");
                DateTime dateCommande = (DateTime)curs.Field("dateCommande");
                double montant = (double)curs.Field("montant");

                int idSuivi = (int)curs.Field("idSuivi");
                string label = (string)curs.Field("label");
                string idLivreDvd = (string)curs.Field("idLivreDvd");
                int nbExemplaire = (int)curs.Field("nbExemplaire");

                CommandeDocument commande = new CommandeDocument(idCommande, dateCommande, montant, idSuivi.ToString(), label, idLivreDvd, nbExemplaire);
                lesCmdDoc.Add(commande);
            }
            curs.Close();

            return lesCmdDoc;
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL INSERT
        /// Insertion d'une ligne de commande dans les tables commande et commandedocument
        /// </summary>
        /// <param name="commande">Objet d'une commande</param>
        /// <returns>Vrai si la création a réussi</returns>
        public static bool CreerCommande(CommandeDocument commande)
        {
            try
            {
                //
                // Requete d'insertion dans la table commande
                string req_c = "insert into commande values (@id, @dateCommande, @montant, @idSuivi);";
                Dictionary<string, object> parameters_c = new Dictionary<string, object>
                {
                    { "@id", commande.IdCommande},
                    { "@dateCommande", commande.DateCommande},
                    { "@montant", commande.Montant},
                    { "@idSuivi", commande.IdSuivi}
                };
                BddMySql curs_c = BddMySql.GetInstance(connectionString);
                curs_c.ReqUpdate(req_c, parameters_c);
                curs_c.Close();


                //
                // Requete d'insertion dans la table commandedocument
                string req_cd = "insert into commandedocument values (@id, @nbExemplaire, @idLivreDvd);";
                Dictionary<string, object> parameters_cd = new Dictionary<string, object>
                {
                    { "@id", commande.IdCommande},
                    { "@nbExemplaire", commande.NbExemplaire},
                    { "@idLivreDvd", commande.IdLivreDvd}
                };
                BddMySql curs_cd = BddMySql.GetInstance(connectionString);
                curs_cd.ReqUpdate(req_cd, parameters_cd);
                curs_cd.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL DELETE
        /// Suppression d'une ligne de commande dans les tables commande et commandedocument
        /// </summary>
        /// <param name="idCommande">Identifiant d'une commande de livre ou de Dvd</param>
        /// <returns>Vrai si la suppression a réussi</returns>
        public static bool SupprimerCmdDocument(string idCommande)
        {
            try
            {
                //
                // Requete de suppression dans la table commandedocument
                string req_cd = "DELETE FROM commandedocument WHERE id = @id;";
                Dictionary<string, object> parameters_cd = new Dictionary<string, object>
                {
                    { "@id", idCommande}
                };
                BddMySql curs_cd = BddMySql.GetInstance(connectionString);
                curs_cd.ReqUpdate(req_cd, parameters_cd);
                curs_cd.Close();


                //
                // Requete de suppression dans la table commande
                string req_c = "DELETE FROM commande WHERE id = @id";
                Dictionary<string, object> parameters_c = new Dictionary<string, object>
                {
                    { "@id", idCommande},
                };
                BddMySql curs_c = BddMySql.GetInstance(connectionString);
                curs_c.ReqUpdate(req_c, parameters_c);
                curs_c.Close();


                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL UPDATE
        /// Mise à jour du statut de suivi d'une commande de livre ou Dvd
        /// </summary>
        /// <param name="idCommande">Identifiant d'une commande de livre ou de Dvd</param>
        /// <param name="idSuivi">Identifiant du suivi d'une commande de livre ou de Dvd</param>
        /// <returns>Vrai si la mise à jour a réussi</returns>
        public static bool UpdateCmdDocument(string idCommande, string idSuivi)
        {
            try
            {
                string req = "UPDATE commande SET idSuivi = @idSuivi WHERE id = @id";

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idCommande},
                    { "@idSuivi", Int32.Parse(idSuivi)}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Récupération du dernier id de la table commande 
        /// Ajoute 1 à sa valeur pour créer le nouvel idCommande
        /// </summary>
        /// <returns>Identifiant de la nouvelle commande</returns>
        public static int GetLastIdCommande()
        {
            string data = "x";
            string req = "SELECT MAX(id) as id FROM commande;";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                data = (curs.Field("id") is DBNull) ? "0" : (string)curs.Field("id");

            }
            curs.Close();

            return 1 + Int32.Parse(data);
        }

        #endregion


        #region Revue

        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Récupération toutes les abonnements de revues de la BDD
        /// </summary>
        /// <param name="idRevue">Identifiant d'une revue</param>
        /// <returns>Liste des abonnements</returns>
        public static List<Abonnement> GetAllAbonnemmentRevue(string idRevue)
        {
            List<Abonnement> lesAboRevue = new List<Abonnement>();
            string req = "Select c.id, c.dateCommande, c.montant, c.idSuivi, a.dateFinAbonnement, a.idRevue, s.label";
            req += " from commande c join suivi s on c.idSuivi = s.idSuivi";
            req += " join abonnement a on c.id = a.id";
            req += " where a.idRevue = @idRevue";
            req += " order by c.dateCommande DESC;";


            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                 { "@idRevue", idRevue}
              };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);


            while (curs.Read())
            {
                string idCommande = (string)curs.Field("id");
                DateTime dateCommande = (DateTime)curs.Field("dateCommande");
                double montant = (double)curs.Field("montant");
                int idSuivi = (int)curs.Field("idSuivi");
                string label = (string)curs.Field("label");

                string idrevue = (string)curs.Field("idLivreDvd");
                DateTime dateFinAbonnement = (DateTime)curs.Field("dateFinAbonnement");

                Abonnement abonnement = new Abonnement(idCommande, dateCommande, montant, idSuivi.ToString(), label, idrevue, dateFinAbonnement);
                lesAboRevue.Add(abonnement);
            }
            curs.Close();

            return lesAboRevue;
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL INSERT
        /// Insertion d'une ligne de commande dans les tables commande et abonnement
        /// </summary>
        /// <param name="abo">Objet abonnement</param>
        /// <returns>Vrai si la création a réussi</returns>
        public static bool CreerAbonnement(Abonnement abo)
        {
            try
            {

                //
                // Requete d'insertion dans la table commande
                string req_c = "insert into commande values (@id, @dateCommande, @montant, @idSuivi);";
                Dictionary<string, object> parameters_c = new Dictionary<string, object>
                {
                    { "@id", abo.IdCommande},
                    { "@dateCommande", abo.DateCommande},
                    { "@montant", abo.Montant},
                    { "@idSuivi", abo.IdSuivi}
                };
                BddMySql curs_c = BddMySql.GetInstance(connectionString);
                curs_c.ReqUpdate(req_c, parameters_c);
                curs_c.Close();


                //
                // Requete d'insertion dans la table abonnemment
                string req_ab = "insert into abonnement values (@id, @DateFin, @idRevue);";
                Dictionary<string, object> parameters_ab = new Dictionary<string, object>
                {
                    { "@id", abo.IdCommande},
                    { "@DateFin", abo.DateFinAbonnement},
                    { "@idRevue", abo.IdRevue}
                };
                BddMySql curs_ab = BddMySql.GetInstance(connectionString);
                curs_ab.ReqUpdate(req_ab, parameters_ab);
                curs_ab.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL DELETE
        /// Suppression d'une ligne de commande dans les tables commande et abonnement
        /// </summary>
        /// <param name="idCommande">Identifiant d'une commande de revue</param>
        /// <returns>Vrai si la suppression a réussi</returns>
        public static bool SupprimerAboRevue(string idCommande)
        {
            try
            {
                //
                // Requete de suppression dans la table abonnement
                string req_ab = "DELETE FROM abonnement WHERE id = @id;";
                Dictionary<string, object> parameters_ab = new Dictionary<string, object>
                {
                    { "@id", idCommande}
                };
                BddMySql curs_ab = BddMySql.GetInstance(connectionString);
                curs_ab.ReqUpdate(req_ab, parameters_ab);
                curs_ab.Close();


                //
                // Requete de suppression dans la table commande
                string req_c = "DELETE FROM commande WHERE id = @id";
                Dictionary<string, object> parameters_c = new Dictionary<string, object>
                {
                    { "@id", idCommande},
                };
                BddMySql curs_c = BddMySql.GetInstance(connectionString);
                curs_c.ReqUpdate(req_c, parameters_c);
                curs_c.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Récupération de la dernière date de parution de la table exemplaire d'une revue 
        /// </summary>
        /// <param name="idRevue">Identifiant d'une revue</param>
        /// <returns>Date de parution d'un abonnement</returns>
        public static DateTime GetDateParution(String idRevue)
        {
            DateTime data = DateTime.MinValue;
            string req = "SELECT Max(dateAchat) as date FROM exemplaire WHERE id=@idRevue;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idRevue", idRevue}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                data = (curs.Field("date") is DBNull) ? DateTime.MinValue : (DateTime)curs.Field("date");

            }
            curs.Close();

            return data;
        }


        #endregion


        #region Categorie

        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne tous les suivis à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets de Suivi</returns>
        public static List<Categorie> GetAllSuivis()
        {
            List<Categorie> lesSuivis = new List<Categorie>();
            string req = "Select * from suivi order by label";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {

                Suivi suivi = new Suivi(((int)curs.Field("idSuivi")).ToString(), (string)curs.Field("label"));
                lesSuivis.Add(suivi);
            }
            curs.Close();
            return lesSuivis;
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public static List<Categorie> GetAllGenres()
        {
            List<Categorie> lesGenres = new List<Categorie>();
            string req = "Select * from genre order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Genre genre = new Genre((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesGenres.Add(genre);
            }
            curs.Close();
            return lesGenres;
        }


        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public static List<Categorie> GetAllRayons()
        {
            List<Categorie> lesRayons = new List<Categorie>();
            string req = "Select * from rayon order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Rayon rayon = new Rayon((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesRayons.Add(rayon);
            }
            curs.Close();
            return lesRayons;
        }

        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public static List<Categorie> GetAllPublics()
        {
            List<Categorie> lesPublics = new List<Categorie>();
            string req = "Select * from public order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Public lePublic = new Public((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesPublics.Add(lePublic);
            }
            curs.Close();
            return lesPublics;
        }

        #endregion


        #region Getter Documents
        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public static List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = new List<Livre>();
            string req = "Select l.id, l.ISBN, l.auteur, d.titre, d.image, l.collection, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from livre l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                string isbn = (string)curs.Field("ISBN");
                string auteur = (string)curs.Field("auteur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string collection = (string)curs.Field("collection");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
                lesLivres.Add(livre);
            }
            curs.Close();

            return lesLivres;
        }

        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public static List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = new List<Dvd>();
            string req = "Select l.id, l.duree, l.realisateur, d.titre, d.image, l.synopsis, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from dvd l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                int duree = (int)curs.Field("duree");
                string realisateur = (string)curs.Field("realisateur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string synopsis = (string)curs.Field("synopsis");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
                lesDvd.Add(dvd);
            }
            curs.Close();

            return lesDvd;
        }

        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public static List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = new List<Revue>();
            string req = "Select l.id, l.empruntable, l.periodicite, d.titre, d.image, l.delaiMiseADispo, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from revue l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                bool empruntable = (bool)curs.Field("empruntable");
                string periodicite = (string)curs.Field("periodicite");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                int delaiMiseADispo = (int)curs.Field("delaimiseadispo");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Revue revue = new Revue(id, titre, image, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon, empruntable, periodicite, delaiMiseADispo);
                lesRevues.Add(revue);
            }
            curs.Close();

            return lesRevues;
        }

        /// <summary>
        /// Methode permettant de lancer une requete SQL SELECT
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <returns>Liste d'objets Exemplaire</returns>
        public static List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            List<Exemplaire> lesExemplaires = new List<Exemplaire>();
            string req = "Select e.id, e.numero, e.dateAchat, e.photo, e.idEtat ";
            req += "from exemplaire e join document d on e.id=d.id ";
            req += "where e.id = @id ";
            req += "order by e.dateAchat DESC";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idDocument}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string idDoc = (string)curs.Field("id");
                int numero = (int)curs.Field("numero");
                DateTime dateAchat = (DateTime)curs.Field("dateAchat");
                string photo = (string)curs.Field("photo");
                string idEtat = (string)curs.Field("idEtat");
                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDoc);
                lesExemplaires.Add(exemplaire);
            }
            curs.Close();

            return lesExemplaires;
        }

        /// <summary>
        /// Methode permettant de lancer une requete SQL INSERT
        /// Création d'un exemplaire dans la base de données
        /// </summary>
        /// <param name="exemplaire">Objet exemplaire</param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool CreerExemplaire(Exemplaire exemplaire)
        {
            try
            {
                string req = "insert into exemplaire values (@idDocument,@numero,@dateAchat,@photo,@idEtat)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idDocument", exemplaire.IdDocument},
                    { "@numero", exemplaire.Numero},
                    { "@dateAchat", exemplaire.DateAchat},
                    { "@photo", exemplaire.Photo},
                    { "@idEtat",exemplaire.IdEtat}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
