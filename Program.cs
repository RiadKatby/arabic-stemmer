var words = File.ReadAllLines("samples/Arabic Text.txt").SelectMany(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)).ToArray();

var stemmer = new ArabicStemmer.Stemmer("files/");

var result = new List<(string, string)>();
foreach (var word in words)
{
    result.Add((word, stemmer.stemWord(word)));
}

File.WriteAllLines("Arabic Text-out.txt", result.Select(x => string.Join(", ", x.Item1, x.Item2)));