using AsliHalwai.DbMigrator;

var options = MigratorOptions.Parse(args);

if (options.ShowHelp)
{
    MigratorOptions.PrintHelp();
    return 0;
}

try
{
    return await Migrator.RunAsync(options);
}
catch (Exception ex)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine("MIGRATION FAILED");
    Console.Error.WriteLine(ex.Message);

    if (options.Verbose)
        Console.Error.WriteLine(ex);

    return 1;
}
