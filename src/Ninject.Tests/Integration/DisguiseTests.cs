using System;
using System.Linq;
using System.Text;
using Moq;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration
{
    public class DisguiseTestContext
    {
        protected readonly StandardKernel kernel;

        public DisguiseTestContext()
        {
            kernel = new StandardKernel();
        }
    }

    class DisguiseTests : DisguiseTestContext
    {
        [Fact]
        public void cannot_redisguise_without_removing()
        {
            kernel.PrepareDisguise();

            Assert.Throws(typeof(InvalidOperationException),
                () =>
                {
                    kernel.PrepareDisguise();
                });
        }

        [Fact]
        public void can_disguise_a_service()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<Ninja>();

            kernel.Get<IWarrior>().ShouldBeInstanceOf<Ninja>();

            kernel.PrepareDisguise();

            Mock<IWarrior> disguise = new Mock<IWarrior>();

            kernel.Bind<IWarrior>().ToConstant(disguise.Object);

            kernel.Get<IWarrior>().ShouldNotBeInstanceOf<Ninja>();
            kernel.Get<IWarrior>().ShouldBe(disguise.Object);
        }

        [Fact]
        public void can_undisguise_a_service()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<Ninja>();

            kernel.PrepareDisguise();

            Mock<IWarrior> disguise = new Mock<IWarrior>();

            kernel.Bind<IWarrior>().ToConstant(disguise.Object);

            kernel.RemoveDisguise();

            kernel.Get<IWarrior>().ShouldBeInstanceOf<Ninja>();
        }

        [Fact]
        public void can_reundisguise()
        {
            //  Cleanup code that removes disguises should be robust
            // towards safe overuse

            kernel.PrepareDisguise();

            kernel.RemoveDisguise();
            kernel.RemoveDisguise();
        }
    }
}
