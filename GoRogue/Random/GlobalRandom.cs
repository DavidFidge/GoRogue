﻿using JetBrains.Annotations;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace GoRogue.Random
{
    /// <summary>
    /// Static class that contains 1 variable, which is a an RNG instance that is used as the default
    /// rng by other features (dice, etc) wherever needed, and can also be used if you need a random
    /// number generator for your own code.
    /// </summary>
    [PublicAPI]
    public static class GlobalRandom
    {
        /// <summary>
        /// Settable field that specifies what <see cref="Troschuetz.Random.IGenerator" /> instance should be considered the default
        /// RNG. Defaults to an <see cref="Troschuetz.Random.Generators.XorShift128Generator" /> with a time-dependent value used as a seed.
        /// </summary>
        public static IGenerator DefaultRNG = new XorShift128Generator();
    }
}
