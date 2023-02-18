namespace BusTour.AppServices.TourOrderProcess.Commands
{
    public static class TourOrderStepCommand
    {
        public static string WaitingForPaiment => nameof(WaitingForPaiment);
        public static string Payment => nameof(Payment);
        public static string Cancel => nameof(Cancel);
    }
}
