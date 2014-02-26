// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Bsw.Wpf.Utilities.Services;
using Bsw.Wpf.Utilities.ViewModels;
using FluentAssertions;
using Microsoft.Practices.Prism.Events;
using MsBw.MsBwUtility;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bsw.Wpf.Testing.Utilities.ViewModels
{
    public abstract class BaseViewModelTest
    {
        Dictionary<string, ValidationAttribute[]> _validators;
        protected List<string> AllBusyIndicatorMessagesShown { get; private set; }
        protected IDisplayMessageBox MessageBoxMock { get; private set; }
        protected IControlBusyIndicator ControlBusyMock { get; private set; }
        protected ICloseCurrentWindow CloseCurrentWindowMock { get; private set; }
        protected IDisplayViewAsDialog ViewAsDialogMock { get; private set; }
        protected Tuple<string, DialogType?> DisplayedMessage { get; private set; }
        protected Exception DisplayedErrorMessage { get; private set; }
        bool? _busyIndicatorLeftOpen;
        protected IEventAggregator EventAggregator { get; private set; }
        protected int NumWindowsClosed { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            EventAggregator = new EventAggregator();
            _busyIndicatorLeftOpen = null;
            InstantiateMocks();
            SetupMessageBoxMock();
            SetupBusyIndicatorMock();
            SetupCloseCurrentWindowMock();
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        void SetupCloseCurrentWindowMock()
        {
            NumWindowsClosed = 0;
            CloseCurrentWindowMock.Stub(c => c.Close())
                                  .Do(new Action(() => NumWindowsClosed++));
        }

        void SetupMessageBoxMock()
        {
            DisplayedMessage = null;
            DisplayedErrorMessage = null;
            MessageBoxMock.Stub(m => m.Display(null,
                                               DialogType.Information))
                          .IgnoreArguments()
                          .Do(new Action<string, DialogType>(
                                  (cap,
                                   dialogType) => DisplayedMessage = new Tuple<string, DialogType?>(cap,
                                                                                                    dialogType)));
            MessageBoxMock.Stub(m => m.DisplayError(null,
                                                    null))
                          .IgnoreArguments()
                          .Do(new Action<string, Exception>((msg,
                                                             e) =>
                                                            DisplayedErrorMessage = e));
        }

        void SetupBusyIndicatorMock()
        {
            AllBusyIndicatorMessagesShown = new List<string>();
            ControlBusyMock.Stub(c => c.Show(null))
                           .IgnoreArguments()
                           .Do(new Func<string, IControlBusyIndicator>(msg =>
                                                                       {
                                                                           _busyIndicatorLeftOpen = true;
                                                                           AllBusyIndicatorMessagesShown.Add(msg);
                                                                           return ControlBusyMock;
                                                                       }));
            ControlBusyMock.Stub(c => c.Dispose())
                           .Do(new Action(() =>
                                          {
                                              if (!_busyIndicatorLeftOpen.HasValue)
                                              {
                                                  Assert.Fail(
                                                              "Tried to hide busy indicator but it wasn't shown in the first place");
                                              }
                                              _busyIndicatorLeftOpen = false;
                                          }));
        }

        protected void AssertBusyIndicatorShownAndHidden()
        {
            _busyIndicatorLeftOpen
                .Should()
                .HaveValue("We expected usage of the busy indicator, but it was not used");
            Debug.Assert(_busyIndicatorLeftOpen != null,
                         "_busyIndicatorLeftOpen != null");
            if (!_busyIndicatorLeftOpen.Value) return;
            var danglingMessages = AllBusyIndicatorMessagesShown.Aggregate((m1,
                                                                            m2) => m1 + ", " + m2);
            Assert.Fail(
                        "Expected a clean busy indicator stack, but the following messages were shown and not hidden: {0}",
                        danglingMessages);
        }

        void InstantiateMocks()
        {
            MessageBoxMock = MockRepository.GenerateMock<IDisplayMessageBox>();
            ControlBusyMock = MockRepository.GenerateMock<IControlBusyIndicator>();
            ViewAsDialogMock = MockRepository.GenerateMock<IDisplayViewAsDialog>();
            CloseCurrentWindowMock = MockRepository.GenerateMock<ICloseCurrentWindow>();
        }

        protected void SetupValidatorAttributes<TTypeUnderTest>()
        {
            _validators = typeof (TTypeUnderTest)
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name,
                              GetValidations);
        }

        protected IEnumerable<string> GetFailingValidationMessages<TTypeUnderTest>(TTypeUnderTest obj,
                                                                                   Expression
                                                                                       <Func<TTypeUnderTest, object>>
                                                                                       lambda)
            where TTypeUnderTest : ValidationViewModelBase
        {
            var propertyInfo = lambda.ToPropertyInfo();
            var propertyName = propertyInfo.Name;
            var value = lambda.Compile().Invoke(obj);
            if (!_validators.ContainsKey(propertyName))
            {
                return Enumerable.Empty<string>();
            }
            return _validators[propertyName]
                .Select(v => obj.GetValidationResult(v,
                                                     value))
                .Where(r => r != ValidationResult.Success)
                .Select(v => v.ErrorMessage);
        }

        static ValidationAttribute[] GetValidations(PropertyInfo property)
        {
            return (ValidationAttribute[]) property.GetCustomAttributes(typeof (ValidationAttribute),
                                                                        true);
        }
    }
}