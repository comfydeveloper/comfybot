using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ComfyBot.Application.Annotations;
using ComfyBot.Application.Shared;
using ComfyBot.Application.Shared.Extensions;
using NUnit.Framework;
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

        Assert.AreEqual(count, target.Count);
    }

    [TestCase("text1")]
    [TestCase("text2")]
    public void ToTextModelsShouldMapStringsToTextModels(string text)
    {
        string[] source = [text, ""];

        IEnumerable<TextModel> result = source.ToTextModels().ToArray();

        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(text, result.First().Text);
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
        Assert.AreEqual(1, callCount);
        models.Remove(model);
        model.Test();
        Assert.AreEqual(1, callCount);
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
        Assert.AreEqual(1, callCount);
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