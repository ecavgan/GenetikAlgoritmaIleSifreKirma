// Yapay Zeka Yontemleri 1. Projesinin 3. Sorusunun Cozumu
// 05200000006, 05200000762

using System;
using System.Linq;

namespace yz_genetik
{
    class Program
    {
        static string password = "Deep Learning 2022";
        static string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 "; // olasi karakterler
        static int generation = 0;

        static void Main(string[] args) // n^^4
        {
            Console.WriteLine("a)");
            PrintInfo(10, 1);
            PrintInfo(30, 3);
            PrintInfo(50, 5);
            PrintInfo(80, 8);
            PrintInfo(100, 10);
            PrintInfo(150, 15);

            password = "DeepLearning";
            Console.WriteLine("c)");
            PrintInfo(10, 1);
            PrintInfo(30, 3);
            PrintInfo(50, 5);
            PrintInfo(80, 8);
            PrintInfo(100, 10);
            PrintInfo(150, 15);
        }
        
        // problem çözme metodunu çalıştırıp sayımlama (istatistik) bilgilerini yazdıran metod
        static void PrintInfo(int chromosome_count, int elite_count)
        {
            Console.WriteLine("Sifre: " + password);
            Console.WriteLine("Kromozom Sayisi: " + chromosome_count);
            Console.WriteLine("Elit Sayısı: " + elite_count);
            Console.WriteLine("------------------------------------------");
            for (int i = 0; i < 3; i++)
                CrackPassword(chromosome_count, elite_count);

            Console.WriteLine("Nesil ortalamasi: " + generation / 3);
            Console.WriteLine("------------------------------------------" + "\n");
            generation = 0;
        }

        // populasyon olusturulur, elitler secilir, 'crossover' gerceklestirilir, %1 mutasyona ugratilip bir dahaki nesilde
        // kullanilmak uzere yeni populasyon olusturulur
        static void CrackPassword(int chromosome_count, int elite_count) // n^^4
        {
            DateTime startTime = DateTime.Now;
            Random rnd = new Random();
            string[] population = new string[chromosome_count];
            for (int i = 0; i < chromosome_count; i++)
                population[i] = CreateChromosome();

            string[] new_population, sorted_population;
            string[] elites, rest;
            for (int j = 0; j < 5000; j++)
            {
                elites = PickElites(elite_count, population);
                rest = Select(population.Length - elite_count, population);

                new_population = new string[elites.Length + rest.Length];
                elites.CopyTo(new_population, 0);
                rest.CopyTo(new_population, elites.Length);

                CrossOver(new_population);
                TwoOptMutate(new_population);
                
                sorted_population = SortPopulation(population = new_population);

                if (GetFitness(sorted_population[0]) == password.Length)
                {
                    j += 2;
                    generation += j;
                    Console.WriteLine("Şifre " + j + ". jenerasyonda bulundu.");
                    Console.WriteLine("Çözüme en yakın sonuç: '" + sorted_population[1] + "'");
                    DateTime endTime = DateTime.Now;
                    TimeSpan timePassed = endTime.Subtract(startTime);
                    Console.WriteLine("Geçen süre: " + timePassed.Milliseconds + " ms");
                    Console.WriteLine("------------------------------------------");
                    break;
                }
            }
        }
        
        static string[] PickElites(int elite_count, string[] population)
        {
            Random rnd = new Random();
            string[] sorted_population = SortPopulation(population);
            string[] elites = new string[elite_count];
            int max_fitness = GetFitness(sorted_population[0]);

            int i = 0;
            int practical_elite_count = 0; // en yuksek 'fitness' degerli elitlerin tutuldu degisken
            while(true)
            {
                if (GetFitness(sorted_population[i++]) == max_fitness)
                    practical_elite_count++;
                else
                    break;

                if (i == population.Length)
                    break;
            }

            // yuksek 'fitness' degerlerine ulasildiginda cesitliligin azalmamasi icin
            // hep en ustteki elitler secilmektense rastgele bakilir
            for (i = 0; i < elite_count; i++)
            {
                if (practical_elite_count < elite_count)
                    elites[i] = sorted_population[i];
                else
                    elites[i] = sorted_population[rnd.Next(practical_elite_count)];
            }

            return elites;
        }

