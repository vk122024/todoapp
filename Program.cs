using System;
using System.Collections.Generic;
using System.IO;
using Terminal.Gui;

namespace SpravceUkolu
{
    class Program
    {
        static void Main(string[] args)
        {
            // Inicializace Terminal.Gui
            Application.Init();

            // Cesta k souboru a seznam v paměti
            string cestaKeSouboru = "ukoly.txt";
            var seznamUkolu = new List<string>();

            // Načtení uložených úkolů
            if (File.Exists(cestaKeSouboru))
            {
                seznamUkolu.AddRange(File.ReadAllLines(cestaKeSouboru));
            }

            // Metoda pro zápis do souboru
            Action uloz = () => File.WriteAllLines(cestaKeSouboru, seznamUkolu);

            // Černé barevné schéma
            var cernePozadiSchema = new ColorScheme() 
            { 
                Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Black), 
                Focus = Terminal.Gui.Attribute.Make(Color.Black, Color.White), 
                HotNormal = Terminal.Gui.Attribute.Make(Color.White, Color.Black), 
                HotFocus = Terminal.Gui.Attribute.Make(Color.Black, Color.White) 
            }; 

            // Modré barevné schéma
            var modryRadekSchema = new ColorScheme() 
            { 
                Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Blue), 
                Focus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Blue), 
                HotNormal = Terminal.Gui.Attribute.Make(Color.White, Color.Blue), 
                HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Blue) 
            }; 

            // Hlavní plocha bez okrajů a rámečků
            var hlavniPohled = new View("TaskRun") 
            { 
                X = 0, Y = 0, 
                Width = Dim.Fill(), Height = Dim.Fill(), 
                ColorScheme = cernePozadiSchema 
            }; 

            // Modře podbarvený řádek pro psaní
            var psaciRadek = new View()
            {
                X = 0, Y = 1,
                Width = Dim.Fill(),
                Height = 1,
                ColorScheme = modryRadekSchema
            };

            // Vstupní pole a popisky
            var popisekUkol = new Label("Úkol:") { X = 1, Y = 0, ColorScheme = modryRadekSchema }; 
            var vstupUkol = new TextField("") { X = 7, Y = 0, Width = 25, ColorScheme = modryRadekSchema }; 

            var popisekKdo = new Label("Kdo:") { X = Pos.Right(vstupUkol) + 2, Y = 0, ColorScheme = modryRadekSchema }; 
            var vstupKdo = new TextField("") { X = Pos.Right(popisekKdo) + 1, Y = 0, Width = 15, ColorScheme = modryRadekSchema }; 

            var popisekKdy = new Label("Do kdy:") { X = Pos.Right(vstupKdo) + 2, Y = 0, ColorScheme = modryRadekSchema }; 
            var vstupKdy = new TextField("") { X = Pos.Right(popisekKdy) + 1, Y = 0, Width = 15, ColorScheme = modryRadekSchema }; 

            // Tlačítko pro přidání
            var tlacitkoPridat = new Button("Přidat") { X = Pos.Right(vstupKdy) + 2, Y = 0, ColorScheme = modryRadekSchema }; 

            // Vložení prvků do psacího řádku
            psaciRadek.Add(popisekUkol, vstupUkol, popisekKdo, vstupKdo, popisekKdy, vstupKdy, tlacitkoPridat);

            // Container pro zobrazení úkolů
            var kontejnerUkoly = new View() 
            { 
                X = 1, Y = 3, 
                Width = Dim.Fill() - 2, Height = Dim.Fill() - 2, 
                ColorScheme = cernePozadiSchema 
            }; 

            // Tlačítko ukončení
            var tlacitkoKonec = new Button("Ukončit") 
            { 
                X = Pos.AnchorEnd(11), Y = Pos.AnchorEnd(1), 
                ColorScheme = cernePozadiSchema 
            }; 

            // Popisek nápovědy
            var napoveda = new Label("[Tab] Přepínání | [Enter] Přidat / Upravit úkol = klik") 
            { 
                X = 1, Y = Pos.AnchorEnd(1), 
                ColorScheme = cernePozadiSchema 
            }; 

            // Vykreslení seznamu úkolů
            Action prekresliUkoly = null!; 

            prekresliUkoly = () => 
            { 
                // Vyčištění starých položek
                kontejnerUkoly.RemoveAll(); 

                // Cyklus pro vykreslení každého úkolu
                for (int i = 0; i < seznamUkolu.Count; i++) 
                { 
                    int aktualniIndex = i; 
                    string textUkolu = seznamUkolu[i]; 

                    // Vytvoření textového řádku úkolu
                    var polozka = new Label($" - {textUkolu}") 
                    { 
                        X = 0, Y = i, 
                        Width = Dim.Fill(), 
                        ColorScheme = cernePozadiSchema, 
                        CanFocus = true 
                    }; 

                    // Dialog pro úpravu a smazání
                    Action akcePolozky = () => 
                    { 
                        var dialog = new Dialog("Úprava úkolu", 60, 8) { ColorScheme = cernePozadiSchema }; 
                        var labelInfo = new Label("Upravit záznam:") { X = 2, Y = 1, ColorScheme = cernePozadiSchema }; 
                        var editPole = new TextField(textUkolu) { X = 2, Y = 2, Width = Dim.Fill() - 4, ColorScheme = modryRadekSchema }; 

                        // Tlačítko pro uložení změn
                        var btnUpravit = new Button("Uložit") { X = 2, Y = 4, ColorScheme = cernePozadiSchema }; 
                        btnUpravit.Clicked += () => 
                        { 
                            string novyText = (editPole.Text?.ToString() ?? "").Trim(); 
                            if (!string.IsNullOrEmpty(novyText)) 
                            { 
                                seznamUkolu[aktualniIndex] = novyText; 
                                uloz(); 
                                prekresliUkoly(); 
                            } 
                            Application.RequestStop(); 
                        }; 

                        // Tlačítko pro smazání
                        var btnSmazat = new Button("Smazat") { X = Pos.Right(btnUpravit) + 2, Y = 4, ColorScheme = cernePozadiSchema }; 
                        btnSmazat.Clicked += () => 
                        { 
                            seznamUkolu.RemoveAt(aktualniIndex); 
                            uloz(); 
                            prekresliUkoly(); 
                            Application.RequestStop(); 
                        }; 

                        // Tlačítko pro zrušení
                        var btnZrusit = new Button("Zrušit") { X = Pos.Right(btnSmazat) + 2, Y = 4, ColorScheme = cernePozadiSchema }; 
                        btnZrusit.Clicked += () => Application.RequestStop(); 

                        // Zobrazení dialogu
                        dialog.Add(labelInfo, editPole, btnUpravit, btnSmazat, btnZrusit); 
                        Application.Run(dialog); 
                    }; 

                    // Reakce na kliknutí myší
                    polozka.MouseClick += (m) => akcePolozky(); 
                    
                    // Reakce na klávesu Enter
                    polozka.KeyDown += (k) => 
                    { 
                        if (k.KeyEvent.Key == Key.Enter) 
                        { 
                            akcePolozky(); 
                            k.Handled = true; 
                        } 
                    }; 

                    kontejnerUkoly.Add(polozka); 
                } 
            }; 

            // Logika pro přidání nového úkolu
            Action pridatUkolAkce = () => 
            { 
                string textCo = (vstupUkol.Text?.ToString() ?? "").Trim(); 
                string textKdo = (vstupKdo.Text?.ToString() ?? "").Trim(); 
                string textKdy = (vstupKdy.Text?.ToString() ?? "").Trim(); 

                // Složení textu a uložení
                if (!string.IsNullOrEmpty(textCo)) 
                { 
                    string kompletniUkol = textCo; 
                    if (!string.IsNullOrEmpty(textKdo)) kompletniUkol += $" | Řeší: {textKdo}"; 
                    if (!string.IsNullOrEmpty(textKdy)) kompletniUkol += $" | Do: {textKdy}"; 

                    seznamUkolu.Add(kompletniUkol); 
                    
                    // Pročištění políček
                    vstupUkol.Text = ""; 
                    vstupKdo.Text = ""; 
                    vstupKdy.Text = ""; 

                    uloz(); 
                    prekresliUkoly(); 
                } 
                
                vstupUkol.SetFocus(); 
            }; 

            // Kliknutí na Přidat
            tlacitkoPridat.Clicked += () => pridatUkolAkce(); 

            // Skok z 1. do 2. pole přes Enter
            vstupUkol.KeyDown += (args) => 
            { 
                if (args.KeyEvent.Key == Key.Enter) 
                { 
                    vstupKdo.SetFocus(); 
                    args.Handled = true; 
                } 
            }; 

            // Skok z 2. do 3. pole přes Enter
            vstupKdo.KeyDown += (args) => 
            { 
                if (args.KeyEvent.Key == Key.Enter) 
                { 
                    vstupKdy.SetFocus(); 
                    args.Handled = true; 
                } 
            }; 

            // Potvrzení a přidání z 3. pole přes Enter
            vstupKdy.KeyDown += (args) => 
            { 
                if (args.KeyEvent.Key == Key.Enter) 
                { 
                    pridatUkolAkce(); 
                    args.Handled = true; 
                } 
            }; 

            // Akce tlačítka Ukončit
            tlacitkoKonec.Clicked += () => Application.RequestStop(); 

            // Sestavení hlavní plochy
            hlavniPohled.Add(
                psaciRadek, 
                kontejnerUkoly, 
                tlacitkoKonec, 
                napoveda
            ); 

            // Přidání na obrazovku
            Application.Top.Add(hlavniPohled); 

            // První vykreslení a start
            prekresliUkoly(); 
            vstupUkol.SetFocus(); 

            Application.Run(); 
            Application.Shutdown(); 
        } 
    } 
}