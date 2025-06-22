namespace RedGaint.Games.DyeHard
{
    // Optional interface to inject the character/user into the power-up
    public interface IInjectUser
    {
        void SetUser(ICharacterPowerUpUser user);
    }
}