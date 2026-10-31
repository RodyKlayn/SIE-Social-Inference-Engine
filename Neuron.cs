using System;

public class Neuron
{
    public double[] Weights;
    public double Bias;

    // COnstrutor: Cria o neurôrio inicializando os pesos e o viés com valores aleatórios
    public Neuron(int inputCount)
    {
        Random rand = new Random();
        Weights = new double[inputCount];

        for (int i = 0; i < inputCount; i++)
        {
            // Inicializa os pesos com valores entre -1.0 e 1.0
            Weights[i] = rand.NextDouble() * 2.0 - 1.0;
        }

        // Viés inicial entre -1.0 e 1.0
        Bias = rand.NextDouble() * 2.0 - 1.0;
    }

    // A função matemática de ativação Sigmoid: 1 / (1 + e^-z)
    private double Sigmoid(double z)
    {
        return 1.0 / (1.0 + Math.Exp(-z));
    }

    // O cálculo principal do neurônio: Soma Ponderada + Ativação
    public double FeedFoward(double[] inputs)
    {
        double z = Bias;

        for (int i = 0; i < inputs.Length; i++)
        {
            z += inputs[i] * Weights[i]; // Entrada x Peso
        }

        return Sigmoid(z); // Retorna o valor disparado entre 0 e 1
    }
}