Set-Variable output -option Constant -value "nupkgs"
Set-Variable source -option Constant -value "C:\NuGet\packages"

# Core
Invoke-Expression "dotnet pack .\Sources\Carcass.Core\Carcass.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Metadata
Invoke-Expression "dotnet pack .\Sources\Carcass.Metadata\Carcass.Metadata.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Metadata.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Swashbuckle
Invoke-Expression "dotnet pack .\Sources\Carcass.Swashbuckle\Carcass.Swashbuckle.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Swashbuckle.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Cli.Logging.Core
Invoke-Expression "dotnet pack .\Sources\Cli\Logging\Carcass.Cli.Logging.Core\Carcass.Cli.Logging.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Cli.Logging.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Cli.Logging.Spectre
Invoke-Expression "dotnet pack .\Sources\Cli\Logging\Carcass.Cli.Logging.Spectre\Carcass.Cli.Logging.Spectre.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Cli.Logging.Spectre.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Data.Core
Invoke-Expression "dotnet pack .\Sources\Data\Carcass.Data.Core\Carcass.Data.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Data.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Data.Elasticsearch
Invoke-Expression "dotnet pack .\Sources\Data\Carcass.Data.Elasticsearch\Carcass.Data.Elasticsearch.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Data.Elasticsearch.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Data.EntityFrameworkCore
Invoke-Expression "dotnet pack .\Sources\Data\Carcass.Data.EntityFrameworkCore\Carcass.Data.EntityFrameworkCore.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Data.EntityFrameworkCore.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Data.EventStoreDb
Invoke-Expression "dotnet pack .\Sources\Data\Carcass.Data.EventStoreDb\Carcass.Data.EventStoreDb.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Data.EventStoreDb.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Data.MongoDb
Invoke-Expression "dotnet pack .\Sources\Data\Carcass.Data.MongoDb\Carcass.Data.MongoDb.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Data.MongoDb.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# DistributedCache.Core
Invoke-Expression "dotnet pack .\Sources\DistributedCache\Carcass.DistributedCache.Core\Carcass.DistributedCache.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.DistributedCache.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# DistributedCache.Redis
Invoke-Expression "dotnet pack .\Sources\DistributedCache\Carcass.DistributedCache.Redis\Carcass.DistributedCache.Redis.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.DistributedCache.Redis.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# FrontMatter.Core
Invoke-Expression "dotnet pack .\Sources\FrontMatter\Carcass.FrontMatter.Core\Carcass.FrontMatter.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.FrontMatter.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# FrontMatter.Markdown
Invoke-Expression "dotnet pack .\Sources\FrontMatter\Carcass.FrontMatter.Markdown\Carcass.FrontMatter.Markdown.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.FrontMatter.Markdown.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# FrontMatter.Razor
Invoke-Expression "dotnet pack .\Sources\FrontMatter\Carcass.FrontMatter.Razor\Carcass.FrontMatter.Razor.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.FrontMatter.Razor.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Json.Core
Invoke-Expression "dotnet pack .\Sources\Json\Carcass.Json.Core\Carcass.Json.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Json.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Json.NewtonsoftJson
Invoke-Expression "dotnet pack .\Sources\Json\Carcass.Json.NewtonsoftJson\Carcass.Json.NewtonsoftJson.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Json.NewtonsoftJson.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Json.SystemTextJson
Invoke-Expression "dotnet pack .\Sources\Json\Carcass.Json.SystemTextJson\Carcass.Json.SystemTextJson.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Json.SystemTextJson.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Logging.Core
Invoke-Expression "dotnet pack .\Sources\Logging\Carcass.Logging.Core\Carcass.Logging.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Logging.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Mapping.AutoMapper
Invoke-Expression "dotnet pack .\Sources\Mapping\Carcass.Mapping.AutoMapper\Carcass.Mapping.AutoMapper.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Mapping.AutoMapper.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Mapping.Core
Invoke-Expression "dotnet pack .\Sources\Mapping\Carcass.Mapping.Core\Carcass.Mapping.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Mapping.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Multitenancy.Core
Invoke-Expression "dotnet pack .\Sources\Multitenancy\Carcass.Multitenancy.Core\Carcass.Multitenancy.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Multitenancy.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Mvc.Core
Invoke-Expression "dotnet pack .\Sources\Mvc\Carcass.Mvc.Core\Carcass.Mvc.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Mvc.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Mvc.Razor.Rendering
Invoke-Expression "dotnet pack .\Sources\Mvc\Razor\Carcass.Mvc.Razor.Rendering\Carcass.Mvc.Razor.Rendering.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Mvc.Razor.Rendering.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# ObjectStorage.Core
Invoke-Expression "dotnet pack .\Sources\ObjectStorage\Carcass.ObjectStorage.Core\Carcass.ObjectStorage.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.ObjectStorage.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# ObjectStorage.Minio
Invoke-Expression "dotnet pack .\Sources\ObjectStorage\Carcass.ObjectStorage.Minio\Carcass.ObjectStorage.Minio.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.ObjectStorage.Minio.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Yaml.Core
Invoke-Expression "dotnet pack .\Sources\Json\Carcass.Json.Core\Carcass.Json.Core.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Json.Core.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"

# Yaml.YamlDotNet
Invoke-Expression "dotnet pack .\Sources\Yaml\Carcass.Yaml.YamlDotNet\Carcass.Yaml.YamlDotNet.csproj -p:PackageVersion=$($args[0]).$($args[1]).$($args[2]) --output $output"
Invoke-Expression "nuget add .\nupkgs\Carcass.Yaml.YamlDotNet.$($args[0]).$($args[1]).$($args[2]).nupkg -source $source"