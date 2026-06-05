using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ComfyBot.Application.Annotations;
using ComfyBot.Application.Shared;
using ComfyBot.Application.Shared.Extensions;
using NUnit.Framework;
using Shouldly;
using System.Collections.ObjectModel;

namespace ComfyBot.Application.Tests.Shared.Extensions;

[TestFixture]
public class CollectionExtensionsTests
{
    [TestCase(5)]
    [TestCase(10)]
    public void AddRangeShouldAddAllElementsToObservableCollection(int count)
    {
        string[] source = Enumerable.Repeat("", count).ToArray();
        ObservableCollection<string> target = [];

        target.AddRange(source);

        target.Count.ShouldBe(count);
    }

    [TestCase("text1")]
    [TestCase("text2")]
    public void ToTextModelsShouldMapStringsToTextModels(string text)
    {
        string[] source = [text, ""];

        IEnumerable<TextModel> result = source.ToTextModels().ToArray();

        result.Count().ShouldBe(2);
        result.First().Text.ShouldBe(text);
    }

    [Test]
    public void RegisterCollectionItemChangedShouldAddActionInvocationRegistration()
    {
        int callCount = 0;
        void TestMethod(object sender, PropertyChangedEventArgs e)
        {
            callCount++;
        }
        NotifyingStub model = new();
        ObservableCollection<NotifyingStub> models = [];

        models.RegisterCollectionItemChanged(TestMethod);

        models.Add(model);
        model.Test();
        callCount.ShouldBe(1);
        models.Remove(model);
        model.Test();
        callCount.ShouldBe(1);
    }

    [Test]
    public void RegisterCollectionItemChangedShouldAddActionInvocationRegistrationToPreviouslyAddedEelements()
    {
        int callCount = 0;
        void TestMethod(object sender, PropertyChangedEventArgs e)
        {
            callCount++;
        }
        NotifyingStub model = new();
        ObservableCollection<NotifyingStub> models = [model];

        models.RegisterCollectionItemChanged(TestMethod);

        model.Test();
        callCount.ShouldBe(1);
    }

    private class NotifyingStub : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Test()
        {
            this.OnPropertyChanged();
        }
    }
}