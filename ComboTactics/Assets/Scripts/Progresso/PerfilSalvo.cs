using UnityEngine;

// Le e grava o Perfil em PlayerPrefs, como JSON. No WebGL o PlayerPrefs fica
// guardado no navegador (IndexedDB), por endereco do site.
//
// Com o domain reload desligado, o perfil carregado sobrevive entre Plays no
// editor; por isso ele e relido do disco no inicio de todo Play.
public static class PerfilSalvo
{
    public const string CHAVE = "ComboTactics.Perfil";

    static Perfil atual;

    public static Perfil Atual
    {
        get
        {
            if (atual == null)
                atual = Carregar();
            return atual;
        }
    }

    public static void Salvar()
    {
        if (atual == null)
            return;
        PlayerPrefs.SetString(CHAVE, JsonUtility.ToJson(atual));
        PlayerPrefs.Save();
    }

    // volta ao perfil novo (300 moedas, sem itens nem evolucoes)
    public static void Apagar()
    {
        PlayerPrefs.DeleteKey(CHAVE);
        PlayerPrefs.Save();
        atual = null;
    }

    static Perfil Carregar()
    {
        string json = PlayerPrefs.GetString(CHAVE, "");
        if (string.IsNullOrEmpty(json))
            return new Perfil();

        try
        {
            return JsonUtility.FromJson<Perfil>(json) ?? new Perfil();
        }
        catch (System.ArgumentException)
        {
            Debug.LogWarning("Perfil salvo ilegivel; comecando um perfil novo.");
            return new Perfil();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RecarregarAoIniciarPlay()
    {
        atual = null;
    }
}
