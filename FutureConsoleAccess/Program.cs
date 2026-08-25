List<Node> nodes = new List<Node>();
Console.WriteLine("What are you thinking of right now?");
string? thought = Console.ReadLine();
if (thought is not null) nodes.Add(new Node (0, 0, thought));

foreach (Node node in nodes) 
    Console.WriteLine("Interesting, you're thinking of " + node.thought);

public struct Node
{
    public int id;
    public int tier;
    public string thought;

    public Node(int id, int tier, string thought)
    {
        this.id = id;
        this.tier = tier;
        this.thought = thought;
    }
}