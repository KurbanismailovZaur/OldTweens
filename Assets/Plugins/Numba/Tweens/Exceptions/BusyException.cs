using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Exceptions
{
    public class BusyException : ApplicationException
    {
        public BusyException(string message) : base(message) { }
    }

}