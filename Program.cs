using System;
using System.Collections.Generic;

public enum TypeIntention
{
    None,
    Intimidate,
    Corrupt,
    Deceive,
    Friendship,
    Fellowship
}

public enum InferredDecision
{
    Yield, // Ceder (entregar o que o jogador quer por medo)
    Bargain, // Negociar (aceitar suborno ou exigir vantagem)
    Defy // Desafiar (recusar, rir da cara ou atacar)
}

public enum PhysicalAction
{
    KeepTalking,
    StepAside,
    SoundAlarm,
    AttackPlayer
}

public class WorldState
{
    public bool PlayerIsArmed { get; set; } = true; // Jogador está com arma na mão?
    public bool PlayerIsHidden { get; set; } = true; // Jogador está nas sombras (Stealth)?
    public bool HasWitnesses { get; set; } = false; // Tem outros guardas olhando?
}

public class NPCMind
{
    // Variavel privada (escondida, so a propria classe mexe)
    private int _fear;
    private int _loyalty;
    private int _greed;
    private Layer brain;

    // Propriedade publica (o "porteiro" que o resto do codigo acessa)
    public int Fear
    {
        get { return _fear; }
        set { _fear = Math.Clamp(value, 0, 100); } // 'value' é o numero novo que esta entrando
    }
    
    public int Loyalty
    {
        get { return _loyalty; }
        set { _loyalty = Math.Clamp(value, 0, 100); }
    }
    
    public int Greed
    {
        get { return _greed; }
        set { _greed = Math.Clamp(value, 0, 100); }
    }

    public int Patience { get; set; } = 3; // Quantas vezes ele tolera conversa fiada ou falhas
    public bool WasThreatened { get; set; } = false; // Ele lembra se você já ameaçou a vida dele!

    public string Name { get; set; } = "";

    public NPCMind(string name)
    {
        Name = name;
        brain = new Layer(3, 5); // O cérebro nasce aqui com 3 neurônios e 5 entradas!
    }


    public PhysicalAction React(TypeIntention intention, string focusWord, WorldState world)
    {
        Random dice = new Random();
        int d20 = dice.Next(1, 21);
       
        // 1. A tua frase altera primeiro o estado emocional base do NPC
        if (intention == TypeIntention.Intimidate && d20 >= 10) Fear += 25;
        if (intention == TypeIntention.Corrupt && d20 >= 10) Greed += 25;

        // Prepara os sentidos do mundo convertidos em numeros decimais de 0.0 a 1.0
        double[] inputs = new double[]
        {
            Fear / 100.0,
            Loyalty / 100.0,
            Greed / 100.0,
            world.PlayerIsArmed ? 1.0 : 0.0,
            world.PlayerIsHidden ? 1.0 : 0.0
        };

        // O cerebro pensa! a camada processa os inputs e devolve as 3 ativacoes (0 a 1)
        double[] outputs = brain.Forward(inputs);

        double yieldScore = outputs[0]; // Força no neuronio de Ceder
        double bargainScore = outputs[1]; // Força do nauronio de neogciar
        double defyScore = outputs[2]; // Força do neuronio de Desafiar

        // Se o jogador ameaçou, penaliza o score de negociação
        if (WasThreatened)
        {
            bargainScore -= 0.4;
        }

        // Descobre qual o neuronio que disparou com mais força (o vencedor)
        InferredDecision decision = InferredDecision.Defy;
        if (yieldscore >= bargainScore && yieldScore >= defyScore) 
        {
            decision = InferredDecision.Yield;
        }

        else if (bargainScore >= yieldScore && bargainScore >= defyScore)
        {
            decision = InferredDecision.Bargain;
        }

        if (Patience <= 0 && decision == InferredDecision.Defy)
        {
            Console.WriteLine($"{Name} perde a paciência de vez e saca a arma: 'Chega de papo! Você vai morrer aqui!'");
            return PhysicalAction.AttackPlayer;
        }

        // Mostra a mente do NPC a trabalhar na consola:
        Console.WriteLine($"\n[D20: {d20}] Pesos -> Ceder: {yieldScore} | Negociar: {bargainScore} | Desafiar: {defyScore}");
        Console.WriteLine($"[DECISÃO INFERIDA]: {decision}");

        // 1. Escolhe a. linguagem corporal baseada no nível de Medo atual
        string[] posturasMedoAlto = { "recua a tropeçar", "engole em seco a tremer", "desvia o olhar a suar frio" };
        string[] posturasCalmo = { "cruza os braços", "encara-te de cima a baixo", "apoia a mão no punho da arma" };

        string postura = (decision == InferredDecision.Yield || Fear > 50)
        ? posturasMedoAlto[dice.Next(posturasMedoAlto.Length)]
        : posturasCalmo[dice.Next(posturasCalmo.Length)];

        // 2. Escolhe a fala verbal baseada na Decisão Inferida
        string[] falasYield = { "'Não é preciso violência, eu coopero!'", "'Está bem, ganhaste! Não me faças mal'", "'Calma! Faz o que quiseres, eu não me meto!'"};
        string[] falasBargain = { "'Isso muda a conversa... estamos entendidos.'", "'Por esse valor, fico cego e surdo.'", "'Gosto da forma como pensas. Negócio fechado.'" };
        string[] falasDefy = { "'Poupa o fôlego, isso não resulta comigo.'", "'Achas mesmo que me convences com isso?'", "'Sai da minha frente antes que perca a paciência.'", "'Não quero saber de {focusWord}, sai daqui!'"};

        // 3. O switch junta a Postura Física + a Fala Sorteada (mantendo a variavel 'fala')
        string fala = (decision, intention, world.PlayerIsArmed) switch
        {
            // Ironia detectada:
            (_, TypeIntention.Friendship, true) => $"{Name} cospe no chão: 'Falar de amizade de arma na mão? Achas que sou parvo?'",

            // Cedeu perante suborno (medo extremo):
            (InferredDecision.Yield, TypeIntention.Corrupt, _) => $"{Name} {postura}: 'Guarda o teu ouro! Só não me mates, passa!'",

            // Decisões dinâmicas sorteadas nos arrays:
            (InferredDecision.Yield, _, _) => $"{Name} {postura}: {falasYield[dice.Next(falasYield.Length)]}",
            (InferredDecision.Bargain, _, _) => $"{Name} {postura}: {falasBargain[dice.Next(falasBargain.Length)]}",
            _ => $"{Name} {postura}: {falasDefy[dice.Next(falasDefy.Length)]}"
        };


        Console.WriteLine(fala);

        if (decision == InferredDecision.Yield || decision == InferredDecision.Bargain)
        {
            return PhysicalAction.StepAside; // Você venceu o encontro social!
        }

        return PhysicalAction.KeepTalking; // A conversa continua tensa
    }
}

