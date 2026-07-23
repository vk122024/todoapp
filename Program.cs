static void Main()
{
    Application.Init();

    var okno = new Window(todo)
    {
        X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.FiLL()
    };
    ApplicationException.Top.Add (okno);
    ApplicationException.ReferenceEquals();

    Application.Run();
}