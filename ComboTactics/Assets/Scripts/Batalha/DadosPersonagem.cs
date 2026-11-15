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

    // quadros de cada animacao, na ordem das linhas do sheet
    [Header("Animacoes")]
    public Sprite[] spritesParado;   // Idle: _0 a _5
    public Sprite[] spritesAtaque;   // Attack: _6 a _11 (golpe basico e tecnica)
    public Sprite[] spritesDano;     // Hurt: _12 a _14
    public Sprite[] spritesDerrota;  // Death: _15 a _20
    public Sprite[] spritesEspecial; // Special: _21 a _28

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