        // 10 kromozom varsa 00000000011111111222222...777889 olmak üzere bir stringden rastgele seçim yapılır
        // bu sayede sıralanmış popülasyonun tepesindekinin seçilme ihtimali en yüksektir ve 9'dur
        static string[] Select(int selection_count, string[] population)
        {
            Random rnd = new Random();

            string[] selected_chromosomes = new string[selection_count];
            string[] sorted_population = SortPopulation(population);
            string[] up = sorted_population.Distinct().ToArray();

            string x = "";
            for (int i = 0; i < up.Length; i++)
            {
                for (int j = 0; j < up.Length - i; j++)
                {
                    if (i == up.Length - 1 && j == up.Length - i - 1)
                        x += i;
                    else
                        x += i + ",";
                }
            }

            string[] x_arr = x.Split(',');
            for (int i = 0; i < selection_count; i++)
                selected_chromosomes[i] = up[Int32.Parse(x_arr[rnd.Next(x_arr.Length)])];

            return selected_chromosomes;
        }
        
        // abcd, efgh kromozomlari abgh, cdef haline getirilir
        static void CrossOver(string[] population)
        {
            Random rnd = new Random();

            int c1_ind, c2_ind;
            string c1, c2;
            string c1_h, c1_l, c2_h, c2_l;
            string new_c1, new_c2;
            for (int i = 0; i < population.Length / 2; i++)
            {
                c1_ind = rnd.Next(population.Length);
                c2_ind = rnd.Next(population.Length);

                c1 = population[c1_ind];
                c2 = population[c2_ind];

                c1_h = c1.Substring(0, c1.Length / 2);
                c1_l = c1.Substring(c1.Length / 2, c1.Length - c1.Length / 2);
                c2_h = c2.Substring(0, c2.Length / 2);
                c2_l = c2.Substring(c2.Length / 2, c2.Length - c2.Length / 2);

                new_c1 = c1_h + c2_l;
                new_c2 = c2_h + c1_l;

                population[c1_ind] = new_c2;
                population[c2_ind] = new_c1;
            }
        }

        // kromozomun tum harfleri sirayla degistirilir
        // orn. ax, bx, cx, ..., zx, 0x, 1x, ...
        // en iyi fitness degeri olan kromozom, eskisinin yerini alir
        // normalde n^^2 calisir, getFitness() metoundan dolayi n^^3'tur
        static void TwoOptMutate(string[] population) // n^^3
        {
            Random rnd = new Random();
            int possibility = rnd.Next(1, 10000 / population.Length); // takriben %1 olasilik bu sekilde saglanir

            if (possibility > 100)
                return;

            int c_num = rnd.Next(population.Length);
            string chromosome = population[c_num];

            int max_fitness = GetFitness(chromosome);
            int cur_fitness;

            string new_chromosome = "";
            string cur_chromosome;
            for (int i = 0; i < password.Length; i++)
            {
                for (int j = 0; j < alphabet.Length; j++)
                {
                    cur_chromosome = "";
                    for (int k = 0; k < password.Length; k++)
                    {
                        if (k == i)
                            cur_chromosome += alphabet[j];
                        else
                            cur_chromosome += chromosome[k];
                    }

                    cur_fitness = GetFitness(cur_chromosome);
                    if (cur_fitness > max_fitness)
                    {
                        max_fitness = cur_fitness;
                        new_chromosome = cur_chromosome;
                    }
                }
            }

            population[c_num] = new_chromosome;
        }


        // populasyonu fitness degerine gore siralamak icin kullanilan metod
        // secmeli siralama(selection sort) mantigiyla calisir
        static string[] SortPopulation(string[] population) // n^^3
        {
            string[] sorted_population = new string[population.Length];
            for (int i = 0; i < population.Length; i++)
                sorted_population[i] = population[i];

            int maxFitnessValue, maxFitnessIndex;
            string temp;
            for (int i = 0; i < population.Length; i++)
            {
                maxFitnessValue = GetFitness(sorted_population[i]);
                maxFitnessIndex = i;
                for (int j = i; j < population.Length; j++)
                {
                    if (maxFitnessValue < GetFitness(sorted_population[j]))
                    {
                        maxFitnessValue = GetFitness(sorted_population[j]);
                        maxFitnessIndex = j;
                    }
                }

                temp = sorted_population[maxFitnessIndex];
                sorted_population[maxFitnessIndex] = sorted_population[i];
                sorted_population[i] = temp;
            }

            return sorted_population;
        }

        // rastgele karakterlerden olusan kromozom olusturan metod
        static string CreateChromosome()
        {
            Random rnd = new Random();
            string chromosome = "";
            for (int i = 0; i < password.Length; i++)
                chromosome += alphabet[rnd.Next(alphabet.Length)];

            return chromosome;
        }

        // fitness degerini hesaplayan metod
        static int GetFitness(string chromosome)
        {
            int counter = 0;
            for (int i = 0; i < password.Length; i++)
                if (chromosome[i] == password[i])
                    counter++;

            return counter;
        }
    }
}
