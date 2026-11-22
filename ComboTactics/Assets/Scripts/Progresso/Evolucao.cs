using UnityEngine;

// Regras da evolucao de atributos (GDD 6.3):
// cada nivel soma 3% do valor base, ate 5 niveis por atributo (+15%).
// O preco sobe com o nivel: 50, 100, 150, 200, 250.
public static class Evolucao
{
    public const int NIVEL_MAXIMO = 5;
    public const int PORCENTO_POR_NIVEL = 3;
    public const int PRECO_POR_NIVEL = 50;

    // a Defesa corta 0,5% do dano por ponto e vai ate 100 (corte de 50%)
    public const int DEFESA_MAXIMA = 100;

    // preco para sair do nivel atual para o seguinte; 0 se ja esta no maximo
    public static int Preco(int nivelAtual)
    {
        if (nivelAtual >= NIVEL_MAXIMO)
            return 0;
        return (nivelAtual + 1) * PRECO_POR_NIVEL;
    }

    // Soma linear sobre o valor base (sem juros compostos), arredondada.
    // Em atributos baixos 3% e menos de 1 ponto (Defesa 25: +0,75) e o
    // arredondamento repetiria o valor de um nivel para o outro; por isso cada
    // nivel vale pelo menos +1, para toda compra aparecer no atributo.
    public static int Valor(int valorBase, int nivel, Atributo atributo)
    {
        nivel = Mathf.Clamp(nivel, 0, NIVEL_MAXIMO);
        int valor = valorBase;
        for (int n = 1; n <= nivel; n++)
        {
            // conta inteira, arredondando meio ponto para cima (sem erro de float)
            int linear = (valorBase * (100 + PORCENTO_POR_NIVEL * n) + 50) / 100;
            valor = Mathf.Max(linear, valor + 1);
        }
        if (atributo == Atributo.Defesa)
            valor = Mathf.Min(valor, DEFESA_MAXIMA);
        return valor;
    }

    // atributos do personagem com os niveis comprados (niveis na ordem do enum Atributo)
    public static Atributos Aplicar(DadosPersonagem dados, int[] niveis)
    {
        Atributos a = Atributos.Base(dados);
        if (niveis == null)
            return a;

        a.vida = Valor(dados.vida, Nivel(niveis, Atributo.Vida), Atributo.Vida);
        a.ataque = Valor(dados.ataque, Nivel(niveis, Atributo.Ataque), Atributo.Ataque);
        a.defesa = Valor(dados.defesa, Nivel(niveis, Atributo.Defesa), Atributo.Defesa);
        a.velocidade = Valor(dados.velocidade, Nivel(niveis, Atributo.Velocidade), Atributo.Velocidade);
        return a;
    }

    static int Nivel(int[] niveis, Atributo atributo)
    {
        int i = (int)atributo;
        return i < niveis.Length ? niveis[i] : 0;
    }
}
