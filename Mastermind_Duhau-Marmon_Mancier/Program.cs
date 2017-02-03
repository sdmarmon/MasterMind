using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; // Nécessaire pour la manipulation de fichiers

namespace Mastermind_Duhau_Marmon_Mancier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(138, 35); // Réglage de la taille de la console adaptée à la plus grande grille de jeu possible

            menu();

            // Tableaux contenant l'ensemble des couleurs/lettres/figures proposées
            string[] elementsCouleurs = new string[11];
            char[] elementsLettres = new char[26];
            string[] elementsFigures = new string[14];

            int[,] niveaux = new int[3, 3]; // Tableau contenant les 3 paramètres des 3 niveaux de difficulté
            chargerParametres(elementsCouleurs, elementsLettres, elementsFigures, niveaux);

            int nbLignes = -1; // Nombre de lignes de la grille de jeu = nombre de tours
            int nbColonnes = -1; // Nombre de colonnes = taille de la combinaison
            int nbElements = -1; // Nombre d'éléments pouvant figurer dans la combinaison
            string pseudonyme = "";
            int difficulteNiveau = -1;
            int thematique = -1;

            choixPseudonyme(ref pseudonyme);
            choixNiveau(ref difficulteNiveau, ref nbLignes, ref nbColonnes, ref nbElements, niveaux);
            choixThematique(ref thematique);

            // Définition des éléments retenus dans la partie
            string[] elements = new string[nbElements];
            defElements(thematique, elements, elementsCouleurs, elementsLettres, elementsFigures);
            int maxCharElements = recupMaxCharElements(elements);

            do
            {
                // Définition de la combinaison secrète
                string[] combinaison = new string[nbColonnes];
                defCombinaison(combinaison, elements);

                affichageValeurs(elements);

                string[,] grille = new string[nbLignes, nbColonnes * 2];// Définition de la grille de jeu
            
                int tour = 0;
                do // Début de la partie
                {
                    jouer(grille, elements, tour, maxCharElements, combinaison);
                    tour++;
                }
                while (gagner(grille, tour, combinaison) == false && tour < nbLignes);

                affichageScore(tour, nbLignes, combinaison);
                enregistrerScore(tour, pseudonyme, difficulteNiveau, thematique);
                afficherMeilleursScores(thematique, difficulteNiveau);
            }
            while (rejouer());
        }

        /*
        Nom : choixPseudonyme
        Description : Demande et stocke le pseudonyme du joueur
        Paramètres d'entrée-sortie :
            in out string pseudonyme
         */
        public static void choixPseudonyme(ref string pseudonyme)
        {
            do
            {
                if (pseudonyme.Length > 15)
                {
                    Console.WriteLine("!!! Votre pseudonyme doit contenir au plus 15 caractères !!!\n");
                }
                Console.WriteLine("Choisissez un pseudonyme\n");
                pseudonyme = Console.ReadLine();
                Console.Clear();
            }
            while ((pseudonyme == "")&&(pseudonyme.Length>15));
        }

        /*
        Nom : choixNiveau
        Description : Choix et affectation des paramètres du niveau
        Paramètres d'entrée-sortie :
            in out int difficulteNiveau
            in out int nbLignes
            in out int nbColonnes
            in out int nbElements
            in int[3][3] niveaux
         */
        public static void choixNiveau(ref int difficulteNiveau, ref int nbLignes, ref int nbColonnes, ref int nbElements, int[,] niveaux)
        {
            // Choix du niveau
            string verification;
            do
            {
                Console.WriteLine("Choisissez votre niveau de difficulté\n");
                Console.WriteLine("Tapez 0 pour le niveau facile : combinaisons de 4 valeurs parmi 8 éléments, 15 tours");
                Console.WriteLine("Tapez 1 pour le niveau normal : combinaisons de 4 valeurs parmi 10 éléments, 12 tours");
                Console.WriteLine("Tapez 2 pour le niveau expert : combinaisons de 6 valeurs parmi 10 éléments, 10 tours\n");
                verification = Console.ReadLine();
                Console.Clear();
            }
            while ((verification != "0") && (verification != "1") && (verification != "2"));

            // Affectation des paramètres du niveau
            difficulteNiveau = int.Parse(verification);
            nbColonnes = niveaux[difficulteNiveau, 0];
            nbLignes = niveaux[difficulteNiveau, 1];
            nbElements = niveaux[difficulteNiveau, 2];
        }

        /*
        Nom : choixThematique
        Description : Choix de la thématique
        Paramètres d'entrée-sortie :
            in out int thematique
         */
        public static void choixThematique(ref int thematique)
        {
            string verification;
            do
            {
                Console.WriteLine("Choisissez votre thématique\n");
                Console.WriteLine("Tapez 0 pour les couleurs");
                Console.WriteLine("Tapez 1 pour les lettres");
                Console.WriteLine("Tapez 2 pour les formes\n");
                verification = Console.ReadLine();
                Console.Clear();
            }
            while ((verification != "0") && (verification != "1") && (verification != "2"));
            thematique = int.Parse(verification);
        }

        /*
        Nom : defElements
        Description : Définition des éléments pouvant former la combinaison
        Paramètres d'entrée-sortie :
            in int thematique
            in string[nbElements] elements
            in string[11] elementsCouleurs
            in string[26] elementsLetrtes
            in string[14] elemetsFigures
         */
        public static void defElements(int thematique, string[] elements,
            string[] elementsCouleurs, char[] elementsLettres, string[] elementsFigures)
        {
            Random rnd = new Random();
            int i = 0;
            if (thematique == 0)
            {
                do
                {
                    bool b = true;
                    int r = rnd.Next(0, elementsCouleurs.Length);
                    for (int j = 0; j < i; j++)
                    {
                        if (elements[j] == elementsCouleurs[r])
                        {
                            b = false;
                        }
                    }
                    if (b)
                    {
                        elements[i] = elementsCouleurs[r];
                        i++;
                    }
                }
                while (i < elements.Length);
            }
            else
            {
                if (thematique == 1)
                {
                    do
                    {
                        bool b = true;
                        int r = rnd.Next(0, elementsLettres.Length);
                        for (int j = 0; j < i; j++)
                        {
                            if (elements[j] == char.ToString(elementsLettres[r]))
                            {
                                b = false;
                            }
                        }
                        if (b)
                        {
                            elements[i] = char.ToString(elementsLettres[r]);
                            i++;
                        }
                    }
                    while (i < elements.Length);
                }
                else
                {
                    if (thematique == 2)
                    {
                        do
                        {
                            bool b = true;
                            int r = rnd.Next(0, elementsFigures.Length);
                            for (int j = 0; j < i; j++)
                            {
                                if (elements[j] == elementsFigures[r])
                                {
                                    b = false;
                                }
                            }
                            if (b)
                            {
                                elements[i] = elementsFigures[r];
                                i++;
                            }
                        }
                        while (i < elements.Length);
                    }
                }
            }
        }

        /*
        Nom : defCombinaison
        Description : Définition de la combinaison secrète
        Paramètres d'entrée-sortie :
            in string[nbColonnes] combinaison
            in string[nbElements] elements
         */
        public static void defCombinaison(string[] combinaison, string[] elements)
        {
            Random rnd = new Random();
            for (int i = 0; i < combinaison.Length; i++)
                combinaison[i] = elements[rnd.Next(0, elements.Length)];
        }

        /*
        Nom : chargerParametres
        Description : Lecture des paramètres de jeu et remplissage des tableaux d'éléments
        Paramètres d'entrée-sortie :
            in string[11] elementsCouleurs
            in string[26] elementsLettres
            in string[14] elementsFigures
            in int[3][3] niveaux
         */
        public static void chargerParametres(string[] elementsCouleurs, char[] elementsLettres,
            string[] elementsFigures, int[,] niveaux)
        {
            string fichier = "../../mastermind_param.txt";

            try
            {
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader monStreamReader = new StreamReader(fichier, encoding);

                string mot = monStreamReader.ReadLine();

                // Lecture ligne par ligne 
                while (mot != null)
                {
                    if (mot.Contains("Couleur"))
                    {
                        mot = mot.Remove(0, 8);
                        string[] mots = mot.Split(';');
                        for (int i = 0; i < elementsCouleurs.Length; i++)
                        {
                            elementsCouleurs[i] = mots[i];
                        }
                    }

                    if (mot.Contains("Lettre"))
                    {
                        mot = mot.Remove(0, 7);
                        mot = mot.ToUpper();
                        mot = mot.Remove(mot.IndexOf(" "), 1);
                        string[] lettres = mot.Split(';');
                        for (int i = 0; i < elementsLettres.Length; i++)
                        {
                            elementsLettres[i] = char.Parse(lettres[i]);
                        }
                    }

                    if (mot.Contains("Figure"))
                    {
                        mot = mot.Remove(0, 7);
                        string[] mots = mot.Split(';');
                        for (int i = 0; i < elementsFigures.Length; i++)
                        {
                            elementsFigures[i] = mots[i];
                        }
                    }

                    if (mot.Contains("Niveaux"))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            mot = monStreamReader.ReadLine();
                            mot = mot.Substring(mot.IndexOf(":") + 1);
                            string[] mots = mot.Split(';');

                            for (int j = 0; j < 3; j++)
                            {
                                niveaux[i, j] = int.Parse(mots[j]);
                            }
                        }
                    }
                    mot = monStreamReader.ReadLine();
                }
                monStreamReader.Close();// Fermeture du StreamReader
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de la lecture des paramètres:");
                Console.WriteLine(ex.Message);
            }
        }

        /*
        Nom : recupMaxCharElements
        Description : Récupère la taille maximale des éléments pouvant former la combinaison
        Paramètres d'entrée-sortie :
            in string[nbElements] elements
            out int max
         */
        public static int recupMaxCharElements(string[] elements)
        {
            int max = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].Length > max)
                {
                    max = elements[i].Length;
                }
            }
            return max;
        }

        /*
        Nom : affichageGrille
        Description : Affiche la grille en Console
        Paramètres d'entrée-sortie :
            in string[nbLignes, 2*nbColonnes] grille
            in int maxCharElements
         */
        public static void affichageGrille(string[,] grille, int maxCharElements)
        {
            int nbLignes = grille.GetLength(0);
            int nbColonnes = (grille.GetLength(1)) / 2;

            afficherSeparation(nbColonnes, maxCharElements);// Trace une ligne séparatrice
            // Parcourt le tableau de bas en haut
            for (int i = nbLignes - 1; i > -1; i--)
            {
                Console.Write("|");
                string c = "";
                for (int j = 0; j < nbColonnes * 2; j++)
                {
                    c = "";
                    if (j == nbColonnes)// Séparation entre la partie droite et gauche de la grille
                    {
                        c += "|";
                    }
                    c += " ";
                    Console.Write(c);

                    if (j >= nbColonnes)// Partie droite de la grille réservée à la vérification
                    {
                        // Affiche le pion rouge en rouge
                        if (grille[i, j] == "O")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(grille[i, j]);
                        }
                        // Affiche le pion blanc en blanc
                        else if (grille[i, j] == "0")
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(grille[i, j]);
                        }
                        else
                        {
                            Console.Write(" ");// Ajoute un espace à la ligne
                        }

                        Console.ResetColor();// Restaure la couleur par défaut
                    }
                    else // Partie gauche de la grille réservée aux réponses du joueur
                    {
                        Console.Write(grille[i, j]);
                    }

                    // Ajustement du nombre de caractères de la case
                    c = "";
                    if (j < nbColonnes)// Partie gauche de la grille réservée aux réponses du joueur
                    {
                        int nbChar = 0;
                        if (grille[i, j] != null)
                        {
                            nbChar = grille[i, j].Length;// Nombre de caractères de l'élément de la grille
                        }
                        for (int k = 0; k < maxCharElements - nbChar; k++)
                        {
                            c += " ";
                        }
                    }
                    c += " ";
                    Console.Write(c + "|");// Sépartion entre colonnes
                }
                Console.Write("\n");// Change de ligne
                afficherSeparation(nbColonnes, maxCharElements);// Trace une ligne séparatrice
            }

        }

        /*
        Nom : afficherSeparation
        Description : Trace une ligne séparatrice entre 2 lignes de la grille
        Paramètres d'entrée-sortie :
            in int nbColonnes
            in int maxCharElements
         */
        public static void afficherSeparation(int nbColonnes, int maxCharElements)
        {
            string separation = "";
            for (int i = 0; i < nbColonnes * 2; i++)
            {
                if (i == nbColonnes)// Séparation entre la partie droite et gauche de la grille
                {
                    separation += "+";
                }
                separation += "+-";// Coin d'une case
                // Partie gauche de la grille réservée aux réponses du joueur
                if (i < nbColonnes)
                {
                    for (int j = 0; j < maxCharElements; j++)// Compte le nombre tirets
                    {
                        separation += "-";
                    }
                }
                else // Partie droite de la grille réservée à la vérification
                {
                    separation += "-";
                }
                separation += "-";
            }
            separation += "+";
            Console.WriteLine(separation);// Afffiche la ligne
        }

        /*
        Nom : jouer
        Description : fonction principale de jeu
        Paramètres d'entrée-sortie :
            in string[nbLignes, 2*nbColonnes] grille
            in string[nbColonnes] elements
            in int tour
            in int maxCharElements
            in string[nbColonnes] combinaison
         */
        public static void jouer(string[,] grille, string[] elements, int tour, int maxCharElements, string[] combinaison)
        {
            int nbLignes = grille.GetLength(0);
            int nbColonnes = (grille.GetLength(1)) / 2;

            for (int i = 0; i < nbColonnes; i++)
            {
                affichageGrille(grille, maxCharElements);
                Console.WriteLine("\nTour " + (tour + 1));
                Console.WriteLine("Rentrez la valeur de la case " + (i + 1));
                Console.WriteLine("\nRappel de la liste de valeurs :");
                // Liste des valeurs possibles
                for (int j = 0; j < elements.Length; j++)
                {
                    Console.Write((j + 1) + ":"); 
                    Console.Write(elements[j] + " ");
                }
                Console.Write("\n\n");

                int verification = 0;
                string entree ;
                bool estNombre = false;
                int elem = -1;
                bool correct = false; 
                do
                {
                    entree = Console.ReadLine();
                    estNombre = int.TryParse(entree, out verification);
                    if (estNombre)
                    {
                        elem = int.Parse(entree);
                    }

                    if (0 < elem && elem <= elements.Length)// La réponse est une valeur correcte
                    {
                        correct = true;
                    }
                    
                    if (correct == false)
                    {
                        Console.WriteLine("!!! La valeur rentrée n'est pas correcte, réessayez !!!");
                    }
                }
                while (!correct);

                // Remplissage de la grille
                grille[tour, i] = elements[elem-1];

                Console.Clear();
            }

            analyse(grille, tour, combinaison); // Analyse de la combinaison
        }

        /*
        Nom : gagner
        Description : Vérifie si la réponse du joueur est identique à la combinaison de départ
        Paramètres d'entrée-sortie :
            in string[nbLignes, 2*nbColonnes] grille
            in int tour
            in string[nbColonnes] combinaison
            out bool
         */
        public static bool gagner(string[,] grille, int tour, string[] combinaison)
        {
            int colonnes = (grille.GetLength(1)) / 2;
            for (int i = 0; i < colonnes; i++)
            {
                if (grille[tour - 1, i] != combinaison[i])
                {
                    return false;
                }
            }
            return true;
        }

        /*
        Nom : analyse
        Description : Analyse la réponse du joueur et ajoute des pions de couleur en conséquence
        Paramètres d'entrée-sortie :
            in string[nbLignes, 2*nbColonnes] grille
            in int tour
            in string[nbColonnes] combinaison
         */
        public static void analyse(string[,] grille, int tour, string[] combinaison)
        {
            int colonnes = (grille.GetLength(1)) / 2;
            string[] copiegrille = new string[colonnes]; // Copie de la combinaison du tour
            for (int i = 0; i < colonnes; i++)
            {
                copiegrille[i] = grille[tour, i];
            }

            string[] copiecombinaison = new string[colonnes]; // Copie de la combinaison secrète
            for (int i = 0; i < colonnes; i++)
            {
                copiecombinaison[i] = combinaison[i];
            }

            // Gestion des pions rouges

            int k = 0; // Compteur de pions
            for (int i = 0; i < colonnes; i++)
            {
                if (grille[tour, i] == combinaison[i])
                {
                    grille[tour, k + colonnes] = "O";
                    copiegrille[i] = "-1"; // De telle sorte qu'un pion rouge ne soit pas compté comme blanc 
                    copiecombinaison[i] = "-2"; // De telle sorte que l'élément associé dans la combinaison secrète ne génère pas un pion supplémentaire
                    k++;
                }
            }

            // Gestion des pions blancs

            for (int i = 0; i < colonnes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    if (copiegrille[i] == copiecombinaison[j])
                    {
                        grille[tour, colonnes + k] = "0";
                        copiecombinaison[j] = "-2";
                        j = colonnes;
                        k++;
                    }
                }
            }
        }

        /*
        Nom : affichageScore
        Description : Affiche le message de fin du jeu
        Paramètres d'entrée-sortie :
            in int tour
            in int nbLignes
            in string[nbColonnes] combinaison
         */
        public static void affichageScore(int tour, int nbLignes, string[] combinaison)
        {
            if (tour == nbLignes) // Défaite
            {
                Console.WriteLine("Dommage ! Vous avez atteint le nombre de tours maximum ...");
                Console.WriteLine("La combinaison était :");
                // Affichage de la combinaison de départ
                foreach (string tab in combinaison)
                {
                    Console.Write(tab + "  ");
                }
            }
            else // Victoire
            {
                if (tour == 1)
                {
                    Console.WriteLine("Mastermind.");
                }
                else
                {
                    Console.WriteLine("Félicitations ! Vous avez gagné en " + tour + " tours.");
                }
            }
        }

        /*
        Nom : affichageValeurs
        Description : Affiche les valeurs du tableau elements dans la console
        Paramètres d'entrée-sortie :
            in string[nbElements] elements
         */
        public static void affichageValeurs(string[] elements)
        {
            Console.WriteLine("Voici les valeurs que peuvent prendre les éléments de la combinaison :\n");
            for (int i = 0; i < elements.Length; i++ )
            {
                Console.WriteLine((i + 1) + ":" + elements[i]);
            }
            Console.WriteLine("\nAppuyez sur entrée pour continuer ...");
            Console.ReadLine();
            Console.Clear();
        }

        /*
        Nom : enregistrerScore
        Description : Enregistre le score dans un fichier texte s'il s'agit d'un record
        Paramètres d'entrée-sortie :
            in int tour
            int string pseudonyme
            in int difficulteNIveau
            in int thematique
        */
        public static void enregistrerScore(int score, string pseudonyme, int difficulteNiveau, int thematique)
        {
            string fichier = "../../scores_" + thematique + "_" + difficulteNiveau + ".txt";

            try
            {
                // Lecture des scores précédents
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader monStreamReader = new StreamReader(fichier, encoding);

                int i = 0;
                string ligne = monStreamReader.ReadLine();
                while (ligne != null)// Lecture ligne par ligne
                {
                    i++; // Compteur de lignes
                    ligne = monStreamReader.ReadLine();
                }
                monStreamReader.Close();
                int[] tableauScores = new int[i];
                string[] tableauPseudonymes = new string[i];

                if (i == 0) // Fichier vierge
                {
                    Console.WriteLine("Nouveau record !!");
                    StreamWriter monStreamWriter = File.CreateText(fichier);
                    monStreamWriter.WriteLine(pseudonyme + ";" + score); // Ecrit dans le fichier
                    monStreamWriter.Close();
                }
                else
                {
                    StreamReader monStreamReader2 = new StreamReader(fichier, encoding);
                    ligne = monStreamReader2.ReadLine();
                    i = 0;
                    while (ligne != null)
                    {
                        string[] meilleurScore = ligne.Split(';');

                        // Remplissage des tableaux
                        tableauPseudonymes[i] = meilleurScore[0];
                        tableauScores[i] = int.Parse(meilleurScore[1]);

                        i++;
                        ligne = monStreamReader2.ReadLine();
                    }
                    monStreamReader2.Close();

                    //Ecriture des scores
                    bool b = true; // Vrai si nouveau record
                    bool b2 = true; // Vrai si le pseudonyme n'est pas référencé

                    for (int j = 0; j < i; j++)
                    {
                        if ((pseudonyme == tableauPseudonymes[j]) && (score >= tableauScores[j])) // Score = tours, donc un score minimum est optimal
                        {
                            b = false;
                            Console.WriteLine("Votre meilleur score est de " + tableauScores[j] + " tour(s).");
                        }
                    }

                    if (b)
                    {
                        Console.WriteLine("Nouveau record !!");

                        string dernierpseudonyme = null;
                        int dernierscore = -1;

                        for (int x = 0; x < tableauPseudonymes.Length; x++)
                        {
                            if (pseudonyme == tableauPseudonymes[x]) // Psuedonyme déjà référencé
                            {
                                b2 = false;

                                while ((tableauScores[x] < tableauScores[x - 1] && (x >= 1))) // Permutations successives
                                {
                                    tableauPseudonymes[x] = tableauPseudonymes[x - 1];
                                    tableauScores[x] = tableauScores[x - 1];
                                    x--;
                                }
                                // Insertion du nouveau record
                                tableauPseudonymes[x] = pseudonyme;
                                tableauScores[x] = score;

                                StreamWriter monStreamWriter = File.CreateText(fichier);
                                for (int y = 0; y < tableauPseudonymes.Length; y++)
                                {
                                    monStreamWriter.WriteLine(tableauPseudonymes[y] + ";" + tableauScores[y]);
                                }
                                monStreamWriter.Close();
                                x = tableauPseudonymes.Length;
                            }
                        }

                        if (b2) // Pseudonyme non-référencé : il faut écrire une nouvelle ligne dans le fichier
                        {
                            if (score >= tableauScores[tableauScores.Length - 1]) // En dernière position
                            {
                                StreamWriter monStreamWriter = File.CreateText(fichier);
                                for (int l = 0; l < tableauPseudonymes.Length; l++)
                                {
                                    monStreamWriter.WriteLine(tableauPseudonymes[l] + ";" + tableauScores[l]);
                                }
                                monStreamWriter.WriteLine(pseudonyme + ";" + score);
                                monStreamWriter.Close();
                            }
                            else
                            {
                                // Sauvegarde du dernier record
                                dernierpseudonyme = tableauPseudonymes[(tableauPseudonymes.Length) - 1]; 
                                dernierscore = tableauScores[(tableauScores.Length) - 1];

                                int k = (tableauScores.Length) - 1;
                                while ((score < tableauScores[k] && (k >= 1)))
                                {
                                    tableauPseudonymes[k] = tableauPseudonymes[k - 1];
                                    tableauScores[k] = tableauScores[k - 1];
                                    k--;
                                }
                                tableauPseudonymes[k] = pseudonyme;
                                tableauScores[k] = score;

                                StreamWriter monStreamWriter = File.CreateText(fichier);
                                for (int l = 0; l < tableauPseudonymes.Length; l++)
                                {
                                    monStreamWriter.WriteLine(tableauPseudonymes[l] + ";" + tableauScores[l]);
                                }
                                monStreamWriter.WriteLine(dernierpseudonyme + ";" + dernierscore);
                                monStreamWriter.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.WriteLine("!!! Une erreur est survenue lors de l'enregistement du score !!!");
                Console.WriteLine(ex.Message);
            }
        }

        /*
        Nom : afficherMeilleursScores
        Description : Affiche les 10 meilleurs scores du niveau et de la thématique associés
        Paramètres d'entrée-sortie :
            in int thematique
            in int difficulteNIveau
        */
        public static void afficherMeilleursScores(int thematique, int difficulteNiveau)
        {
            string[,] correspondanceThematiqueNiveau = new string[3, 2] { { "couleurs", "facile" }, { "lettres", "normal" }, { "formes", "expert" } };
            Console.WriteLine("\n\nVoici les scores des 10 meilleurs joueurs pour la thématique des "
            + correspondanceThematiqueNiveau[thematique, 0] + " et le niveau de difficulté " + correspondanceThematiqueNiveau[difficulteNiveau, 1] + " :\n");
            string fichier = "../../scores_" + thematique + "_" + difficulteNiveau + ".txt";
            string[,] dixMeilleursJoueurs = new string[10, 2];

            //Lecture des scores précédents
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            StreamReader monStreamReader = new StreamReader(fichier, encoding);
            int k = 0;
            string ligne = monStreamReader.ReadLine();

            try
            {
                while ((ligne != null) && (k < 10))
                {
                    string[] meilleurScore = ligne.Split(';');

                    dixMeilleursJoueurs[k, 0] = meilleurScore[0];
                    dixMeilleursJoueurs[k, 1] = (meilleurScore[1]);

                    k++;
                    ligne = monStreamReader.ReadLine();
                }
                monStreamReader.Close();

                // Afficher un beau tableau
                int maxCharElements = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (dixMeilleursJoueurs[i, 0] !=null && dixMeilleursJoueurs[i, 0].Length > maxCharElements)
                    {
                        maxCharElements = dixMeilleursJoueurs[i, 0].Length;
                    }
                }

                separationSimple(maxCharElements);// Trace une ligne séparatrice

                for (int i = 0; i < 10; i++)
                {
                    Console.Write("|");
                    string c = "";
                    for (int j = 0; j < 2; j++)
                    {
                        Console.Write(" ");
                        Console.Write(dixMeilleursJoueurs[i, j]);

                        // Ajustement du nombre de caractères de la case
                        c = "";
                        int nbChar = 0;
                        if (dixMeilleursJoueurs[i, j] != null)
                        {
                            nbChar = dixMeilleursJoueurs[i, j].Length;// Nombre de caractères de l'élément
                        }
                        for (int l = 0; l < maxCharElements - nbChar; l++)
                        {
                            c += " ";
                        }
                        c += " ";
                        Console.Write(c + "|");// Sépartion entre colonnes
                    }
                    Console.Write("\n");// Change de ligne
                    separationSimple(maxCharElements);// Trace une ligne séparatrice

                }
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.WriteLine("!!! Une erreur est survenue lors de l'enregistement du score !!!");
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
            Console.Clear();
        }

        /*
       Nom : separationSimple
       Description : Trace une ligne séparatrice entre 2 lignes de la table des scores
       Paramètres d'entrée-sortie :
           in int nbColonnes
           in int maxCharElements
        */
        public static void separationSimple(int maxCharElements)
        {
            string separation = "";
            for (int i = 0; i < 2; i++)
            {
                separation += "+-";// Coin d'une case
                for (int j = 0; j < maxCharElements; j++)// Compte le nombre tirets
                {
                    separation += "-";
                }
                separation += "-";
            }
            separation += "+";
            Console.WriteLine(separation);// Afffiche la ligne
        }

        /*
       Nom : rejouer
       Description : Permet de rejouer avec le même thème, le même niveau de difficulté et les mêmes éléments
       Paramètres d'entrée-sortie :
           in int thematique
           in int difficulteNiveau
           in string[] elements
           in int maxCharElements
           in string pseudonyme
           in int nbLignes
           in int nbColonnes
        */
        public static bool rejouer()
        {
            string commande;
            Console.WriteLine("Rentrez rejouer pour rejouer avec le même thème, le même niveau de difficulté et les mêmes éléments");
            Console.WriteLine("Appuyez sur entrée pour quitter\n");
            commande = Console.ReadLine();

            if (commande == "rejouer")
            {
                Console.Clear();
                return (true);
            }
            else
            {
                return (false);
            }
        }

        /*
        Nom : menu
        Description : Affiche un menu permettant 3 actions (aide, ràz, jouer)
        Pas de paramètres d'entrée-sortie
        */
        public static void menu()
        {
            Console.Clear();
            string choix;
            Console.WriteLine("Bienvenue dans Master#mind\n");
            Console.WriteLine("Rentrez aide pour une introduction au mastermind et les règles du jeu");
            Console.WriteLine("Rentrez ràz pour une remise à zéro des meilleurs scores\n");
            Console.WriteLine("Appuyez sur entrée pour jouer\n");
            choix = Console.ReadLine();

            if (choix == "aide")
            {
                Console.Clear();
                aide();
                menu();
            }
            else
            {
                if (choix == "ràz")
                {
                    Console.Clear();
                    remiseAZeroScores();
                    menu();
                }
                else
                {
                    Console.Clear();
                }
            }
        }

        /*
        Nom : remiseAZeroScores
        Description : Efface l'intégralité des 9 fichiers .txt stockant les meilleurs scores
        Pas de paramètres d'entrée-sortie
        */
        public static void remiseAZeroScores()
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        string fichier = "../../scores_" + i + "_" + j + ".txt";
                        StreamWriter monStreamWriter = File.CreateText(fichier);
                        monStreamWriter.Close();
                    }
                }
                Console.WriteLine("Remise à zéro effectuée");
                Console.WriteLine("Appuyez sur entrée pour revenir au menu ...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.WriteLine("!!! Une erreur est survenue lors de la remise à zéro des meilleurs scores !!!");
                Console.WriteLine(ex.Message);
            }
        }

        /*
        Nom : aide
        Description : Affiche l'aide pour les nouevaux joueurs à la console
        Pas de paramètres d'entrée-sortie :
        */
        public static void aide()
        {
            string fichier = "../../aide.txt";
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            StreamReader monStreamReader = new StreamReader(fichier, encoding);
            string ligne = monStreamReader.ReadLine();
            int i = 0;
            string[] aide = new string[50];
            while (ligne != null)
            {
                string[] meilleurScore = ligne.Split(';');
                aide[i] = meilleurScore[0];
                i++;
                ligne = monStreamReader.ReadLine();
            }
            for (int j = 0; j<20 ; j++)
            {
                Console.WriteLine(aide[j]);
            }
            Console.ReadLine();
            Console.Clear();
            for (int j = 20; j < 50; j++)
            {
                Console.WriteLine(aide[j]);
            }
            Console.ReadLine();
        }
    }
}
