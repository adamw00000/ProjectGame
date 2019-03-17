namespace GameLib.Actions
{
    internal interface IActionResponse
    {
        int WaitUntilTime { get; }
        void Handle(Agent agent);
    }
}