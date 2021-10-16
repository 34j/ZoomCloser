using NUnit.Framework;
using ZoomCloser.Services;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZoomCloser.Tests
{
    [TestFixture(4)]
    public class JudgingtByRatioServiceTest
    {
        public JudgingtByRatioServiceTest(int x)
        {
            this.x = x;
        }

        private IJudgingWhetherToExitByRatioService judger;
        private int x = 0;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            judger = new JudgingWhetherToExitByRatioService(x);
        }



        [Test]
        public void JudgeTest([Random(0, 10000, 5)][Values(0, 1, int.MaxValue)] int count)
        {
            //Act
            judger.Judge(count);

            //Assert
            Assert.Multiple(() =>
            {
                if (judger.MaximumCountToExit > judger.ThresholdToActivation)
                {
                    Assert.IsFalse(judger.Judge(judger.MaximumCountToExit + 1));
                    Assert.IsTrue(judger.Judge(judger.MaximumCountToExit));
                }
                else
                {
                    Assert.IsFalse(judger.Judge(0));
                }
            });
        }

        public static IEnumerable<IEnumerable<int>> MaximumCountTestSource = Enumerable.Repeat(0, 5).Select(_ => Enumerable.Repeat(0, 5).Select(_ => new Random().Next(0, 10000)));
        [TestCaseSource(nameof(MaximumCountTestSource))]
        public void MaximumCountTest(IEnumerable<int> counts)
        {
            counts = counts.ToList();

            //Act
            foreach (int count in counts)
            {
                _ = judger.Judge(count);
                TestContext.WriteLine(count);
            }

            //Assert
            Assert.AreEqual(counts.Max(), judger.MaximumCount);
        }

        [Test]
        [Repeat(3)]
        public void ThresholdTest()
        {
            int max = judger.ThresholdToActivation;
            int testCase = new Random().Next(0, max + 1);

            //Act
            judger.Judge(testCase);

            //Assert
            Assert.IsFalse(judger.IsOverThresholdToActivation);
        }

        [Test]
        public void ResetTest([Random(0, 10000, 5)][Values(0, int.MaxValue)] int count)
        {
            //Act
            judger.Judge(count);
            judger.Reset();

            //Assert
            Assert.AreEqual(0, judger.CurrentCount);
            Assert.AreEqual(0, judger.MaximumCount);
        }
    }

    [TestFixture]
    public class ZoomExitByRatioTest
    {
        [Test]
        public void ExitTest()
        {
            Assert.Multiple(async () =>
            {
                bool flag = false;

                var handling = new Mock<IZoomHandlingService>();
                handling.Setup(s => s.Exit()).Callback(() => flag = true);
                var judger = new Mock<IJudgingWhetherToExitByRatioService>();
                var closer = new ZoomExitByRatioService(handling.Object, judger.Object, new System.Timers.Timer());

                //First Act : Check if Exit() is called in false condition.
                judger.Setup(s => s.Judge(It.IsAny<int>())).Returns(false);

                await Task.Delay(1000).ConfigureAwait(false);
                //Assert
                Assert.IsFalse(flag);
                flag = false;

                //Second Act : Check if Exit() is called in true condition.
                judger.Setup(s => s.Judge(It.IsAny<int>())).Returns(true);

                await Task.Delay(1000).ConfigureAwait(false);

                //Assert
                Assert.IsTrue(flag);
            });
        }
    }
}