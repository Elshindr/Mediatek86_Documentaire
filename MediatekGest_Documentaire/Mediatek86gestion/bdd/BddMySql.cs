namespace Mediatek86.bdd
{
    using MySql.Data.MySqlClient;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="BddMySql" />.
    /// </summary>
    public class BddMySql
    {
        /// <summary>
        /// Unique instance de la classe.
        /// </summary>
        private static BddMySql instance = null;

        /// <summary>
        /// objet de connexion à la BDD à partir d'une chaîne de connexion.
        /// </summary>
        private readonly MySqlConnection connection;

        /// <summary>
        /// objet contenant le résultat d'une requête "select" (curseur).
        /// </summary>
        private MySqlDataReader reader;

        /// <summary>
        /// Prevents a default instance of the <see cref="BddMySql"/> class from being created.
        /// </summary>
        /// <param name="stringConnect">chaine de connexion.</param>
        private BddMySql(string stringConnect)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.Console()
                   .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                   .WriteTo.File("logs/Error_log.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
                   .WriteTo.File("logs/Fatal_log.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Fatal)
                   .CreateLogger();

                connection = new MySqlConnection(stringConnect);
                connection.Open();
            }
            catch (MySqlException e)
            {
                ErreurGraveBddNonAccessible(e, stringConnect);
            }
        }

        /// <summary>
        /// Crée une instance unique de la classe.
        /// </summary>
        /// <param name="stringConnect">chaine de connexion.</param>
        /// <returns>instance unique de la classe.</returns>
        public static BddMySql GetInstance(string stringConnect)
        {
            if (instance is null)
            {
                instance = new BddMySql(stringConnect);
            }
            return instance;
        }

        /// <summary>
        /// Exécute une requête type "select" et valorise le curseur.
        /// </summary>
        /// <param name="stringQuery">requête select.</param>
        /// <param name="parameters">The parameters<see cref="Dictionary{string, object}"/>.</param>
        public void ReqSelect(string stringQuery, Dictionary<string, object> parameters)
        {
            MySqlCommand command;

            try
            {
                command = new MySqlCommand(stringQuery, connection);
                if (!(parameters is null))
                {
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        command.Parameters.Add(new MySqlParameter(parameter.Key, parameter.Value));
                    }
                }
                command.Prepare();
                reader = command.ExecuteReader();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                Log.Error("BddMySql.ReqSelect catch stringQuery={0} erreur={1}", stringQuery, e.Message);
            }
            catch (InvalidOperationException e)
            {
                ErreurGraveBddNonAccessible(e, "");
            }
        }

        /// <summary>
        /// Tente de lire la ligne suivante du curseur.
        /// </summary>
        /// <returns>false si fin de curseur atteinte.</returns>
        public bool Read()
        {
            if (reader is null)
            {
                return false;
            }
            try
            {
                return reader.Read();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retourne le contenu d'un champ dont le nom est passé en paramètre.
        /// </summary>
        /// <param name="nameField">nom du champ.</param>
        /// <returns>valeur du champ.</returns>
        public object Field(string nameField)
        {
            if (reader is null)
            {
                return null;
            }
            try
            {
                return reader[nameField];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Exécution d'une requête autre que "select".
        /// </summary>
        /// <param name="stringQuery">requête autre que select.</param>
        /// <param name="parameters">dictionnire contenant les parametres.</param>
        public void ReqUpdate(string stringQuery, Dictionary<string, object> parameters)
        {
            MySqlCommand command;
            try
            {
                command = new MySqlCommand(stringQuery, connection);
                if (!(parameters is null))
                {
                    Log.Debug("BddMySql.ReqUpdate stringQuery={0}", stringQuery);
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        Log.Debug("BddMySql.ReqUpdate parameter.Key={0} parameter.value={1}", parameter.Key, parameter.Value);
                        command.Parameters.Add(new MySqlParameter(parameter.Key, parameter.Value));
                    }
                }
                command.Prepare();
                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {

                Console.WriteLine(e.Message);
                Log.Error("BddMySql.ReqUpdate catch stringQuery={0} erreur={1}", stringQuery, e.Message);
            }
            catch (InvalidOperationException e)
            {
                ErreurGraveBddNonAccessible(e, "");
            }
        }


        /// <summary>
        /// Fermeture du curseur.
        /// </summary>
        public void Close()
        {
            if (!(reader is null))
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Pas d'accès à la BDD : arrêt de l'application.
        /// </summary>
        /// <param name="e">The e<see cref="Exception"/>.</param>
        private void ErreurGraveBddNonAccessible(Exception e, String stringConnect)
        {
            Log.Fatal("BddMySql.BddMySql catch stringConnect={0} erreur={1}", stringConnect, e.Message);
            MessageBox.Show("Base de données non accessibles", "Erreur grave");
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }
    }
}
