using CSharpFunctionalExtensions;

namespace ArmaforcesMissionBot.Features.Signups.Missions.Validators
{
    public class MissionValidator : IMissionValidator
    {
        public Result CheckSignupsComplete(IMission mission) => throw new System.NotImplementedException();
    }

    public interface IMissionValidator
    {
        Result CheckSignupsComplete(IMission mission);
    }
}
