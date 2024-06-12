﻿using BindingEngine.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Illusion.Utility;


namespace Illusion.Utility.Tests
{
    [TestClass]
    public class WeakMethodBindingTests
    {
        private const string Name1 = "Yohan1";
        private const string Name2 = "Yohan2";

        [TestMethod]
        public void Test_WeakMethodBinding_SourceMethod()
        {
            var viewModel = new TestViewModel();
            var button = new Button();

            new WeakMethodBinding(viewModel, null, button, null)
                .Initialize<WeakMethodBinding>()
                .AttachTargetMethod("AddAge", null)
                .AttachTargetCanInvokeMethod("CanAddAge", null)
                .AttachSourceEvent("Click");

            Assert.AreEqual(0, viewModel.Age);

            button.PerformClick();

            Assert.AreEqual(1, viewModel.Age);
        }

        [TestMethod]
        public void Test_WeakMethodBinding_SourceMethod_2()
        {
            var viewModel2 = new TestViewModel2();
            var viewModel = new TestViewModel();
            var button = new Button();

            var binding = new WeakMethodBinding(viewModel2, null, button, null)
                .Initialize<WeakMethodBinding>()
                .AttachTargetMethod(new BindContext(viewModel, null), "AddAge", null)
                .AttachTargetCanInvokeMethod(new BindContext(viewModel, null), "CanAddAge", null)
                .AttachSourceEvent("Click")
                .OfType<WeakMethodBinding>();

            Assert.AreEqual(0, viewModel.Age);

            button.PerformClick();

            Assert.AreEqual(1, viewModel.Age);

            button.PerformClick();
            Assert.AreEqual(2, viewModel.Age);

            button.PerformClick();
            Assert.AreEqual(2, viewModel.Age);

            binding.DetachTargetCanInvokeMethod();
            button.PerformClick();
            Assert.AreEqual(3, viewModel.Age);

            binding.DetachTargetMethod();
            button.PerformClick();
            Assert.AreEqual(3, viewModel.Age);
        }

        [TestMethod]
        public void Test_WeakMethodBinding_TargetMethod()
        {
            var viewModel = new TestViewModel();
            var button = new Button();

            new WeakMethodBinding(button, null, viewModel, null)
                .Initialize<WeakMethodBinding>()
                .AttachSourceMethod("AddAge", null)
                .AttachSourceCanInvokeMethod("CanAddAge", null)
                .AttachTargetEvent("Click");

            Assert.AreEqual(0, viewModel.Age);

            button.PerformClick();

            Assert.AreEqual(1, viewModel.Age);
        }

        [TestMethod]
        public void Test_WeakMethodBinding_TargetMethod_2()
        {
            var viewModel2 = new TestViewModel2();
            var viewModel = new TestViewModel();
            var button = new Button();

            var binding = new WeakMethodBinding(button, null, viewModel2, null)
                .Initialize<WeakMethodBinding>()
                .AttachSourceMethod(new BindContext(viewModel, null), "AddAge", null)
                .AttachSourceCanInvokeMethod(new BindContext(viewModel, null), "CanAddAge", null)
                .AttachTargetEvent("Click")
                .OfType<WeakMethodBinding>();

            Assert.AreEqual(0, viewModel.Age);

            button.PerformClick();

            Assert.AreEqual(1, viewModel.Age);

            button.PerformClick();
            Assert.AreEqual(2, viewModel.Age);

            button.PerformClick();
            Assert.AreEqual(2, viewModel.Age);

            binding.DetachSourceCanInvokeMethod();
            button.PerformClick();
            Assert.AreEqual(3, viewModel.Age);

            binding.DetachSourceMethod();
            button.PerformClick();
            Assert.AreEqual(3, viewModel.Age);
        }

        [TestMethod]
        public void Test_MethodInvoker_Basic()
        {
            var viewModel5 = new TestViewModel5();

            var canMethodInvoker = new MethodInvoker(new BindContext(viewModel5, "TestViewModel"), "CanAddAge", null);
            var methodInvoker = new MethodInvoker(new BindContext(viewModel5, "TestViewModel"), "AddAge", null);

            bool value = canMethodInvoker.CanInvoke();
            Assert.IsFalse(value);
            methodInvoker.Invoke();

            viewModel5.TestViewModel = new TestViewModel();
            value = canMethodInvoker.CanInvoke();
            Assert.IsTrue(value);
            methodInvoker.Invoke();

            Assert.AreEqual(1, viewModel5.TestViewModel.Age);

            var canMethodInvoker2 = new MethodInvoker(new BindContext(viewModel5, "TestViewModel"), "AddAge", null);

            MissingMemberException exception = null;
            try
            {
                canMethodInvoker2.CanInvoke();
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            var vm = new TestViewModel();

            var canMethodInvoker3 = new MethodInvoker(new BindContext(viewModel5, "TestViewModel"), "CanSetAge",
                new[] { new BindContext(vm, "Age") });
            var methodInvoker2 = new MethodInvoker(new BindContext(viewModel5, "TestViewModel"), "SetAge",
                new[] { new BindContext(vm, "Age") });

            Assert.IsTrue(canMethodInvoker3.CanInvoke());
            vm.Age = 6;
            Assert.AreNotEqual(6, viewModel5.TestViewModel.Age);

            methodInvoker2.Invoke();
            Assert.AreEqual(6, viewModel5.TestViewModel.Age);
        }
    }
}