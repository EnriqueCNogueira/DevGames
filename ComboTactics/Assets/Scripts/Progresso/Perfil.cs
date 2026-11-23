using System;
using System.Collections.Generic;

// O progresso do aparelho: um perfil so, compartilhado pelos dois jogadores do PvP.
// Guarda moedas, estoque de itens e evolucoes. E C# puro e serializavel em JSON;
// quem le e grava no disco e o PerfilSalvo.
[Serializable]
public class Perfil
{
    public const int MOEDAS_INICIAIS = 300;

    [Serializable]
    public class EvolucaoPersonagem
    {
        public string id; // nome do asset do personagem (ex.: "GuerreiroFogo")
        public int[] niveis = new int[Atributos.QUANTIDADE];
    }

    public int moedas = MOEDAS_INICIAIS;
    public int[] estoque = new int[Itens.QUANTIDADE];
    public List<EvolucaoPersonagem> evolucoes = new List<EvolucaoPersonagem>();

    // ---------- moedas ----------

    public void Ganhar(int quantidade)
    {
        if (quantidade > 0)
            moedas += quantidade;
    }

    bool Gastar(int quantidade)
    {
        if (quantidade <= 0 || moedas < quantidade)
            return false;
        moedas -= quantidade;
        return true;
    }

    // ---------- itens ----------

    public int Estoque(TipoItem tipo)
    {
        Normalizar();
        return estoque[(int)tipo];
    }

    public bool PodeComprar(TipoItem tipo)
    {
        return Itens.Disponivel(tipo)
            && Estoque(tipo) < Itens.ESTOQUE_MAXIMO
            && moedas >= Itens.Preco(tipo);
    }

    public bool Comprar(TipoItem tipo)
    {
        if (!PodeComprar(tipo) || !Gastar(Itens.Preco(tipo)))
            return false;
        estoque[(int)tipo]++;
        return true;
    }

    // chamado quando um item e usado na batalha
    public bool Consumir(TipoItem tipo)
    {
        if (Estoque(tipo) <= 0)
            return false;
        estoque[(int)tipo]--;
        return true;
    }

    // ---------- evolucoes ----------

    public int Nivel(DadosPersonagem personagem, Atributo atributo)
    {
        EvolucaoPersonagem e = Buscar(Id(personagem), false);
        return e == null ? 0 : e.niveis[(int)atributo];
    }

    public int[] Niveis(DadosPersonagem personagem)
    {
        EvolucaoPersonagem e = Buscar(Id(personagem), false);
        return e == null ? new int[Atributos.QUANTIDADE] : (int[])e.niveis.Clone();
    }

    public Atributos AtributosDe(DadosPersonagem personagem)
    {
        return Evolucao.Aplicar(personagem, Niveis(personagem));
    }

    // preco da proxima evolucao; 0 se ja esta no maximo
    public int PrecoEvolucao(DadosPersonagem personagem, Atributo atributo)
    {
        return Evolucao.Preco(Nivel(personagem, atributo));
    }

    public bool PodeEvoluir(DadosPersonagem personagem, Atributo atributo)
    {
        int nivel = Nivel(personagem, atributo);
        if (nivel >= Evolucao.NIVEL_MAXIMO || moedas < Evolucao.Preco(nivel))
            return false;

        // a Defesa para em 100: nao vende evolucao que nao muda o valor
        int valorBase = Atributos.ValorBase(personagem, atributo);
        return Evolucao.Valor(valorBase, nivel + 1, atributo) > Evolucao.Valor(valorBase, nivel, atributo);
    }

    public bool Evoluir(DadosPersonagem personagem, Atributo atributo)
    {
        if (!PodeEvoluir(personagem, atributo))
            return false;
        Gastar(PrecoEvolucao(personagem, atributo));
        Buscar(Id(personagem), true).niveis[(int)atributo]++;
        return true;
    }

    static string Id(DadosPersonagem personagem)
    {
        return personagem.name;
    }

    EvolucaoPersonagem Buscar(string id, bool criar)
    {
        foreach (EvolucaoPersonagem e in evolucoes)
        {
            if (e.id == id)
            {
                if (e.niveis == null || e.niveis.Length != Atributos.QUANTIDADE)
                    Array.Resize(ref e.niveis, Atributos.QUANTIDADE);
                return e;
            }
        }
        if (!criar)
            return null;

        EvolucaoPersonagem nova = new EvolucaoPersonagem { id = id };
        evolucoes.Add(nova);
        return nova;
    }

    // um save antigo pode ter menos itens que o catalogo atual
    void Normalizar()
    {
        if (estoque == null)
            estoque = new int[Itens.QUANTIDADE];
        else if (estoque.Length != Itens.QUANTIDADE)
            Array.Resize(ref estoque, Itens.QUANTIDADE);
    }
}
