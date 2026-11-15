using System.Collections;
using UnityEngine;

// Mostra o personagem ativo de um dos lados da arena
public class PersonagemNaArena : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public bool olharParaEsquerda; // o jogador 2 fica virado para o jogador 1
    public float quadrosPorSegundo = 6f;

    Sprite[] quadros;
    float tempo;
    Vector3 posicaoInicial;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.flipX = olharParaEsquerda;
        posicaoInicial = transform.localPosition;
    }

    public void Mostrar(DadosPersonagem dados)
    {
        StopAllCoroutines();
        transform.localPosition = posicaoInicial;
        spriteRenderer.color = Color.white;

        quadros = dados.spritesParado;
        tempo = 0f;
        TrocarQuadro();
    }

    void Update()
    {
        tempo += Time.deltaTime;
        TrocarQuadro();
    }

    void TrocarQuadro()
    {
        if (quadros == null || quadros.Length == 0)
            return;

        int i = (int)(tempo * quadrosPorSegundo) % quadros.Length;
        spriteRenderer.sprite = quadros[i];
    }

    public void Atacar()
    {
        StartCoroutine(AvancarEVoltar());
    }

    public void LevarDano()
    {
        StartCoroutine(Piscar());
    }

    public void Derrotado()
    {
        StopAllCoroutines();
        transform.localPosition = posicaoInicial;
        spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f, 0.6f);
    }

    // da um passo na direcao do inimigo e volta
    IEnumerator AvancarEVoltar()
    {
        float direcao = olharParaEsquerda ? -1f : 1f;
        Vector3 frente = posicaoInicial + new Vector3(0.4f * direcao, 0f, 0f);

        yield return Mover(posicaoInicial, frente, 0.1f);
        yield return Mover(frente, posicaoInicial, 0.15f);
    }

    IEnumerator Mover(Vector3 de, Vector3 para, float duracao)
    {
        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(de, para, t / duracao);
            yield return null;
        }
        transform.localPosition = para;
    }

    // fica vermelho algumas vezes
    IEnumerator Piscar()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.08f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.08f);
        }
    }
}
