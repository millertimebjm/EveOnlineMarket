namespace Eve.Services.Interfaces.EveApi.Characters;

public interface ICharacterService
{
    Task<int> GetCharacterId(string accessToken);
}