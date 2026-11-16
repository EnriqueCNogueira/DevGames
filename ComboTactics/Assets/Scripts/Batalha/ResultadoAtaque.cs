// O que aconteceu no ultimo golpe, para a tela mostrar sem refazer a conta
public struct ResultadoAtaque
{
    public int Atacante;   // 0 ou 1
    public TipoGolpe Golpe;
    public int Dano;
    public int Saldo;      // saldo de vantagens (ver Efetividade)
    public bool Derrotou;  // o alvo caiu com este golpe
}
