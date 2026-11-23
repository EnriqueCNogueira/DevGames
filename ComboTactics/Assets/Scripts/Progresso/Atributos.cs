public enum Atributo
{
    Vida,
    Ataque,
    Defesa,
    Velocidade
}

// Os quatro atributos de um personagem, ja com as evolucoes aplicadas
public struct Atributos
{
    public const int QUANTIDADE = 4;

    public int vida;
    public int ataque;
    public int defesa;
    public int velocidade;

    public static Atributos Base(DadosPersonagem dados)
    {
        return new Atributos
        {
            vida = dados.vida,
            ataque = dados.ataque,
            defesa = dados.defesa,
            velocidade = dados.velocidade,
        };
    }

    public int Valor(Atributo atributo)
    {
        switch (atributo)
        {
            case Atributo.Vida: return vida;
            case Atributo.Ataque: return ataque;
            case Atributo.Defesa: return defesa;
            default: return velocidade;
        }
    }

    public static int ValorBase(DadosPersonagem dados, Atributo atributo)
    {
        return Base(dados).Valor(atributo);
    }

    public static string Nome(Atributo atributo)
    {
        switch (atributo)
        {
            case Atributo.Vida: return "Vida";
            case Atributo.Ataque: return "Ataque";
            case Atributo.Defesa: return "Defesa";
            default: return "Velocidade";
        }
    }
}
