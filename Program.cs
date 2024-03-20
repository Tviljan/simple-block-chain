using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

Blockchain simpleChain = new Blockchain();
Console.WriteLine("A simple blockchain in C#.\nCommands: add [data], validate, exit");

while (true)
{
    Console.Write("> ");
    string input = Console.ReadLine();
    string[] parts = input?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

    if (parts == null || parts.Length == 0)
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
            Console.WriteLine("Blockchain loaded from blockchain.txt");
            break;
        default:
            Console.WriteLine("Unknown command. Available commands: add [data], validate, exit");
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

    public Block(DateTime timeStamp, string previousHash, string data)
    {
        Index = 0;
        TimeStamp = timeStamp;
        PreviousHash = previousHash;
        Data = data;
        Hash = CalculateHash();
    }

    public string CalculateHash()
    {
        SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash ?? ""}-{Data}");
        byte[] outputBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToBase64String(outputBytes);
    }
}

public class Blockchain
{
    public IList<Block> Chain { set; get; }

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

    public void AddBlock(string data)
    {
        Block latestBlock = GetLatestBlock();
        Block newBlock = new Block(DateTime.Now, latestBlock.Hash, data);
        newBlock.Index = latestBlock.Index + 1;
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
        File.WriteAllText(fileName, json);
    }

    public void LoadFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"File not found: {fileName}");
            return;
        }

        string json = File.ReadAllText(fileName);
        this.Chain = JsonSerializer.Deserialize<List<Block>>(json);
        if (this.Chain == null)
        {
            InitializeChain();
            AddGenesisBlock();
        }
    }

}