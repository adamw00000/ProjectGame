namespace GameLib.Actions
{
    internal interface IActionResponse
    {
        void Handle(Agent agent);

        int WaitUntilTime { get; }
    }
}