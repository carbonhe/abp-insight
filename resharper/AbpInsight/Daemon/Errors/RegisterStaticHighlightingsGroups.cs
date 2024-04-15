using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AbpInsight.Daemon.Errors;

[RegisterStaticHighlightingsGroup("Abp Errors", true)]
public class AbpErrors;

[RegisterStaticHighlightingsGroup("Abp Warnings", true)]
public class AbpWarnings;

[RegisterStaticHighlightingsGroup("Abp Gutter Marks", true)]
public class AbpGutterMarks;