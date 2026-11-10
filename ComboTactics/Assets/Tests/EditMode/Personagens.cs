using UnityEngine;

// Personagens com os status do GDD, so para usar nos testes
public static class Personagens
{
    static DadosPersonagem Criar(string nome, Classe classe, Elemento elemento,
        int vida, int ataque, int defesa, int velocidade)
    {
        DadosPersonagem p = ScriptableObject.CreateInstance<DadosPersonagem>();
        p.nome = nome;
        p.classe = classe;
        p.elemento = elemento;
        p.vida = vida;
        p.ataque = ataque;
        p.defesa = defesa;
        p.velocidade = velocidade;
        p.golpeBasico = "Golpe";
        p.tecnica = "Tecnica";
        p.especial = "Especial";
        return p;
    }

    public static DadosPersonagem GuerreiroFogo() { return Criar("Guerreiro de Fogo", Classe.Guerreiro, Elemento.Fogo, 620, 130, 45, 77); }
    public static DadosPersonagem GuerreiroAgua() { return Criar("Guerreiro de Agua", Classe.Guerreiro, Elemento.Agua, 620, 119, 55, 90); }
    public static DadosPersonagem MagoFogo() { return Criar("Mago de Fogo", Classe.Mago, Elemento.Fogo, 560, 171, 15, 52); }
    public static DadosPersonagem MagoAgua() { return Criar("Mago de Agua", Classe.Mago, Elemento.Agua, 560, 160, 25, 65); }
    public static DadosPersonagem TanqueFogo() { return Criar("Tanque de Fogo", Classe.Tanque, Elemento.Fogo, 880, 73, 75, 27); }
    public static DadosPersonagem TanqueAgua() { return Criar("Tanque de Agua", Classe.Tanque, Elemento.Agua, 880, 68, 85, 40); }
    public static DadosPersonagem TanquePlanta() { return Criar("Tanque de Planta", Classe.Tanque, Elemento.Planta, 880, 63, 95, 23); }
    public static DadosPersonagem Boitata() { return Criar("Boitata", Classe.Chefe, Elemento.Fogo, 2000, 150, 60, 60); }
}
