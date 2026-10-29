using System;

public enum TypeIntention
{
    None,
    Intimidate,
    Corrupt,
    Deceive,
    Friendship,
    Fellowship
}

public class NPCMind
{
    // Variavel privada (escondida, so a propria classe mexe)
    private int _fear;

    // Propriedade publica (o "porteiro" que o resto do codigo acessa)
    public int Fear
    {
        get { return _fear; }
        set { _fear = Math.Clamp(value, 0, 100); } // 'value' é o numero novo que esta entrando
    }

    public string Name { get; set; } = "";
}

class Program
{
    static void Main()
    {
        // 1. Instanciar o NPC na memoria
        NPCMind guarda = new NPCMind();
        guarda.Name = "Roberto";

        // 2. Tentar forçar o atributo para 250
        guarda.Fear = 250;

        // 3. Imprimir nas consola (o sinal '$' antes das aspas permite colocar variaveis dentro de {})
        Console.WriteLine($"O nível de modo do {guarda.Name} é: {guarda.Fear}");
    }
}