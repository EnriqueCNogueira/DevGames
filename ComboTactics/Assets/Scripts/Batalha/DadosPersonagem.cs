using UnityEngine;

// Status base de cada personagem (valores do GDD, secao 4)
[CreateAssetMenu(fileName = "NovoPersonagem", menuName = "Combo Tactics/Personagem")]
public class DadosPersonagem : ScriptableObject
{
    public string nome;
    public Classe classe;
    public Elemento elemento;

    public int vida;
    public int ataque;
    public int defesa;
    public int velocidade;

    [Header("Golpes")]
    public string golpeBasico;
    public string tecnica;
    public string especial;

    [Header("Visual")]
    public Sprite[] spritesParado; // quadros da animacao Idle (primeira linha do sheet)

    public string NomeDoGolpe(TipoGolpe tipo)
    {
        switch (tipo)
        {
            case TipoGolpe.Basico: return golpeBasico;
            case TipoGolpe.Tecnica: return tecnica;
            default: return especial;
        }
    }
}
