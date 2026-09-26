using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

if (args.Length != 2) throw new ArgumentException("Uso: Doc72SourceValidator <manifest.json> <repo-root>");
var manifestPath = Path.GetFullPath(args[0]);
var repoRoot = Path.GetFullPath(args[1]);
using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
var manifest = document.RootElement;
var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".git", "bin", "obj", "packages", "node_modules" };
var files = Directory.EnumerateFiles(repoRoot, "*.vb", SearchOption.AllDirectories)
    .Where(path => !path.Split(Path.DirectorySeparatorChar).Any(excluded.Contains));
var roots = files.Select(path => VisualBasicSyntaxTree.ParseText(File.ReadAllText(path), path: path).GetRoot()).ToArray();
var typeBlocks = roots.SelectMany(root => root.DescendantNodes().Where(node => node is ClassBlockSyntax or InterfaceBlockSyntax)).ToArray();
var errors = new List<string>();

string TypeName(TypeSyntax? type)
{
    if (type is null) return "System.Void";
    if (type is PredefinedTypeSyntax predefined)
    {
        return predefined.Keyword.Kind() switch
        {
            SyntaxKind.StringKeyword => "System.String",
            SyntaxKind.BooleanKeyword => "System.Boolean",
            SyntaxKind.IntegerKeyword => "System.Int32",
            SyntaxKind.LongKeyword => "System.Int64",
            SyntaxKind.DateKeyword => "System.DateTime",
            _ => predefined.Keyword.ValueText
        };
    }
    if (type is GenericNameSyntax generic)
        return $"{generic.Identifier.ValueText}<{string.Join(',', generic.TypeArgumentList.Arguments.Select(TypeName))}>";
    if (type is IdentifierNameSyntax identifier)
    {
        return identifier.Identifier.ValueText switch
        {
            "String" => "System.String",
            "Boolean" => "System.Boolean",
            "Integer" => "System.Int32",
            "Long" => "System.Int64",
            "DateTime" => "System.DateTime",
            _ => identifier.Identifier.ValueText
        };
    }
    var value = type.ToString().Replace("Global.", "").Replace(" ", "");
    return value.StartsWith("System.", StringComparison.Ordinal) ? value : value;
}

string BlockName(SyntaxNode node) => node switch
{
    ClassBlockSyntax c => c.ClassStatement.Identifier.ValueText,
    InterfaceBlockSyntax i => i.InterfaceStatement.Identifier.ValueText,
    _ => string.Empty
};
SyntaxNode[] FindTypes(string name) => typeBlocks.Where(block => BlockName(block) == name).ToArray();

foreach (var symbol in manifest.GetProperty("dotnetSymbols").EnumerateObject())
{
    var expected = symbol.Value;
    var typeName = expected.GetProperty("type").GetString()!;
    var methodName = expected.GetProperty("method").GetString()!;
    var types = FindTypes(typeName);
    if (types.Length == 0) { errors.Add($"{symbol.Name}: tipo {typeName} inexistente"); continue; }
    var expectedParams = expected.GetProperty("parameters").EnumerateArray().Select(x => x.GetString()!).ToArray();
    var expectedReturn = expected.GetProperty("return").GetString()!;
    var expectedFile = expected.TryGetProperty("file", out var fileElement)
        ? Path.GetFullPath(Path.Combine(repoRoot, fileElement.GetString()!.Replace('/', Path.DirectorySeparatorChar)))
        : null;
    var expectedNonPublic = expected.TryGetProperty("visibility", out var visibility) && visibility.GetString() == "nonpublic";
    var candidates = types.SelectMany(type => type.DescendantNodes().OfType<MethodStatementSyntax>())
        .Where(m => m.Identifier.ValueText == methodName)
        .Where(m => expectedFile is null || string.Equals(Path.GetFullPath(m.SyntaxTree.FilePath), expectedFile, StringComparison.OrdinalIgnoreCase))
        .ToArray();
    var matches = candidates.Where(method =>
    {
        var actualParams = method.ParameterList?.Parameters.Select(parameter =>
        {
            var simple = parameter.AsClause as SimpleAsClauseSyntax;
            var name = TypeName(simple?.Type);
            return parameter.Modifiers.Any(SyntaxKind.ByRefKeyword) ? name + "&" : name;
        }).ToArray() ?? Array.Empty<string>();
        var actualReturn = method.SubOrFunctionKeyword.IsKind(SyntaxKind.SubKeyword)
            ? "System.Void"
            : TypeName((method.AsClause as SimpleAsClauseSyntax)?.Type);
        var nonPublic = method.Modifiers.Any(SyntaxKind.PrivateKeyword) || method.Modifiers.Any(SyntaxKind.FriendKeyword) || method.Modifiers.Any(SyntaxKind.ProtectedKeyword);
        return actualParams.SequenceEqual(expectedParams) && actualReturn == expectedReturn && (!expectedNonPublic || nonPublic);
    }).ToArray();
    if (matches.Length != 1) errors.Add($"{symbol.Name}: firma inexistente o ambigua en {typeName}.{methodName} (coincidencias={matches.Length})");
}

foreach (var dto in manifest.GetProperty("dotnetTypes").EnumerateObject())
{
    var types = FindTypes(dto.Name);
    if (types.Length == 0) { errors.Add($"DTO {dto.Name} inexistente"); continue; }
    var properties = types.SelectMany(type => type.DescendantNodes().OfType<PropertyStatementSyntax>()).ToArray();
    foreach (var contract in dto.Value.EnumerateArray().Select(x => x.GetString()!))
    {
        var parts = contract.Split(':', 2);
        var property = properties.SingleOrDefault(p => p.Identifier.ValueText == parts[0]);
        if (property is null) { errors.Add($"DTO {dto.Name}: propiedad {parts[0]} inexistente"); continue; }
        var actual = TypeName((property.AsClause as SimpleAsClauseSyntax)?.Type);
        if (actual != parts[1]) errors.Add($"DTO {dto.Name}.{parts[0]}: tipo {actual}, esperado {parts[1]}");
    }
}

if (errors.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
var contractName = manifest.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : "DOC-72";
Console.WriteLine($"{contractName} Roslyn symbols: PASS ({manifest.GetProperty("dotnetSymbols").EnumerateObject().Count()} firmas, {manifest.GetProperty("dotnetTypes").EnumerateObject().Count()} tipos).");
