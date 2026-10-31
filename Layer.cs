using System;

public class Layer
{
    public Neuron[] Neurons;

    // Construtor: Cria uma camada com 'neuroCount' neuronios, onde cada um recebe 'inputCount' entradas
    public Layer(int neuronCount, int inputCount)
    {
        Neurons = new Neuron[neuronCount];

        for (int i = 0; i < neuronCount; i++)
        {
            Neurons[i] = new Neuron(inputCount);
        }
    }

    // Passa os inputs do mundo por todos os neuronios da camada e devolve um array com todas as respostas
    public double[] Forward(double[] inputs)
    {
        double[] outputs = new double[Neurons.Length];

        for (int i = 0; i < Neurons.Length; i++)
        {
            outputs[i] = Neurons[i].FeedFoward(inputs);
        }

        return outputs;
    }
}