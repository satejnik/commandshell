//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandShell.Helpers;

namespace CommandShell
{
    public class TwoWayEnumerator<T> : IEnumerator<T>
    {
        #region Fields

        private readonly T[] source;
        private int index;

        #endregion

        #region Constructor

        public TwoWayEnumerator(IEnumerable<T> enumeratorSource)
        {
            // ReSharper disable PossibleMultipleEnumeration
            Asserts.ArgumentNotNull(enumeratorSource, "enumeratorSource");
            source = enumeratorSource.ToArray();
            // ReSharper restore PossibleMultipleEnumeration
            index = -1;
        }

        #endregion

        #region Properties

        public bool IsFirst
        {
            get { return index < source.Length && index == 0; }
        }

        public T Previous
        {
            get
            {
                Asserts.OperationNotAllowed(index <= 0, "Previous enumeration state isn't defined.");
                return source[index - 1];
            }
        }

        public T Current
        {
            get
            {
                Asserts.OperationNotAllowed(index < 0, "Call MoveNext method before getting current enumerator state.");
                Asserts.OperationNotAllowed(index >= source.Length, "Enumerated collection contains no more elements.");
                return source[index];
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public T Next
        {
            get
            {
                Asserts.OperationNotAllowed(index >= source.Length, "Next enumeration state isn't defined.");
                return source[index + 1];
            }
        }

        public bool IsLast
        {
            get { return index >= 0 && index == source.Length - 1; }
        }

        public int Length
        {
            get { return source.Length; }
        }

        #endregion

        #region Methods

        public bool MovePrevious()
        {
            if (index < 0) return false;
            return --index >= 0;
        }

        public bool MoveNext()
        {
            return ++index < source.Length;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {
        }

        #endregion
    }
}
