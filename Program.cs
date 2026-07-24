using System;
using System.Collections.Generic;
using System.IO;
using Terminal.Gui; // Import knihoven pro systém, kolekce, soubory a rozhraní

namespace SpravceUkolu
{
    class Program
    {
        static void Main(string[] args)
        {
            // Inicializace grafického prostředí v terminálu
            Application.Init();

            // Názvy souborů pro ukládání dat
            string cestaKeSouboru = "ukoly.txt";
            string cestaKHistorii = "historie.txt";

            // Seznamy v paměti pro aktivní a splněné úkoly
            var seznamUkolu = new List<string>();
            var historieUkolu = new List<string>();

            // Načtení aktivních úkolů ze souboru, pokud soubor existuje
            if (File.Exists(cestaKeSouboru))
            {
                seznamUkolu.AddRange(File.ReadAllLines(cestaKeSouboru));
            }

            // Načtení splněných úkolů z historie
            if (File.Exists(cestaKHistorii))
            {
                historieUkolu.AddRange(File.ReadAllLines(cestaKHistorii));
            }

            // Funkce pro zápis seznamů do souborů
            Action ulozUkoly = () => File.WriteAllLines(cestaKeSouboru, seznamUkolu);
            Action ulozHistorii = () => File.WriteAllLines(cestaKHistorii, historieUkolu);

            // Nastavení černého pozadí pro hlavní prvky
            var cernePozadiSchema = new ColorScheme() 
            { 
                Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Black), 
                Focus = Terminal.Gui.Attribute.Make(Color.Black, Color.White), 
                HotNormal = Terminal.Gui.Attribute.Make(Color.White, Color.Black), 
                HotFocus = Terminal.Gui.Attribute.Make(Color.Black, Color.White) 
            }; 

