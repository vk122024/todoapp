using Terminal.Gui;
using System;

Application.Init();

var okno = new Window()
{
    X = 0,
    Y = 0,
    Width = Dim.Fill(),
    Height = Dim.Fill()
};

// Vytvoření tlačítka pro ukončení programu
var tlacitkoKonec = new Button("Ukončit")
{
   X = Pos.AnchorEnd(11), 
    
   Y = Pos.AnchorEnd(1),   
};

// Definice akce: Co se stane, když uživatel na tlačítko klikne nebo stiskne Enter
tlacitkoKonec.Clicked += () => {
    Application.RequestStop(); // Bezpečně zastaví aplikaci a uvolní soubory
};

// Přidání tlačítka DO okna
okno.Add(tlacitkoKonec);

// Přidání okna na hlavní plochu
Application.Top.Add(okno);

Application.Run();

Application.Shutdown();
