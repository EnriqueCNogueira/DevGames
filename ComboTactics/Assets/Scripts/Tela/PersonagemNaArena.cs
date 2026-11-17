using System.Collections;
using UnityEngine;

// Mostra o personagem ativo de um dos lados da arena e toca as animacoes dele
public class PersonagemNaArena : MonoBehaviour
{
    // quadros por segundo de cada animacao (SPRITES.md)
    const float FPS_PARADO = 6f;
    const float FPS_GOLPE = 10f;
    const float FPS_DANO = 10f;
    const float FPS_DERROTA = 8f;

    public SpriteRenderer spriteRenderer;
    public bool olharParaEsquerda; // o jogador 2 fica virado para o jogador 1

    DadosPersonagem dados;
    Sprite[] quadros;
    float fps;
    bool repetir;
    bool derrotado;
    float tempo;
    Vector3 posicaoInicial;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.flipX = olharParaEsquerda;
        posicaoInicial = transform.localPosition;
    }

    public void Mostrar(DadosPersonagem novo)
    {
        StopAllCoroutines();
        transform.localPosition = posicaoInicial;
        spriteRenderer.color = Color.white;

        dados = novo;
        derrotado = false;
        Tocar(dados.spritesParado, FPS_PARADO, true);
    }

    // troca a animacao atual. Se a lista estiver vazia, continua na que estava.
    bool Tocar(Sprite[] novosQuadros, float novoFps, bool emLoop)
    {
        if (novosQuadros == null || novosQuadros.Length == 0)
            return false;

        quadros = novosQuadros;
        fps = novoFps;
        repetir = emLoop;
        tempo = 0f;
        spriteRenderer.sprite = quadros[0];
        return true;
    }

    void Update()
    {
        if (quadros == null || quadros.Length == 0)
            return;

        tempo += Time.deltaTime;
        int i = (int)(tempo * fps);

        if (i >= quadros.Length)
        {
            if (repetir)
                i %= quadros.Length;
            else if (derrotado)
                i = quadros.Length - 1; // fica no ultimo quadro da derrota
            else
            {
                // golpe ou dano acabou: volta a ficar parado
                Tocar(dados.spritesParado, FPS_PARADO, true);
                return;
            }
        }
        spriteRenderer.sprite = quadros[i];
    }

    public void Atacar(bool especial)
    {
        Tocar(especial ? dados.spritesEspecial : dados.spritesAtaque, FPS_GOLPE, false);
        StartCoroutine(AvancarEVoltar());
    }

    public void LevarDano()
    {
        // sem a animacao de dano, pisca em vermelho
        if (!Tocar(dados.spritesDano, FPS_DANO, false))
            StartCoroutine(Piscar());
    }

    public void Derrotado()
    {
        StopAllCoroutines();
        transform.localPosition = posicaoInicial;
        derrotado = true;

        // sem a animacao de derrota, so fica cinza
        if (!Tocar(dados.spritesDerrota, FPS_DERROTA, false))
            spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f, 0.6f);
    }

    // da um passo na direcao do inimigo e volta
    IEnumerator AvancarEVoltar()
    {
        float direcao = olharParaEsquerda ? -1f : 1f;
        Vector3 frente = posicaoInicial + new Vector3(0.4f * direcao, 0f, 0f);

        yield return Mover(posicaoInicial, frente, 0.12f);
        yield return new WaitForSeconds(0.25f);
        yield return Mover(frente, posicaoInicial, 0.2f);
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
