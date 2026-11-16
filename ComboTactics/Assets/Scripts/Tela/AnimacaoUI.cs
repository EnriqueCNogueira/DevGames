using UnityEngine;
using UnityEngine.UI;

// Toca uma sequencia de quadros num Image da UI (ex.: o idle do personagem)
[RequireComponent(typeof(Image))]
public class AnimacaoUI : MonoBehaviour
{
    public float quadrosPorSegundo = 6f; // mesmo ritmo do idle na arena (SPRITES.md)

    Image imagem;
    Sprite[] quadros;
    float tempo;

    void Awake()
    {
        imagem = GetComponent<Image>();
    }

    public void Tocar(Sprite[] novosQuadros)
    {
        if (imagem == null)
            imagem = GetComponent<Image>();

        quadros = novosQuadros;
        tempo = 0f;
        bool temQuadros = quadros != null && quadros.Length > 0;
        imagem.enabled = temQuadros;
        if (temQuadros)
            imagem.sprite = quadros[0];
    }

    void Update()
    {
        if (quadros == null || quadros.Length < 2)
            return;

        tempo += Time.unscaledDeltaTime;
        int quadro = (int)(tempo * quadrosPorSegundo) % quadros.Length;
        imagem.sprite = quadros[quadro];
    }
}