            // Nastavení modrého pozadí pro zadávací řádek
            var modryRadekSchema = new ColorScheme() 
            { 
                Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Blue), 
                Focus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Blue), 
                HotNormal = Terminal.Gui.Attribute.Make(Color.White, Color.Blue), 
                HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Blue) 
            }; 

            // Horní menu lišta (otevírá se klávesou F10)
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_Historie", new MenuItem [] {
                    new MenuItem ("_Zobrazit splněné úkoly", "", () => {
                        // Vyskakovací dialog pro zobrazení historie
                        var dialogHistorie = new Dialog("Historie splněných úkolů", 70, 15) { ColorScheme = cernePozadiSchema };
                        
                        // Komponenta pro seznam splněných úkolů
                        var seznamView = new ListView(historieUkolu) { X = 1, Y = 1, Width = Dim.Fill() - 2, Height = Dim.Fill() - 3, ColorScheme = cernePozadiSchema };
                        
                        // Tlačítko pro zavření okna historie
                        var btnZavrit = new Button("Zavřít") { X = Pos.Center(), Y = Pos.AnchorEnd(1), ColorScheme = cernePozadiSchema };
                        btnZavrit.Clicked += () => Application.RequestStop(); // Zavře dialog
                        
                        dialogHistorie.Add(seznamView, btnZavrit);
                        Application.Run(dialogHistorie); // Spustí dialog
                    })
                })
            }) { ColorScheme = cernePozadiSchema };

            // Hlavní kontejner aplikace pod menu
            var hlavniPohled = new View() 
            { 
                X = 0, Y = 1, 
                Width = Dim.Fill(), Height = Dim.Fill() - 1, 
                ColorScheme = cernePozadiSchema 
            }; 

            // Modrý pruh pro zadávání úkolu
            var psaciRadek = new View()
            {
                X = 0, Y = 0,
                Width = Dim.Fill(), Height = 1,
                ColorScheme = modryRadekSchema
            };

            // Vstupní pole a textové popisy
            var popisekUkol = new Label("Úkol:") { X = 1, Y = 0, ColorScheme = modryRadekSchema }; 
            var vstupUkol = new TextField("") { X = 7, Y = 0, Width = 25, ColorScheme = modryRadekSchema }; 

            var popisekKdo = new Label("Kdo:") { X = Pos.Right(vstupUkol) + 2, Y = 0, ColorScheme = modryRadekSchema }; 
            var vstupKdo = new TextField("") { X = Pos.Right(popisekKdo) + 1, Y = 0, Width = 15, ColorScheme = modryRadekSchema }; 

            var popisekKdy = new Label("Do kdy:") { X = Pos.Right(vstupKdo) + 2, Y = 0, ColorScheme = modryRadekSchema }; 
            var vstupKdy = new TextField("") { X = Pos.Right(popisekKdy) + 1, Y = 0, Width = 15, ColorScheme = modryRadekSchema }; 

            // Tlačítko pro uložení nového úkolu
            var tlacitkoPridat = new Button("Přidat") { X = Pos.Right(vstupKdy) + 2, Y = 0, ColorScheme = modryRadekSchema }; 

            // Vložení prvků do modrého řádku
            psaciRadek.Add(popisekUkol, vstupUkol, popisekKdo, vstupKdo, popisekKdy, vstupKdy, tlacitkoPridat);

            // Container pro zobrazení seznamu aktivních úkolů
            var kontejnerUkoly = new View() 
            { 
                X = 1, Y = 2, 
                Width = Dim.Fill() - 2, Height = Dim.Fill() - 2, 
                ColorScheme = cernePozadiSchema 
            }; 

            // Tlačítko pro ukončení celého programu
            var tlacitkoKonec = new Button("Ukončit") 
            { 
                X = Pos.AnchorEnd(11), Y = Pos.AnchorEnd(1), 
                ColorScheme = cernePozadiSchema 
            }; 

            // Popisek s nápovědou a počítadlem dole
            var napoveda = new Label("") 
            { 
                X = 1, Y = Pos.AnchorEnd(1), 
                ColorScheme = cernePozadiSchema 
            }; 

            // Funkce pro aktualizaci spodního textu a počtu splněných úkolů
            Action aktualizujNapovedu = () => {
                napoveda.Text = $"[F10] Menu | [Tab] Přepínání | [Enter] Upravit | Splněno: {historieUkolu.Count}";
            };

            // Funkce pro prekreslení seznamu úkolů na obrazovce
            Action prekresliUkoly = null!; 

            prekresliUkoly = () => 
            { 
                kontejnerUkoly.RemoveAll(); // Smaže staré vykreslené prvky
                aktualizujNapovedu(); // Obnoví počítadlo

                // Projde všechny úkoly a vytvoří pro ně řádek
                for (int i = 0; i < seznamUkolu.Count; i++) 
                { 
                    int aktualniIndex = i; 
                    string textUkolu = seznamUkolu[i]; 

                    // Vytvoření řádku úkolu
                    var polozka = new Label($" - {textUkolu}") 
                    { 
                        X = 0, Y = i, 
                        Width = Dim.Fill(), 
                        ColorScheme = cernePozadiSchema, 
                        CanFocus = true // Umožní vybrání klávesnicí
                    }; 

                    // Funkce pro otevření možností úkolu
                    Action akcePolozky = () => 
                    { 
                        // Okno s možnostmi
                        var dialog = new Dialog("Možnosti úkolu", 65, 8) { ColorScheme = cernePozadiSchema }; 
                        var labelInfo = new Label("Upravit záznam:") { X = 2, Y = 1, ColorScheme = cernePozadiSchema }; 
                        var editPole = new TextField(textUkolu) { X = 2, Y = 2, Width = Dim.Fill() - 4, ColorScheme = modryRadekSchema }; 

                        // Tlačítko pro přesun úkolu do splněných
                        var btnSplneno = new Button("✓ Splněno") { X = 2, Y = 4, ColorScheme = cernePozadiSchema };
                        btnSplneno.Clicked += () =>
                        {
                            // Uložení do historie s časem splnění
                            historieUkolu.Add($"{textUkolu} (Splněno: {DateTime.Now:dd.MM.yyyy HH:mm})");
                            seznamUkolu.RemoveAt(aktualniIndex); // Odstranění z aktivních
                            
                            ulozUkoly(); // Zápis na disk
                            ulozHistorii();
                            prekresliUkoly(); // Obnovení obrazovky
                            Application.RequestStop(); // Zavření dialogu
                        };

                        // Tlačítko pro uložení úprav
                        var btnUpravit = new Button("Uložit") { X = Pos.Right(btnSplneno) + 2, Y = 4, ColorScheme = cernePozadiSchema }; 
                        btnUpravit.Clicked += () => 
                        { 
                            string novyText = (editPole.Text?.ToString() ?? "").Trim(); 
                            if (!string.IsNullOrEmpty(novyText)) 
                            { 
                                seznamUkolu[aktualniIndex] = novyText; 
                                ulozUkoly(); 
                                prekresliUkoly(); 
                            } 
                            Application.RequestStop(); 
                        }; 

                        // Tlačítko pro smazání úkolu bez splnění
                        var btnSmazat = new Button("Smazat") { X = Pos.Right(btnUpravit) + 2, Y = 4, ColorScheme = cernePozadiSchema }; 
                        btnSmazat.Clicked += () => 
                        { 
                            seznamUkolu.RemoveAt(aktualniIndex); 
                            ulozUkoly(); 
                            prekresliUkoly(); 
                            Application.RequestStop(); 
                        }; 

                        // Tlačítko Storno
                        var btnZrusit = new Button("Zrušit") { X = Pos.Right(btnSmazat) + 2, Y = 4, ColorScheme = cernePozadiSchema }; 
                        btnZrusit.Clicked += () => Application.RequestStop(); 

                        dialog.Add(labelInfo, editPole, btnSplneno, btnUpravit, btnSmazat, btnZrusit); 
                        Application.Run(dialog); 
                    }; 

                    // Nastavení reakce na myš i Enter
                    polozka.MouseClick += (m) => akcePolozky(); 
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

            // Funkce pro přidání nového úkolu ze zadaných textů
            Action pridatUkolAkce = () => 
            { 
                string textCo = (vstupUkol.Text?.ToString() ?? "").Trim(); 
                string textKdo = (vstupKdo.Text?.ToString() ?? "").Trim(); 
                string textKdy = (vstupKdy.Text?.ToString() ?? "").Trim(); 

                if (!string.IsNullOrEmpty(textCo)) 
                { 
                    // Složení řetězce
                    string kompletniUkol = textCo; 
                    if (!string.IsNullOrEmpty(textKdo)) kompletniUkol += $" | Řeší: {textKdo}"; 
                    if (!string.IsNullOrEmpty(textKdy)) kompletniUkol += $" | Do: {textKdy}"; 

                    seznamUkolu.Add(kompletniUkol); // Přidání do seznamu
                    
                    // Vyčištění textových polí
                    vstupUkol.Text = ""; 
                    vstupKdo.Text = ""; 
                    vstupKdy.Text = ""; 

                    ulozUkoly(); // Zápis do souboru
                    prekresliUkoly(); // Aktualizace zobrazení
                } 
                
                vstupUkol.SetFocus(); // Vracení kurzoru na první pole
            }; 

            // Kliknutí na tlačítko Přidat
            tlacitkoPridat.Clicked += () => pridatUkolAkce(); 

            // Přechod klávesou Enter z 1. pole do 2. pole
            vstupUkol.KeyDown += (args) => 
            { 
                if (args.KeyEvent.Key == Key.Enter) 
                { 
                    vstupKdo.SetFocus(); 
                    args.Handled = true; 
                } 
            }; 

            // Přechod klávesou Enter z 2. pole do 3. pole
            vstupKdo.KeyDown += (args) => 
            { 
                if (args.KeyEvent.Key == Key.Enter) 
                { 
                    vstupKdy.SetFocus(); 
                    args.Handled = true; 
                } 
            }; 

            // Potvrzení a uložení z 3. pole stiskem Enter
            vstupKdy.KeyDown += (args) => 
            { 
                if (args.KeyEvent.Key == Key.Enter) 
                { 
                    pridatUkolAkce(); 
                    args.Handled = true; 
                } 
            }; 

            // Akce ukončení po kliknutí na tlačítko Ukončit
            tlacitkoKonec.Clicked += () => Application.RequestStop(); 

            // Složení prvků do hlavního pohledu
            hlavniPohled.Add(
                psaciRadek, 
                kontejnerUkoly, 
                tlacitkoKonec, 
                napoveda
            ); 

            // Přidání menu a pohledu do aplikace
            Application.Top.Add(menu, hlavniPohled); 

            // První spuštění vykreslení a nastavení kurzoru
            prekresliUkoly(); 
            vstupUkol.SetFocus(); 

            // Spuštění grafického cyklu a následný úklid
            Application.Run(); 
            Application.Shutdown(); 
        } 
    } 
}