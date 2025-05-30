using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

Blockchain simpleChain = new Blockchain();
Console.WriteLine("A simple blockchain in C#.\nCommands: add [data], validate, exit, save, load, print");

while (true)
{
    Console.Write("> ");
    string? input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }
    string[] parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length == 0)
    {
        continue;
    }

    string command = parts[0].ToLower();

    switch (command)
    {
        case "add":
            if (parts.Length < 2)
            {
                Console.WriteLine("Usage: add [data]");
            }
            else
            {
                simpleChain.AddBlock(parts[1]);
                Console.WriteLine($"Block added: {parts[1]}");
            }
            break;

        case "validate":
            Console.WriteLine($"Blockchain valid? {simpleChain.IsValid()}");
            break;

        case "exit":
            return;
        case "save":
            simpleChain.SaveToFile("blockchain.txt");
            Console.WriteLine("Blockchain saved to blockchain.txt");
            break;

        case "load":
            simpleChain.LoadFromFile("blockchain.txt");
            break;
        case "print":
            foreach (var block in simpleChain.Chain)
            {
                Console.WriteLine($"Index: {block.Index}, Time: {block.TimeStamp}, Hash: {block.Hash}, PreviousHash: {block.PreviousHash}, Data: {block.Data}, Nonce: {block.Nonce}");
            }
            break;
        default:
            Console.WriteLine("Unknown command. Available commands: add [data], validate, exit, save, load, print");
            break;
    }
}


public class Block
{
    public int Index { get; set; }
    public DateTime TimeStamp { get; set; }
    public string PreviousHash { get; set; }
    public string Hash { get; set; }
    public string Data { get; set; }
    public int Nonce { get; set; } // Added nonce

    public Block(DateTime timeStamp, string previousHash, string data)
    {
        Index = 0;
        TimeStamp = timeStamp;
        PreviousHash = previousHash;
        Data = data;
        Nonce = 0;
        Hash = CalculateHash();
    }

    public string CalculateHash()
    {
        SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes($"{Index}-{TimeStamp}-{PreviousHash ?? ""}-{Data}-{Nonce}");
        byte[] outputBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToBase64String(outputBytes);
    }
}

public class Blockchain
{
    public IList<Block> Chain { set; get; }
    public int Difficulty { get; set; } = 2; // Simple difficulty
    private readonly TimeSpan TargetBlockTime = TimeSpan.FromSeconds(10);
    private readonly int AdjustmentInterval = 5; // Adjust difficulty every N blocks
    private List<TimeSpan> RecentMiningTimes = new List<TimeSpan>();

    public Blockchain()
    {
        InitializeChain();
        AddGenesisBlock();
    }

    public void InitializeChain()
    {
        Chain = new List<Block>();
    }

    public Block CreateGenesisBlock()
    {
        return new Block(DateTime.Now, null, "{}");
    }

    public void AddGenesisBlock()
    {
        Chain.Add(CreateGenesisBlock());
    }

    public Block GetLatestBlock()
    {
        return Chain[Chain.Count - 1];
    }

 public void MineBlock(Block block)
    {
        // Increase Nonce until hash starts with 'Difficulty' zeroes
        string prefix = new string('0', Difficulty);
        var startTime = DateTime.Now;
        while (!block.Hash.StartsWith(prefix))
        {
            block.Nonce++;
            block.Hash = block.CalculateHash();
        }
        var miningDuration = DateTime.Now - startTime;
        RecentMiningTimes.Add(miningDuration); // Store mining time
        
        Console.WriteLine($"Mined block #{block.Index} with difficulty {Difficulty} in {miningDuration.TotalMilliseconds:F2} milliseconds.");
        
        // Adjust difficulty if needed
        if (block.Index % AdjustmentInterval == 0 && block.Index > 0)
        {
            AdjustDifficulty();
        }
    }
    
    private void AdjustDifficulty()
    {
        // Calculate average mining time from recent blocks
        if (RecentMiningTimes.Count == 0) return;
        
        TimeSpan averageMiningTime = TimeSpan.FromMilliseconds(
            RecentMiningTimes.Take(AdjustmentInterval).Average(t => t.TotalMilliseconds));
        
        Console.WriteLine($"Average mining time: {averageMiningTime.TotalSeconds:F2} seconds (Target: {TargetBlockTime.TotalSeconds} seconds)");
        
        // Adjust difficulty based on comparison with target time
        if (averageMiningTime < TargetBlockTime * 0.5)
        {
            // Too fast, increase difficulty
            Difficulty++;
            Console.WriteLine($"Mining too fast. Increasing difficulty to {Difficulty}");
        }
        else if (averageMiningTime > TargetBlockTime * 2)
        {
            // Too slow, decrease difficulty (but not below 1)
            if (Difficulty > 1)
            {
                Difficulty--;
                Console.WriteLine($"Mining too slow. Decreasing difficulty to {Difficulty}");
            }
        }
        
        // Clear old mining times after adjustment
        if (RecentMiningTimes.Count > AdjustmentInterval * 2)
        {
            RecentMiningTimes = RecentMiningTimes.Skip(RecentMiningTimes.Count - AdjustmentInterval).ToList();
        }
    }

    public void AddBlock(string data)
    {
        Block latestBlock = GetLatestBlock();
        Block newBlock = new Block(DateTime.Now, latestBlock.Hash, data);
        newBlock.Index = latestBlock.Index + 1;
        MineBlock(newBlock);
        Chain.Add(newBlock);
    }

    public bool IsValid()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            Block currentBlock = Chain[i];
            Block previousBlock = Chain[i - 1];

            if (currentBlock.Hash != currentBlock.CalculateHash())
            {
                return false;
            }

            if (currentBlock.PreviousHash != previousBlock.Hash)
            {
                return false;
            }
        }
        return true;
    }


    public void SaveToFile(string fileName)
    {
        string json = JsonSerializer.Serialize(this.Chain, new JsonSerializerOptions { WriteIndented = true });
        try
        {
            File.WriteAllText(fileName, json);
            Console.WriteLine($"Blockchain saved to {fileName}");
        }       
        catch (UnauthorizedAccessException) 
        {
            Console.WriteLine($"Access denied when trying to save blockchain to {fileName}");
        }        
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"Access denied when trying to save blockchain to {fileName}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error when saving blockchain to {fileName}: {ex.Message}");
        }
        catch (System.Exception)
        {
            Console.WriteLine($"Error saving blockchain to {fileName}");
        }
    }

    public void LoadFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"File not found: {fileName}");
            return;
        }

        try
        {
            string json = File.ReadAllText(fileName);

            this.Chain = JsonSerializer.Deserialize<List<Block>>(json) ?? new();

            if (this.Chain?.Count == 0)
            {
                InitializeChain();
                AddGenesisBlock();
            }
            Console.WriteLine($"Blockchain loaded from {fileName}");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Access denied when trying to load blockchain from {fileName}");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"Directory not found when trying to load blockchain from {fileName}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error when loading blockchain from {fileName}: {ex.Message}");
        }
        catch (System.Exception)
        {
            Console.WriteLine($"Error loading blockchain from {fileName}");
        }
    }

}