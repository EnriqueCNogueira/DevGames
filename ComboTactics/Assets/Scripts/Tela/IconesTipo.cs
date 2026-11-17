using UnityEngine;

// Icones de classe e elemento, num asset so, para a Selecao e a Batalha usarem
[CreateAssetMenu(fileName = "IconesTipo", menuName = "Combo Tactics/Icones de tipo")]
public class IconesTipo : ScriptableObject
{
    public Sprite fogo;
    public Sprite agua;
    public Sprite planta;
    public Sprite guerreiro;
    public Sprite mago;
    public Sprite tanque;

    public Sprite DoElemento(Elemento elemento)
    {
        switch (elemento)
        {
            case Elemento.Fogo: return fogo;
            case Elemento.Agua: return agua;
            default: return planta;
        }
    }

    // o Chefe nao tem icone de classe
    public Sprite DaClasse(Classe classe)
    {
        switch (classe)
        {
            case Classe.Guerreiro: return guerreiro;
            case Classe.Mago: return mago;
            case Classe.Tanque: return tanque;
            default: return null;
        }
    }
}
