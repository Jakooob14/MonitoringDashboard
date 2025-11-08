namespace MonitoringDashboard.Helpers;

public static class RandomWordGenerator
{
    private static readonly string[] Nouns = new[]
    {
        "potato", "waffle", "llama", "nugget", "toaster", "penguin", "burrito", "pickle", "unicorn",
        "sock", "banana", "blobfish", "cupcake", "sloth", "toenail", "donut", "spatula", "meatball",
        "hamster", "muffin", "pancake", "otter", "cactus", "marshmallow", "platypus", "biscuit",
        "taco", "snail", "noodle", "pigeon", "mushroom", "dustbunny", "crayon", "scooter", "gravyboat",
        "pineapple", "goblin", "pebble", "baguette", "cabbage", "lawnmower", "trampoline", "bubble",
        "cheeseball", "keyboard", "microwave", "sandwich", "octopus", "raccoon", "blender", "zucchini"
    };

    private static readonly string[] Adjectives = new[]
    {
        "sparkly", "grumpy", "funky", "confused", "dancing", "crunchy", "awkward", "soggy", "turbo",
        "sleepy", "sassy", "fluffy", "derpy", "chaotic", "melty", "noisy", "sneaky", "chunky", "tiny",
        "explosive", "boiling", "lazy", "nervous", "radiant", "mysterious", "hyperactive", "shiny",
        "dramatic", "goofy", "crispy", "bewildered", "crooked", "hollow", "jumpy", "sticky", "wonky",
        "frantic", "giggly", "thirsty", "electric", "plastic", "gravy-powered", "half-baked", "galactic",
        "quantum", "pixelated", "haunted", "boneless", "questionable", "feral"
    };
    
    public static string GenerateRandomNoun(bool capitalizeFirstLetter = false)
    {
        return GenerateRandomWordFromList(Nouns, capitalizeFirstLetter);
    }
    
    public static string GenerateRandomAdjective(bool capitalizeFirstLetter = false)
    {
        return GenerateRandomWordFromList(Adjectives, capitalizeFirstLetter);
    }
    
    private static string GenerateRandomWordFromList(string[] words, bool capitalizeFirstLetter = false)
    {
        var random = new Random();
        var word = words[random.Next(words.Length)];
        return capitalizeFirstLetter ? char.ToUpper(word[0]) + word.Substring(1) : word;
    }
}