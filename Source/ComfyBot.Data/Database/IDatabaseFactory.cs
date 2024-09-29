using ComfyBot.Data.Wrappers;
using System;

namespace ComfyBot.Data.Database;

[Obsolete]
public interface IDatabaseFactory
{
    IDatabase Create();
}