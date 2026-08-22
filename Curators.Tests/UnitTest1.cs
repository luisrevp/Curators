using Curators.Domain;
using Curators.Domain.Aggregates;
using Curators.Domain.Aggregates.ConversationAggregate;
using Curators.Domain.ValueObjects;

namespace Curators.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Names_ShoulBeEqual()
        {
            // arrange
            FullName fullname1 = FullName.Create("Calor", "Revi");
            // second (should break)
            FullName fullname2 = FullName.Create("Carlos", "Revi");
            // assert
            Assert.NotEqual(fullname1, fullname2);
        }

        [Fact]
        public void Names_ShouldNotBeEqual()
        {
            // arrange
            FullName fullname1 = FullName.Create("Luis", "Revilla");
            // second (should break)
            FullName fullname2 = FullName.Create("Luis", "Revilla", "luisrevp");
            // assert
            Assert.NotEqual(fullname1, fullname2);
        }

        [Fact]
        public void Names_ShouldThrowError() 
        {
            // act and assert: Exception
            Assert.Throws<ArgumentException>(() => FullName.Create("Lu", "R"));
        }
    }
}
