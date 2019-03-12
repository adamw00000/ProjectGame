namespace GameLib.Actions
{
    interface IActionResponse
    {
        void Handle(Agent agent);
        int WaitUntilTime { get; }
    }
}
