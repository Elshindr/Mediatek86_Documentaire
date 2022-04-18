using Mediatek86.controleur;
using System;
using System.Windows.Forms;


namespace Mediatek86
{
    /// <summary>
    /// Classe d'entrée dans le programme
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#pragma warning disable S1848 // Objects should not be created to be dropped immediately without being used
            new Controle();
#pragma warning restore S1848 // Objects should not be created to be dropped immediately without being used

        }
    }
}
