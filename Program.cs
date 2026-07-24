using Terminal.Gui;
using System;
using System.Collections.Generic;

Application.Init();

// 1. DATA (Seznam úkolů)
var seznamUkolu = new List<string> {};

// 2. BARVY
var mojeBarvy = new ColorScheme();
mojeBarvy.Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.White); // černá,bílá
mojeBarvy.Focus = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightGreen); //černá,zelená

// 3. VZHLED
var okno = new Window("To do app")
{
    X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(),
    ColorScheme = mojeBarvy
};

var popisek = new Label("Napiš nový úkol:") { X = 2, Y = 1 };

var vstupniPole = new TextField("") 
{ 
    X = 2, Y = 2, Width = Dim.Percent(50), 
    ColorScheme = mojeBarvy 
};

var tlacitkoPridat = new Label(" Přidat ") 
{ 
    X = Pos.Right(vstupniPole) + 2, Y = 2, 
    ColorScheme = mojeBarvy,
    CanFocus = true // Umožní na popisek najet tab
};

// kontejner ukoly
var kontejnerUkoly = new View()
{
    X = 2,
    Y = 5,
    Width = Dim.Percent(50),
    Height = Dim.Fill() - 2,
    ColorScheme = mojeBarvy
};

var tlacitkoKonec = new Button("Ukončit")
{
    Y = Pos.AnchorEnd(1),
    ColorScheme = mojeBarvy
};
tlacitkoKonec.X = Pos.AnchorEnd(11);

var napoveda = new Label(" [Tab] Přepínání prvků | Kliknutím na úkol ho upravíš / smažeš")
{
    X = 2,
    Y = Pos.AnchorEnd(1),
    ColorScheme = Colors.Menu
};

Action prekresliUkoly = null!;

prekresliUkoly = () => {
    kontejnerUkoly.RemoveAll();

    int radek = 0;
    for (int i = 0; i < seznamUkolu.Count; i++)
    {
        int aktualniIndex = i;
        string textUkolu = seznamUkolu[i];

        var polozka = new Label($" - {textUkolu}")
        {
            X = 0,
            Y = radek,
            ColorScheme = mojeBarvy,
            CanFocus = true 
        };

        Action akcePolozky = () => {
            var dialog = new Dialog("Úprava úkolu", 50, 10);
            var labelInfo = new Label($"Vybráno: {textUkolu}") { X = 2, Y = 1 };
            var editPole = new TextField(textUkolu) { X = 2, Y = 3, Width = Dim.Fill() - 4 };

            var btnUpravit = new Button("Uložit") { X = 2, Y = 5 };
            btnUpravit.Clicked += () => {
                string novyText = (editPole.Text?.ToString() ?? "").Trim();
                if (!string.IsNullOrEmpty(novyText))
                {
                    seznamUkolu[aktualniIndex] = novyText;
                    prekresliUkoly();
                }
                Application.RequestStop();
            };

            var btnSmazat = new Button("Smazat") { X = Pos.Right(btnUpravit) + 2, Y = 5 };
            btnSmazat.Clicked += () => {
                seznamUkolu.RemoveAt(aktualniIndex);
                prekresliUkoly();
                Application.RequestStop();
            };

            var btnZrusit = new Button("Zrušit") { X = Pos.Right(btnSmazat) + 2, Y = 5 };
            btnZrusit.Clicked += () => Application.RequestStop();

            dialog.Add(labelInfo, editPole, btnUpravit, btnSmazat, btnZrusit);
            Application.Run(dialog);
        };

        // Kliknuti radek s ukolem
        polozka.MouseClick += (m) => akcePolozky();
        
        // Stisknutí Enteru na radku
        polozka.KeyDown += (k) => {
            if (k.KeyEvent.Key == Key.Enter) {
                akcePolozky();
                k.Handled = true;
            }
        };

        kontejnerUkoly.Add(polozka);
        radek++; 
    }
};

Action pridatUkolAkce = () => {
    string novyUkol = (vstupniPole.Text?.ToString() ?? "").Trim(); 
    if (!string.IsNullOrEmpty(novyUkol)) 
    {
        seznamUkolu.Add(novyUkol);         
        vstupniPole.Text = "";             
        prekresliUkoly(); 
    }
    vstupniPole.SetFocus(); 
};

// Kliknutí myší na "Přidat"
tlacitkoPridat.MouseClick += (m) => pridatUkolAkce();

tlacitkoPridat.KeyDown += (k) => {
    if (k.KeyEvent.Key == Key.Enter) {
        pridatUkolAkce();
        k.Handled = true;
    }
};

vstupniPole.KeyDown += (args) => {
    if (args.KeyEvent.Key == Key.Enter)
    {
        pridatUkolAkce();
        args.Handled = true; 
    }
};

tlacitkoKonec.Clicked += () => {
    Application.RequestStop(); 
};

okno.Add(popisek, vstupniPole, tlacitkoPridat, kontejnerUkoly, tlacitkoKonec, napoveda);
Application.Top.Add(okno);

vstupniPole.SetFocus();

Application.Run();
Application.Shutdown();