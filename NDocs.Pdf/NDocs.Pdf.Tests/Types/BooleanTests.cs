using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NDocs.Pdf.Tests.Types
{
    [TestFixture]
    public class BooleanTests
    {
        [Test]
        public void IEquatableIsImplementedProperly()
        {
            //var @true = Boolean.True;
            //var @false = Boolean.False;
            //var true2 = Boolean.True;
            //var true3 = true;
            //var true4 = (object)true;
            //var false2 = Boolean.False;
            //var false3 = false;
            //var false4 = (object)false;

            //Assert.IsTrue(@true.Equals(true2));
            //Assert.IsTrue(@true.Equals(true3));
            //Assert.IsTrue(@true.Equals(true4));
            //Assert.IsFalse(@true.Equals(false2));
            //Assert.IsFalse(@true.Equals(false3));
            //Assert.IsFalse(@true.Equals(false4));
            //Assert.IsFalse(@true.Equals("true"));
            //Assert.IsTrue(@false.Equals(false2));
            //Assert.IsTrue(@false.Equals(false3));
            //Assert.IsTrue(@false.Equals(false4));
            //Assert.IsFalse(@false.Equals(true2));
            //Assert.IsFalse(@false.Equals(true3));
            //Assert.IsFalse(@false.Equals(true4));
            //Assert.IsFalse(@false.Equals("false"));
        }

        [Test]
        public void IComparableIsImplementedProperly()
        {

        }

        [Test]
        public void CanGetProperHashCode()
        {
            //Assert.AreEqual(1, Boolean.True.GetHashCode());
            //Assert.AreEqual(0, Boolean.False.GetHashCode());
        }
    }
}
