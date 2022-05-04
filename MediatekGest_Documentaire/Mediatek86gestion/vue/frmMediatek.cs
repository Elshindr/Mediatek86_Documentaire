using Mediatek86.controleur;
using Mediatek86.metier;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mediatek86.vue
{
    /// <summary>
    /// Classe Mediatek86 de gestion des fonctionnalités graphiques 
    /// Herite de la Classe Form
    /// Permet l'affichage de la fenêtre de l'application
    /// </summary>
    public partial class FrmMediatek : Form
    {

        #region Variables globales

        private readonly Controle controle;
        const string ETATNEUF = "00001";

        private readonly BindingSource bdgLivreCmdListe = new BindingSource();
        private readonly BindingSource bdgDvdCmdListe = new BindingSource();
        private readonly BindingSource bdgRevueCmdListe = new BindingSource();

        private readonly BindingSource bdgSuivis = new BindingSource();
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private readonly BindingSource bdgExemplairesListe = new BindingSource();

        private List<CommandeDocument> lesCmdLivre = new List<CommandeDocument>();
        private List<CommandeDocument> lesCmdDvd = new List<CommandeDocument>();
        private List<Abonnement> lesCmdRevue = new List<Abonnement>();

        private List<Livre> lesLivres = new List<Livre>();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Revue> lesRevues = new List<Revue>();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        #endregion


        /// <summary>
        /// Constructeur de la Classe
        /// Récupére l'instance du controleur de la Classe Controle
        /// </summary>
        /// <param name="controle">Instance du controleur</param>
        internal FrmMediatek(Controle controle)
        {
            Log.Logger = new LoggerConfiguration()
                  .MinimumLevel.Information()
                  .WriteTo.Console()
                  .WriteTo.File("logs/App_log.txt", rollingInterval: RollingInterval.Day)
                  .WriteTo.File("logs/App_Error_log.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
                  .CreateLogger();

            InitializeComponent();
            this.controle = controle;
        }

        #region Acces
        /// <summary>
        /// Methode de supression d'access aux onglets de commandes
        /// </summary>
        public void AccesServicePrets()
        {
            tabOngletsApplication.TabPages.Remove(tabCmdRevue);
            tabOngletsApplication.TabPages.Remove(tabCmdDvd);
            tabOngletsApplication.TabPages.Remove(tabCmdLivre);
            tabOngletsApplication.TabPages.Remove(tabReceptionRevue);
        }

        /// <summary>
        /// Methode d'affichage des fin abonnements
        /// </summary>
        public void AfficheFinAbo()
        {
            MessageBox.Show(controle.GetEndingTitleDate(), "Information: Fins Abonnements");
        }

        public void AfficheServiceCulture()
        {
            MessageBox.Show("Application indisponible pour le service Culture", "Informations");
        }

        public void AfficheServiceNone()
        {
            MessageBox.Show("Application indisponible: service non détecté", "Erreur!");
        }

        #endregion

        #region RevuesCommandes
        //-----------------------------------------------------------
        // ONGLET "COMMANDES D'UNE REVUE"
        //-----------------------------------------------------------

        /// <summary>
        /// Evenement sur le clic d'entrée dans l'onglet commande de revue
        /// Bloque l'accés à la saisie d'une nouvelle commmande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCmdRevue_Enter(object sender, EventArgs e)
        {
            VideAboRevueInfos();
            AccesNewAboRevueGroupBox(false);

        }


        /// <summary>
        /// Evenement de clic sur le bouton de recherche de revue
        /// Permet l'affichage d'une revue dont le numéro est saisi.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCmdRevueRechercher_Click(object sender, EventArgs e)
        {
            if (!txbCmdRevueNumero.Text.Equals(""))
            {

                Revue revue = lesRevues.Find(x => x.Id.Equals(txbCmdRevueNumero.Text));

                if (revue != null)
                {
                    AfficheCmdRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("Numéro de revue introuvable");
                    txbCmdRevueNumero.Text = "";
                }
            }
        }


        /// <summary>
        /// Evenement de clic sur le bouton de Valider un abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNewAboRevueValider_Click(object sender, EventArgs e)
        {
            if (numNewAboRevueMontant.Value != 0 && dtpNewAboRevueDateCommande.Value < dtpNewAboRevueDateFin.Value)
            {
                var result = MessageBox.Show("Valider Commande?", "Création d'abonnement",
                          MessageBoxButtons.OKCancel,
                          MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    try
                    {
                        string strId = "00000";
                        string strMaxId = "";

                        if (controle.GetLastIdCommande().ToString() == null)
                        {
                            strMaxId = "1";
                        }
                        else
                        {
                            strMaxId = controle.GetLastIdCommande().ToString();
                        }


                        int lenMaxId = strMaxId.Length;

                        string idCommande = strId.Remove(0, lenMaxId) + strMaxId;
                        string idDocument = txbCmdRevueNumero.Text;
                        DateTime dateFinAbo = dtpNewAboRevueDateFin.Value;
                        DateTime dateCommande = dtpNewAboRevueDateCommande.Value;
                        double montant = (double)numNewAboRevueMontant.Value;

                        Abonnement abo = new Abonnement(idCommande, dateCommande, montant, "1", "en cours", idDocument, dateFinAbo);

                        if (controle.CreerAbonnement(abo))
                        {
                            lesCmdRevue = controle.GetAllAbonnemmentRevue(idDocument);
                            RemplirCmdRevueListe(lesCmdRevue);
                            VideNewAboRevueInfos();
                        }
                        else
                        {
                            Log.Information("Erreur dans la creation de l'abonnement : date='{0}' Document='{1}'", DateTime.Now.ToString(), txbCmdRevueNumero.Text);
                            MessageBox.Show("Erreur dans la creation de l'abonnement", "Erreur");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Erreur dans la creation de l'abonnement : date='{0}' Document='{1}'", DateTime.Now.ToString(), txbCmdRevueNumero.Text);
                        MessageBox.Show(ex.Message);
                        dtpNewAboRevueDateFin.Focus();
                    }
                }
            }
            else
            {
                MessageBox.Show("Erreur dans la saisie de l'abonnement: \nMontant doit être différent de 0.\n La date de fin d'abonnement doit être différente de celle de la commande.", "Information");
            }
        }


        /// <summary>
        /// Evenement de clic sur le bouton Supprimer un abonnement
        /// Appelle la methode ParutionDansAbonnement afin de valider ou non la suppression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifAboRevueSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvCmdRevueListe.RowCount != 0)
            {
                var result = MessageBox.Show("Supprimer Commande?", "Suppression d'abonnement",
                         MessageBoxButtons.OKCancel,
                         MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    string idRevue = txbCmdRevueNumero.Text;

                    DateTime dateCommande = (DateTime)dgvCmdRevueListe.CurrentRow.Cells["dateCommande"].Value;
                    DateTime dateFin = (DateTime)dgvCmdRevueListe.CurrentRow.Cells["dateFinAbonnement"].Value;
                    DateTime dateParution = controle.GetDateParution(idRevue);

                    if (controle.ParutionDansAbonnement(dateCommande, dateFin, dateParution))
                    {

                        string idCommande = dgvCmdRevueListe.CurrentRow.Cells["idCommande"].Value.ToString();
                        if (controle.SupprimerAboRevue(idCommande))
                        {
                            lesCmdRevue = controle.GetAllAbonnemmentRevue(txbCmdRevueNumero.Text);
                            RemplirCmdRevueListe(lesCmdRevue);
                            VideNewAboRevueInfos();
                        }
                        else
                        {
                            Log.Information("Erreur dans la suppression de l'abonnement : date='{0}' Document='{1}'", DateTime.Now.ToString(), idRevue);
                            MessageBox.Show("Erreur dans la suppression de l'abonnement ", "Erreur");
                        }
                    }
                    else
                    {
                        Log.Error("Erreur dans la creation de l'abonnement : date='{0}' Document='{1}'", DateTime.Now.ToString(), idRevue);
                        MessageBox.Show("Abonnement non supprimable.\nDate de Commande:" + dateCommande.ToShortDateString() + "\nDate de parution: " + dateParution.ToShortDateString() + "\nDate de fin d'abonnement: " + dateFin.ToShortDateString(), "Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Nombre de ligne selectionné incorrecte ", "Erreur");
            }

        }


        /// <summary>
        /// Evenement sur le changement de valeur saisie dans le champs de recherche de numero de revue dans l'onglet Commande de revue
        /// Bloque l'accès au bloc de commande de revue
        /// Vide les champs des infos de revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbCmdRevueNumero_TextChanged(object sender, EventArgs e)
        {
            AccesNewAboRevueGroupBox(false);
            VideNewAboRevueInfos();
            if (txbCmdRevueNumero.Text == "")
            {
                VideAboRevueInfos();
            }

        }


        /// <summary>
        /// Evenemement de changement de selection de ligne dans la liste de l'onglet commande de revue
        /// Met l'affichage à jour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvCmdRevueListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCmdRevueListe.RowCount != 0)
            {
                txbModifAboRevueIdCommande.Text = dgvCmdRevueListe.CurrentRow.Cells["idCommande"].Value.ToString();
                txbModifAboRevueIdRevue.Text = txbCmdRevueNumero.Text;
                dtpModifAboRevueDateCommande.Value = (DateTime)dgvCmdRevueListe.CurrentRow.Cells["dateCommande"].Value;
                dtpModifAboRevueDateFin.Value = (DateTime)dgvCmdRevueListe.CurrentRow.Cells["dateFinAbonnement"].Value;
            }
        }


        /// <summary>
        /// Evenement sur le clic de l'entete d'une colonne de la liste des revues
        /// Trie la colonne par ordre croissant 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvCmdRevueListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideNewCmdLivresInfos();
            string titreColonne = dgvCmdRevueListe.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "IdCommande":
                    sortedList = lesCmdRevue.OrderBy(o => o.IdCommande).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesCmdRevue.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = lesCmdRevue.OrderBy(o => o.Montant).ToList();
                    break;
                case "DateFinAbonnement":
                    sortedList = lesCmdRevue.OrderBy(o => o.DateFinAbonnement).ToList();
                    break;

            }
            RemplirCmdRevueListe(sortedList);

        }





        /// <summary>
        /// Methode d'affichage des informations de la revue sélectionnée 
        /// </summary>
        /// <param name="revue">Objet revue</param>
        private void AfficheCmdRevueInfos(Revue revue)
        {
            txbCmdRevueNumero.Text = revue.Id;
            txbCmdRevueTitre.Text = revue.Titre;
            txbCmdRevuePeriodicite.Text = revue.Periodicite;
            txbCmdRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();

            txbCmdRevueGenre.Text = revue.Genre;
            txbCmdRevuePublic.Text = revue.Public;
            txbCmdRevueRayon.Text = revue.Rayon;

            chkCmdRevueEmpruntable.Checked = revue.Empruntable;
            pcbCmdRevueImage.Text = revue.Image;


            string image = revue.Image;
            try
            {
                pcbCmdRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbCmdRevueImage.Image = null;
            }
            string idRevue = txbCmdRevueNumero.Text;
            lesCmdRevue = controle.GetAllAbonnemmentRevue(idRevue);
            RemplirCmdRevueListe(lesCmdRevue);
            AccesNewAboRevueGroupBox(true);
        }


        /// <summary>
        /// Methode d'affichage de la liste des revues commandées dans le datagridview associée
        /// </summary>
        /// <param name="abonn">Collection d'abonnements</param>
        private void RemplirCmdRevueListe(List<Abonnement> abonn)
        {
            bdgRevueCmdListe.DataSource = abonn;
            dgvCmdRevueListe.DataSource = bdgRevueCmdListe;
            dgvCmdRevueListe.Columns["idCommande"].Visible = false;
            dgvCmdRevueListe.Columns["idSuivi"].Visible = false;

            dgvCmdRevueListe.Columns["label"].Visible = false;
            dgvCmdRevueListe.Columns["idRevue"].Visible = false;
            dgvCmdRevueListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dgvCmdRevueListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCmdRevueListe.Columns["montant"].DisplayIndex = 1;
            dgvCmdRevueListe.Columns["dateFinAbonnement"].DisplayIndex = 2;
        }



        /// <summary>
        /// Methode qui permet ou bloque l'accès à la gestion de prise de commandes de DVD
        /// Vide les objets graphiques de l'onglet commande de revue
        /// </summary>
        /// <param name="acces"></param>
        private void AccesNewAboRevueGroupBox(bool acces)
        {
            VideNewAboRevueInfos();
            gpbNewAboRevue.Enabled = acces;
            gpbModifAboRevue.Enabled = acces;
        }


        /// <summary>
        /// Methode qui vide les zones d'affichage des informations du bloc de Commandes de revue
        /// </summary>
        private void VideNewAboRevueInfos()
        {
            numNewCmdDvdMontant.Value = 0;
            dtpNewAboRevueDateFin.Value = DateTime.Now;
            dtpNewAboRevueDateCommande.Value = DateTime.Now;
        }


        /// <summary>
        /// Methode qui vide les zones d'affichage des informations du bloc Commandes de revue
        /// Vide le datagridview associé
        /// Bloque l'accès aux prises de commandes.
        /// </summary>
        private void VideAboRevueInfos()
        {

            txbCmdRevueNumero.Text = "";
            txbCmdRevueTitre.Text = "";
            txbCmdRevuePeriodicite.Text = "";
            txbCmdRevueDelaiMiseADispo.Text = "";

            txbCmdRevueGenre.Text = "";
            txbCmdRevuePublic.Text = "";
            txbCmdRevueRayon.Text = "";

            chkCmdRevueEmpruntable.Checked = false;
            pcbCmdRevueImage.Text = "";

            pcbCmdRevueImage.Image = null;

            // Vide le datagridview
            lesCmdRevue = new List<Abonnement>();
            RemplirCmdRevueListe(lesCmdRevue);
        }
        #endregion

        #region DVDCommandes

        //-----------------------------------------------------------
        // ONGLET "COMMANDES D'UN DVD"
        //-----------------------------------------------------------


        /// <summary>
        /// Evenement sur le clic d'entrée dans l'onglet commande de DVD
        /// Bloque l'accés à la saisie d'une nouvelle commmande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCmdDvd_Enter(object sender, EventArgs e)
        {
            VideCmdDvdInfos();
            AccesNewCmdDvdGroupBox(false);
            RemplirComboCategorie(controle.GetAllSuivis(), bdgSuivis, cbxModifCmdDvd);
        }


        /// <summary>
        /// Evenement de clic sur le bouton de recherche
        /// Permet l'affichage du Dvd dont le numéro est saisi.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCmdDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCmdDvdNumero.Text.Equals(""))
            {

                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbCmdDvdNumero.Text));

                if (dvd != null)
                {
                    AfficheCmdDvdInfos(dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    txbCmdDvdNumero.Text = "";
                }
            }

        }


        /// <summary>
        /// Evenement sur le clic du bouton valider une nouvelle commande de DVD
        /// dans l'onglet Commande de DVD
        /// Appelle Methode d'insertion de la nouvelle commande
        /// Vide l'affichage de la nouvelle commande 
        /// Met à jour la liste des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        private void BtnNewCmdDvdValider_Clic(object sender, EventArgs e)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            if (numNewCmdDvdMontant.Value != 0 && numNewCmdDvdNbExemplaire.Value != 0)
            {
                var result = MessageBox.Show("Valider Commande?", "Confirmation de Commande",
                              MessageBoxButtons.OKCancel,
                              MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    try
                    {
                        string strId = "00000";
                        string strMaxId = "";
                        if (controle.GetLastIdCommande().ToString() == null)
                        {
                            strMaxId = "1";
                        }
                        else
                        {
                            strMaxId = controle.GetLastIdCommande().ToString();
                        }


                        int lenMaxId = strMaxId.Length;

                        string idCommande = strId.Remove(0, lenMaxId) + strMaxId;
                        string idDocument = txbCmdDvdNumero.Text;
                        int nbExemplaire = (int)numNewCmdDvdNbExemplaire.Value;
                        DateTime dateCommande = dtpNewCmdDvdDate.Value;
                        double montant = (double)numNewCmdDvdMontant.Value;

                        CommandeDocument commande = new CommandeDocument(idCommande, dateCommande, montant, "1", "en cours", idDocument, nbExemplaire);

                        if (controle.CreerCommande(commande))
                        {
                            lesCmdDvd = controle.GetAllCommandesDocument(idDocument);
                            RemplirCmdDvdListe(lesCmdDvd);
                            VideNewCmdDvdInfos();
                        }
                        else
                        {
                            MessageBox.Show("Erreur dans la creation de commande", "Erreur");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        numNewCmdDvdNbExemplaire.Focus();
                    }
                }
            }
            else
            {
                MessageBox.Show("Montant et nombre d'exemplaires doivent être différents de 0", "Information");
            }

        }


        /// <summary>
        /// Evenement sur le clic du bouton Modifier Suivi de l'onglet Commande de DVD
        /// - Modifie le status du suivi de la commande du DVD sélectionné par requete UPDATE vers la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifCmdDvdModifier_Click(object sender, EventArgs e)
        {
            if (dgvCmdDvdListe.RowCount != 0)
            {
                var result = MessageBox.Show("Modifier Suivi?", "Confirmation de Modification",
                                 MessageBoxButtons.OKCancel,
                                 MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    string idCommande = dgvCmdDvdListe.CurrentRow.Cells["idCommande"].Value.ToString();

                    int tabIdSuivi = Int32.Parse(dgvCmdDvdListe.CurrentRow.Cells["idSuivi"].Value.ToString());
                    int cbxIdSuivi = cbxModifCmdDvd.SelectedIndex;

                    if (tabIdSuivi != 2 && cbxIdSuivi == 2)
                    {
                        MessageBox.Show("Commande non livrée", "Information");
                    }
                    else if ((tabIdSuivi == 2 || tabIdSuivi == 3) && (cbxIdSuivi == 0 || cbxIdSuivi == 3))
                    {
                        MessageBox.Show("Commande déjà livrée ou réglée", "Information");
                    }
                    else if (tabIdSuivi == 3 && cbxIdSuivi <= 3)
                    {
                        MessageBox.Show("Commande déjà réglée", "Information");
                    }
                    else
                    {
                        UpdateCmdDvd(idCommande, (cbxIdSuivi + 1).ToString());
                    }
                }

            }
            else
            {
                MessageBox.Show("Nombre de ligne selectionné incorrecte ", "Erreur");
            }
        }


        /// <summary>
        /// Evenement sur le clic du bouton Supprimer Commande de l'onglet Commande de DVD
        /// Supprimer une commande du livre sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifCmdDvdSuppr_Click(object sender, EventArgs e)
        {
            if (dgvCmdDvdListe.RowCount != 0)
            {
                var result = MessageBox.Show("Supprimer Commande?", "Confirmation de Supression",
                               MessageBoxButtons.OKCancel,
                               MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    if (dgvCmdDvdListe.CurrentRow.Cells["idSuivi"].Value.ToString() != "2" && dgvCmdDvdListe.CurrentRow.Cells["idSuivi"].Value.ToString() != "3")
                    {
                        string idCommande = dgvCmdDvdListe.CurrentRow.Cells["idCommande"].Value.ToString();
                        if (controle.SupprimerCmdDocument(idCommande))
                        {
                            lesCmdDvd = controle.GetAllCommandesDocument(txbCmdDvdNumero.Text);
                            RemplirCmdDvdListe(lesCmdDvd);
                            VideNewCmdDvdInfos();
                        }
                        else
                        {
                            MessageBox.Show("Erreur dans la suppresion de commande", "Erreur");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Commande déjà livrée", "Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Nombre de ligne selectionné incorrecte ", "Erreur");
            }
        }


        /// <summary>
        /// Evenement sur le changement de valeur saisie dans le champs de recherche de numero de dvd dans l'onglet Commande de dvd
        /// - Bloque l'accès au bloc de commande de dvd
        /// - Vide les champs des infos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbCmdDvdNumRecherche_TextChanged(object sender, EventArgs e)
        {

            AccesNewCmdDvdGroupBox(false);
            VideNewCmdDvdInfos();
            if (txbCmdDvdNumero.Text == "")
            {
                VideCmdDvdInfos();
            }
        }


        /// <summary>
        /// Evenemement de changement de selection de ligne dans la liste de l'onglet commande de livre.
        /// Met l'affichage à jour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvCmdDvd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCmdDvdListe.RowCount != 0)
            {
                txbModifCmdDvdIdCommande.Text = dgvCmdDvdListe.CurrentRow.Cells["idCommande"].Value.ToString();
                txbModifCmdDvdIdDvd.Text = dgvCmdDvdListe.CurrentRow.Cells["idLivreDvd"].Value.ToString();
                dtpModifCmdDvdDateCmd.Value = (DateTime)dgvCmdDvdListe.CurrentRow.Cells["dateCommande"].Value;
                txbModifCmdDvdNbExemplaire.Text = dgvCmdDvdListe.CurrentRow.Cells["nbExemplaire"].Value.ToString();
                cbxModifCmdDvd.SelectedIndex = (-1) + Int32.Parse(dgvCmdDvdListe.CurrentRow.Cells["idSuivi"].Value.ToString());

            }
        }


        /// <summary>
        /// Evenement sur le clic de l'entete d'une colonne d'une liste
        /// Trie la colonne par ordre croissant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvCmdDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideNewCmdLivresInfos();
            string titreColonne = dgvCmdDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "IdCommande":
                    sortedList = lesCmdDvd.OrderBy(o => o.IdCommande).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesCmdDvd.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = lesCmdDvd.OrderBy(o => o.Montant).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCmdDvd.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Label":
                    sortedList = lesCmdDvd.OrderBy(o => o.Label).ToList();
                    break;

            }
            RemplirCmdDvdListe(sortedList);
        }

        /// <summary>
        /// Methode de mise à jour de commande de DVD 
        /// Appelle les methodes pour remplir la liste des commandes
        /// Vides les informations de la commande
        /// </summary>
        /// <param name="idCommande"></param>
        /// <param name="idSuivi"></param>
        public void UpdateCmdDVD(string idCommande, string idSuivi)
        {
            if (controle.UpdateCmdDocument(idCommande, idSuivi))
            {
                lesCmdDvd = controle.GetAllCommandesDocument(txbCmdDvdNumero.Text);
                RemplirCmdDvdListe(lesCmdDvd);
                VideNewCmdDvdInfos();
            }
            else
            {
                MessageBox.Show("Erreur dans l'update de suivi commande", "Erreur");
            }

        }


        /// <summary>
        /// Methode d'affichage des informations du livre sélectionné dans l'onglet des commandes de DVD
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheCmdDvdInfos(Dvd dvd)
        {
            txbCmdDvdNumero.Text = dvd.Id;
            txbCmdDvdTitre.Text = dvd.Titre;
            txbCmdDvdRealisateur.Text = dvd.Realisateur;
            txbCmdDvdSynopsis.Text = dvd.Synopsis;
            txbCmdDvdGenre.Text = dvd.Genre;
            txbCmdDvdPublic.Text = dvd.Public;
            txbCmdDvdDuree.Text = dvd.Duree.ToString();
            txbCmdDvdRayon.Text = dvd.Rayon;
            txbCmdDvdImage.Text = dvd.Image;

            string image = dvd.Image;
            try
            {
                pcbCmdDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbCmdDvdImage.Image = null;
            }
            string idDocument = txbCmdDvdNumero.Text;
            lesCmdDvd = controle.GetAllCommandesDocument(idDocument);
            RemplirCmdDvdListe(lesCmdDvd);
            AccesNewCmdDvdGroupBox(true);
        }


        /// <summary>
        /// Methode qui remplit le datagridview avec la liste des commandes de DVD reçues en paramètre
        /// </summary>
        /// <param name="commandes">Liste des commandes de livres</param>
        private void RemplirCmdDvdListe(List<CommandeDocument> commandes)
        {
            bdgDvdCmdListe.DataSource = commandes;
            dgvCmdDvdListe.DataSource = bdgDvdCmdListe;
            dgvCmdDvdListe.Columns["idCommande"].Visible = false;
            dgvCmdDvdListe.Columns["idSuivi"].Visible = false;
            dgvCmdDvdListe.Columns["idLivreDvd"].Visible = false;

            dgvCmdDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCmdDvdListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCmdDvdListe.Columns["montant"].DisplayIndex = 1;
            dgvCmdDvdListe.Columns["nbExemplaire"].DisplayIndex = 2;
            dgvCmdDvdListe.Columns["label"].DisplayIndex = 3;
        }

        /// <summary>
        /// Methode de mise à jour de commande de livres 
        /// Appelle les methodes pour remplir la liste des commandes
        /// Vides les informations de la commande
        /// </summary>
        /// <param name="idCommande"></param>
        /// <param name="idSuivi"></param>
        public void UpdateCmdDvd(string idCommande, string idSuivi)
        {
            if (controle.UpdateCmdDocument(idCommande, idSuivi))
            {
                lesCmdDvd = controle.GetAllCommandesDocument(txbCmdDvdNumero.Text);
                RemplirCmdDvdListe(lesCmdDvd);
                VideNewCmdDvdInfos();
            }
            else
            {
                MessageBox.Show("Erreur dans la update de suivi commande", "Erreur");
            }

        }

        /// <summary>
        /// Methode qui permet ou bloque l'accès à la gestion de prise de commandes de DVD
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces"></param>
        private void AccesNewCmdDvdGroupBox(bool acces)
        {
            VideNewCmdDvdInfos();
            gpbNewCmdDvd.Enabled = acces;
            gpbModifCmdDvd.Enabled = acces;
        }


        /// <summary>
        /// Methode qui vide les zones d'affichage des informations du bloc de commandes de DVD
        /// </summary>
        private void VideNewCmdDvdInfos()
        {
            numNewCmdDvdNbExemplaire.Value = 0;
            numNewCmdDvdMontant.Value = 0;
            dtpNewCmdDvdDate.Value = DateTime.Now;
        }


        /// <summary>
        /// Methode qui vide les zones d'affichage des informations du bloc de DVD à commander
        /// Vide le datagridview associé
        /// Bloque l'accès aux prises de commandes.
        /// </summary>
        private void VideCmdDvdInfos()
        {
            txbCmdDvdNumero.Text = "";
            txbCmdDvdTitre.Text = "";
            txbCmdDvdRealisateur.Text = "";
            txbCmdDvdSynopsis.Text = "";

            txbCmdDvdGenre.Text = "";
            txbCmdDvdPublic.Text = "";
            txbCmdDvdDuree.Text = "";
            txbCmdDvdRayon.Text = "";

            txbCmdDvdImage.Text = "";
            pcbCmdDvdImage.Image = null;

            // Vide le datagridview
            lesCmdDvd = new List<CommandeDocument>();
            RemplirCmdDvdListe(lesCmdDvd);
        }


        #endregion

        #region LivresCommandes

        //-----------------------------------------------------------
        // ONGLET "COMMANDES D'UN LIVRE"
        //-----------------------------------------------------------

        /// <summary>
        /// Evenement sur le clic d'entrée dans l'onglet commande de livre
        /// Bloque l'accés à la saisie d'une nouvelle commmande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCmdLivre_Enter(object sender, EventArgs e)
        {
            VideCmdLivresInfos();
            AccesNewCmdLivresGroupBox(false);
            RemplirComboCategorie(controle.GetAllSuivis(), bdgSuivis, cbxModifCmdLivresSuivi);
        }


        /// <summary>
        /// Evenement de clic sur le bouton de recherche
        /// Permet l'affichage du livre dont le numéro est saisi.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCmdLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCmdLivresNumero.Text.Equals(""))
            {

                Livre livre = lesLivres.Find(x => x.Id.Equals(txbCmdLivresNumero.Text));

                if (livre != null)
                {
                    AfficheCmdLivresInfos(livre);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    txbCmdLivresNumero.Text = "";
                }
            }

        }

        /// <summary>
        /// Evenement sur le clic du bouton Supprimer Commande de l'onglet Commande de Livre
        /// Supprimer une commande du livre sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifCmdLivresSuppr_Click(object sender, EventArgs e)
        {

            if (dgvCmdLivresListe.RowCount != 0)
            {
                var result = MessageBox.Show("Supprimer Commande?", "Confirmation de Suppression",
                             MessageBoxButtons.OKCancel,
                             MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    if (dgvCmdLivresListe.CurrentRow.Cells["idSuivi"].Value.ToString() != "2" && dgvCmdLivresListe.CurrentRow.Cells["idSuivi"].Value.ToString() != "3")
                    {
                        string idCommande = dgvCmdLivresListe.CurrentRow.Cells["idCommande"].Value.ToString();
                        if (controle.SupprimerCmdDocument(idCommande))
                        {
                            lesCmdLivre = controle.GetAllCommandesDocument(txbCmdLivresNumero.Text);
                            RemplirCmdLivresListe(lesCmdLivre);
                            VideNewCmdLivresInfos();
                        }
                        else
                        {
                            MessageBox.Show("Erreur dans la suppresion de commande", "Erreur");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Commande déjà livrée", "Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Nombre de ligne selectionné incorrecte ", "Erreur");
            }
        }

        /// <summary>
        /// Evenement sur le clic du bouton valider une nouvelle commande 
        /// dans l'onglet Commande de livre 
        /// Appelle Methode d'insertion de la nouvelle commande
        /// Vide l'affichage de la nouvelle commande 
        /// Met à jour la liste des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNewCmdLivresValider_Click(object sender, EventArgs e)
        {
            if (numNewCmdLivresMontant.Value != 0 && numNewCmdLivresNbExemplaire.Value != 0)
            {
                var result = MessageBox.Show("Valider Commande?", "Confirmation de Commande",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    try
                    {
                        string strId = "00000";
                        string strMaxId = "";
                        if (controle.GetLastIdCommande().ToString() == null)
                        {
                            strMaxId = "1";
                        }
                        else
                        {
                            strMaxId = controle.GetLastIdCommande().ToString();
                        }


                        int lenMaxId = strMaxId.Length;

                        string idCommande = strId.Remove(0, lenMaxId) + strMaxId;
                        string idLivre = txbCmdLivresNumero.Text;

                        DateTime dateCommande = dtpNewCmdLivresDate.Value;
                        int nbExemplaire = (int)numNewCmdLivresNbExemplaire.Value;
                        double montant = (double)numNewCmdLivresMontant.Value;

                        string idDocument = txbCmdLivresNumero.Text;
                        CommandeDocument commande = new CommandeDocument(idCommande, dateCommande, montant, "1", "en cours", idLivre, nbExemplaire);

                        if (controle.CreerCommande(commande))
                        {
                            lesCmdLivre = controle.GetAllCommandesDocument(idDocument);
                            RemplirCmdLivresListe(lesCmdLivre);
                            VideNewCmdLivresInfos();
                        }
                        else
                        {
                            MessageBox.Show("Erreur dans la creation de commande", "Erreur");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "merde");
                        numNewCmdLivresNbExemplaire.Focus();
                    }
                }
            }
            else
            {
                MessageBox.Show("Montant et nombre d'exemplaires doivent être différents de 0", "Information");
            }

        }


        /// <summary>
        /// Evenement sur le clic du bouton Modifier Suivi de l'onglet Commande de Livre
        /// - Modifie le status du suivi de la commande du livre sélectionné par requete UPDATE vers la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifCmdLivresModifier_Click(object sender, EventArgs e)
        {
            if (dgvCmdLivresListe.RowCount != 0)
            {
                var result = MessageBox.Show("Modifier Suivi de Commande?", "Modification de Commande",
                           MessageBoxButtons.OKCancel,
                           MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    string idCommande = dgvCmdLivresListe.CurrentRow.Cells["idCommande"].Value.ToString();

                    int tabIdSuivi = Int32.Parse(dgvCmdLivresListe.CurrentRow.Cells["idSuivi"].Value.ToString());
                    int cbxIdSuivi = cbxModifCmdLivresSuivi.SelectedIndex;

                    if (tabIdSuivi != 2 && cbxIdSuivi == 2)
                    {
                        MessageBox.Show("Commande non livrée", "Information");
                    }
                    else if ((tabIdSuivi == 2 || tabIdSuivi == 3) && (cbxIdSuivi == 0 || cbxIdSuivi == 3))
                    {
                        MessageBox.Show("Commande déjà livrée ou réglée", "Information");
                    }
                    else if (tabIdSuivi == 3 && cbxIdSuivi <= 3)
                    {
                        MessageBox.Show("Commande déjà réglée", "Information");
                    }
                    else
                    {
                        UpdateCmdLivres(idCommande, (cbxIdSuivi + 1).ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Nombre de ligne selectionné incorrecte ", "Erreur");
            }
        }


        /// <summary>
        /// Evenement sur le changement de valeur saisie dans le champs de recherche de numero de livre dans l'onglet Commande de livre
        /// - Bloque l'accès au bloc de commande de livres
        /// - Vide les champs des infos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbCmdLivresNumRecherche_TextChanged(object sender, EventArgs e)
        {

            AccesNewCmdLivresGroupBox(false);
            VideNewCmdLivresInfos();
            if (txbCmdLivresNumero.Text == "")
            {
                VideCmdLivresInfos();
            }
        }


        /// <summary>
        /// Evenemement de changement de selection de ligne dans la liste de l'onglet commande de livre.
        /// Met l'affichage à jour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresCmdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCmdLivresListe.RowCount != 0)
            {
                txbModifCmdLivresIdCmd.Text = dgvCmdLivresListe.CurrentRow.Cells["idCommande"].Value.ToString();
                txbModifCmdIdLivre.Text = dgvCmdLivresListe.CurrentRow.Cells["idLivreDvd"].Value.ToString();
                dtpModifCmdLivresDateComd.Value = (DateTime)dgvCmdLivresListe.CurrentRow.Cells["dateCommande"].Value;
                txbModifCmdLivresNbExemplaire.Text = dgvCmdLivresListe.CurrentRow.Cells["nbExemplaire"].Value.ToString();
                cbxModifCmdLivresSuivi.SelectedIndex = (-1) + Int32.Parse(dgvCmdLivresListe.CurrentRow.Cells["idSuivi"].Value.ToString());
            }


        }


        /// <summary>
        /// Methode de mise à jour de commande de livres 
        /// Appelle les methodes pour remplir la liste des commandes
        /// Vides les informations de la commande
        /// </summary>
        /// <param name="idCommande"></param>
        /// <param name="idSuivi"></param>
        public void UpdateCmdLivres(string idCommande, string idSuivi)
        {
            if (controle.UpdateCmdDocument(idCommande, idSuivi))
            {
                lesCmdLivre = controle.GetAllCommandesDocument(txbCmdLivresNumero.Text);
                RemplirCmdLivresListe(lesCmdLivre);
                VideNewCmdLivresInfos();
            }
            else
            {
                MessageBox.Show("Erreur dans la update de suivi commande", "Erreur");
            }

        }


        /// <summary>
        /// Methode d'affichage des informations du livre sélectionné dans l'onglet des commandes de livre
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheCmdLivresInfos(Livre livre)
        {
            txbCmdLivresAuteur.Text = livre.Auteur;
            txbCmdLivresCollection.Text = livre.Collection;
            pcbCmdLivresImage.Text = livre.Image;
            txbCmdLivresIsbn.Text = livre.Isbn;
            txbCmdLivresNumero.Text = livre.Id;
            txbCmdLivresGenre.Text = livre.Genre;
            txbCmdLivresPublic.Text = livre.Public;
            txbCmdLivresRayon.Text = livre.Rayon;
            txbCmdLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbCmdLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbCmdLivresImage.Image = null;
            }
            string idDocument = txbCmdLivresNumero.Text;
            lesCmdLivre = controle.GetAllCommandesDocument(idDocument);
            RemplirCmdLivresListe(lesCmdLivre);
            AccesNewCmdLivresGroupBox(true);
        }


        /// <summary>
        /// Methode qui remplit le datagridview avec la liste des commandes de livre reçue en paramètre
        /// </summary>
        /// <param name="commandes">Liste des commandes du document</param>
        private void RemplirCmdLivresListe(List<CommandeDocument> commandes)
        {
            bdgLivreCmdListe.DataSource = commandes;
            dgvCmdLivresListe.DataSource = bdgLivreCmdListe;
            dgvCmdLivresListe.Columns["idCommande"].Visible = false;
            dgvCmdLivresListe.Columns["idSuivi"].Visible = false;
            dgvCmdLivresListe.Columns["idLivreDvd"].Visible = false;

            dgvCmdLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCmdLivresListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCmdLivresListe.Columns["montant"].DisplayIndex = 1;
            dgvCmdLivresListe.Columns["nbExemplaire"].DisplayIndex = 2;
            dgvCmdLivresListe.Columns["label"].DisplayIndex = 3;

        }


        /// <summary>
        /// Methode qui permet ou bloque l'accès à la gestion de prise de commandes de livres
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces"></param>
        private void AccesNewCmdLivresGroupBox(bool acces)
        {
            VideNewCmdLivresInfos();
            gpbNewCmdLivres.Enabled = acces;
            gpbModifCmdLivres.Enabled = acces;
        }


        /// <summary>
        /// Methode qui vide les zones d'affichage des informations du bloc de commandes de livres
        /// </summary>
        private void VideNewCmdLivresInfos()
        {
            numNewCmdLivresNbExemplaire.Value = 0;
            numNewCmdLivresMontant.Value = 0;
            dtpNewCmdLivresDate.Value = DateTime.Now;
        }


        /// <summary>
        /// Methode qui vide les zones d'affichage des informations du bloc de livre à commander
        /// Vide le datagridview associé
        /// Bloque l'accès aux prises de commandes.
        /// </summary>
        private void VideCmdLivresInfos()
        {
            txbCmdLivresNumero.Text = "";
            txbCmdLivresIsbn.Text = "";
            txbCmdLivresTitre.Text = "";
            txbCmdLivresAuteur.Text = "";
            txbCmdLivresCollection.Text = "";
            txbCmdLivresGenre.Text = "";
            txbCmdLivresPublic.Text = "";
            txbCmdLivresRayon.Text = "";
            txbCmdLivresImage.Text = "";
            pcbCmdLivresImage.Image = null;

            // Vide le datagridview
            lesCmdLivre = new List<CommandeDocument>();
            RemplirCmdLivresListe(lesCmdLivre);
        }


        /// <summary>
        /// Evenement sur le clic d'une colonne de la liste
        /// - Tri la liste selon l'entete de colonne cliquée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvCmdLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            VideNewCmdLivresInfos();
            string titreColonne = dgvCmdLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesCmdLivre.OrderBy(o => o.IdCommande).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesCmdLivre.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = lesCmdLivre.OrderBy(o => o.Montant).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCmdLivre.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Label":
                    sortedList = lesCmdLivre.OrderBy(o => o.Label).ToList();
                    break;

            }
            RemplirCmdLivresListe(sortedList);

        }

        #endregion

        #region modules communs

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        #endregion

        #region Revues
        //-----------------------------------------------------------
        // ONGLET "Revues"
        //------------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["empruntable"].Visible = false;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>();
                    revues.Add(revue);
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            chkRevuesEmpruntable.Checked = revue.Empruntable;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            chkRevuesEmpruntable.Checked = false;
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }


        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        #endregion

        #region Livres
        //-----------------------------------------------------------
        // ONGLET "LIVRES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controle.GetAllLivres();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>();
                    livres.Add(livre);
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);

                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        #endregion

        #region Dvd
        //-----------------------------------------------------------
        // ONGLET "DVD"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controle.GetAllDvd();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>();
                    Dvd.Add(dvd);
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        #endregion

        #region Réception Exemplaire de presse
        //-----------------------------------------------------------
        // ONGLET "RECEPTION DE REVUES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgExemplairesListe.DataSource = exemplaires;
            dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
            dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
            dgvReceptionExemplairesListe.Columns["idDocument"].Visible = false;
            dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
            dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    VideReceptionRevueInfos();
                }
            }
            else
            {
                VideReceptionRevueInfos();
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            AccesReceptionExemplaireGroupBox(false);
            VideReceptionRevueInfos();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            chkReceptionRevueEmpruntable.Checked = revue.Empruntable;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
            // accès à la zone d'ajout d'un exemplaire
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Methode qui affiche la liste des Exemplaires d'une Revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controle.GetExemplairesRevue(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations de la revue
        /// </summary>
        private void VideReceptionRevueInfos()
        {
            txbReceptionRevuePeriodicite.Text = "";
            chkReceptionRevueEmpruntable.Checked = false;
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            lesExemplaires = new List<Exemplaire>();
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de l'exemplaire
        /// </summary>
        private void VideReceptionExemplaireInfos()
        {
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces"></param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            VideReceptionExemplaireInfos();
            grpReceptionExemplaire.Enabled = acces;
        }


        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }


        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                var result = MessageBox.Show("Valider la réception?", "Confirmation de réception",
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Question);

                // If the no button was pressed ...
                if (result == DialogResult.OK)
                {
                    try
                    {
                        int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                        DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                        string photo = txbReceptionExemplaireImage.Text;
                        string idEtat = ETATNEUF;
                        string idDocument = txbReceptionRevueNumero.Text;
                        Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                        if (controle.CreerExemplaire(exemplaire))
                        {
                            VideReceptionExemplaireInfos();
                            AfficheReceptionExemplairesRevue();
                        }
                        else
                        {
                            MessageBox.Show("numéro de publication déjà existant", "Erreur");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("le numéro de parution doit être numérique", "Information");
                        txbReceptionExemplaireNumero.Text = "";
                        txbReceptionExemplaireNumero.Focus();
                    }
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }


        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// Sélection d'une ligne complète et affichage de l'image sz l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }




        #endregion


    }
}