class SemanticParser
{
    private Dictionary<string, TypeIntention> _vocabulary = new()
    {
        // Intimidação (palavras e expressões)
        { "matar", TypeIntention.Intimidate },
        { "sangue", TypeIntention.Intimidate },
        { "acabar com", TypeIntention.Intimidate },
        { "dormir com os peixes", TypeIntention.Intimidate },

        // Suborno / Corrupção
        { "ouro", TypeIntention.Corrupt },
        { "moedas", TypeIntention.Corrupt },
        { "vista grossa", TypeIntention.Corrupt },
        { "molhar a mao", TypeIntention.Corrupt },

        // Enganação / Amizade
        { "amigo", TypeIntention.Friendship },
        { "ajudar", TypeIntention.Friendship },
        { "mentira", TypeIntention.Deceive }
        
    };

    public (TypeIntention, string) Analyze(string text)
    {
        // 1. Limpa o texto (minúsculas e sem pontuação) e divide nos espaços
        string cleanText = text.ToLower().Replace(",", "").Replace("!", "").Replace(".", "");

        TypeIntention foundIntent = TypeIntention.None;

        // 2. Percorre cada par (Expressao -> Intencao) do dicionario
        foreach (var item in _vocabulary)
        {
            // Se a frase inteira contém a palavra ou expressão do dicionário:
            if (cleanText.Contains(item.Key))
            {
                foundIntent = item.Value;
                break;
            }
        }

        // TRUQUE DO ESPELHO: Captura a palavra mais longa/incomum que o jogador usou
        string[] words = cleanText.Split(' ');
        string focusWord = "isso"; // Palavra padrão caso a frase seja curta

        foreach (string w in words)
        {
            // Se a palavra tiver mais de 4 letras e nao estiver no vocabulario base, é o "tempero" do jogador!
            if (w.Length > 4 && !_vocabulary.ContainsKey(w) && w != "passar" && w != "deixar")
            {
                focusWord = w;
            }
        }

        return (foundIntent, focusWord);
    }
}

class Program
{
    static void Main()
    {
        // 1. Instanciar o NPC na memoria
        NPCMind guarda = new NPCMind("Roberto");
        guarda.Fear = 20;
        guarda.Loyalty = 60;
        guarda.Greed = 40;

        SemanticParser parser = new SemanticParser();

        Console.WriteLine($"--- Encontraste o guarda {guarda.Name} (Modo inicial: {guarda.Fear}) ---");

        WorldState mundo = new WorldState();

       // 2. Ciclo Infinito de conversa
       while (true)
       {
        Console.Write("\nDiz algo ao guarda (ou escreve 'sair'): ");
        string input = Console.ReadLine() ?? "";

        // Se escreveres 'sair', o comando 'break' encerra o programa
        if (input.ToLower() == "sair")
        {
            break;
        }

        // 3. O tradutor descobre a intenção e o guarda reage!
        (TypeIntention detectada, string palavraFoco) = parser.Analyze(input);

        PhysicalAction acao = guarda.React(detectada, palavraFoco, mundo);

        if (acao == PhysicalAction.AttackPlayer)
        {
            Console.WriteLine("\n[SISTEMA DE JOGO]: COMBATE INICIADO!");
            break; // Sai do diálogo!
        }
        else if (acao == PhysicalAction.StepAside)
        {
            Console.WriteLine("\n[SISTEMA DE JOGO]: O caminho está livre. Você passou pelo guarda!");
            break;
        }
       }
    }
}