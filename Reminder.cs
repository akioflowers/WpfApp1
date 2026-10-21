namespace WpfApp1
{
    public class Reminder
    {
        public int Hour {get; set;}
        public int Minute {get; set;}

        public string Text {get; set;} = "";

        public bool IsShown {get; set;}
    }
}