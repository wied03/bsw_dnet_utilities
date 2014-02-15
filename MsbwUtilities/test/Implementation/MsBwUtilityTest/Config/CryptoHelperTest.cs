// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MsBw.MsBwUtility;
using MsBw.MsBwUtility.Config;
using NUnit.Framework;

namespace MsBwUtilityTest.Config
{
    [TestFixture]
    public class CryptoHelperTest : BaseTest
    {
        ICryptoHelper _crypto;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _crypto = new CryptoHelper();
        }

        #endregion

        #region Utility Methods

        #endregion

        #region Tests

        [Test]
        public void Encrypt_red_bytes()
        {
            // arrange
            var bytes = "AB".ToByteArrayFromHex();

            // act
            var encrypted = _crypto.Encrypt(bytes);
            var decrypted = _crypto.DecryptAsBytes(encrypted);

            // assert
            var asHex = decrypted.ToHex();
            asHex
                .Should()
                .Be("AB");
            encrypted
                .Should()
                .NotBe("AB",
                       "Crypto?");
        }

        [Test]
        public void Encrypt_nullable_int_has_value()
        {
            // arrange
            int? stuff = 2;

            // act
            var encrypted = _crypto.Encrypt(stuff);
            var decrypted = _crypto.DecryptNullableInt(encrypted);

            // assert
            decrypted
                .Should()
                .Be(2);
            encrypted
                .Should()
                .NotBe("2");
        }

        [Test]
        public void Encrypt_nullable_int_no_value()
        {
            // arrange
            int? stuff = null;

            // act
// ReSharper disable once ExpressionIsAlwaysNull
            var encrypted = _crypto.Encrypt(stuff);
            var decrypted = _crypto.DecryptNullableInt(encrypted);

            // assert
            decrypted
                .Should()
                .NotHaveValue();
            encrypted
                .Should()
                .BeNull();
        }

        [Test]
        public void Encrypt_string()
        {
            // arrange
            const string text = "AB";

            // act
            var encrypted = _crypto.Encrypt(text);
            var decrypted = _crypto.Decrypt(encrypted);

            // assert
            decrypted
                .Should()
                .Be("AB");
            encrypted
                .Should()
                .NotBe("AB",
                       "Crypto?");
        }

        #endregion
    }
}