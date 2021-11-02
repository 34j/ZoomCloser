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
        public void ExitTest([Values(true, false)] bool judgeValue)
        {
            bool hasExitFunctionCalled = false;

            var handling = new Mock<IZoomHandlingService2>(MockBehavior.Strict);
            handling.Setup(s => s.RefreshParticipantCount()).ReturnsAsync(true);
            handling.Setup(s => s.Exit()).ReturnsAsync(true).Callback(() => hasExitFunctionCalled = true);
            var judger = new Mock<IJudgingWhetherToExitByRatioService>(MockBehavior.Strict);
            judger.Setup(s => s.Judge(It.IsAny<int>())).Returns(judgeValue);
            judger.Setup(s => s.Reset());

            var zoomExitByRatioService = new ZoomExitByRatioService(handling.Object, judger.Object, new System.Timers.Timer());

            Task.Delay(500).Wait();

            //Assert
            Assert.AreEqual(judgeValue, hasExitFunctionCalled);
        }
    }
}