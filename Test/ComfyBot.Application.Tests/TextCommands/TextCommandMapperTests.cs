using System.Linq;
using ComfyBot.Application.Shared;
using ComfyBot.Application.TextCommands;
using ComfyBot.Data.Models;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.TextCommands;

[TestFixture]
public class TextCommandMapperTests
{
    private TextCommand entity;
    private TextCommandModel model;

    private TextCommandMapper mapper;

    [SetUp]
    public void Setup()
    {
        entity = new TextCommand();
        model = new TextCommandModel();

        mapper = new TextCommandMapper();
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    [TestCase("00000000-0000-0000-0000-000000000001")]
    public void MapToEntityShouldMapId(string id)
    {
        model.Id = id;

        mapper.MapToEntity(model, entity);

        Assert.AreEqual(id, entity.Id);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void MapToEntityShouldMapTimeout(int timeout)
    {
        model.Timeout = timeout;

        mapper.MapToEntity(model, entity);

        Assert.AreEqual(timeout, entity.TimeoutInSeconds);
    }

    [TestCase("reply1")]
    [TestCase("reply2")]
    public void MapToEntityShouldMapReplies(string reply)
    {
        model.Replies.Add(new TextModel { Text = reply });

        mapper.MapToEntity(model, entity);

        Assert.AreEqual(1, entity.Replies.Count);
        Assert.AreEqual(reply, entity.Replies.First());
    }

    [TestCase("reply1")]
    [TestCase("reply2")]
    public void MapToEntityShouldMapCommands(string reply)
    {
        model.Commands.Add(new TextModel { Text = reply });

        mapper.MapToEntity(model, entity);

        Assert.AreEqual(1, entity.Commands.Count);
        Assert.AreEqual(reply, entity.Commands.First());
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    [TestCase("00000000-0000-0000-0000-000000000001")]
    public void MapToModelShouldMapId(string id)
    {
        entity.Id = id;

        mapper.MapToModel(entity, model);

        Assert.AreEqual(id, model.Id);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void MapToModelShouldMapTimeout(int timeout)
    {
        entity.TimeoutInSeconds = timeout;

        mapper.MapToModel(entity, model);

        Assert.AreEqual(timeout, model.Timeout);
    }

    [TestCase("reply1")]
    [TestCase("reply2")]
    public void MapToModelShouldMapReplies(string reply)
    {
        entity.Replies.Add(reply);

        mapper.MapToModel(entity, model);

        Assert.AreEqual(1, model.Replies.Count);
        Assert.AreEqual(reply, model.Replies.First().Text);
    }

    [Test]
    public void MapToModelShouldOrderReplies()
    {
        entity.Replies.Add("B");
        entity.Replies.Add("C");
        entity.Replies.Add("A");

        mapper.MapToModel(entity, model);

        string[] replies = model.Replies.Select(r => r.Text).ToArray();
        Assert.AreEqual("A", replies[0]);
        Assert.AreEqual("B", replies[1]);
        Assert.AreEqual("C", replies[2]);
    }

    [Test]
    public void MapToModelShouldOrderCommands()
    {
        entity.Commands.Add("B");
        entity.Commands.Add("C");
        entity.Commands.Add("A");

        mapper.MapToModel(entity, model);

        string[] commands = model.Commands.Select(r => r.Text).ToArray();
        Assert.AreEqual("A", commands[0]);
        Assert.AreEqual("B", commands[1]);
        Assert.AreEqual("C", commands[2]);
    }

    [TestCase("reply1")]
    [TestCase("reply2")]
    public void MapToModelShouldMapCommands(string reply)
    {
        entity.Commands.Add(reply);

        mapper.MapToModel(entity, model);

        Assert.AreEqual(1, model.Commands.Count);
        Assert.AreEqual(reply, model.Commands.First().Text);
    }
}